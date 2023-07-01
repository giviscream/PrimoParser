using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ProjectHierarchy
{
    public class Project
    {
        public Guid Id { get; private init; }
        public string Name { get; set; }
        public DateTime DateTime { get; set; }

    }
}
