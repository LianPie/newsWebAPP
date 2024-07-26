using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class TheNews
    {
        public int ID { get; set; }
        public string title { get; set; }
        public string image { get; set; }
        public string link { get; set; }
        public string summary { get; set; }
        public string cat { get; set; }
        public string tag { get; set; }
        public string publishDate { get; set; }
        public string userID { get; set; }
        public string content { get; set; }
        public string views { get; set; }

    }
}