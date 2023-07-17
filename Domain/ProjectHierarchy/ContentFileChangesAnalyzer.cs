using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ProjectHierarchy
{
    public class ContentFileChangesAnalyzer
    {
        public Guid Id { get; private init; }
        public string Name { get; set; }
        public string Path { get; set; }

        public string FullPath { get; set; }
        public ContentItemType ContentItemType { get; set; }

        public SysState SysState { get; set; } = SysState.None;

        public Guid ProjectVersionId { get; set; }

        public List<ContentFileChangesAnalyzer> ChildContent { get; set; }

        public ContentFileChangesAnalyzer()
        {
            Id = Guid.NewGuid();
        }
    }
}
