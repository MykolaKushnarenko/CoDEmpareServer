using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace COURCEClientServer2
{
    public class AddingCodeObject
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string CompileType { get; set; }
        public bool IsSearch { get; set; }
        public string FileMane { get; set; }
        public byte[] Code { get; set; }
    }
}