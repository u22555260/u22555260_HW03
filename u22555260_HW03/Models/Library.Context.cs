﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace u22555260_HW03.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class LibraryEntities1 : DbContext
    {
        public LibraryEntities1()
            : base("name=LibraryEntities1")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<authors> authors { get; set; }
        public virtual DbSet<books> books { get; set; }
        public virtual DbSet<borrows> borrows { get; set; }
        public virtual DbSet<students> students { get; set; }
        public virtual DbSet<types> types { get; set; }
        public virtual DbSet<DownloadedFiles> DownloadedFiles { get; set; }
    }
}
