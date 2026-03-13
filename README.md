# PDF IT Yourself

PDF IT Yourself is a web application that allows users to manipulate PDF files directly in the browser.
All operations are performed locally on the client, meaning that no document is uploaded to a server.

Live application:
https://lemon-sea-02aeed810.6.azurestaticapps.net/

--------------------------------------------------
## Context

Most online PDF tools require users to upload their documents to remote servers before processing them.
This approach can create several issues:

- Privacy risks for sensitive documents
- Upload delays for large files
- Dependence on external infrastructure

PDF IT Yourself was created to provide a privacy-first alternative where PDF processing happens entirely in the browser.

--------------------------------------------------
## Objective

The goal of this project is to build a client-side PDF toolkit capable of performing common PDF operations
without sending files to a server.

The project combines:

- Blazor WebAssembly for the application UI and workflows
- JavaScript PDF libraries for document manipulation
- JS Interop to connect the C# and JavaScript layers

This architecture allows leveraging the strengths of both technologies.

--------------------------------------------------
## Features

The application currently supports:

- Extract pages from a PDF
- Merge multiple PDFs
- Delete pages
- Reorder pages
- Split PDFs by page ranges
- Add text watermarks
- Add page numbers
- Compress PDFs (lossless)
- Convert images to PDF
- Preview pages before processing

All operations run 100% locally in the browser.

--------------------------------------------------
## Architecture Overview

The application follows a layered architecture separating UI, services, and processing logic.
```text
┌──────────────────────────────────────────────┐
│ Blazor UI (Pages & Components)               │
│ User interface and interactions              │
└───────────────────────┬──────────────────────┘
                        │
                        ▼
┌──────────────────────────────────────────────┐
│ Application Tools                            │
│ Feature-specific workflows                   │
└───────────────────────┬──────────────────────┘
                        │
                        ▼
┌──────────────────────────────────────────────┐
│ Services Layer                               │
│ Validation, preview, export, page operations │
└───────────────────────┬──────────────────────┘
                        │
                        ▼
┌──────────────────────────────────────────────┐
│ JS Interop                                   │
│ Bridge between C# and JavaScript             │
└───────────────────────┬──────────────────────┘
                        │
                        ▼
┌──────────────────────────────────────────────┐
│ JavaScript Bundle (Vite)                     │
│ Compiled low-level PDF logic                 │
└───────────────────────┬──────────────────────┘
                        │
                        ▼
┌──────────────────────────────────────────────┐
│ PDF Libraries                                │
│ pdf-lib / pdfjs-dist / pdfcpu-wasm           │
└──────────────────────────────────────────────┘
```

Libraries used:
- pdf-lib
- pdfjs-dist
- pdfcpu-wasm

--------------------------------------------------
## JavaScript Processing Layer

Low-level PDF manipulation is implemented in JavaScript using specialized libraries.

Main libraries used:

- pdf-lib – editing and generating PDF files
- pdfjs-dist – rendering PDF pages for preview
- pdfcpu-wasm – additional PDF processing and compression

These libraries are bundled using Vite to produce a single JavaScript bundle used by the Blazor application.

--------------------------------------------------
## Blazor Application Layer

The Blazor application is responsible for:

- User interface
- User workflows
- Validation
- Calling JavaScript functions through JS Interop

The application uses several service layers to organize responsibilities such as:

- PDF validation
- Page operations
- Export and download
- Preview rendering

--------------------------------------------------
## Deployment

The application is deployed using Azure Static Web Apps.

Azure Static Web Apps provides:

- Static hosting for the Blazor WebAssembly application
- Global CDN distribution
- Simplified deployment

Live application:
https://lemon-sea-02aeed810.6.azurestaticapps.net/

--------------------------------------------------
## Privacy

A core principle of the project is privacy-first processing.

All PDF operations are executed locally inside the browser:

- No file upload
- No external processing
- No document storage

This ensures that users maintain full control over their documents.

--------------------------------------------------
## Author

Ibrahim Jallouli

This project explores how Blazor WebAssembly and modern JavaScript PDF libraries
can be combined to build a fully client-side PDF toolkit.
