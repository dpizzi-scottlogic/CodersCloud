using System;
using System.ComponentModel.DataAnnotations;

namespace CodersCloud.Web.Models
{
    [Serializable]
    public class BuildRequestModel
    {
        [Display(Name = "Source Code")]
        [Required]
        public string Source { get; set; }
    }
}