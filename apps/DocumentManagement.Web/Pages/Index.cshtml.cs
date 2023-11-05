using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DocumentManagement.Core.Events;
using DocumentManagement.Core.Models;
using DocumentManagement.Core.Repositories.Interfaces;
using DocumentManagement.Core.Services;
using DocumentManagement.Core.Services.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Open.Linq.AsyncExtensions;

namespace DocumentManagement.Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IDocumentTypeStore _documentTypeStore;
        private readonly IDocumentService _documentService;
        private readonly IMediator _mediator;

        public IndexModel(IDocumentTypeStore documentTypeStore, IDocumentService documentService, IMediator mediator)
        {
            _documentTypeStore = documentTypeStore;
            _documentService = documentService;
            _mediator = mediator;
        }

        [BindProperty] public string DocumentTypeId { get; set; } = default!;
        [BindProperty] public List<IFormFile> FileUploads { get; set; } = default!;

        public ICollection<SelectListItem> DocumentTypes { get; set; } = new List<SelectListItem>();

        public async Task OnGetAsync(CancellationToken cancellationToken)
        {
            var documentTypes = await _documentTypeStore.ListAsync(cancellationToken).ToList();
            DocumentTypes = documentTypes.Select(x => new SelectListItem(x.Name, x.Id)).ToList();
        }

        public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
        {
            /*var extension = Path.GetExtension(FileUpload.FileName);
            var fileName = $"{Guid.NewGuid()}{extension}";
            var fileStream = FileUpload.OpenReadStream();
            var document = await _documentService.SaveDocumentAsync(fileName, fileStream, DocumentTypeId, cancellationToken);*/
            var fileGroup = Guid.NewGuid().ToString();
            var listOfDocuments = new List<Document>();
            foreach (var file in FileUploads)
            {
                if (file.Length > 0)
                {
                    var extension = Path.GetExtension(file.FileName);
                    var fileName = $"{Guid.NewGuid()}{extension}";
                    var document = await _documentService.SaveDocumentAsync(fileGroup, fileName, file.OpenReadStream(),
                        DocumentTypeId, cancellationToken);
                    listOfDocuments.Add(document);
                    
                }
            }
            
            await _mediator.Publish(new NewDocumentReceived(listOfDocuments), cancellationToken);

            return RedirectToPage("FileReceived", new { DocumentId = fileGroup.ToString() });
        }
    }
}