using PDF_IT_Yourself.Interop;

namespace PDF_IT_Yourself.Services
{
    public sealed class PdfExport
    {
        private readonly PdfInterop _interop;

        public PdfExport(PdfInterop interop)
        {
            _interop = interop;
        }

        public Task DownloadPdfAsync(string filename, byte[] pdfBytes)
        {
            PdfDocumentLoader.ValidatePdfBytes(pdfBytes);

            filename = NormalizePdfFilename(filename);

            return _interop.DownloadBytesAsync(filename, pdfBytes, "application/pdf").AsTask();
        }

        private static string NormalizePdfFilename(string filename)
        {
            if (string.IsNullOrWhiteSpace(filename))
                filename = "document.pdf";

            if (!filename.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                filename += ".pdf";

            return filename;
        }
    }
}