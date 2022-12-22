using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class SerializationRoot
    {
        public int WorkflowType { get; set; }
        public bool IsTestCase { get; set; }
        public bool UseArgs { get; set; }
        public string ScriptType { get; set; }
        public SerializationComponent RootContainer { get; set; }
    }
}
