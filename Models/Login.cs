using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class Login
    {
        [Key]
        public int ID { get; set; }
        public string username { get; set; }
        public string password { get; set; }
    }
}