using System.IO.Compression;
using Antlr4.Runtime.Misc;
using DocumentManagement.Core.Models;
using DocumentManagement.Core.Repositories.Interfaces;
using DocumentManagement.Core.Services.Interfaces;
using DocumentManagement.Persistence.DbContexts;
using Elsa.Extensions;
using Elsa.Workflows.Core;
using Elsa.Workflows.Core.Models;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Namotion.Reflection;

namespace DocumentManagement.Workflows.Activities;

public class MergeFiles : CodeActivity
{
    private IDocumentService _documentService;
    private IFileStorage _fileStorage;
    private IDocumentStore _documentStore;
    public IDictionary<string, object> Doc { get; set; } = default!;
    public List<Stream> Output { get; set; } = default!;
    public Stream MergedFile { get; set; } = default!;


    /*private IDocumentStore? _documentStore;
    private IFileStorage? _fileStorage;*/


    protected override async ValueTask ExecuteAsync(ActivityExecutionContext context)
    {
        Doc = context.Input;

        _documentStore = context.GetRequiredService<IDocumentStore>();
        _fileStorage = context.GetRequiredService<IFileStorage>();
        _documentService = context.GetRequiredService<IDocumentService>();

        var docs = (List<Document>)Doc["docs"];
        var listOfFiles = new List<Stream>();
        var batchId = string.Empty;
        var documentTypeId = string.Empty;
        var fileList = await _documentStore.GetDocsAsync(docs.First().BatchId, context.CancellationToken);
        foreach (var d in fileList.Where(d => d != null))
        {
            var fileStream = await _fileStorage.ReadAsync(d!.FileName, context.CancellationToken);
            batchId = d.BatchId;
            documentTypeId = d.DocumentTypeId;
            listOfFiles.Add(fileStream);
        }

        var mergefileName = $"{Guid.NewGuid()}.pdf";
        var outputStream = new FileStream(mergefileName, FileMode.OpenOrCreate, FileAccess.Write);
        MergePdfDocuments(listOfFiles, outputStream);
        foreach (var stream in listOfFiles)
        {
            stream.Close();
            stream.Dispose();
        }
        
        outputStream = new FileStream(mergefileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        
        var document = await _documentService.SaveDocumentAsync(batchId, mergefileName, outputStream, documentTypeId);
        outputStream.Close();
        outputStream.Dispose();
        
        var mergeDocument = new Document()
        {
            Id = document.Id,
            FileName = document.FileName,
            BatchId = document.BatchId,
            DocumentTypeId = document.DocumentTypeId,
            IsMerged = true
        };
        await _documentStore.SaveAsync(mergeDocument);

        // Close and dispose streams in the listOfFiles collection


        // Do not close or dispose outputStream here
        await context.CompleteActivityAsync("Done");
    }

    private void MergePdfDocuments(List<Stream> pdfStreams, Stream outputStream)
    {
        var mergedDocument = new iTextSharp.text.Document();
        var pdfCopy = new PdfCopy(mergedDocument, outputStream);
        mergedDocument.Open();

        foreach (var pdfStream in pdfStreams)
        {
            var pdfReader = new PdfReader(pdfStream);
            var pageCount = pdfReader.NumberOfPages;

            for (var pageIndex = 1; pageIndex <= pageCount; pageIndex++)
            {
                pdfCopy.AddPage(pdfCopy.GetImportedPage(pdfReader, pageIndex));
            }

            pdfReader.Close();
        }

        pdfCopy.Close();
        mergedDocument.Close();
    }
}