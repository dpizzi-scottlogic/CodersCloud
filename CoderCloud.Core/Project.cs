using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CoderCloud.Core
{
    [DataContract]
    class Project
    {
        [DataMember(Name = "name")]
        private string _name = "";
        [DataMember(Name = "creation")]
        private DateTime _creation = DateTime.Now;

        [DataMember(Name = "namespaces")]
        private List<Namespace> _namespaces = new List<Namespace>();
    }
}
