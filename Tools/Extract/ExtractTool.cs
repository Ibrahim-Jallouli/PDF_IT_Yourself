using PDF_IT_Yourself.Core;

namespace PDF_IT_Yourself.Tools.Extract
{
    public sealed class ExtractTool
    {
        private readonly PdfPageOperations _ops;
        private readonly PdfExport _export;

        public ExtractTool(PdfPageOperations ops, PdfExport export)
        {
            _ops = ops;
            _export = export;
        }

        public async Task ExtractAndDownloadAsync(byte[] pdfBytes, int[] pages1Based, string filename)
        {
            var resultBytes = await _ops.ExtractAsync(pdfBytes, pages1Based);
            await _export.DownloadPdfAsync(filename, resultBytes);
        }
    }
}