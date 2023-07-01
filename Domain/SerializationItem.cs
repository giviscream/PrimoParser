using System.Xml.Linq;

namespace Domain
{
    public class SerializationItem
    {
        public string Name { get; set; }
        public bool IsListValue { get; set; }
        public string Value { get; set; }

        public static SerializationItem ParseXml(XElement xElement)
        {
            bool isFullSerializationItem = xElement.Name.LocalName == "SerializationItem" && xElement.HasElements;

            if (!isFullSerializationItem)
            {
                return null;
            }

            SerializationItem serializationItem = new SerializationItem();
            serializationItem.Name = xElement.Element("Name")?.Value;
            serializationItem.IsListValue = Convert.ToBoolean(xElement.Element("IsListValue")?.Value);

            XElement? itemValXml = xElement.Element("Value");

            if (itemValXml == null)
            {
                return serializationItem;
            }

            serializationItem.Value = itemValXml.Value;

            return serializationItem;
        }

    }


}
