using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CoderCloud.Core
{
    [DataContract]
    class Namespace
    {
        [DataMember(Name = "name")]
        private string _name = "";

        [DataMember(Name = "namespaces")]
        private List<Namespace> _namespaces = new List<Namespace>();
        [DataMember(Name = "components")]
        private List<Component> _components = new List<Component>();
    }
}
