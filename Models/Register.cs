using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class Register
    {
        [Key]
        public int ID { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string ConfirmPassword { get; set; }
        public string displayname { get; set; }
    }
}