using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ProjectHierarchy
{
    public class ContentFileChanges
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }

        public string FullPath { get; set; }
        public ContentItemType ContentItemType { get; set; }

        public SysState SysState { get; set; } = SysState.None;

        public Guid ProjectVersionId { get; set; }

        public List<ContentFileChanges> ChildContent { get; set; }

        public ContentFileChanges() { }

        public ContentFileChanges(ContentFile contentFile)
        {
            SetHeaderData(contentFile);

            if (contentFile.ChildContent != null)
                SetChildrenData(contentFile);
        }

        public ContentFileChanges(ContentFile contentFile, SysState derivedState)
        {
            SetHeaderData(contentFile, derivedState);

            if (contentFile.ChildContent != null)
                SetChildrenData(contentFile, derivedState);
        }

        public void SetHeaderData(ContentFile contentFile, SysState derivedState = SysState.None)
        {
            this.Id = contentFile.Id;
            this.Name = contentFile.Name;
            this.Path = contentFile.Path;
            this.FullPath = contentFile.FullPath;
            this.ContentItemType = contentFile.ContentItemType;
            this.ProjectVersionId = contentFile.ProjectVersion.Id;

            this.SysState = derivedState;
        }
        public void SetChildrenData(ContentFile contentFile, SysState derivedState = SysState.None)
        {
            this.ChildContent = new List<ContentFileChanges>();
            this.ChildContent.AddRange(
                contentFile.ChildContent.Select(x => new ContentFileChanges(x) {SysState = derivedState })
            );
        }
    }
}
