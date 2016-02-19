using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CoderCloud.Core
{
    [DataContract]
    class Solution
    {
        [DataMember(Name ="name")]
        private string _name = "";
        [DataMember(Name = "creation")]
        private DateTime _creation = DateTime.Now;

        [DataMember(Name = "environments")]
        private List<Environment> _environments = new List<Environment>();
        [DataMember(Name = "projects")]
        private List<Project> _projects = new List<Project>();

    }
}
