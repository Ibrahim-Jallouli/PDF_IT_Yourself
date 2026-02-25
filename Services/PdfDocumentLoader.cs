using PDF_IT_Yourself.Interop;

namespace PDF_IT_Yourself.Services
{
    public sealed class PdfDocumentLoader
    {
        private readonly PdfInterop _interop;

        public PdfDocumentLoader(PdfInterop interop)
        {
            _interop = interop;
        }

        public async Task<int> GetValidatedPageCountAsync(byte[] pdfBytes)
        {
            ValidatePdfBytes(pdfBytes);

            var count = await _interop.GetPageCountAsync(pdfBytes);
            if (count <= 0)
                throw new Exception("Le PDF ne contient aucune page.");

            return count;
        }

        public static void ValidatePdfBytes(byte[] pdfBytes)
        {
            if (pdfBytes is null || pdfBytes.Length == 0)
                throw new Exception("Fichier vide.");

            // Check signature "%PDF-"
            if (pdfBytes.Length < 5 ||
                pdfBytes[0] != (byte)'%' ||
                pdfBytes[1] != (byte)'P' ||
                pdfBytes[2] != (byte)'D' ||
                pdfBytes[3] != (byte)'F' ||
                pdfBytes[4] != (byte)'-')
            {
                throw new Exception("Ce fichier ne ressemble pas à un PDF valide.");
            }
        }
    }
}