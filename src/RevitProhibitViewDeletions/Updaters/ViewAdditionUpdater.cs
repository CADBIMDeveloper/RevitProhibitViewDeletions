using System;
using Autodesk.Revit.DB;
using RevitProhibitViewDeletions.Entities;

namespace RevitProhibitViewDeletions.Updaters
{
    public class ViewAdditionUpdater : IUpdater
    {
        private readonly DocumentsCollection documents;
        private readonly UpdaterId updaterId;

        public ViewAdditionUpdater(AddInId addInId, DocumentsCollection documents)
        {
            updaterId = new UpdaterId(addInId, new Guid("1689f432-66a8-4cd4-b9ec-1af60c2654bc"));

            this.documents = documents;
        }

        public bool IsSuspended { get; set; }

        public void Execute(UpdaterData data)
        {
            if (IsSuspended)
                return;

            var document = data.GetDocument();

            var viewCache = documents.GetCacheForDocument(document);

            viewCache.Refresh();
        }

        public UpdaterId GetUpdaterId() => updaterId;

        public ChangePriority GetChangePriority() => ChangePriority.Views;

        public string GetUpdaterName() => "Views addition updater";

        public string GetAdditionalInformation() => "Tracks views addition";
    }
}