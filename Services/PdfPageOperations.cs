using PDF_IT_Yourself.Interop;
using System.Collections;

namespace PDF_IT_Yourself.Services
{
    public sealed class PdfPageOperations
    {
        private readonly PdfInterop _interop;

        public PdfPageOperations(PdfInterop interop)
        {
            _interop = interop;
        }
        public Task<byte[]> ExtractAsync(byte[] pdfBytes, int[] pages1Based)
        {
            PdfDocumentLoader.ValidatePdfBytes(pdfBytes);
            var indices = To0BasedIndices(pages1Based);
            return _interop.ExtractPagesAsync(pdfBytes, indices);
        }

        public Task<byte[]> DeleteAsync(byte[] pdfBytes, int[] pages1BasedToDelete)
        {
            PdfDocumentLoader.ValidatePdfBytes(pdfBytes);
            var indices = To0BasedIndices(pages1BasedToDelete);
            return _interop.DeletePagesAsync(pdfBytes, indices);
        }

        public Task<byte[]> ReorderAsync(byte[] pdfBytes, int[] newOrder1Based)
        {
            PdfDocumentLoader.ValidatePdfBytes(pdfBytes);

            var indices0 = To0BasedIndices(newOrder1Based);
            ValidatePermutation(indices0);

            return _interop.ReorderPagesAsync(pdfBytes, indices0);
        }


        public Task<byte[]> MergeAsync(IReadOnlyList<byte[]> pdfsBytes)
        {
            if (pdfsBytes is null || pdfsBytes.Count < 2)
                throw new Exception("Il faut au moins 2 PDFs pour fusionner.");

            foreach (var b in pdfsBytes)
                PdfDocumentLoader.ValidatePdfBytes(b);

            return _interop.MergePdfsAsync(pdfsBytes);
        }

        public Task<byte[][]> SplitByRangesAsync(byte[] pdfBytes, PageRange1Based[] ranges1Based)
        {
            PdfDocumentLoader.ValidatePdfBytes(pdfBytes);

            if (ranges1Based is null || ranges1Based.Length == 0)
                throw new Exception("Aucune plage fournie.");

            var ranges0 = ranges1Based.Select(r => new PdfInterop.PageRange0Based
            {
                start = r.Start-1,
                end = r.End-1,
            }).ToArray();

            return _interop.SplitByRangesAsync(pdfBytes, ranges0);
        }

        public Task<byte[]> AddTextWatermarkAsync(byte[] pdfBytes, PdfInterop.WatermarkOptions? options = null)
        {
            PdfDocumentLoader.ValidatePdfBytes(pdfBytes);
            return _interop.AddTextWatermarkAsync(pdfBytes, options);
        }

        public Task<byte[]> AddPageNumbersAsync(byte[] pdfBytes, PdfInterop.PageNumberOptions? options = null)
        {
            PdfDocumentLoader.ValidatePdfBytes(pdfBytes);
            return _interop.AddPageNumbersAsync(pdfBytes, options);
        }

        // -------------------------
        // Helpers
        // -------------------------

        public sealed class PageRange1Based
        {
            public int Start { get; set; }
            public int End { get; set; }
        }

        private static int[] To0BasedIndices(int[] pages1Based)
        {
            if (pages1Based == null || pages1Based.Length == 0)
                throw new Exception("Aucune page fournie.");

            var result = new int[pages1Based.Length];

            for (int i = 0; i < pages1Based.Length; i++)
            {
                int p = pages1Based[i];

                if (p <= 0)
                    throw new Exception("Les pages doivent être >= 1.");

                result[i] = p - 1;
            }

            return result;
        }


        private static void ValidatePermutation(int[] newOrder0Based)
        {
            if (newOrder0Based.Length == 0)
                throw new Exception("Ordre vide.");

            var n = newOrder0Based.Length;
            var seen = new bool[n];

            foreach (var idx in newOrder0Based)
            {
                if (idx < 0 || idx >= n)
                    throw new Exception($"Ordre invalide: index hors limites {idx} (attendu 0..{n - 1}).");

                if (seen[idx])
                    throw new Exception($"Ordre invalide: page dupliquée (index {idx}).");

                seen[idx] = true;
            }
        }
    }
}