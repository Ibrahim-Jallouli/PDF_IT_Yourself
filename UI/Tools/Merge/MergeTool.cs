using PDF_IT_Yourself.Services;

namespace PDF_IT_Yourself.Tools.Merge
{
    public sealed class MergeTool
    {
        private readonly PdfPageOperations _ops;
        private readonly PdfExport _export;

        public MergeTool(PdfPageOperations ops, PdfExport export)
        {
            _ops = ops;
            _export = export;
        }

        public async Task MergeAndDownloadAsync(IReadOnlyList<byte[]> pdfsBytes, string filename)
        {
            var merged = await _ops.MergeAsync(pdfsBytes);
            await _export.DownloadPdfAsync(filename, merged);
        }
    }
}