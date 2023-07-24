using Domain.Documents;
using Domain.ProjectHierarchy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Analyzers
{
    public class ContentFileChangesAnalyzer : ChangesAnalyzer<ContentFile, ContentFileChanges>
    {
        public override ContentFileChanges GetChanges(ContentFile newVersion, ContentFile prevVersion)
        {
            ContentFileChanges changes = new ContentFileChanges();
            changes.SetHeaderData(prevVersion);
            changes.ChildContent = new List<ContentFileChanges>();

            var newChildren = newVersion.ChildContent;
            var prevChildren = prevVersion.ChildContent;

            var sameContentFiles = newChildren.Join(
                                        prevChildren,
                                        n => n.Path,
                                        p => p.Path,
                                        (n, p) => new ContentFileChanges(n) { SysState = IsDifferent(n, p) ? SysState.Modified : SysState.New}
                                    );
            var newContentFiles = newChildren.Where(n => !sameContentFiles.Any(s => s.Id == n.Id)).Select(x => new ContentFileChanges(x, SysState.New));
            var delContentFiles = prevChildren.Where(p => !sameContentFiles.Any(s => s.Id == p.Id)).Select(x => new ContentFileChanges(x, SysState.Deleted));

            changes.ChildContent.AddRange(
                    newContentFiles
                    .Concat(delContentFiles)
                );

            foreach (var contentFile in sameContentFiles)
            {
                ContentFile sameNew = newChildren.First(x => x.Id == contentFile.Id);
                ContentFile samePrev = prevChildren.First(x => x.Id == contentFile.Id);

                changes.ChildContent.Add(new ContentFileChangesAnalyzer().GetChanges(sameNew, samePrev));
            }

            return changes;
        }

        public override bool IsDifferent(ContentFile newVersion, ContentFile prevVersion)
        {
            Document newVersionDoc = Document.LoadFromXml(newVersion.FullPath);
            Document prevVersionDoc = Document.LoadFromXml(prevVersion.FullPath);

            DocumentChangesAnalyzer documentChangesAnalyzer = new DocumentChangesAnalyzer();


            return documentChangesAnalyzer.IsDifferent(newVersionDoc, prevVersionDoc);
        }
    }
}
