using Application.Analyzers;
using Domain.DocumentComponents;
using Domain.DocumentComponentsChanges;
using Domain.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Domain.DocumentsChanges
{
    public class DocumentChanges
    {
        public SerializationRootChanges Root { get; set; }
        public SysState SysState { get; set; } = SysState.None;

    }
}
