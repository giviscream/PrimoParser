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

            return serializationComponentChanges;
        }

        public override bool IsDifferent(SerializationComponent newVersion, SerializationComponent prevVersion)
        {
            throw new NotImplementedException();
        }
    }
}
