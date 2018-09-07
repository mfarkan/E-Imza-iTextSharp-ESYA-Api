extern alias ITextSharp;
using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Deployment.Application;
using DesktopPdfSigner.DTO.PDFSignDTO;
using System.Windows.Forms;
using System.Web;
using System.IO;
using ITextSharp::iTextSharp.text.pdf;
using System.Collections.Generic;
using ITextSharp::iTextSharp.text.pdf.security;

namespace DesktopPdfSigner
{

    public partial class Form1 : Form
    {
        private PdfRequestDTO requestDTO;
        public Form1()
        {
            InitializeComponent();
            requestDTO = new PdfRequestDTO();
            bckWorker.DoWork += bckWorker_doWork;
            bckWorker.RunWorkerCompleted += bckWorker_RunWorkerCompleted;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            //InitalizeQuery();
        }
        /// <summary>
        /// ClickOnce uygulamasını publish ederseniz queryString ile çağırdğınız da aşağıdaki methodu çağırmalısınız.
        /// </summary>
        private void InitalizeQuery()
        {
            string queryString = "";

            NameValueCollection nameValueTable = new NameValueCollection();
            if (ApplicationDeployment.IsNetworkDeployed)
            {
                queryString = ApplicationDeployment.CurrentDeployment.ActivationUri.Query;
                nameValueTable = HttpUtility.ParseQueryString(queryString);
            }
        }

        private void btnSign_Click(object sender, EventArgs e)
        {

            if (!string.IsNullOrEmpty(txtUsbDonglePassword.Text))
            {
                btnSign.Text = "İmzalanıyor , lütfen bekleyiniz.";
                btnSign.Enabled = false;
                this.requestDTO.DonglePassword = txtUsbDonglePassword.Text;
                bckWorker.RunWorkerAsync();
            }
            else
            {
                MessageBox.Show("USB-Dongle şifrenizi giriniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private void bckWorker_doWork(object sender, DoWorkEventArgs e)
        {
            SignatureManager.SignatureManager signManager = new SignatureManager.SignatureManager();
            signManager.SignPdf(this.requestDTO);
        }
        private void bckWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            btnSign.Enabled = true;
            btnSign.Text = "İmzala";
            if (e.Error != null)
            {
                MessageBox.Show("Hata oluştu : " + e.Error.ToString(), "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (e.Cancelled == true)
            {
                MessageBox.Show("Operasyon iptal edildi!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                MessageBox.Show("Pdf başarıyla imzalandı ve kaydedildi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void chBoxPassword_CheckedChanged(object sender, EventArgs e)
        {
            txtUsbDonglePassword.PasswordChar = chBoxPassword.Checked ? '\0' : '*';
        }

        private void btnFileUpload_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog upload = new OpenFileDialog())
            {
                upload.Filter = "Pdf Files|*.pdf";
                upload.Title = "İmzalanacak pdf dosyasını seçiniz.";
                if (upload.ShowDialog() != DialogResult.OK)
                    return;
                string fileName = upload.FileName;
                this.requestDTO.pdfContent = File.ReadAllBytes(fileName);
            }

        }
        private string checkSignature(byte[] pdfContent)
        {
            PdfReader reader = new PdfReader(pdfContent);

            AcroFields fields = reader.AcroFields;
            List<String> names = fields.GetSignatureNames();

            // Signature eklenmiş PDF dosyası buraya yollanmalı. Yoksa Verification Gerçekleşemez.

            if (names.Count == 0)
            {
                return "İlgili PDF'e ait imza(lar) bulunamamıştır.";
            }
            string message = string.Empty;
            for (int i = 1; i < names.Count + 1; i++)
            {
                string temp = string.Empty;
                PdfPKCS7 pkcs7 = fields.VerifySignature(names[i - 1]);
                var result = pkcs7.Verify();
                if (result)
                {
                    temp = string.Format("{0}.imza geçerli.", i);
                }
                else
                {
                    temp = string.Format("{0}.imza geçersiz.", i);
                }
                message += temp;
            }
            reader.Close();
            return message;
        }
        private void btnCheckSignature_Click(object sender, EventArgs e)
        {
            var message = checkSignature(this.requestDTO.pdfContent);
            MessageBox.Show(message, "Mesaj", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
