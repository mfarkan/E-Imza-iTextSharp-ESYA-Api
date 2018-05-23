extern alias ITextSharp;
extern alias ma3Bouncy;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using tr.gov.tubitak.uekae.esya.api.asn.x509;
using System.Configuration;
using ITextSharp.iTextSharp.text.pdf.security;
using ITextSharp.iTextSharp.text.pdf;
using ITextSharp.Org.BouncyCastle.X509;
using X509Certificate = ITextSharp.Org.BouncyCastle.X509.X509Certificate;
using DesktopPdfSigner.SmartCard;
using tr.gov.tubitak.uekae.esya.api.common.util;
using tr.gov.tubitak.uekae.esya.api.certificate.validation.policy;
using tr.gov.tubitak.uekae.esya.api.certificate.validation;
using tr.gov.tubitak.uekae.esya.api.certificate.validation.check.certificate;
using System.Windows.Forms;
using DesktopPdfSigner.DTO.PDFSignDTO;
using tr.gov.tubitak.uekae.esya.api.xmlsignature;

namespace DesktopPdfSigner.PdfSigner
{
    public class PdfSigner
    {
        public PdfSigner()
        {
            LicenseUtil.setLicenseXml(new FileStream(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "lisans.xml"), FileMode.Open, FileAccess.Read));

        }
        static public ICrlClient crl;
        static public List<ICrlClient> crlList;
        static public OcspClientBouncyCastle ocsp;
        private static System.Object lockSign = new System.Object();
        private static System.Object lockToken = new System.Object();
        private X509Certificate2[] generateCertificateChain(X509Certificate2 signingCertificate)
        {
            X509Chain Xchain = new X509Chain();
            Xchain.ChainPolicy.ExtraStore.Add(signingCertificate);
            Xchain.Build(signingCertificate); // Whole chain!
            X509Certificate2[] chain = new X509Certificate2[Xchain.ChainElements.Count];
            int index = 0;
            foreach (X509ChainElement element in Xchain.ChainElements)
            {
                chain[index++] = element.Certificate;
            }
            return chain;
        }
        /// <summary>
        /// PDF imzalar.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="PDFContent"></param>
        /// <returns></returns>
        public byte[] SignPDF(PdfRequestDTO request
            , byte[] PDFContent
            )
        {
            //if (PDFContent == null || request == null)
            //{
            //    return null;
            //}
            X509Certificate2 signingCertificate;
            IExternalSignature externalSignature;
            this.SelectSignature(request, out signingCertificate, out externalSignature);
            X509Certificate2[] chain = generateCertificateChain(signingCertificate);
            ICollection<X509Certificate> Bouncychain = chainToBouncyCastle(chain);
            ocsp = new OcspClientBouncyCastle();
            crl = new ITextSharp.iTextSharp.text.pdf.security.CrlClientOnline(Bouncychain);
            PdfReader pdfReader = new PdfReader(PDFContent);
            MemoryStream stream = new MemoryStream();
            PdfStamper pdfStamper = PdfStamper.CreateSignature(pdfReader, stream, '\0', "", true);
            PdfSignatureAppearance signatureAppearance = pdfStamper.SignatureAppearance;
            crlList = new List<ICrlClient>();
            crlList.Add(crl);
            lock (lockSign)
            {
                ITextSharp.iTextSharp.text.pdf.security.MakeSignature.SignDetached(signatureAppearance, externalSignature, Bouncychain, crlList, ocsp, null, 0, CryptoStandard.CMS);
            }
            return stream.ToArray();
        }
        private static ICollection<ITextSharp.Org.BouncyCastle.X509.X509Certificate> chainToBouncyCastle(X509Certificate2[] chain)
        {
            ITextSharp.Org.BouncyCastle.X509.X509CertificateParser cp = new ITextSharp.Org.BouncyCastle.X509.X509CertificateParser();

            ICollection<ITextSharp.Org.BouncyCastle.X509.X509Certificate> Bouncychain = new List<ITextSharp.Org.BouncyCastle.X509.X509Certificate>();
            int index = 0;
            foreach (var item in chain)
            {
                Bouncychain.Add(cp.ReadCertificate(item.RawData));
            }
            return Bouncychain;

        }
        private void SelectSignature(
           PdfRequestDTO request,
           out X509Certificate2 CERTIFICATE,
           out IExternalSignature externalSignature)
        {
            try
            {
                SmartCardManager smartCardManager = SmartCardManager.getInstance();
                var smartCardCertificate = smartCardManager.getSignatureCertificate(false, false);
                var signer = smartCardManager.getSigner(request.DonglePassword, smartCardCertificate);
                CERTIFICATE = smartCardCertificate.asX509Certificate2();
                externalSignature = new SmartCardSignature(signer, CERTIFICATE, "SHA-256");

            }
            catch (Exception ex)
            {
                CERTIFICATE = null;
                externalSignature = null;
                MessageBox.Show(ex.Message);
            }

        }
    }
}
