using Emgu.CV;
using Emgu.CV.Structure;
using feature1.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using feature1.Util;

namespace feature1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        string pathFolder1;//“选择文件夹1”的路径
        string pathFolder2;//“选择文件夹2”的路径
        string[] pathImageFiles1;

        private void 选择文件夹1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string path = DialogHelper.ShowOpenFolder();
            if (string.IsNullOrEmpty(path))
            {
                return;
            }
            pathFolder1 = path;
        }

        private void 选择文件夹2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string path = DialogHelper.ShowOpenFolder();
            if (string.IsNullOrEmpty(path))
            {
                return;
            }
            pathFolder2 = path;
        }//private

        private void 相似度测试ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            xiangsiduTest();
        }
        
        /// <summary>
        /// 相似度测试 包括相似度识别和生成图片报告
        /// </summary>
        private void xiangsiduTest()
        {
            string pathOld = pathFolder1;
            string pathNew = pathFolder2;

            string[] filePath = Directory.GetFiles(pathOld).OrderBy(x => x.Length).ThenBy(x => x).ToArray();
            string[] filePathNew = Directory.GetFiles(pathNew).OrderBy(x => x.Length).ThenBy(x => x).ToArray();
            //新建一张图，在图上绘制报告图
            Bitmap bit = new Bitmap(290, 4380);
            Graphics g = Graphics.FromImage(bit);
            g.FillRectangle(Brushes.White, new Rectangle(0, 0, 290, 4380));
            SolidBrush drawBrush = new SolidBrush(Color.Black);

            //初始化进度条
            if (toolStripProgressBar1.Value!=0)
            {
                toolStripProgressBar1.Value = 0;
            }
            toolStripProgressBar1.Maximum = filePath.Length;

            for (int i = 0; i < filePath.Length; i++)
            {
                Image<Bgr, Byte> imgOld = new Image<Bgr, Byte>(filePath[i]);
                Image<Bgr, Byte> imgNew = new Image<Bgr, Byte>(filePathNew[i]);                
                PointF drawPoint0 = new PointF(0, i * 30);//图0放在0，0位置画30*30大小 这个画字体
                PointF drawPoint1 = new PointF(200, i * 30);//图1放在30，0位置画30*30大小
                PointF drawPoint2 = new PointF(230, i * 30);//图2放在60，0位置画30*30大小
                PointF drawPoint3 = new PointF(260, i * 30);//图3放在90，0位置画30*30大小
                Font drawFont0 = new Font("宋体", 10, FontStyle.Regular);
                Font drawFont = new Font("宋体", 16, FontStyle.Regular);
                g.DrawString(Path.GetFileNameWithoutExtension(filePath[i]), drawFont0, drawBrush, drawPoint0);//把字体画出来
                g.DrawImage(imgOld.ToBitmap(), drawPoint1);
                g.DrawImage(imgNew.ToBitmap(), drawPoint2);
                int diffNum=ImageDealClass.getHashDiff(imgOld,imgNew);
                g.DrawString(diffNum.ToString(), drawFont, drawBrush, drawPoint3);//把差异数画到坐标
                toolStripProgressBar1.Value++;
            }

            bit.Save(@"C:\Users\zbx\Desktop\结果.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
            MessageBox.Show(@"生成图片位于：C:\Users\zbx\Desktop\结果.jpg");
        }

        private void 选择图像文件1可多选ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string[] pathOld = DialogHelper.ShowOpenImageFiles();
            if (pathOld.Length == 0)
            {
                return;
            }
            pathImageFiles1 = pathOld;
        }

        private void 裁掉空白ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //在原路径建立同名加new的文件夹存储处理后的图片
            string direName = Path.GetDirectoryName(pathImageFiles1[0]) + "new";
            Directory.CreateDirectory(direName);

            for(int i=0;i<pathImageFiles1.Length;i++)
            {
                Bitmap bit = new Bitmap(pathImageFiles1[i]);
                Bitmap gray = ImageDealClass. huiduhua(bit);
                Bitmap thresholdImage = ImageDealClass.getez(gray);

                int x, y, width, height;
                ImageDealClass.FindPicContent(thresholdImage, out x, out y, out width, out height);
                Bitmap bitSave = new Bitmap(width, height);//要保存的大小
                Graphics g = Graphics.FromImage(bitSave);
                Rectangle rect1 = new Rectangle(x, y, width, height);//原图的哪个部分哪个大小拿来绘制
                Rectangle destRectangle2 = new Rectangle(0, 0, width, height);// 在新图上什么位置开始绘制什么大小
                g.DrawImage(thresholdImage, destRectangle2, rect1, GraphicsUnit.Pixel);
                bitSave.Save(direName + "\\" + Path.GetFileName(pathImageFiles1[i]));

            }//for
            MessageBox.Show("ok");
        }//private

        private void 工具帮助ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Help help = new Help();
            help.Show();
        }

        private void 输出特定字的所有字体到文件夹ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            #region 循环输出字到文件夹
            string[] strfont = new string[] {"贷","货","袋","款", "敕", "赔"
            ,"賠","贵","责","款","軟","债","偾","履","屜"};

            foreach (var item in strfont)
            {
                string path = @"C:\Users\zbx\Desktop\字体\" + item;
                Directory.CreateDirectory(path);
            }

            foreach (var onefontfamily in FontFamily.Families)
            {
                for (int i = 0; i < strfont.Length; i++)
                {
                    Bitmap bit = new Bitmap(50, 50);
                    Graphics g = Graphics.FromImage(bit);
                    g.FillRectangle(Brushes.White, new Rectangle(0, 0, 50, 50));
                    SolidBrush drawBrush = new SolidBrush(Color.Black);
                    PointF drawPoint = new PointF(5F, 5F);
                    Font drawFont = new Font(onefontfamily, 16, FontStyle.Regular);
                    g.DrawString(strfont[i], drawFont, drawBrush, drawPoint);

                    string path = @"C:\Users\zbx\Desktop\字体\" + strfont[i];
                    bit.Save(path + "\\" + onefontfamily.Name + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                    bit.Dispose();
                    g.Dispose();

                }
            }
            MessageBox.Show("accomplish");
            #endregion
        }

        private void 输出特定字的所有字体到图片ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            #region 输出字体到一个图片
            string[] strfont = new string[] {"贷","货","袋","款", "敕", "赔"
            ,"賠","贵","责","款","軟","债","偾","履","屜"};

            foreach (var item in strfont)//对于每个字
            {
                string path = @"C:\Users\zbx\Desktop\字体\" + item;

                Bitmap bit = new Bitmap(1000, 500);
                Graphics g = Graphics.FromImage(bit);
                g.FillRectangle(Brushes.White, new Rectangle(0, 0, 1000, 500));
                SolidBrush drawBrush = new SolidBrush(Color.Black);
                for (int i = 0; i < FontFamily.Families.Length; i++)
                {
                    int x = i / 10 * 50;
                    int y = i % 10 * 50;
                    //PointF drawPoint = new PointF(0.0F, 0.0F);
                    PointF drawPoint = new PointF(x, y);
                    Font drawFont = new Font(FontFamily.Families[i], 16, FontStyle.Regular);
                    g.DrawString(item, drawFont, drawBrush, drawPoint);
                }

                bit.Save(path + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                bit.Dispose();
                g.Dispose();

            }//foreach

            MessageBox.Show("accomplish");
            #endregion
        }

        private void 输出宋特定字的宋体到文件夹ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            #region 循环输出字到文件夹
            string[] strfont = new string[] {"可","司","位","住", "借", "债"
            ,"配","酬"};

            foreach (var item in strfont)
            {
                string path = @"C:\Users\zbx\Desktop\字体\" + item;
                Directory.CreateDirectory(path);
            }

            for (int i = 0; i < strfont.Length; i++)
            {
                Bitmap bit = new Bitmap(50, 50);
                Graphics g = Graphics.FromImage(bit);
                g.FillRectangle(Brushes.White, new Rectangle(0, 0, 50, 50));
                SolidBrush drawBrush = new SolidBrush(Color.Black);
                PointF drawPoint = new PointF(5F, 5F);
                Font drawFont = new Font("宋体", 16, FontStyle.Regular);
                g.DrawString(strfont[i], drawFont, drawBrush, drawPoint);

                string path = @"C:\Users\zbx\Desktop\字体\" + strfont[i];
                bit.Save(path + "\\" + "宋体" + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                bit.Dispose();
                g.Dispose();

            }

            MessageBox.Show("accomplish");

            #endregion
        }

        private void 输出中文字符到图片ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap bit = new Bitmap(3000,3000);
            Graphics g = Graphics.FromImage(bit);
            g.FillRectangle(Brushes.White, new Rectangle(0, 0, 3000, 3000));
            SolidBrush drawBrush = new SolidBrush(Color.Black);

            string strChi = File.ReadAllText("汉字表.txt");
            string[] filter = new string[] {"0","1","2","3","4","5","6","7","8","9"," ","\r\n", "０", "１", "２", "３", "４", "５", "６", "７", "８", "９", "　" };
            string[] strfont = strChi.Split(filter, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < strfont.Length; i++)
            {
                int x = i / 100 * 30;
                int y = i % 100 * 30;
                PointF drawPoint = new PointF(x, y);
                Font drawFont = new Font("宋体", 16, FontStyle.Regular);
                g.DrawString(strfont[i], drawFont, drawBrush, drawPoint);

            }
            string path = @"C:\Users\zbx\Desktop\汉字表";
            bit.Save(path + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
            bit.Dispose();
            g.Dispose();

        
            MessageBox.Show("accomplish");
        }
    }//class
}//namespace
