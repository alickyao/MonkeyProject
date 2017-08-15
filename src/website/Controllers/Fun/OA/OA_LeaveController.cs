using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using monkey.service.Fun.OA;
using monkey.service;
using monkey.service.Users;
using monkey.service.Logs;

namespace website.Controllers.Fun.OA
{
    /// <summary>
    /// 请假申请
    /// </summary>
    public class OA_LeaveController : ApiController
    {
        /// <summary>
        /// [后台角色权限]创建/编辑一个请假申请
        /// </summary>
        /// <param name="info">请假信息</param>
        /// <returns></returns>
        [HttpPost]
        [ApiAuthorize(RoleType = SysRolesType.后台)]
        public BaseResponse<LeaveInfo> EditLeave(LeaveInfoEditRequest info) {
            info.User = UserManager.getUserById(User.Identity.Name);
            if (string.IsNullOrEmpty(info.Id))
            {
                var result = LeaveInfo.CreateLeaveInfo(info);
                UserLog.create("请假申请已创建", "请假申请", info.User, result);
                return BaseResponse.getResult(result, "创建成功");
            }
            else {
                var leaveInfo = new LeaveInfo(info.Id);
                //这里可进行一些判断例如状态判断什么的
                if (leaveInfo.OrderStatus == monkey.service.WorkFlow.WorkOrderStatus.待提交 || leaveInfo.OrderStatus == monkey.service.WorkFlow.WorkOrderStatus.已终止)
                {
                    var result = leaveInfo.EditLeaveInfo(info);
                    UserLog.create("请假申请被重新编辑", "请假申请", info.User, result);
                    return BaseResponse.getResult(result, "保存成功");
                }
                else {
                    throw new ValiDataException(string.Format("状态为[{0}]的请假申请不能被编辑", leaveInfo.OrderStatusString));
                }
            }
        }

        /// <summary>
        /// [登录权限]获取我的请假申请列表
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public BaseResponse<BaseResponseList<LeaveInfo>> GetMyLeavesList(BaseRequest condtion) {
            return BaseResponse.getResult(LeaveInfo.SearchMyLeavesList(User.Identity.Name, condtion));
        }

        /// <summary>
        /// [后台角色权限]删除一个请假申请
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [ApiAuthorize(RoleType = SysRolesType.后台)]
        public BaseResponse DelLeave(string id) {
            var userInfo = UserManager.getUserById(User.Identity.Name);
            var info = new LeaveInfo(id);
            info.Del();
            UserLog.create("删除请假申请", "请假申请", userInfo, info);
            return BaseResponse.getResult("删除成功");
        }
    }
}
