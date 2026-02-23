using PDF_IT_Yourself.Core;

namespace PDF_IT_Yourself.Tools.Delete
{
    public sealed class DeleteTool
    {
        private readonly PdfPageOperations _ops;
        private readonly PdfExport _export;

        public DeleteTool(PdfPageOperations ops, PdfExport export)
        {
            _ops = ops;
            _export = export;
        }

        public async Task DeleteAndDownloadAsync(byte[] pdfBytes, int[] pages1BasedToDelete, string filename)
        {
            var result = await _ops.DeleteAsync(pdfBytes, pages1BasedToDelete);
            await _export.DownloadPdfAsync(filename, result);
        }
    }
}