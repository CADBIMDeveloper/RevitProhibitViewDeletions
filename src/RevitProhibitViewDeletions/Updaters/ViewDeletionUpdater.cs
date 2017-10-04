using System;
using System.Linq;
using Autodesk.Revit.DB;
using RevitProhibitViewDeletions.Entities;

namespace RevitProhibitViewDeletions.Updaters
{
    public class ViewDeletionUpdater : IUpdater
    {
        private readonly DocumentsCollection documents;
        private readonly UpdaterId updaterId;
        private readonly FailureDefinitionId viewDeletionProhibitedFailureId = new FailureDefinitionId(new Guid("264cbde1-fdbb-4637-9c37-7a46d316c02c"));

        public ViewDeletionUpdater(AddInId addInId, DocumentsCollection documents)
        {
            this.documents = documents;
            updaterId = new UpdaterId(addInId, new Guid("cca7e63d-3ed1-4a2c-ba3b-cee4f313c177"));

            FailureDefinition.CreateFailureDefinition(viewDeletionProhibitedFailureId, FailureSeverity.Error,
                "View deletion is prohibited");
        }

        public bool IsSuspended { get; set; }

        public void Execute(UpdaterData data)
        {
            if (IsSuspended)
                return;

            var document = data.GetDocument();

            var viewCache = documents.GetCacheForDocument(document);

            if (data.GetDeletedElementIds().Any(viewCache.Contains))
                document.PostFailure(new FailureMessage(viewDeletionProhibitedFailureId));
        }

        public UpdaterId GetUpdaterId() => updaterId;
        
        public ChangePriority GetChangePriority()
        {
            return ChangePriority.Views;
        }

        public string GetUpdaterName() => "View deletion prohibited updater";

        public string GetAdditionalInformation() => "Prohibits view deletion";
    }
}