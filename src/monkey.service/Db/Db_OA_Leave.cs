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
    
    public partial class Db_OA_Leave : Db_BaseWorkOrder
    {
        public byte LeaveType { get; set; }
        public System.DateTime BeginTime { get; set; }
        public System.DateTime EndTime { get; set; }
        public string Descript { get; set; }
        public string UserId { get; set; }
    }
}