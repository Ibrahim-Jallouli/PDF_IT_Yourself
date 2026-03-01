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
            string colorHex)
        {
            var options = new PdfInterop.WatermarkOptions
            {
                text = text,
                fontSize = fontSize,
                opacity = opacity,
                color = HexToRgb01(colorHex),

                placement = "diagonal"
            };

            var result = await _ops.AddTextWatermarkAsync(pdfBytes, options);
            await _export.DownloadPdfAsync("watermarked.pdf", result);
        }

        private static PdfInterop.Rgb01 HexToRgb01(string? hex)
        {
            var fallback = new PdfInterop.Rgb01 { r = 0.2, g = 0.2, b = 0.2 };

            if (string.IsNullOrWhiteSpace(hex))
                return fallback;

            hex = hex.Trim();
            if (hex.StartsWith("#"))
                hex = hex[1..];
            if (hex.Length == 3)
                hex = string.Concat(hex.Select(c => $"{c}{c}"));
            if (hex.Length != 6)
                return fallback;

            try
            {
                var r = Convert.ToInt32(hex.Substring(0, 2), 16) / 255.0;
                var g = Convert.ToInt32(hex.Substring(2, 2), 16) / 255.0;
                var b = Convert.ToInt32(hex.Substring(4, 2), 16) / 255.0;

                return new PdfInterop.Rgb01 { r = r, g = g, b = b };
            }
            catch
            {
                return fallback;
            }
        }
    }
}