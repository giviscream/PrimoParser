using Domain.Components;
using Domain.DocumentComponents;
using Domain.DocumentComponentsChanges;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Analyzers
{
    public class SerializationComponentChangesAnalyzer : ChangesAnalyzer<SerializationComponent, SerializationComponentChanges>
    {
        public override SerializationComponentChanges GetChanges(SerializationComponent newVersion, SerializationComponent prevVersion)
        {
            SerializationComponentChanges serializationComponentChanges = new SerializationComponentChanges();
            serializationComponentChanges.AssemblyName = newVersion.AssemblyName;
            serializationComponentChanges.ClassName = newVersion.ClassName;

            serializationComponentChanges.Properties = newVersion.Properties.Join(prevVersion.Properties, 
                                                            n => n.Name, 
                                                            p => p.Name, 
                                                            (n, p) => new SerializationItemChanges()
                                                            {
                                                                Name = n.Name,
                                                                IsListValue= n.IsListValue,
                                                                NewValue = n.Value,
                                                                OldValue = p.Value
                                                            }).ToList();
            serializationComponentChanges.SysState = serializationComponentChanges.Properties.Any(x => x.NewValue != x.OldValue) ? SysState.Modified : SysState.None;

            

            return serializationComponentChanges;
        }

        private List<SerializationComponentChanges> MapSerializationComponents(List<SerializationComponent> newVersionCpmponents, List<SerializationComponent> oldVersionCpmponents)
        {
            List<SerializationComponentChanges> serializationComponentChanges = new List<SerializationComponentChanges>();

            return serializationComponentChanges;
        }

        public override bool IsDifferent(SerializationComponent newVersion, SerializationComponent prevVersion)
        {
            throw new NotImplementedException();
        }
    }
}
