using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Components;

namespace Domain.Analyzer
{
    internal class LtwDocumentAnalyzer
    {
        private readonly Document _ltwDocument;

        public LtwDocumentAnalyzer(Document ltwDocument)
        {
            _ltwDocument = ltwDocument;
        }

        public Document GetLtwVersionDifferences(Document newVersionLtwDocument)
        {
            Document diffDocument = newVersionLtwDocument.Clone() as Document;

            if (diffDocument.DocHash == newVersionLtwDocument.DocHash)
                return diffDocument;

            SerializationComponent baseSrComponent = _ltwDocument.Root.RootContainer;
            SerializationComponent newSrComponent = diffDocument.Root.RootContainer;



            return diffDocument;
        }
    }
}
