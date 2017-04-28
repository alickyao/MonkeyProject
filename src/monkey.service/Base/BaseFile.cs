using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using monkey.service.Db;

namespace monkey.service
{
    /// <summary>
    /// 文件上传的目录类型
    /// </summary>
    public enum UploadDirType
    {
        /// <summary>
        /// 文档类
        /// </summary>
        Doc
    }

    /// <summary>
    /// 基本文件信息
    /// </summary>
    public class BaseFile
    {
        /// <summary>
        /// 文件的ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 存放的路径
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// 文件名称
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 文件的扩展名
        /// </summary>
        public string ExName { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// 创建时间 - 文本
        /// </summary>
        public string CreatedOnString { get; set; }

        /// <summary>
        /// 构造方法 （从数据库）
        /// </summary>
        /// <param name="row"></param>
        public BaseFile(Db_BaseFile row) {
            this.Id = row.Id;
            this.Path = row.Path;
            this.FileName = row.FileName;
            this.ExName = row.ExName;
            this.CreatedOn = row.CreatedOn;
            this.CreatedOnString = this.CreatedOn.ToString("yyyy-MM-dd HH:mm");
        }

        /// <summary>
        /// 根据ID获取详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static BaseFile GetInstance(string id) {
            using (var db = new DefaultContainer()) {
                var row = db.Db_BaseFileSet.SingleOrDefault(p => p.Id == id);
                if (row == null) {
                    throw new ValiDataException(string.Format("传入的文件ID：{0} 有误，未能找到匹配的信息",id));
                }
                return new BaseFile(row);
            }
        }

        /// <summary>
        /// 创建基本文件信息
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static BaseFile CreateBaseFile(string path) {

            if (string.IsNullOrEmpty(path)) {
                throw new ValiDataException("文件的存放路径不能为空,path is null");
            }

            string fileName;
            int x = path.LastIndexOf("/");
            if (x > 0)
            {
                fileName = path.Substring(x + 1);
            }
            else {
                fileName = path;
            }
            if (string.IsNullOrEmpty(fileName)) {
                throw new ValiDataException(string.Format("路径：{0}，格式不正确", path));
            }
            string exName = string.Empty;
            x = fileName.LastIndexOf(".");
            if (x > 0)
            {
                exName = fileName.Substring(x);
            }

            Db_BaseFile dbFile = new Db_BaseFile()
            {
                Id = Guid.NewGuid().ToString(),
                CreatedOn = DateTime.Now,
                Path = path,
                FileName = fileName,
                ExName = string.IsNullOrEmpty(exName) ? null : exName
            };
            using (var db = new DefaultContainer()) {
                db.Db_BaseFileSet.Add(dbFile);
                db.SaveChanges();
            }
            return new BaseFile(dbFile);
        }

        /// <summary>
        /// 查询所有的历史记录
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        public static BaseResponseList<BaseFile> SearchBaseFileList(BaseRequest condtion) {
            BaseResponseList<BaseFile> result = new BaseResponseList<BaseFile>();

            using (var db = new DefaultContainer()) {
                var rows = (from c in db.Db_BaseFileSet
                            select c
                            );
                result.total = rows.Count();
                if (result.total > 0 && condtion.getRows) {
                    rows = rows.OrderByDescending(p => p.CreatedOn);
                    if (condtion.page > 0)
                    {
                        rows = rows.Skip(condtion.getSkip()).Take(condtion.pageSize);
                    }
                    result.rows = rows.AsEnumerable().Select(p => new BaseFile(p)).ToList();
                }
            }

            return result;
        }
    }
}
