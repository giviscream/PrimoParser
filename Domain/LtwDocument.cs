using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Domain
{
    public class LtwDocument
    {
        public SerializationRoot Root { get; set; }
        //to do: Correct Deserialization
        public static LtwDocument LoadFromXml(string xmlFilePath)
        {
            LtwDocument ltwDocument = new LtwDocument();

            XDocument xDoc = XDocument.Load(xmlFilePath);
            XElement? xRoot = xDoc.Root;

            if (xRoot == null)
            {
                throw new Exception("LtwDocument Root is not Found!");
            }

            if (xRoot.Name != "SerializationRoot")
            {
                throw new Exception("Element SerializationRoot is not found");
            }

            ltwDocument.Root = new SerializationRoot();
            ltwDocument.Root.WorkflowType = Convert.ToInt32(xRoot.Element("WorkflowType").Value);
            ltwDocument.Root.ScriptType = xRoot.Element("ScriptType").Value;
            ltwDocument.Root.UseArgs = Convert.ToBoolean(xRoot.Element("UseArgs").Value);
            ltwDocument.Root.IsTestCase = Convert.ToBoolean(xRoot.Element("IsTestCase").Value);

            ltwDocument.Root.RootContainer = SerializationComponent.ParseXml(xRoot.Element("RootContainer"));


            return ltwDocument;
        }
    }
}
