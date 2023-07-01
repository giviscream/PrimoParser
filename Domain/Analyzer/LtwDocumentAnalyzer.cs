using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Analyzer
{
    internal class LtwDocumentAnalyzer
    {
        private readonly LtwDocument _ltwDocument;

        public LtwDocumentAnalyzer(LtwDocument ltwDocument)
        {
            _ltwDocument = ltwDocument;
        }

        public LtwDocument GetLtwVersionDifferences(LtwDocument newVersionLtwDocument)
        {
            LtwDocument diffDocument = newVersionLtwDocument.Clone() as LtwDocument;

            if (diffDocument.DocHash == newVersionLtwDocument.DocHash)
                return diffDocument;

            SerializationComponent baseSrComponent = _ltwDocument.Root.RootContainer;
            SerializationComponent newSrComponent = diffDocument.Root.RootContainer;



            return diffDocument;
        }
    }
}
