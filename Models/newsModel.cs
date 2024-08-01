using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class newsModel
    {
        public string DisplayName { get; set; }
        public int NewsID { get; set; }
        public string Title { get; set; }
        public string Image { get; set; }
        public string category { get; set; }
        public string tag { get; set; }
        public DateTime? date { get; set; }
        public int? views { get; set; }
    }
}