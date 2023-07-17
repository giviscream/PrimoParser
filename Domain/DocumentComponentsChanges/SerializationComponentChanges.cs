using Domain.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DocumentComponentsChanges
{
    public class SerializationComponentChanges
    {
        public string ClassName { get; set; }
        public string AssemblyName { get; set; }

        public Guid SysID { get; set; }

        public List<SerializationItemChanges> Properties { get; set; }

        public List<SerializationComponentChanges> Components { get; set; }
    }
}
