using System.IO.Compression;
using DocumentManagement.Core.Models;
using DocumentManagement.Core.Repositories.Interfaces;
using DocumentManagement.Core.Services.Interfaces;
using Elsa.Extensions;
using Elsa.Workflows.Core;
using Elsa.Workflows.Core.Models;
using iTextSharp.text.pdf;
using Namotion.Reflection;

namespace DocumentManagement.Workflows.Activities;

public class MergeFiles : CodeActivity
{
    /*private IDocumentStore? _documentStore;
    private IFileStorage? _fileStorage;*/

    public IDictionary<string, object> Doc { get; set; } = default!;
    public List<Stream> Output { get; set; } = default!;
    public Stream MergedFile { get; set; } = default!;

    protected override async ValueTask ExecuteAsync(ActivityExecutionContext context)
    {
        Doc = context.Input;

        // var doc = (Document)Doc["doc"];
        //
        // _documentStore = context.GetRequiredService<IDocumentStore>();
        // _fileStorage = context.GetRequiredService<IFileStorage>();
        //
        // var document = await _documentStore.GetAsync(doc.Id, context.CancellationToken);
        // var fileStream = await _fileStorage.ReadAsync(document!.FileName, context.CancellationToken);

        // Output = fileStream;


        var data = context.GetVariable<List<Stream>>("docs");

        // Output = data;
        using (Stream outputStream = new FileStream("merged.pdf", FileMode.Create, FileAccess.Write))
        {
            MergePdfDocuments(data, outputStream);
            Output = new List<Stream>();
            Output.Add(outputStream);

        } // Close and dispose the input streams        foreach (Stream stream in pdfStreams)         {             stream.Close();             stream.Dispose();         }



        context.SetVariable("docs", Output);

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