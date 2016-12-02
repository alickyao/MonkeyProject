using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using monkey.service;
using monkey.service.Logs;
using monkey.service.Users;

namespace website.Controllers.Logs
{
    /// <summary>
    /// 系统日志相关接口
    /// </summary>
    public class LogsController : ApiController
    {
        /// <summary>
        /// [后台权限]检索系统日志列表
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        [HttpPost]
        [ApiAuthorize(RoleType = SysRolesType.后台)]
        public BaseResponse<BaseResponseList<BaseLog>> getLogs(BaseLogSearchReqeust condtion) {
            var result = BaseLog.searchList(condtion);
            return BaseResponse.getResult(result);
        }

        /// <summary>
        /// [后台权限]检索用户日志
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        [HttpPost]
        [ApiAuthorize(RoleType = SysRolesType.后台)]
        public BaseResponse<BaseResponseList<UserLog>> getUserLogs(UserLogSearchRequest condtion) {
            var result = UserLog.searchList(condtion);
            return BaseResponse.getResult(result);
        }

        /// <summary>
        /// [后台权限]检索异常日志
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        [HttpPost]
        [ApiAuthorize(RoleType = SysRolesType.后台)]
        public BaseResponse<BaseResponseList<ExceptionLog>> getExceptionLogs(ExceptionLogSearchRequest condtion) {
            var result = ExceptionLog.searchList(condtion);
            return BaseResponse.getResult(result);
        }
    }
}
