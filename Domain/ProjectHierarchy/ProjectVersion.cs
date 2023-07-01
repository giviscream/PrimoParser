using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ProjectHierarchy
{
    public class ProjectVersion
    {
        public Guid Id { get; set; }
        public int Num { get; set; }

        public string Path { get; set; }

        public DateTime DateTime { get; set; }

        public Project Project { get; set; }

    }
}
