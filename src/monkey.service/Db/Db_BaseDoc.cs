//------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    
    public partial class Db_BaseDoc
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Db_BaseDoc()
        {
            this.Db_BaseDocFile = new HashSet<Db_BaseDocFile>();
            this.Db_BaseDocTree = new HashSet<Db_BaseDocTree>();
        }
    
        public string Id { get; set; }
        public int DocType { get; set; }
        public string Caption { get; set; }
        public string Code { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public int Seq { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsDisabled { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Db_BaseDocFile> Db_BaseDocFile { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Db_BaseDocTree> Db_BaseDocTree { get; set; }
    }
}
