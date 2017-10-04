using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using RevitProhibitViewDeletions.Entities;
using RevitProhibitViewDeletions.Updaters;

namespace RevitProhibitViewDeletions
{
    public class RevitApplication : IExternalApplication
    {
        private readonly DocumentsCollection documents = new DocumentsCollection();
        private ViewDeletionUpdater viewDeletionUpdater;
        private ViewAdditionUpdater viewAdditionUpdater;

        public Result OnStartup(UIControlledApplication application)
        {
            CreateViewDeletionUpdater(application.ActiveAddInId);

            CreateViewAdditionUpdater(application.ActiveAddInId);

            application.ControlledApplication.DocumentOpened += ControlledApplicationDocumentOpened;
            application.ControlledApplication.DocumentCreated += ControlledApplicationOnDocumentCreated;
            application.ControlledApplication.DocumentClosing += ControlledApplicationOnDocumentClosing;
            application.ControlledApplication.DocumentSynchronizingWithCentral += ControlledApplicationOnDocumentSynchronizingWithCentral;
            application.ControlledApplication.DocumentSynchronizedWithCentral += ControlledApplicationOnDocumentSynchronizedWithCentral;

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            application.ControlledApplication.DocumentOpened -= ControlledApplicationDocumentOpened;
            application.ControlledApplication.DocumentCreated -= ControlledApplicationOnDocumentCreated;
            application.ControlledApplication.DocumentClosing -= ControlledApplicationOnDocumentClosing;
            application.ControlledApplication.DocumentSynchronizingWithCentral -= ControlledApplicationOnDocumentSynchronizingWithCentral;
            application.ControlledApplication.DocumentSynchronizedWithCentral -= ControlledApplicationOnDocumentSynchronizedWithCentral;

            return Result.Succeeded;
        }

        private void CreateViewDeletionUpdater(AddInId addInId)
        {
            viewDeletionUpdater = new ViewDeletionUpdater(addInId, documents);

            RegisterUpdater(viewDeletionUpdater, Element.GetChangeTypeElementDeletion());
        }

        private void CreateViewAdditionUpdater(AddInId addInId)
        {
            viewAdditionUpdater = new ViewAdditionUpdater(addInId, documents);

            RegisterUpdater(viewAdditionUpdater, Element.GetChangeTypeElementAddition());
        }

        private static void RegisterUpdater(IUpdater updater, ChangeType changeType)
        {
            UpdaterRegistry.RegisterUpdater(updater, false);

            UpdaterRegistry.AddTrigger(updater.GetUpdaterId(), new ElementClassFilter(typeof(View)), changeType);
        }

        private void ControlledApplicationOnDocumentCreated(object sender, DocumentCreatedEventArgs e)
        {
            documents.CreateCacheForDocument(e.Document);
        }

        private void ControlledApplicationDocumentOpened(object sender, DocumentOpenedEventArgs e)
        {
            documents.CreateCacheForDocument(e.Document);
        }

        private void ControlledApplicationOnDocumentSynchronizingWithCentral(object sender, DocumentSynchronizingWithCentralEventArgs e)
        {
            viewDeletionUpdater.IsSuspended = true;

            viewAdditionUpdater.IsSuspended = true;
        }

        private void ControlledApplicationOnDocumentSynchronizedWithCentral(object sender, DocumentSynchronizedWithCentralEventArgs e)
        {
            viewDeletionUpdater.IsSuspended = false;

            viewAdditionUpdater.IsSuspended = false;
        }

        private void ControlledApplicationOnDocumentClosing(object sender, DocumentClosingEventArgs e)
        {
            documents.RemoveCacheForDocument(e.Document);
        }
    }
}
