using Domain.ProjectHierarchy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ProjectVersions
{
    public class ProjectVersionDto
    {
        public Guid Id { get; set; }
        public int Num { get; set; }

        public string Path { get; set; }

        public DateTime DateTime { get; set; }

        public Guid ProjectId { get; set; }
    }
}
