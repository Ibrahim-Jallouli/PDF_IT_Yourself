using Microsoft.JSInterop;

namespace PDF_IT_Yourself.Interop
{
    public sealed class PdfInterop
    {
        private readonly IJSRuntime _js;

        public PdfInterop(IJSRuntime js)
        {
            _js = js;
        }

        // -------------------------
        // Core low-level operations
        // -------------------------

        public Task<int> GetPageCountAsync(byte[] pdfBytes)
            => _js.InvokeAsync<int>("PdfTools.getPageCount", pdfBytes).AsTask();

        public Task<byte[]> ExtractPagesAsync(byte[] pdfBytes, int[] pageIndices0Based)
            => _js.InvokeAsync<byte[]>("PdfTools.extractPages", pdfBytes, pageIndices0Based).AsTask();

        public async Task<byte[]> MergePdfsAsync(IReadOnlyList<byte[]> pdfsBytes)
        {
            var arr = pdfsBytes as byte[][] ?? pdfsBytes.ToArray();

            return await _js.InvokeAsync<byte[]>(
                "PdfTools.mergePdfs",
                new object[] { arr }
            );
        }

        public Task<byte[]> DeletePagesAsync(byte[] pdfBytes, int[] pageIndicesToDelete0Based)
            => _js.InvokeAsync<byte[]>("PdfTools.deletePages", pdfBytes, pageIndicesToDelete0Based).AsTask();

        public Task<byte[]> ReorderPagesAsync(byte[] pdfBytes, int[] newOrder0Based)
            => _js.InvokeAsync<byte[]>("PdfTools.reorderPages", pdfBytes, newOrder0Based).AsTask();

        public Task<byte[][]> SplitByRangesAsync(byte[] pdfBytes, PageRange0Based[] ranges)
            => _js.InvokeAsync<byte[][]>("PdfTools.splitByRanges", pdfBytes, ranges).AsTask();


        public Task<byte[]> AddTextWatermarkAsync(byte[] pdfBytes, WatermarkOptions? options = null)
            => _js.InvokeAsync<byte[]>("PdfTools.addTextWatermark", pdfBytes, options ?? new WatermarkOptions()).AsTask();

        public Task<byte[]> AddPageNumbersAsync(byte[] pdfBytes, PageNumberOptions? options = null)
            => _js.InvokeAsync<byte[]>("PdfTools.addPageNumbers", pdfBytes, options ?? new PageNumberOptions()).AsTask();

        public ValueTask DownloadBytesAsync(string filename, byte[] bytes, string mimeType = "application/pdf")
            => _js.InvokeVoidAsync("PdfTools.downloadBytes", filename, bytes, mimeType);

        public ValueTask<IJSObjectReference> LoadPdfAsync(
         byte[] pdfBytes,
         CancellationToken cancellationToken = default)
         => _js.InvokeAsync<IJSObjectReference>(
             "PdfTools.loadPdf",
             cancellationToken,
             pdfBytes);

        public ValueTask<string> RenderPageToObjectUrlAsync(
            IJSObjectReference pdf,
            int pageIndex0Based,
            double scale = 0.25,
            string mime = "image/png",
            double quality = 0.92,
            CancellationToken cancellationToken = default)
        {
            return _js.InvokeAsync<string>(
                "PdfTools.renderPageToObjectUrl",
                cancellationToken,
                pdf,
                pageIndex0Based,
                new { scale, mime, quality }
            );
        }

        public ValueTask RevokeObjectUrlAsync(string url, CancellationToken cancellationToken = default)
        {
            return _js.InvokeVoidAsync("PdfTools.revokeObjectUrl", cancellationToken, url);
        }

        public ValueTask DestroyPdfAsync(
            IJSObjectReference pdf,
            CancellationToken cancellationToken = default)
            => _js.InvokeVoidAsync(
                "PdfTools.destroyPdf",
                cancellationToken,
                pdf);
        // -------------------------
        // DTOs sent to JS
        // -------------------------

        public sealed class PageRange0Based
        {
            public int start { get; set; }
            public int end { get; set; }
        }

        public sealed class RgbColor
        {
            public double r { get; set; } = 0;
            public double g { get; set; } = 0;
            public double b { get; set; } = 0;
        }

        public sealed class WatermarkOptions
        {
            public string text { get; set; } = "CONFIDENTIAL";
            public int fontSize { get; set; } = 48;
            public double opacity { get; set; } = 0.15;
            public int rotationDegrees { get; set; } = 45;

            public RgbColor color { get; set; } = new RgbColor { r = 0.2, g = 0.2, b = 0.2 };

            public string placement { get; set; } = "diagonal";

            public double offsetX { get; set; } = 0;
            public double offsetY { get; set; } = 0;
        }

        public sealed class PageNumberOptions
        {
            public int fontSize { get; set; } = 12;
            public double opacity { get; set; } = 0.9;

            public RgbColor color { get; set; } = new RgbColor { r = 0, g = 0, b = 0 };

            public double marginX { get; set; } = 40;
            public double marginY { get; set; } = 25;

            // "bottom-right" | "bottom-center" | "bottom-left" | "top-right" | ...
            public string position { get; set; } = "bottom-right";

            // supports {n} and {total}
            public string template { get; set; } = "{n}/{total}";

            public int startAt { get; set; } = 1;
        }
    }
}