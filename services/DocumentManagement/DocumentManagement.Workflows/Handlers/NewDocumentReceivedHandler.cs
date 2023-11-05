using System.Threading;
using System.Threading.Tasks;
using DocumentManagement.Core.Events;
using Elsa.Models;
using Elsa.Workflows.Management.Contracts;
using Elsa.Workflows.Management.Filters;
using Elsa.Workflows.Runtime.Contracts;
using Elsa.Workflows.Runtime.Requests;
using MediatR;

namespace DocumentManagement.Workflows.Handlers;

public class NewDocumentReceivedHandler : INotificationHandler<NewDocumentReceived>
{
    private readonly IWorkflowDefinitionStore _workflowDefinitionStore;
    private readonly IWorkflowDispatcher _workflowDispatcher;


    public NewDocumentReceivedHandler(IWorkflowDefinitionStore workflowDefinitionStore, IWorkflowDispatcher workflowDispatcher)
    {
        _workflowDefinitionStore = workflowDefinitionStore;
        _workflowDispatcher = workflowDispatcher;
    }

    public async Task Handle(NewDocumentReceived notification, CancellationToken cancellationToken)
    {
        var documents = notification.Documents;

        var workflow = await _workflowDefinitionStore.FindAsync(new WorkflowDefinitionFilter
        {
            SearchTerm = "NewDocument"
        }, cancellationToken);

        if (workflow == null)
        {
            return; // Do nothing.
        }

        var docDict = new Dictionary<string, object> { { "docs", documents } };

        // Dispatch the workflow.
        await _workflowDispatcher.DispatchAsync(new DispatchWorkflowDefinitionRequest
        {
            Input = docDict,
            DefinitionId = workflow!.DefinitionId,
            CorrelationId = Guid.NewGuid().ToString()
        }, cancellationToken);
    }
}