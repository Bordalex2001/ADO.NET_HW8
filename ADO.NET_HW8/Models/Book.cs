//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ADO.NET_HW8.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int AuthorId { get; set; }
        public Nullable<int> CategoryId { get; set; }
        public int PublisherId { get; set; }
        public int Year { get; set; }
        public Nullable<int> Pages { get; set; }
    
        public virtual Author Authors { get; set; }
        public virtual Category Categories { get; set; }
        public virtual Publisher Publishers { get; set; }
    }
}
