using Microsoft.JSInterop;
using PDF_IT_Yourself.Interop;

namespace PDF_IT_Yourself.Services
{
    public sealed class PagePickerTool : IAsyncDisposable
    {
        private readonly PdfInterop _interop;

        private IJSObjectReference? _pdf;
        private string? _pdfKey;

        // protège EnsureLoadedAsync contre appels concurrents
        private readonly SemaphoreSlim _loadGate = new(1, 1);

        public PagePickerTool(PdfInterop interop) => _interop = interop;

        private static string ComputeKey(byte[] bytes)
        {
            int len = bytes.Length;
            uint head = len >= 4 ? BitConverter.ToUInt32(bytes, 0) : 0;
            uint tail = len >= 4 ? BitConverter.ToUInt32(bytes, len - 4) : 0;
            return $"{len}:{head}:{tail}";
        }

        public async ValueTask EnsureLoadedAsync(byte[] pdfBytes, CancellationToken ct = default)
        {
            var key = ComputeKey(pdfBytes);
            if (_pdf is not null && _pdfKey == key) return;

            await _loadGate.WaitAsync(ct);
            try
            {
                // recheck après lock
                if (_pdf is not null && _pdfKey == key) return;

                await UnloadAsync(ct);
                _pdf = await _interop.LoadPdfAsync(pdfBytes, ct);
                _pdfKey = key;
            }
            finally
            {
                _loadGate.Release();
            }
        }

        public async ValueTask<string> GetThumbnailObjectUrlAsync(byte[] pdfBytes, int pageIndex0Based, double scale = 0.25, CancellationToken ct = default)
        {
            await EnsureLoadedAsync(pdfBytes, ct);
            return await _interop.RenderPageToObjectUrlAsync(
                _pdf!,
                pageIndex0Based,
                scale,
                "image/webp",
                0.85,
                ct);
        }

        public ValueTask RevokeObjectUrlAsync(string url, CancellationToken ct = default)
            => _interop.RevokeObjectUrlAsync(url, ct);

        private async ValueTask UnloadAsync(CancellationToken ct = default)
        {
            if (_pdf is null) return;

            try { await _interop.DestroyPdfAsync(_pdf, ct); } catch { }
            try { await _pdf.DisposeAsync(); } catch { }

            _pdf = null;
            _pdfKey = null;
        }

        public async ValueTask DisposeAsync()
        {
            // pas obligatoire, mais propre
            _loadGate.Dispose();
            await UnloadAsync();
        }
    }
}