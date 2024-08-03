using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class ticketlist
    {
        public int ticketID { get; set; }
        public int userID { get; set; }
        public string username { get; set; }
        public string ticketTitle { get; set; }
        public int priority { get; set; }
        public int? status { get; set; }
    }
}