using DocumentManagement.Core.Models;
using MediatR;

namespace DocumentManagement.Core.Events
{
    public record NewDocumentReceived(Document Document) : INotification;
}