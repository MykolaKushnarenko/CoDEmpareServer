using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace COURCEClientServer2.ObjectDataSender
{
    public class ResultCompareObject
    {
        public string MainCodeText { get; set; }
        public string ChildCodeText { get; set; }
        public List<string> ResultCompare { get; set; } = new List<string>();
        public bool IsLocalCompare { get; set; }
        public List<string> TokkingMainCode { get; set; }
        public List<string> TokkingChildCode { get; set; }
        public ResultCompareObject() { }


    }
}