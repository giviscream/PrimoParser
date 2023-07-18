using Domain.DocumentComponents;
using Domain.DocumentComponentsChanges;
using Domain.DocumentsChanges;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Analyzers
{
    public class SerializationRootChangesAnalyzer : ChangesAnalyzer<SerializationRoot, SerializationRootChanges>
    {
        public override SerializationRootChanges GetChanges(SerializationRoot newVersion, SerializationRoot prevVersion)
        {
            SerializationRootChanges serializationRootChanges = new SerializationRootChanges();
            serializationRootChanges.WorkflowType = newVersion.WorkflowType;
            serializationRootChanges.IsTestCase = newVersion.IsTestCase;
            serializationRootChanges.UseArgs = newVersion.UseArgs;
            serializationRootChanges.ScriptType = newVersion.ScriptType;

            serializationRootChanges.RootContainer = new SerializationComponentChangesAnalyzer().GetChanges(newVersion.RootContainer, prevVersion.RootContainer);

            return serializationRootChanges;

        }

        public override bool IsDifferent(SerializationRoot newVersion, SerializationRoot prevVersion)
        {
            throw new NotImplementedException();
        }
    }
}
