using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;

namespace RevitProhibitViewDeletions.Entities
{
    public class DocumentsCollection
    {
        private readonly IList<DocumentViewsCache> documentViewsCaches = new List<DocumentViewsCache>();

        public DocumentViewsCache GetCacheForDocument(Document document)
        {
            return documentViewsCaches
                .SingleOrDefault(x => x.IsDocumentCache(document));
        }

        public DocumentViewsCache CreateCacheForDocument(Document document)
        {
            var existing = GetCacheForDocument(document);

            if (existing != null) return existing;

            var newDocumentViewsCache = new DocumentViewsCache(document);

            documentViewsCaches.Add(newDocumentViewsCache);

            return newDocumentViewsCache;
        }

        public void RemoveCacheForDocument(Document document)
        {
            var documentViewsCache = GetCacheForDocument(document);

            if (documentViewsCache != null)
                documentViewsCaches.Remove(documentViewsCache);
        }
    }
}