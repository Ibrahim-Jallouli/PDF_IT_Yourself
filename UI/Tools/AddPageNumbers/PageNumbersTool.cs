using PDF_IT_Yourself.Services;
using PDF_IT_Yourself.Interop;

namespace PDF_IT_Yourself.Tools.PageNumbers
{
    public sealed class PageNumbersTool
    {
        private readonly PdfPageOperations _ops;
        private readonly PdfExport _export;

        public PageNumbersTool(PdfPageOperations ops, PdfExport export)
        {
            _ops = ops;
            _export = export;
        }

        public async Task AddPageNumbersAndDownloadAsync(
            byte[] pdfBytes,
            string template,
            string position,
            int fontSize,
            double opacity)
        {
            var options = new PdfInterop.PageNumberOptions
            {
                template = template,
                position = position,
                fontSize = fontSize,
                opacity = opacity
            };

            var result = await _ops.AddPageNumbersAsync(pdfBytes, options);
            await _export.DownloadPdfAsync("numbered.pdf", result);
        }
    }
}