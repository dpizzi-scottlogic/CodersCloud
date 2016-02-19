using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CoderCloud.Core
{
    [DataContract]
    class Component
    {
        [DataMember(Name = "name")]
        private string _name = "";

        [DataMember(Name = "interfaces")]
        private List<string> _interfaces = new List<string>();
        [DataMember(Name = "code")]
        private string _code = "";
    }
}
