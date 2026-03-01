using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using PDF_IT_Yourself.Interop;

public class ConversionIMGtoPDFTool : ComponentBase
{
    [Inject] public PdfInterop PdfInterop { get; set; } = default!;
    [Inject] public IJSRuntime JS { get; set; } = default!;

    // UI
    protected bool _dragOver;
    protected bool _isBusy;
    protected string _error = "";

    // Image
    protected byte[]? _imageBytes;
    protected bool _imageLoaded;
    protected string? _fileName;
    protected string? _contentType;
    protected string? _imageDataUrl;

    protected string _page = "A4";
    protected double _margin = 10;
    protected string _fit = "contain";

    protected void OnDragEnter(DragEventArgs e) => _dragOver = true;
    protected void OnDragOver(DragEventArgs e) => _dragOver = true;
    protected void OnDragLeave(DragEventArgs e) => _dragOver = false;

    protected void OnDrop(DragEventArgs e)
    {
        _dragOver = false;
        StateHasChanged();
    }

    protected async Task OnImageSelected(InputFileChangeEventArgs e)
    {
        _error = "";
        _imageLoaded = false;
        _isBusy = false;

        _imageBytes = null;
        _fileName = null;
        _contentType = null;
        _imageDataUrl = null;

        var file = e.File;
        if (file is null) return;

        try
        {
            const long maxSize = 25 * 1024 * 1024; // 25MB
            if (file.Size > maxSize)
            {
                _error = $"File too large: {file.Name} (max 25MB).";
                return;
            }

            _fileName = file.Name;
            _contentType = file.ContentType;

            using var stream = file.OpenReadStream(maxAllowedSize: maxSize);
            using var ms = new MemoryStream();
            await stream.CopyToAsync(ms);
            _imageBytes = ms.ToArray();

            // Preview as data-url
            _imageDataUrl = $"data:{_contentType};base64,{Convert.ToBase64String(_imageBytes)}";
            _imageLoaded = true;
        }
        catch (Exception ex)
        {
            _error = $"Error loading image: {ex.Message}";
            _imageLoaded = false;
            _imageBytes = null;
        }
    }

    protected async Task ConvertAndDownload()
    {
        _error = "";

        if (_imageBytes is null || !_imageLoaded)
        {
            _error = "No image loaded.";
            return;
        }

        if (_margin < 0 || _margin > 30)
        {
            _error = "Margin must be between 0 and 30mm.";
            return;
        }

        _isBusy = true;
        try
        {
            var outName = BuildOutName(_fileName);

            var options = new PdfInterop.ImageToPdfOptions
            {
                Page = _page,
                Margin = _margin,
                Fit = _fit,
                Filename = outName
            };
            var pdfBytes = await PdfInterop.ImageToPdfAsync(_imageBytes, options);

            await JS.InvokeVoidAsync("PdfTools.downloadBytes", outName, pdfBytes, "application/pdf");
        }
        catch (Exception ex)
        {
            _error = $"Conversion error: {ex.Message}";
        }
        finally
        {
            _isBusy = false;
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await JS.InvokeVoidAsync("pdfDrop.wireDropZone", ".dropzone-img2pdf", "imgInput");
        }
    }

    protected static string BuildOutName(string? fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            return "image.pdf";

        var baseName = Path.GetFileNameWithoutExtension(fileName);
        if (string.IsNullOrWhiteSpace(baseName))
            return "image.pdf";

        return $"{baseName}.pdf";
    }

    protected static string FormatBytes(long bytes)
    {
        if (bytes <= 0) return "0 B";
        string[] units = { "B", "KB", "MB", "GB" };
        double size = bytes;
        int unit = 0;
        while (size >= 1024 && unit < units.Length - 1)
        {
            size /= 1024;
            unit++;
        }
        return $"{size:0.##} {units[unit]}";
    }
}