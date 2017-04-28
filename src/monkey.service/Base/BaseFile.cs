using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using monkey.service.Db;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.IO;
using System.Drawing;

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
        #region - 基本属性

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

        #endregion

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
        
        /// <summary>
        /// 获取缩略图
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        public string GetImgFileThumbnailPath(GetImgFileThumbnailRequest condtion) {
            return GetImgFileThumbnailPath(this.Path, condtion);
        }

        /// <summary>
        /// 获取缩略图
        /// </summary>
        /// <param name="path">目标文件的存放路径</param>
        /// <param name="condtion"></param>
        /// <returns></returns>
        public static string GetImgFileThumbnailPath(string path, GetImgFileThumbnailRequest condtion) {
            ValiDatas.valiData(condtion);
            int width = condtion.Width;
            int height = condtion.Height;
            string imgUrl = path;
            string fileName = HttpContext.Current.Server.MapPath(imgUrl);

            if (!File.Exists(fileName)) throw new DataNotFundException("指定的文件不存在");
            string fileExt = System.IO.Path.GetExtension(fileName).ToLower();
            string[] imgTypes = { ".gif", ".jpg", ".jpeg", ".png", ".bmp" };
            if (!imgTypes.Contains(fileExt)) throw new ValiDataException("只有图片文件才能生成缩略图");
            if (imgUrl.IndexOf("thumbnail", StringComparison.OrdinalIgnoreCase) != -1) throw new ValiDataException("传入的文件已经是缩略图地址了，不能再生成缩略图");

            string imgDir = string.Format("{0}/thumbnail/{1}_{2}",imgUrl.Substring(0, imgUrl.LastIndexOf('.')), width, height);
            string imgFile = string.Format("{0}/{1}", imgDir, imgUrl.Substring(imgUrl.LastIndexOf("/", StringComparison.OrdinalIgnoreCase)));
            //判断目录是否存在
            if (!Directory.Exists(HttpContext.Current.Server.MapPath(imgDir))) Directory.CreateDirectory(HttpContext.Current.Server.MapPath(imgDir));
            //创建缩略图
            if (!File.Exists(HttpContext.Current.Server.MapPath(imgFile))) MakeThumbnail(fileName, HttpContext.Current.Server.MapPath(imgFile), width, height);
            return imgFile;
        }

        /// <summary>
        /// 获取缩略图
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <param name="newPath"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        private static void MakeThumbnail(string sourcePath, string newPath, int width, int height)
        {
            Image ig = Image.FromFile(sourcePath);
            int towidth = width;
            int toheight = height;
            int x = 0;
            int y = 0;
            int ow = ig.Width;
            int oh = ig.Height;
            if ((double)ig.Width / (double)ig.Height > (double)towidth / (double)toheight)
            {
                oh = ig.Height;
                ow = ig.Height * towidth / toheight;
                y = 0;
                x = (ig.Width - ow) / 2;

            }
            else
            {
                ow = ig.Width;
                oh = ig.Width * height / towidth;
                x = 0;
                y = (ig.Height - oh) / 2;
            }
            Image bitmap = new Bitmap(towidth, toheight);
            Graphics g = Graphics.FromImage(bitmap);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g.Clear(Color.Transparent);
            g.DrawImage(ig, new Rectangle(0, 0, towidth, toheight), new Rectangle(x, y, ow, oh), GraphicsUnit.Pixel);
            try
            {
                bitmap.Save(newPath, System.Drawing.Imaging.ImageFormat.Jpeg);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                ig.Dispose();
                bitmap.Dispose();
                g.Dispose();
            }
        }
    }

    /// <summary>
    /// 获取缩略图请求
    /// </summary>
    public class GetImgFileThumbnailRequest
    {
        private int _width = 200;
        /// <summary>
        /// 宽度（单位：像素） 默认为200
        /// </summary>
        [Required]
        [Range(100, 2000)]
        public int Width {
            get { return _width; }
            set { _width = value; }
        }

        private int _height = 200;
        /// <summary>
        /// 高度（单位：像素） 默认为200
        /// </summary>
        [Required]
        [Range(100, 2000)]
        public int Height {
            get { return _height; }
            set { _height = value; }
        }
    }
}
