//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebApplication1.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class ticket
    {
        public int ID { get; set; }
        public string title { get; set; }
        public string content { get; set; }
        public string priority { get; set; }
        public Nullable<int> senderid { get; set; }
    }
}