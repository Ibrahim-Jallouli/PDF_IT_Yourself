using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using PDF_IT_Yourself;
using PDF_IT_Yourself.Services;
using PDF_IT_Yourself.Interop;
using PDF_IT_Yourself.Tools.Delete;
using PDF_IT_Yourself.Tools.Extract;
using PDF_IT_Yourself.Tools.Merge;
using PDF_IT_Yourself.Tools.Reorder;
using PDF_IT_Yourself.Tools.Split;
using PDF_IT_Yourself.Tools.PageNumbers;
using PDF_IT_Yourself.Tools.Watermark;
using PDF_IT_Yourself.Tools.CompressionPDF;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped<PdfInterop>();
builder.Services.AddScoped<PdfDocumentLoader>();
builder.Services.AddScoped<PdfPageOperations>();
builder.Services.AddScoped<PdfExport>();
builder.Services.AddScoped<PagePickerTool>();
builder.Services.AddScoped<ExtractTool>();
builder.Services.AddScoped<DeleteTool>();
builder.Services.AddScoped<ReorderTool>();
builder.Services.AddScoped<MergeTool>();
builder.Services.AddScoped<PageNumbersTool>();
builder.Services.AddScoped<SplitTool>();
builder.Services.AddScoped<WatermarkTool>();
builder.Services.AddScoped<CompressionTool>();
builder.Services.AddScoped<ConversionIMGtoPDFTool>();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

await builder.Build().RunAsync();
