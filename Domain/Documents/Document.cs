using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Domain.Components;
using Domain.DocumentComponents;

namespace Domain.Documents
{
    public class Document : ICloneable
    {
        public SerializationRoot Root { get; private set; }
        public string OriginalValue { get; private set; }

        //to do: Correct Deserialization
        public static Document LoadFromXml(string xmlFilePath)
        {

            Document document = new Document();
            XElement? xRoot = null;

            using (StreamReader reader = new StreamReader(xmlFilePath, true))
            {
                XDocument xDoc = XDocument.Load(reader);
                xRoot = xDoc.Root;
            }

            if (xRoot == null)
            {
                throw new Exception("Document Root is not Found!");
            }

            if (xRoot.Name != "SerializationRoot")
            {
                throw new Exception("Element SerializationRoot is not found");
            }

            document.OriginalValue = xRoot.Document.ToString();

            document.Root = SerializationRoot.GetFromXml(xRoot);

            return document;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
