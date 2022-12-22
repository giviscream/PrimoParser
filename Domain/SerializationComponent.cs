using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Domain
{
    public class SerializationComponent
    {
        public string ClassName { get; set; }
        public string AssemblyName { get; set; }

        public List<SerializationItem> Properties { get; set; }

        public List<SerializationComponent> Components { get; set; }

        public static SerializationComponent ParseXml(XElement xElement)
        {
            bool isFullContainer = (xElement.Name.LocalName == "RootContainer" || xElement.Name.LocalName == "SerializationComponent")
                                    && xElement.HasElements;

            if (!isFullContainer)
            {
                return null;
            }

            SerializationComponent serializationComponent = new SerializationComponent();

            foreach (XElement subElementXml in xElement.Elements())
            {
                if (subElementXml.Name.LocalName == "ClassName")
                {
                    serializationComponent.ClassName = subElementXml.Value;
                }
                else if (subElementXml.Name.LocalName == "AssemblyName")
                {
                    serializationComponent.AssemblyName = subElementXml.Value;
                }
                else if (subElementXml.Name.LocalName == "Properties")
                {
                    serializationComponent.Properties = new List<SerializationItem>();

                    foreach (XElement propXml in subElementXml.Elements("SerializationItem"))
                    {
                        SerializationItem serializationItem = SerializationItem.ParseXml(propXml);

                        serializationComponent.Properties.Add(serializationItem);
                    }
                }
                else if (subElementXml.Name.LocalName == "Components")
                {
                    serializationComponent.Components = new List<SerializationComponent>();

                    foreach (XElement componentXml in subElementXml.Elements("SerializationComponent"))
                    {
                        SerializationComponent subComponent = SerializationComponent.ParseXml(componentXml);

                        serializationComponent.Components.Add(subComponent);
                    }
                }
            }

            return serializationComponent;
        }

    }
}
