using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Http;
using System.Web.Security;
using System.Security.Principal;

namespace website
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            // 在应用程序启动时运行的代码
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        protected void Application_AuthorizeRequest(object sender, System.EventArgs e)
        {
            HttpApplication App = (HttpApplication)sender;
            HttpContext Ctx = App.Context;   //获取本次Http请求相关的HttpContext对象  
            if (Ctx.Request.IsAuthenticated == true)   //验证过的用户才进行role的处理  
            {
                FormsIdentity Id = (FormsIdentity)Ctx.User.Identity;
                FormsAuthenticationTicket Ticket = Id.Ticket;   //取得身份验证票  
                string[] Roles = Ticket.UserData.Split(',');   //将身份验证票中的role数据转成字符串数组  
                Ctx.User = new GenericPrincipal(Id, Roles);   //将原有的Identity加上角色信息新建一个GenericPrincipal表示当前用户,这样当前用户就拥有了role信息  
            }
        }
    }

}