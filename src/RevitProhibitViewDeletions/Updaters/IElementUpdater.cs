using Autodesk.Revit.DB;

namespace RevitProhibitViewDeletions.Updaters
{
    public interface IElementUpdater : IUpdater
    {
        bool IsSuspended { get; set; }
    }
}