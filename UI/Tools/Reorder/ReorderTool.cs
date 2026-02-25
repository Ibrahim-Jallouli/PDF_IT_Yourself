using PDF_IT_Yourself.Services;

namespace PDF_IT_Yourself.Tools.Reorder
{
    public sealed class ReorderTool
    {
        private readonly PdfPageOperations _ops;
        private readonly PdfExport _export;

        public ReorderTool(PdfPageOperations ops, PdfExport export)
        {
            _ops = ops;
            _export = export;
        }

        public async Task ReorderAndDownloadAsync(byte[] pdfBytes, int[] newOrder1Based, string filename)
        {
            var result = await _ops.ReorderAsync(pdfBytes, newOrder1Based);
            await _export.DownloadPdfAsync(filename, result);
        }
    }
}