using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Domain
{
    public class LtwDocument : ICloneable
    {
        public SerializationRoot Root { get; private set; }

        public string DocHash { get; private set; }

        //to do: Correct Deserialization
        public static LtwDocument LoadFromXml(string xmlFilePath)
        {

            LtwDocument ltwDocument = new LtwDocument();
            XElement? xRoot = null;

            using (StreamReader reader = new StreamReader(xmlFilePath, true))
            {
                XDocument xDoc = XDocument.Load(reader);
                xRoot = xDoc.Root;
            }

            if (xRoot == null)
            {
                throw new Exception("LtwDocument Root is not Found!");
            }

            if (xRoot.Name != "SerializationRoot")
            {
                throw new Exception("Element SerializationRoot is not found");
            }

            ltwDocument.SetDocHash(xRoot.Document.ToString());

            ltwDocument.Root = new SerializationRoot();
            ltwDocument.Root.WorkflowType = Convert.ToInt32(xRoot.Element("WorkflowType").Value);
            ltwDocument.Root.ScriptType = xRoot.Element("ScriptType").Value;
            ltwDocument.Root.UseArgs = Convert.ToBoolean(xRoot.Element("UseArgs").Value);
            ltwDocument.Root.IsTestCase = Convert.ToBoolean(xRoot.Element("IsTestCase").Value);

            ltwDocument.Root.RootContainer = SerializationComponent.ParseXml(xRoot.Element("RootContainer"));


            return ltwDocument;
        }

        private void SetDocHash(string input)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                // Compute hash from the input data
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = sha256.ComputeHash(inputBytes);

                // Convert the hash bytes to a hexadecimal string
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    builder.Append(hashBytes[i].ToString("x2"));
                }

                DocHash = builder.ToString();
            }
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
