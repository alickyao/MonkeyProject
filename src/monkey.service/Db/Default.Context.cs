﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace monkey.service.Db
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class DefaultContainer : DbContext
    {
        public DefaultContainer()
            : base("name=DefaultContainer")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Db_BaseUser> Db_BaseUserSet { get; set; }
        public virtual DbSet<Db_BaseLog> Db_BaseLogSet { get; set; }
        public virtual DbSet<Db_BaseUserRole> Db_BaseUserRoleSet { get; set; }
        public virtual DbSet<Db_BaseTree> Db_BaseTreeSet { get; set; }
        public virtual DbSet<Db_BaseDoc> Db_BaseDocSet { get; set; }
        public virtual DbSet<Db_BaseFile> Db_BaseFileSet { get; set; }
        public virtual DbSet<Db_BaseDocFile> Db_BaseDocFileSet { get; set; }
    }
}
