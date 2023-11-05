using DocumentManagement.Core.Models;
using MediatR;

namespace DocumentManagement.Core.Events
{
    public record NewDocumentReceived(List<Document> Documents) : INotification;
}