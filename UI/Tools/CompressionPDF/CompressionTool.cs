using PDF_IT_Yourself.Interop;
using PDF_IT_Yourself.Services;

namespace PDF_IT_Yourself.Tools.CompressionPDF
{
    public sealed class CompressionTool
    {
        private readonly PdfPageOperations _ops;
        private readonly PdfExport _export;

        public CompressionTool(PdfPageOperations ops, PdfExport export)
        {
            _ops = ops;
            _export = export;
        }

        public async Task<byte[]> CompressAsync(byte[] pdfBytes, PdfInterop.CompressOptions? options = null)
        {
            return await _ops.CompressLosslessAsync(pdfBytes, options);
        }

        public async Task CompressAndDownloadAsync(byte[] pdfBytes, string filename, PdfInterop.CompressOptions? options = null)
        {
            var result = await _ops.CompressLosslessAsync(pdfBytes, options);
            await _export.DownloadPdfAsync(filename, result);
        }
    }
}