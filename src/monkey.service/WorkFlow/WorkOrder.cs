using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using monkey.service.Db;
using monkey.service.Logs;

namespace monkey.service.WorkFlow
{
    
    /// <summary>
    /// 工单类型
    /// </summary>
    public enum WorkOrderType
    {
        /// <summary>
        /// 基础工单 - 无类具体类型
        /// </summary>
        无类型
    }

    /// <summary>
    /// 工单检索请求
    /// </summary>
    public class BaseWorkOrderSearchRequest : BaseRequest {

    }

    /// <summary>
    /// 基础工单
    /// </summary>
    public class BaseWorkOrder:ILogStringable
    {

        #region
        /// <summary>
        /// 记录的ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 工单类型
        /// </summary>
        public WorkOrderType OrderType { get; set; }

        /// <summary>
        /// 工单类型 - 文本
        /// </summary>
        public string OrderTypeString { get; set; }

        /// <summary>
        /// 使用工作流的ID
        /// </summary>
        public string WorkFlowDefinitionId { get; set; }

        /// <summary>
        /// 工作流书签ID
        /// </summary>
        public string WorkFlowBookMarkId { get; set; }

        /// <summary>
        /// 工单备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 工单创建时间
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public string CreatedOnString { get; set; }
        #endregion

        /// <summary>
        /// 从数据库对象构造
        /// </summary>
        /// <param name="row"></param>
        public BaseWorkOrder(Db_BaseWorkOrder row) {
            this.Id = row.Id;
            this.OrderType = (WorkOrderType)row.OrderType;
            this.OrderTypeString = this.OrderType.ToString();
            this.WorkFlowDefinitionId = row.WorkFlowDefinitionId;
            this.WorkFlowBookMarkId = row.WorkFlowBookMarkId;
            this.Remark = row.Remark;
            this.CreatedOn = row.CreatedOn;
            this.CreatedOnString = this.CreatedOn.ToString("yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        /// 检索工单信息
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        public static BaseResponseList<BaseWorkOrder> SearchBaseWorkOrderList(BaseWorkOrderSearchRequest condtion) {

            BaseResponseList<BaseWorkOrder> result = new BaseResponseList<BaseWorkOrder>();

            using (var db = new DefaultContainer()) {
                var rows = (from c in db.Db_BaseWorkOrderSet
                            select c
                            );
                result.total = rows.Count();
                if (condtion.getRows && result.total > 0)
                {
                    if (condtion.page > 1)
                    {
                        rows = rows.OrderByDescending(p => p.CreatedOn).Skip(condtion.getSkip()).Take(condtion.pageSize);
                    }

                    result.rows = rows.AsEnumerable().Select(p => new BaseWorkOrder(p)).ToList();
                }
            }

            return result;
        }

        /// <summary>
        /// 批量添加基础工单信息
        /// </summary>
        /// <param name="remarks"></param>
        /// <returns></returns>
        public static List<BaseWorkOrder> CreateBaseWorkOrders(List<string> remarks) {
            List<BaseWorkOrder> result = new List<BaseWorkOrder>();
            using (var db = new DefaultContainer()) {
                List<Db_BaseWorkOrder> dbRows = new List<Db_BaseWorkOrder>();
                foreach (var item in remarks) {
                    var newRow = new Db_BaseWorkOrder()
                    {
                        CreatedOn = DateTime.Now,
                        Id = Guid.NewGuid().ToString(),
                        OrderType = (byte)WorkOrderType.无类型.GetHashCode(),
                        Remark = item
                    };
                    dbRows.Add(newRow);
                    result.Add(new BaseWorkOrder(newRow));
                }
                if (dbRows.Count > 0) {
                    db.Db_BaseWorkOrderSet.AddRange(dbRows);
                    db.SaveChanges();
                }
            }
            return result;
        }

        public string getIdString()
        {
            return this.Id;
        }

        public string getNameString()
        {
            return "基础工单";
        }
    }
}
