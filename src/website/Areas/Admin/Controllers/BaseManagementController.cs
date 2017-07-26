using monkey.service;
using monkey.service.Users;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace website.Areas.Admin.Controllers
{
    /// <summary>
    /// 基础项维护
    /// </summary>
    public class BaseManagementController : BaseController
    {
        // GET: Admin/BaseMangement

        /// <summary>
        /// 基本树网格
        /// </summary>
        /// <returns></returns>
        [SysAuthorize(RoleType = SysRolesType.后台)]
        public ActionResult BaseTreeGrid(string pageId = null)
        {
            ViewBag.pageId = getPageId(pageId);
            return View();
        }

        /// <summary>
        /// 在线文本编辑器文件上传
        /// </summary>
        public void UploadFileForWebEdit() {

            Hashtable hash = new Hashtable();
            Response.AddHeader("Content-Type", "text/html; charset=UTF-8");
            bool error = false;
            //获取上传的文件
            HttpFileCollectionBase Files = Request.Files;

            string UploadPath = "/UploadImg/WebEdit/" + DateTime.Now.ToString("yyMMdd") + "/";

            //定义允许上传的文件扩展名
            Hashtable extTable = new Hashtable();
            extTable.Add("image", "gif,jpg,jpeg,png,bmp");
            extTable.Add("flash", "swf,flv");
            extTable.Add("media", "swf,flv,mp3,wav,wma,wmv,mid,avi,mpg,asf,rm,rmvb");
            extTable.Add("file", "doc,docx,xls,xlsx,ppt,htm,html,txt,zip,rar,gz,bz2");

            //最大文件大小
            int maxsize = 8;//M;
            maxsize = maxsize * 1000000;
            //文件类型设定
            string dirName = Request.QueryString["dir"];
            if (String.IsNullOrEmpty(dirName))
            {
                dirName = "image";
            }
            if (Files == null)
            {
                hash["error"] = 1;
                hash["message"] = "请选择上传的文件";
                error = true;
            }
            if (!error)
            {
                if (Files.Count > 0)
                {
                    HttpPostedFileBase postedFile = Files[0];

                    //取得上传得文件名
                    string fileName = System.IO.Path.GetFileName(postedFile.FileName);
                    //取得文件的扩展名
                    string fileExtension = System.IO.Path.GetExtension(fileName).ToLower();
                    //重新定义的文件名
                    string newFileName = SysHelps.GetNewId();


                    if (postedFile.InputStream == null)
                    {
                        hash["error"] = 1;
                        hash["message"] = "请选择上传的文件";
                        error = true;
                    }
                    if (postedFile.InputStream.Length > maxsize)
                    {
                        hash["error"] = 1;
                        hash["message"] = "文件超出了限制的" + (maxsize / 1000000).ToString() + "M，无法上传";
                        error = true;
                    }

                    if (String.IsNullOrEmpty(fileExtension) || Array.IndexOf(((String)extTable[dirName]).Split(','), fileExtension.Substring(1).ToLower()) == -1)
                    {
                        hash["error"] = 1;
                        hash["message"] = "上传文件扩展名是不允许的扩展名。\n只允许" + ((String)extTable[dirName]) + "格式。";
                        error = true;
                    }
                    if (!error)
                    {
                        string SavePath = Server.MapPath(UploadPath);
                        if (!Directory.Exists(SavePath))
                        {
                            Directory.CreateDirectory(SavePath);
                        }
                        //完整的保存路径
                        string NewFileUrl = SavePath + newFileName + fileExtension;
                        postedFile.SaveAs(NewFileUrl);

                        hash["error"] = 0;
                        hash["url"] = UploadPath + newFileName + fileExtension;
                        string result = JsonConvert.SerializeObject(hash);
                        Response.Write(result);
                        Response.End();
                    }
                }
                else {
                    hash["error"] = 1;
                    hash["message"] = "请选择上传的文件";
                }
                string errorresponse = JsonConvert.SerializeObject(hash);

                Response.Write(errorresponse);
                Response.End();
            }
        }
    }
}