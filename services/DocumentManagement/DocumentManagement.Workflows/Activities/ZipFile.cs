using System.IO;
using System.IO.Compression;
using DocumentManagement.Core.Models;
using DocumentManagement.Core.Repositories.Interfaces;
using DocumentManagement.Core.Services.Interfaces;
using Elsa.Extensions;
using Elsa.Workflows.Core;

namespace DocumentManagement.Workflows.Activities;

public class ZipFile : CodeActivity
{
    private IDocumentStore? _documentStore;
    private IFileStorage? _fileStorage;
    private IDocumentService? _documentService;

    public IDictionary<string, object> Doc { get; set; } = default!;
    public Stream Output { get; set; } = default!;

    protected override async ValueTask ExecuteAsync(ActivityExecutionContext context)
    {
        Doc = context.Input;

        var doc = (Document)Doc["doc"];

        _documentStore = context.GetRequiredService<IDocumentStore>();
        _fileStorage = context.GetRequiredService<IFileStorage>();
        _documentService = context.GetRequiredService<IDocumentService>();

        var document = await _documentStore.GetAsync(doc.Id, context.CancellationToken);
        var fileStream = await _fileStorage.ReadAsync(document!.FileName, context.CancellationToken);

        using (var outputStream = new MemoryStream())
        {
            using (var zipArchive = new ZipArchive(outputStream, ZipArchiveMode.Create, true))
            {
                var zipEntry = zipArchive.CreateEntry(doc.FileName, CompressionLevel.Optimal);

                await using (var zipStream = zipEntry.Open())
                {
                    await fileStream.CopyToAsync(zipStream);
                }
            }

            outputStream.Seek(0, SeekOrigin.Begin);

            Output = outputStream;
        }

        var zippedDoc = await _documentService.SaveDocumentAsync(doc.FileName, Output, doc.DocumentTypeId, context.CancellationToken);

        await context.CompleteActivityAsync("Done");
    }
}