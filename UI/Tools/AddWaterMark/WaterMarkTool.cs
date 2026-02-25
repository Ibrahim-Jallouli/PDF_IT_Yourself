using PDF_IT_Yourself.Services;
using PDF_IT_Yourself.Interop;

namespace PDF_IT_Yourself.Tools.Watermark
{
    public sealed class WatermarkTool
    {
        private readonly PdfPageOperations _ops;
        private readonly PdfExport _export;

        public WatermarkTool(PdfPageOperations ops, PdfExport export)
        {
            _ops = ops;
            _export = export;
        }

        public async Task AddWatermarkAndDownloadAsync(
            byte[] pdfBytes,
            string text,
            int fontSize,
            double opacity,
            int rotation)
        {
            var options = new PdfInterop.WatermarkOptions
            {
                text = text,
                fontSize = fontSize,
                opacity = opacity,
                rotationDegrees = rotation
            };

            var result = await _ops.AddTextWatermarkAsync(pdfBytes, options);

            await _export.DownloadPdfAsync("watermarked.pdf", result);
        }
    }
}