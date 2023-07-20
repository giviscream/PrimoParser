using System.Xml.Linq;
using Domain.DocumentComponents;

namespace Domain.Components
{
    public class SerializationComponent
    {
        public string ClassName { get; set; }
        public string AssemblyName { get; set; }

        public Guid SysID { get; set; }

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
                serializationComponent.Components = new List<SerializationComponent>();

                switch (subElementXml.Name.LocalName)
                {
                    case "ClassName":
                        serializationComponent.ClassName = subElementXml.Value;
                        break;
                    case "AssemblyName":
                        serializationComponent.AssemblyName = subElementXml.Value;
                        break;
                    case "Properties":
                        serializationComponent.Properties = new List<SerializationItem>();

                        foreach (XElement propXml in subElementXml.Elements("SerializationItem"))
                        {
                            SerializationItem serializationItem = SerializationItem.ParseXml(propXml);

                            serializationComponent.Properties.Add(serializationItem);

                            if (serializationItem.Name == "ComponentID")
                                serializationComponent.SysID = Guid.Parse(serializationItem.Value);
                        }
                        break;
                    case "Components":
                        foreach (XElement componentXml in subElementXml.Elements("SerializationComponent"))
                        {
                            SerializationComponent subComponent = ParseXml(componentXml);

                            serializationComponent.Components.Add(subComponent);
                        }
                        break;
                }


            }

            return serializationComponent;
        }

    }
}
