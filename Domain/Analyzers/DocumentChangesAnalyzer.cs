using Domain.Documents;
using Domain.DocumentsChanges;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Analyzers
{
    public class DocumentChangesAnalyzer : ChangesAnalyzer<Document, DocumentChanges>
    {
        public override DocumentChanges GetChanges(Document newVersion, Document prevVersion)
        {
            DocumentChanges documentChanges = new DocumentChanges();

            documentChanges.Root = new SerializationRootChangesAnalyzer().GetChanges(newVersion.Root, prevVersion.Root);

            return documentChanges;
        }

        public override bool IsDifferent(Document newVersion, Document prevVersion)
        {
            return this.GetHash(newVersion.OriginalValue) != this.GetHash(newVersion.OriginalValue);
        }
    }
}
