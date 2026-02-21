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

        /// <summary>
        /// Extrait des pages d'un PDF en gardant l'ordre donné (indices 0-based).
        /// </summary>
        public async Task<byte[]> ExtractPagesAsync(byte[] pdfBytes, int[] pageIndices)
        {
            // Appelle window.PdfTools.extractPages(pdfBytes, pageIndices)
            // Retour JS = Uint8Array -> mappé en byte[] côté .NET
            return await _js.InvokeAsync<byte[]>(
                "PdfTools.extractPages",
                pdfBytes,
                pageIndices
            );
        }
    }
}