using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using monkey.service;
using System.Web;
using System.IO;
using monkey.service.Users;

namespace website.Controllers.SysBase
{
    /// <summary>
    /// 文件处理
    /// </summary>
    public class FileController : ApiController
    {
        /// <summary>
        /// [登录权限]通用上传文件 - 返回上传后的ID与存放路径 可批量上传
        /// </summary>
        /// <param name="path">目录类型</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public BaseResponse<List<BaseFile>> UploadFile(UploadDirType path = UploadDirType.Doc) {
            List<BaseFile> files = new List<BaseFile>();

            string savePath = string.Format("/UploadImg/{0}/{1}", path.ToString(), DateTime.Now.ToString("yyyyMMdd"));

            DirectoryInfo dirinfo = new DirectoryInfo(HttpContext.Current.Server.MapPath(savePath));
            if (!dirinfo.Exists)
            {
                dirinfo.Create();
            }

            HttpFileCollection Files = HttpContext.Current.Request.Files;

            if (Files.Count > 0)
            {
                for (int i = 0; i < Files.Count; i++)
                {
                    HttpPostedFile file = Files[i];

                    string fileName, fileExtension;
                    //取得上传得文件名
                    fileName = Path.GetFileName(file.FileName);
                    //取得文件的扩展名
                    fileExtension = Path.GetExtension(fileName).ToLower();
                    string newFileName = Guid.NewGuid().ToString().Replace("-", "").ToLower();
                    string newFilePath = string.Format("{0}/{1}{2}", savePath, newFileName, fileExtension);
                    file.SaveAs(HttpContext.Current.Server.MapPath(newFilePath));
                    files.Add(BaseFile.CreateBaseFile(newFilePath));
                }
                return BaseResponse.getResult(files);
            }
            else {
                throw new ValiDataException("没有上传任何文件");
            }
        }

        /// <summary>
        /// [后台角色权限]获取所有已上传的文件列表
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        [ApiAuthorize(RoleType = SysRolesType.后台)]
        [HttpPost]
        public BaseResponseList<BaseFile> SearchFiles(BaseRequest condtion) {
            return BaseFile.SearchBaseFileList(condtion);
        }
    }
}
