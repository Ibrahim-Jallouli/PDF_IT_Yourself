using PDF_IT_Yourself.Services;

namespace PDF_IT_Yourself.Tools.Split
{
    public sealed class SplitTool
    {
        private readonly PdfPageOperations _ops;
        private readonly PdfExport _export;

        public SplitTool(PdfPageOperations ops, PdfExport export)
        {
            _ops = ops;
            _export = export;
        }

        public async Task SplitAndDownloadAsync(
            byte[] pdfBytes,
            PdfPageOperations.PageRange1Based[] ranges1Based,
            string baseFilename)
        {
            var parts = await _ops.SplitByRangesAsync(pdfBytes, ranges1Based);

            // Télécharge chaque fichier séparément
            for (int i = 0; i < parts.Length; i++)
            {
                var name = $"{baseFilename}_part{i + 1}.pdf";
                await _export.DownloadPdfAsync(name, parts[i]);
            }
        }
    }
}