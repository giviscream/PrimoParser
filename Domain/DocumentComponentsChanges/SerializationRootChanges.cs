using Domain.Components;
using Domain.DocumentComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DocumentComponentsChanges
{
    public class SerializationRootChanges
    {
        public int WorkflowType { get; set; }
        public bool IsTestCase { get; set; }
        public bool UseArgs { get; set; }
        public string ScriptType { get; set; }
        public SysState SysState { get; set; } = SysState.None;
        public SerializationComponentChanges RootContainer { get; set; }

    }
}
