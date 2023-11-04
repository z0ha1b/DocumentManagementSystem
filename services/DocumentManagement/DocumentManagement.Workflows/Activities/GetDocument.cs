using DocumentManagement.Core.Models;
using DocumentManagement.Core.Repositories.Interfaces;
using DocumentManagement.Core.Services.Interfaces;
using Elsa.Extensions;
using Elsa.Workflows.Core;
using Elsa.Workflows.Core.Activities.Flowchart.Models;

namespace DocumentManagement.Workflows.Activities;

public class GetDocument : CodeActivity
{
    private IDocumentStore? _documentStore;
    private IFileStorage? _fileStorage;

    public IDictionary<string, object> Doc { get; set; } = default!;
    public Stream Output { get; set; } = default!;

    protected override async ValueTask ExecuteAsync(ActivityExecutionContext context)
    {
        Doc = context.Input;

        var doc = (Document)Doc["doc"];

        _documentStore = context.GetRequiredService<IDocumentStore>();
        _fileStorage = context.GetRequiredService<IFileStorage>();

        var document = await _documentStore.GetAsync(doc.Id, context.CancellationToken);
        var fileStream = await _fileStorage.ReadAsync(document!.FileName, context.CancellationToken);


        Output = fileStream;

        await context.CompleteActivityAsync("Done");
    }
}