using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DesktopPdfSigner.DTO.PDFSignDTO
{
    [Serializable]
    public class PdfRequestDTO
    {
        public string DonglePassword { get; set; }
        public byte[] pdfContent { get; set; }
    }
}
