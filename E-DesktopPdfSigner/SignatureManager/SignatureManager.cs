using DesktopPdfSigner.DTO.PDFSignDTO;
using System;

namespace DesktopPdfSigner.SignatureManager
{
    public class SignatureManager
    {
        private PdfSigner.PdfSigner _pdfSigner;
        public SignatureManager()
        {
            _pdfSigner = new PdfSigner.PdfSigner();
        }
        public void SignPdf(PdfRequestDTO requestDTO)
        {
            try
            {
                var pdfContentWithSign = _pdfSigner.SignPDF(requestDTO, requestDTO.pdfContent);
            }
            catch (Exception ex)
            {
                throw new Exception("Dosyanın imzalanması sırasında bir hata oluştu.");
            }
        }
    }
}
