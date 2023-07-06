using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ProjectHierarchy
{
    public enum ContentItemType
    {
        File,
        Folder
    }

    public class ContentFile
    {
        public Guid Id { get; private init; }
        public string Name { get; set; }
        public string Path { get; set; }

        public string FullPath => System.IO.Path.Combine(ProjectVersion.Path, Path);
        public ContentItemType ContentItemType { get; set; }

        public SysState SysState { get; set; } = SysState.None;

        public ProjectVersion ProjectVersion { get; set; }

        public List<ContentFile> ChildContent { get; set; }

        public ContentFile() { }

        public ContentFile(ProjectVersion projectVersion)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(projectVersion.Path);

            ContentItemType = ContentItemType.Folder;
            Name = directoryInfo.Name;
            ChildContent = new List<ContentFile>();
            Path = directoryInfo.FullName;
            ProjectVersion = projectVersion;

            this.ChildContent.AddRange(GetDirectoryContent(directoryInfo));
        }

        private List<ContentFile> GetDirectoryContent(DirectoryInfo directoryInfo)
        {
            List<ContentFile> content = new List<ContentFile>();

            foreach (DirectoryInfo subDirectoryInfo in directoryInfo.GetDirectories())
            {
                ContentFile subDirContent = new ContentFile()
                {
                    ContentItemType = ContentItemType.Folder,
                    Name = subDirectoryInfo.Name,
                    ChildContent = new List<ContentFile>(),
                    //Path = Uri.UnescapeDataString(new Uri(ProjectVersion.Path).MakeRelativeUri(new Uri(subDirectoryInfo.FullName)).ToString()).Replace('/', System.IO.Path.DirectorySeparatorChar),
                    Path = System.IO.Path.GetRelativePath(ProjectVersion.Path, subDirectoryInfo.FullName),
                    ProjectVersion = this.ProjectVersion
                };

                content.Add(subDirContent);
                subDirContent.ChildContent.AddRange(GetDirectoryContent(subDirectoryInfo));
            }

            //var filesContent = directoryInfo.GetFiles().Select(x => new ContentFile() { ContentItemType = ContentItemType.File, Name = x.Name, Path = Uri.UnescapeDataString(new Uri(ProjectVersion.Path).MakeRelativeUri(new Uri(x.FullName)).ToString().Replace('/', System.IO.Path.DirectorySeparatorChar)), ProjectVersion = this.ProjectVersion });
            var filesContent = directoryInfo.GetFiles().Select(x => new ContentFile() { ContentItemType = ContentItemType.File, Name = x.Name, Path = System.IO.Path.GetRelativePath(ProjectVersion.Path, x.FullName), ProjectVersion = this.ProjectVersion });
            content.AddRange(filesContent);

            return content;
        }

        public ContentFile GetDifferentContent(ContentFile baseContent)
        {
            ContentFile diffContent = this.MemberwiseClone() as ContentFile;
            diffContent.SetDifferences(baseContent);

            return diffContent;
        }

        public void SetDifferences(ContentFile baseContent)
        {
            this.MarkState(baseContent);
            this.SetDeleted(baseContent);
        }

        private void MarkState(ContentFile baseContent)
        {
            if (this.ChildContent == null)
                return;

            foreach (var childContent in this.ChildContent)
            {

                if (childContent.SysState != SysState.None)
                    continue;

                ContentFile? baseChildContent = baseContent.ChildContent?.FirstOrDefault(x => x.Path == childContent.Path);

                if (baseChildContent == null)
                {
                    childContent.SysState = SysState.New;
                    childContent.SetChildrenState();
                }
                else
                {
                    if ((System.IO.Path.GetExtension(childContent.Path).ToLower() == ".ltw")
                        && childContent.IsModifiedLtwDoc(baseChildContent))
                       childContent.SysState = SysState.Modified;

                    childContent.SetDifferences(baseChildContent);
                }

            }
        }

        private bool IsModifiedLtwDoc(ContentFile baseContent)
        {
           
            LtwDocument doc = LtwDocument.LoadFromXml(this.FullPath);
            LtwDocument baseDoc = LtwDocument.LoadFromXml(baseContent.FullPath);

            return doc.DocHash == baseDoc.DocHash;
        }

        private void SetDeleted(ContentFile baseContent)
        {
            if (baseContent.ChildContent == null)
                return;

            foreach (var baseChildContent in baseContent.ChildContent)
            {
                if (baseChildContent.SysState != SysState.None)
                    continue;

                ContentFile? childContent = this.ChildContent?.FirstOrDefault(x => x.Path == baseChildContent.Path);

                if (childContent == null)
                {
                    ContentFile newContent = baseChildContent.MemberwiseClone() as ContentFile;
                    newContent.SysState = SysState.Deleted;
                    newContent.SetChildrenState();

                    if (this.ChildContent == null)
                        this.ChildContent = new List<ContentFile> {};

                    this.ChildContent.Add(newContent);
                }
                else
                {
                    childContent.SetDifferences(baseChildContent);
                }

            }
        }

        private void SetChildrenState()
        {
            if (this.ChildContent == null)
                return;

            foreach (var childContent in this.ChildContent)
            {
                childContent.SysState = this.SysState;

                childContent.SetChildrenState();
            }
        }

    }
}
