using System.Collections.Generic;
using Autodesk.Revit.DB;

namespace RevitProhibitViewDeletions.Entities
{
    public class DocumentViewsCache
    {
        private readonly Document document;
        private readonly HashSet<ElementId> viewsIds = new HashSet<ElementId>(); 

        public DocumentViewsCache(Document document)
        {
            this.document = document;

            Refresh();
        }

        public bool IsValid => document.IsValidObject;

        public bool IsDocumentCache(Document doc)
        {
            return IsValid && document.Equals(doc);
        }

        public bool Contains(ElementId viewId)
        {
            return viewsIds.Contains(viewId);
        }

        public void Refresh()
        {
            viewsIds.Clear();

            var collector = new FilteredElementCollector(document);

            var views = collector.OfClass(typeof (View));

            foreach (var view in views)
                viewsIds.Add(view.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DocumentViewsCache) obj);
        }

        public override int GetHashCode()
        {
            return document?.GetHashCode() ?? 0;
        }

        protected bool Equals(DocumentViewsCache other)
        {
            return Equals(document, other.document);
        }
    }
}