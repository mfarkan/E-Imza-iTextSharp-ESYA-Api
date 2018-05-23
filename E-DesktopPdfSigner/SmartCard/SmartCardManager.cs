extern alias ITextSharp;
using iaik.pkcs.pkcs11.wrapper;
using ITextSharp::iTextSharp.text.pdf.security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using tr.gov.tubitak.uekae.esya.api.asn.x509;
using tr.gov.tubitak.uekae.esya.api.common;
using tr.gov.tubitak.uekae.esya.api.common.crypto;
using tr.gov.tubitak.uekae.esya.api.common.util;
using tr.gov.tubitak.uekae.esya.api.common.util.bag;
using tr.gov.tubitak.uekae.esya.api.smartcard.pkcs11;

namespace DesktopPdfSigner.SmartCard
{
    public class SmartCardSignature : IExternalSignature
    {
        /// <summary>
        /// The certificate with the private key
        /// </summary>
        private X509Certificate2 certificate;
        /** The hash algorithm. */
        private String hashAlgorithm;
        /** The encryption algorithm (obtained from the private key) */
        private String encryptionAlgorithm;

        private BaseSigner mSigner = null;

        /// <summary>
        /// Creates a signature using a X509Certificate2. It supports smartcards without 
        /// exportable private keys.
        /// </summary>
        /// <param name="certificate">The certificate with the private key</param>
        /// <param name="hashAlgorithm">The hash algorithm for the signature. As the Windows CAPI is used
        /// to do the signature the only hash guaranteed to exist is SHA-1</param>
        public SmartCardSignature(X509Certificate2 certificate, String hashAlgorithm)
        {
            if (!certificate.HasPrivateKey)
                throw new ArgumentException("No private key.");
            this.certificate = certificate;
            this.hashAlgorithm = DigestAlgorithms.GetDigest(DigestAlgorithms.GetAllowedDigests(hashAlgorithm));
            if (certificate.PrivateKey is RSACryptoServiceProvider)
                encryptionAlgorithm = "RSA";
            else if (certificate.PrivateKey is DSACryptoServiceProvider)
                encryptionAlgorithm = "DSA";
            else
                throw new ArgumentException("Unknown encryption algorithm " + certificate.PrivateKey);
        }

        public SmartCardSignature(BaseSigner signer, X509Certificate2 certificate, String hashAlgorithm)
        {
            mSigner = signer;
            this.certificate = certificate;
            this.hashAlgorithm = DigestAlgorithms.GetDigest(DigestAlgorithms.GetAllowedDigests(hashAlgorithm));
            encryptionAlgorithm = "RSA";
        }

        public virtual byte[] Sign(byte[] message)
        {
            if (mSigner != null)
            {
                return mSigner.sign(message);
            }
            if (certificate.PrivateKey is RSACryptoServiceProvider)
            {
                RSACryptoServiceProvider rsa = (RSACryptoServiceProvider)certificate.PrivateKey;
                return rsa.SignData(message, hashAlgorithm);
            }
            else
            {
                DSACryptoServiceProvider dsa = (DSACryptoServiceProvider)certificate.PrivateKey;
                return dsa.SignData(message);
            }
        }

        /**
         * Returns the hash algorithm.
         * @return  the hash algorithm (e.g. "SHA-1", "SHA-256,...")
         * @see com.itextpdf.text.pdf.security.ExternalSignature#getHashAlgorithm()
         */
        public virtual String GetHashAlgorithm()
        {
            return hashAlgorithm;
        }

        /**
         * Returns the encryption algorithm used for signing.
         * @return the encryption algorithm ("RSA" or "DSA")
         * @see com.itextpdf.text.pdf.security.ExternalSignature#getEncryptionAlgorithm()
         */
        public virtual String GetEncryptionAlgorithm()
        {
            return encryptionAlgorithm;
        }
    }
    public class SmartCardManager
    {
        /// <summary>
        /// The msc manager
        /// </summary>
        private static SmartCardManager mSCManager;

        /// <summary>
        /// The m slot count
        /// </summary>
        private int mSlotCount = 0;

        /// <summary>
        /// The m serial number
        /// </summary>
        private readonly String mSerialNumber;

        /// <summary>
        /// The m signature cert
        /// </summary>
        private ECertificate mSignatureCert;

        /// <summary>
        /// The m encryption cert
        /// </summary>
        private ECertificate mEncryptionCert;

        /// <summary>
        /// The BSC
        /// </summary>
        protected IBaseSmartCard bsc;

        /// <summary>
        /// The m signer
        /// </summary>
        protected BaseSigner mSigner;

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static SmartCardManager getInstance()
        {
            if (mSCManager == null)
            {
                mSCManager = new SmartCardManager();
                return mSCManager;
            }
            else
            {
                //Check is there any change
                try
                {
                    //If there is a new card in the system, user will select a smartcard. 
                    //Create new SmartCard.
                    if (mSCManager.getSlotCount() < SmartOp.getCardTerminals().Length)
                    {
                        Console.WriteLine("New card pluged in to system");
                        mSCManager = null;
                        return getInstance();
                    }

                    //If used card is removed, select new card.
                    String availableSerial = null;
                    try
                    {
                        availableSerial = StringUtil.ToString(mSCManager.getBasicSmartCard().getSerial());
                    }
                    catch (SmartCardException ex)
                    {
                        Console.WriteLine("Card removed");
                        mSCManager = null;
                        return getInstance();
                    }
                    if (!mSCManager.getSelectedSerialNumber().Equals(availableSerial))
                    {
                        Console.WriteLine("Serial number changed. New card is placed to system");
                        mSCManager = null;
                        return getInstance();
                    }

                    return mSCManager;
                }
                catch (SmartCardException e)
                {
                    mSCManager = null;
                    throw;
                }
            }
        }
        private SmartCardManager()
        {
            try
            {
                Console.WriteLine("New SmartCardManager will be created");
                String[] terminals = SmartOp.getCardTerminals();
                String terminal;
                //List<CardTypeConfig> cards = new SmartCardConfigParser().readConfig();
                //CardType.applyCardTypeConfig(cards);

                if (terminals == null || terminals.Length == 0)
                    throw new SmartCardException("Kart takılı kart okuyucu bulunamadı");

                if (terminals.Length > 1)
                    throw new SmartCardException("Birden fazla kart okuyucu bulundu.");

                Console.WriteLine("Kart okuyucu sayısı : " + terminals.Length);

                int index = 0;
                if (terminals.Length == 1)
                    terminal = terminals[index];
                else
                {
                    //index = askOption(null, null, terminals, "Okuyucu Listesi", new String[] { "Tamam" });
                    terminal = terminals[index];
                }
                Console.WriteLine("PKCS11 Smartcard will be created");
                Pair<long, CardType> slotAndCardType = SmartOp.getSlotAndCardType(terminal);

                bsc = new P11SmartCard(slotAndCardType.getmObj2());
                bsc.openSession(slotAndCardType.getmObj1());

                mSerialNumber = StringUtil.ToString(bsc.getSerial());
                mSlotCount = terminals.Length;

            }
            catch (SmartCardException e)
            {
                throw new SmartCardException("Kart Okunurken bir hata oluştu(SC).", e);
            }
            catch (PKCS11Exception e)
            {
                throw new SmartCardException("Kart okunurken bir hata oluştu(PKCS11).", e);
            }
            catch (IOException e)
            {
                throw new SmartCardException("Kart okunurken bir hata oluştu(IO).", e);
            }
            catch (Exception e)
            {
                throw new SmartCardException("Genel bir hata oluştu(E)", e);
            }
        }
        [MethodImpl(MethodImplOptions.Synchronized)]
        public BaseSigner getSigner(String aCardPIN, ECertificate aCert)
        {

            if (mSigner == null)
            {
                bsc.login(aCardPIN);
                mSigner = bsc.getSigner(aCert, Algorithms.SIGNATURE_RSA_SHA256);
            }
            return mSigner;
        }
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void logout()
        {
            mSigner = null;
            bsc.logout();
        }
        [MethodImpl(MethodImplOptions.Synchronized)]
        public ECertificate getSignatureCertificate(bool checkIsQualified, bool checkBeingNonQualified)
        {
            if (mSignatureCert == null)
            {
                List<byte[]> allCerts = bsc.getSignatureCertificates();
                mSignatureCert = selectCertificate(checkIsQualified, checkBeingNonQualified, allCerts);
            }

            return mSignatureCert;
        }
        private ECertificate selectCertificate(bool checkIsQualified, bool checkBeingNonQualified, List<byte[]> aCerts)
        {
            if (aCerts != null && aCerts.Count == 0)
                throw new ESYAException("Kartta sertifika bulunmuyor");

            if (checkIsQualified && checkBeingNonQualified)
                throw new ESYAException(
                    "Bir sertifika ya nitelikli sertifikadir, ya niteliksiz sertifikadir. Hem nitelikli hem niteliksiz olamaz");

            List<ECertificate> certs = new List<ECertificate>();

            foreach (byte[] bs in aCerts)
            {
                ECertificate cert = new ECertificate(bs);

                if (checkIsQualified)
                {
                    if (cert.isQualifiedCertificate())
                        certs.Add(cert);
                }
                else if (checkBeingNonQualified)
                {
                    if (!cert.isQualifiedCertificate())
                        certs.Add(cert);
                }
                else
                {
                    certs.Add(cert);
                }
            }

            ECertificate selectedCert = null;

            if (certs.Count == 0)
            {
                if (checkIsQualified)
                    throw new ESYAException("Kartta nitelikli sertifika bulunmuyor");
                else if (checkBeingNonQualified)
                    throw new ESYAException("Kartta niteliksiz sertifika bulunmuyor");
            }
            else if (certs.Count == 1)
            {
                selectedCert = certs[0];
            }
            else
            {
                selectedCert = certs[0];
            }
            return selectedCert;
        }
        private String getSelectedSerialNumber()
        {
            return mSerialNumber;
        }
        private int getSlotCount()
        {
            return mSlotCount;
        }
        public IBaseSmartCard getBasicSmartCard()
        {
            return bsc;
        }
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void reset()
        {
            mSCManager = null;
        }

    }
}
