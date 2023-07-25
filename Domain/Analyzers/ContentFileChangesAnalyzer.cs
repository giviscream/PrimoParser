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
            changes.SetHeaderData(newVersion ?? prevVersion);
            changes.ChildContent = new List<ContentFileChanges>();

            var newChildren = newVersion?.ChildContent ?? new List<ContentFile>();
            var prevChildren = prevVersion?.ChildContent ?? new List<ContentFile>();

            var sameContentFiles = newChildren.Join(
                                    prevChildren,
                                    n => n.Path,
                                    p => p.Path,
                                    (n, p) => new { NewId = n.Id, PrevId = p.Id }

                                ).ToList();

            var newContentFiles = newChildren.Where(n => !sameContentFiles.Any(s => s.NewId == n.Id))
                                            .Select(x => new ContentFileChanges(x, SysState.New)).ToList()!;

            var delContentFiles = prevChildren.Where(p => !sameContentFiles.Any(s => s.PrevId == p.Id))
                                            .Select(x => new ContentFileChanges(x, SysState.Deleted)).ToList()!;

            changes.ChildContent.AddRange(
                    newContentFiles
                    .Concat(delContentFiles)
                );

            foreach (var cF in sameContentFiles)
            {
                ContentFile sameNew = newChildren.First(x => x.Id == cF.NewId);
                ContentFile samePrev = prevChildren.First(x => x.Id == cF.PrevId);

                var subChanges = new ContentFileChangesAnalyzer().GetChanges(sameNew, samePrev);

                bool isModified = Path.GetExtension(subChanges?.FullPath) == ".ltw" && IsDifferent(sameNew, samePrev);

                if (isModified)
                    subChanges.SysState = SysState.Modified;

                if (subChanges != null)
                    changes.ChildContent.Add(subChanges);

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
