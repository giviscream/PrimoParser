using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Domain.Components;
using Domain.DocumentComponentsChanges;

namespace Domain.DocumentComponents
{
    public class SerializationRoot
    {
        public int WorkflowType { get; set; }
        public bool IsTestCase { get; set; }
        public bool UseArgs { get; set; }
        public string ScriptType { get; set; }
        public SerializationComponent RootContainer { get; set; }

        public static SerializationRoot GetFromXml(XElement root)
        {
            SerializationRoot serializationRoot = new SerializationRoot();
            serializationRoot.WorkflowType = Convert.ToInt32(root.Element("WorkflowType").Value);
            serializationRoot.ScriptType = root.Element("ScriptType").Value;
            serializationRoot.UseArgs = Convert.ToBoolean(root.Element("UseArgs").Value);
            serializationRoot.IsTestCase = Convert.ToBoolean(root.Element("IsTestCase").Value);

            serializationRoot.RootContainer = SerializationComponent.ParseXml(root.Element("RootContainer"));

            return serializationRoot;
        }
    }
}
