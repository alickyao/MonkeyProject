using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Filters;
using monkey.service.Logs;

namespace monkey.service
{

    /// <summary>
    /// 基本返回对象
    /// </summary>
    public class BaseResponse
    {
        private string _Message = "0";
        /// <summary>
        /// 返回代码 
        /// [0 成功],
        /// [400 提交的请求参数为空],
        /// [403 数据验证错误 服务器拒绝执行],
        /// [404 未能通过指定的条件找到对应的信息],
        /// [500 服务器错误],[501 该方法还未在服务器实现],[504 超时]
        /// </summary>
        public string Message {
            get { return _Message; }
            set { _Message = value; }
        }

        private string _MessageDetail = "success";
        /// <summary>
        /// 返回消息提示文本
        /// </summary>
        public string MessageDetail {
            get { return _MessageDetail; }
            set { _MessageDetail = value; }
        }

        /// <summary>
        /// 获取返回值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">放置在item中的对象</param>
        /// <param name="msg">可选，消息提示文本</param>
        /// <returns></returns>
        public static BaseResponse<T> getResult<T>(T obj,string msg = "success")
        {
            return new BaseResponse<T>()
            {
                MessageDetail = msg,
                item = obj
            };
        }

        /// <summary>
        /// 获取返回值
        /// </summary>
        /// <param name="msg">可选，消息提示文本</param>
        /// <returns></returns>
        public static BaseResponse getResult(string msg = "success") {
            return new BaseResponse()
            {
                MessageDetail = msg
            };
        }
    }

    /// <summary>
    /// 基本返回对象
    /// </summary>
    /// <typeparam name="T">当执行成功时，具体的返回对象类型</typeparam>
    public class BaseResponse<T> : BaseResponse
    {
        /// <summary>
        /// 数据
        /// </summary>
        public T item { get; set; }

        
    }

    /// <summary>
    /// 标准可翻页的列表返回对象
    /// </summary>
    /// <typeparam name="T">数据行的类型</typeparam>
    public class BaseResponseList<T> {
        /// <summary>
        /// 数据行
        /// </summary>
        public List<T> rows { get; set; }
        /// <summary>
        /// 总数
        /// </summary>
        public long total { get; set; }
    }

    /// <summary>
    /// WebApi异常拦截器
    /// </summary>
    public class WebApiExceptionFilterAttribute : ExceptionFilterAttribute {
        /// <summary>
        /// 重写当API接口发生异常时
        /// </summary>
        /// <param name="actionExecutedContext"></param>
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            HttpError error = new HttpError();
            if (actionExecutedContext.Exception is ValiDataException)
            {
                //数据验证错误
                error.Message = HttpStatusCode.Forbidden.GetHashCode().ToString();
                error.MessageDetail = actionExecutedContext.Exception.Message;
                throw new HttpResponseException(
                    actionExecutedContext.Request.CreateErrorResponse(HttpStatusCode.OK, error));
            }
            else if (actionExecutedContext.Exception is RequestIsNullException)
            {
                //提交的请求参数为空
                error.Message = HttpStatusCode.BadRequest.GetHashCode().ToString();
                error.MessageDetail = "提交的参数为空，请检查";
                throw new HttpResponseException(
                    actionExecutedContext.Request.CreateErrorResponse(HttpStatusCode.OK, error));
            }
            else if (actionExecutedContext.Exception is DataNotFundException)
            {
                //未能通过指定的条件找到对应数据 例如通过ID找对应的信息
                error.Message = HttpStatusCode.NotFound.GetHashCode().ToString();
                error.MessageDetail = actionExecutedContext.Exception.Message;
                throw new HttpResponseException(
                    actionExecutedContext.Request.CreateErrorResponse(HttpStatusCode.OK, error));
            }
            else if (actionExecutedContext.Exception is NotImplementedException)
            {
                //调用了未实现的方法
                error.Message = HttpStatusCode.NotImplemented.GetHashCode().ToString();
                error.MessageDetail = actionExecutedContext.Exception.Message;
                throw new HttpResponseException(
                    actionExecutedContext.Request.CreateErrorResponse(HttpStatusCode.OK, error));
            }
            else if (actionExecutedContext.Exception is TimeoutException)
            {
                //已超时
                error.Message = HttpStatusCode.GatewayTimeout.GetHashCode().ToString();
                error.MessageDetail = "服务器已超时";
                try
                {
                    ExceptionLog.create(HttpStatusCode.GatewayTimeout, "服务器超时", actionExecutedContext.Exception.StackTrace);
                }
                catch { }
                throw new HttpResponseException(
                    actionExecutedContext.Request.CreateErrorResponse(HttpStatusCode.OK, error));
            }
            else {
                //其他都归结于 500 服务器错误
                error.Message = HttpStatusCode.InternalServerError.GetHashCode().ToString();
                error.MessageDetail = actionExecutedContext.Exception.Message;
                try
                {
                    ExceptionLog.create(HttpStatusCode.InternalServerError, error.MessageDetail, actionExecutedContext.Exception.StackTrace);
                }
                catch { }
                throw new HttpResponseException(
                    actionExecutedContext.Request.CreateErrorResponse(HttpStatusCode.OK, error));
            }
            //base.OnException(actionExecutedContext);
        }
    }
}
