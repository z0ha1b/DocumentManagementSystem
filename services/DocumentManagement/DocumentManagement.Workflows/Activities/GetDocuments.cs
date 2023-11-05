using DocumentManagement.Core.Models;
using DocumentManagement.Core.Repositories.Interfaces;
using DocumentManagement.Core.Services.Interfaces;
using Elsa.Extensions;
using Elsa.Workflows.Core;

namespace DocumentManagement.Workflows.Activities;

public class GetDocuments : CodeActivity
{
    private IDocumentStore? _documentStore;
    private IFileStorage? _fileStorage;

    public IDictionary<string, object> Doc { get; set; } = default!;
    public List<Stream> Output { get; set; } = default!;

    protected override async ValueTask ExecuteAsync(ActivityExecutionContext context)
    {
        Doc = context.Input;

        var docs = (List<Document>)Doc["docs"];
        var listOfFileStreams = new List<Stream>();

        _documentStore = context.GetRequiredService<IDocumentStore>();
        _fileStorage = context.GetRequiredService<IFileStorage>();

        var documents = await _documentStore.GetDocsAsync(docs.First().BatchId, context.CancellationToken);
        foreach (var d in documents.Where(d => d != null))
        {
            var fileStream = await _fileStorage.ReadAsync(d!.FileName, context.CancellationToken);
            listOfFileStreams.Add(fileStream);
        }

        Output = listOfFileStreams;

        // we should not save streams in the context but it is only POC.
        context.SetVariable("docs", Output);

        await context.CompleteActivityAsync("Done");
    }
}