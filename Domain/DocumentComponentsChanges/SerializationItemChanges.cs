﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DocumentComponentsChanges
{
    public class SerializationItemChanges
    {
        public string Name { get; set; }
        public bool IsListValue { get; set; }
        public string Value { get; set; }
    }
}