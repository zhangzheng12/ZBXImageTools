using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using System.Drawing.Imaging;

namespace feature1.Util
{
    public class ImageBJ
    {
        private Bitmap myBitmap;
        private Bitmap myBitmap2;
        private string threshold;//二值化的系数传进来
        private List<Point> chayi = new List<Point>();
        public List<Rectangle> chayik = new List<Rectangle>();

        public ImageBJ(Bitmap myBitmap, Bitmap myBitmap2, String threshold)
        {
            this.myBitmap = myBitmap;
            this.myBitmap2 = myBitmap2;
            this.threshold = threshold;
        }
        public void DisposePro()
        {
            this.myBitmap.Dispose();
            this.myBitmap2.Dispose();
            chayi.Clear();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        /// <summary>
        /// 图片相减，把图2设置为相减后的颜色
        /// </summary>
        public Bitmap Imagexj()
        {
            //差异点和差异框清空
            chayi.Clear();
            chayik.Clear();
            try
            {
                ChangePic(myBitmap, myBitmap2, int.Parse(threshold));
            }
            catch
            {
                MessageBox.Show("changePic error");
            }
            

            //如果不重叠，就增加框（chayik）
            foreach (Point cy in chayi)
            {
                Rectangle rt = new Rectangle(cy, new Size(31, 31));
                Boolean xj = false;
                foreach (Rectangle rtl in chayik)
                {
                    if (rt.IntersectsWith(rtl))
                    {

                        xj = true;
                        break;
                    }
                }
                if (xj == false)
                {
                    if (rt.X < 15 || rt.Y < 15 || rt.X > myBitmap.Width - 15 || rt.Y > myBitmap.Height - 15)//在x,y在15像素外边框的都不画框。
                    {

                    }
                    else
                    {
                        chayik.Add(rt);
                    }
                }
            }

            ////筛选chayik，将周围8个像素都没有不同的那个单独的像素删掉。

            //for (int i = 0; i < chayik.Count; i++)
            //{
            //    Rectangle rect = chayik[i];
            //    Rectangle rect1 = new Rectangle(rect.X - 1, rect.Y - 1, 31, 31);
            //    Rectangle rect2 = new Rectangle(rect.X, rect.Y - 1, 31, 31);
            //    Rectangle rect3 = new Rectangle(rect.X + 1, rect.Y - 1, 31, 31);
            //    Rectangle rect4 = new Rectangle(rect.X - 1, rect.Y, 31, 31);
            //    Rectangle rect5 = new Rectangle(rect.X + 1, rect.Y, 31, 31);
            //    Rectangle rect6 = new Rectangle(rect.X - 1, rect.Y + 1, 31, 31);
            //    Rectangle rect7 = new Rectangle(rect.X, rect.Y + 1, 31, 31);
            //    Rectangle rect8 = new Rectangle(rect.X + 1, rect.Y + 1, 31, 31);
            //    if (chayik.Contains(rect1) || chayik.Contains(rect2) || chayik.Contains(rect3) || chayik.Contains(rect4) || chayik.Contains(rect5) || chayik.Contains(rect6) || chayik.Contains(rect7) || chayik.Contains(rect8))
            //    {
            //        //如果这个像素，它的周围存在其他像素。则不删掉这个像素。
            //    }
            //    else
            //    {
            //        //这个像素是孤立的，删除他
            //        chayik.Remove(rect);
            //    }
            //}


            return myBitmap;

        }



        public Bitmap ImagexjForLight()
        {
            //差异点和差异框清空
            chayi.Clear();
            chayik.Clear();
            try
            {
                ChangePicForLight(myBitmap, myBitmap2, int.Parse(threshold));
            }
            catch
            {
                MessageBox.Show("changepic error");

            }
            //如果不重叠，就增加框（chayik）
            foreach (Point cy in chayi)
            {
                Rectangle rt = new Rectangle(cy, new Size(31, 31));
                Boolean xj = false;
                foreach (Rectangle rtl in chayik)
                {
                    if (rt.IntersectsWith(rtl))
                    {

                        xj = true;
                        break;
                    }
                }
                if (xj == false)
                {
                    if (rt.X < 15 || rt.Y < 15)
                    {

                    }
                    else
                    {
                        chayik.Add(rt);
                    }
                }
            }
            return myBitmap;

        }


        public Bitmap ChangePic(Bitmap curBitmap, Bitmap bit2, int threshold)
        {
            if (curBitmap != null)
            {
                int width = curBitmap.Width;
                int height = curBitmap.Height;
                BitmapData data = curBitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                System.IntPtr Scan0 = data.Scan0;
                int stride = data.Stride;

                BitmapData data2 = bit2.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                
                System.IntPtr Scan02 = data2.Scan0;
                int stride2 = data2.Stride;
                int bytes = curBitmap.Width * curBitmap.Height * 3;
                byte[] rgbvalues = new byte[bytes];
                System.Runtime.InteropServices.Marshal.Copy(Scan0, rgbvalues, 0, bytes);

                System.Runtime.InteropServices.Marshal.Copy(Scan02, rgbvalues, 0, bytes);
                unsafe
                {
                    byte* p = (byte*)Scan0;
                    byte* p2 = (byte*)Scan02;
                    int offset = stride - width * 3;
                    double red = 0;
                    double green = 0;
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            if (Math.Abs(p[2] - p2[2]) > threshold || Math.Abs(p[1] - p2[1]) > threshold)//|| Math.Abs(p[3] - p2[3]) > threshold
                            {
                                //原来变白的差异点，现在用绿色
                                red = 27; green = 255;
                                chayi.Add(new Point(x - 15, y - 15));
                            }
                            else
                            {
                                //黑色
                                red = 0; green = 0;
                            }
                            p[0] = (byte)red;
                            p[1] = (byte)green;
                            p[2] = 0;
                            p += 3;
                            p2 += 3;
                        }
                        p += offset;
                        p2 += offset;
                    }
                }
                curBitmap.UnlockBits(data);//
                bit2.UnlockBits(data2);//解锁图2内存占用
                data = null;
                data2 = null;
                rgbvalues = null;
                return curBitmap;
            }
            else
            {
                MessageBox.Show("没有图片");
                return null;

            }
        }
        public Bitmap ChangePicwhiteBg(Bitmap curBitmap, Bitmap bit2, int threshold)
        {
            if (curBitmap != null)
            {
                int width = curBitmap.Width;
                int height = curBitmap.Height;
                BitmapData data = curBitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                System.IntPtr Scan0 = data.Scan0;
                int stride = data.Stride;

                BitmapData data2 = bit2.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

                System.IntPtr Scan02 = data2.Scan0;
                int stride2 = data2.Stride;
                int bytes = curBitmap.Width * curBitmap.Height * 3;
                byte[] rgbvalues = new byte[bytes];
                System.Runtime.InteropServices.Marshal.Copy(Scan0, rgbvalues, 0, bytes);

                System.Runtime.InteropServices.Marshal.Copy(Scan02, rgbvalues, 0, bytes);
                unsafe
                {
                    byte* p = (byte*)Scan0;
                    byte* p2 = (byte*)Scan02;
                    int offset = stride - width * 3;
                    double red = 0;
                    double green = 0;
                    double black = 0;
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            if (Math.Abs(p[2] - p2[2]) > threshold || Math.Abs(p[1] - p2[1]) > threshold)//|| Math.Abs(p[3] - p2[3]) > threshold
                            {
                                //差异点为黑色
                                red = 0; green = 0; black = 0;
                                chayi.Add(new Point(x - 15, y - 15));
                            }
                            else
                            {
                                //背景为白色
                                red = 255; green =255;black = 255;
                            }
                            p[0] = (byte)red;
                            p[1] = (byte)green;
                            p[2] = 255;
                            p += 3;
                            p2 += 3;
                        }
                        p += offset;
                        p2 += offset;
                    }
                }
                curBitmap.UnlockBits(data);//
                bit2.UnlockBits(data2);//解锁图2内存占用
                data = null;
                data2 = null;
                rgbvalues = null;
                return curBitmap;
            }
            else
            {
                MessageBox.Show("没有图片");
                return null;

            }
        }
        public Bitmap ChangePicRgbAnd1bpp(Bitmap curBitmap, Bitmap bit2, int threshold)
        {
            if (curBitmap != null)
            {
                int width = curBitmap.Width;
                int height = curBitmap.Height;
                BitmapData data = curBitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                System.IntPtr Scan0 = data.Scan0;
                int stride = data.Stride;
                BitmapData data2 = bit2.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format1bppIndexed);
                System.IntPtr Scan02 = data2.Scan0;
                int stride2 = data2.Stride;
                int bytes = curBitmap.Width * curBitmap.Height * 3;
                byte[] rgbvalues = new byte[bytes];
                int bytes1bpp = curBitmap.Width * curBitmap.Height;
                byte[] values1bpp = new byte[bytes1bpp];

                    System.Runtime.InteropServices.Marshal.Copy(Scan0, rgbvalues, 0, bytes);
                System.Runtime.InteropServices.Marshal.Copy(Scan02, values1bpp, 0, bytes1bpp);
                unsafe
                {
                    byte* p = (byte*)Scan0;
                    byte* p2 = (byte*)Scan02;
                    int offset = stride - width * 3;
                    double red = 0;
                    double green = 0;
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            if (Math.Abs(p[2] - p2[2]) > threshold || Math.Abs(p[1] - p2[1]) > threshold)//|| Math.Abs(p[3] - p2[3]) > threshold
                            {
                                //原来变白的差异点，现在用绿色
                                red = 27; green = 255;
                                chayi.Add(new Point(x - 15, y - 15));
                            }
                            else
                            {
                                //黑色
                                red = 0; green = 0;
                            }
                            p[0] = (byte)red;
                            p[1] = (byte)green;
                            p[2] = 0;
                            p += 3;
                            p2 += 3;
                        }
                        p += offset;
                        p2 += offset;
                    }
                }
                curBitmap.UnlockBits(data);//
                bit2.UnlockBits(data2);//解锁图2内存占用
                data = null;
                data2 = null;
                rgbvalues = null;
                return curBitmap;
            }
            else
            {
                MessageBox.Show("没有图片");
                return null;

            }
        }

        public Bitmap ChangePicForLight(Bitmap curBitmap, Bitmap bit2, int threshold)
        {
            if (curBitmap != null)
            {
                int width = curBitmap.Width;
                int height = curBitmap.Height;
                BitmapData data = curBitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                System.IntPtr Scan0 = data.Scan0;
                int stride = data.Stride;

                BitmapData data2 = bit2.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                System.IntPtr Scan02 = data2.Scan0;
                int stride2 = data2.Stride;
                int bytes = curBitmap.Width * curBitmap.Height * 3;
                byte[] rgbvalues = new byte[bytes];
                System.Runtime.InteropServices.Marshal.Copy(Scan0, rgbvalues, 0, bytes);
                System.Runtime.InteropServices.Marshal.Copy(Scan02, rgbvalues, 0, bytes);
                unsafe
                {
                    byte* p = (byte*)Scan0;
                    byte* p2 = (byte*)Scan02;
                    int offset = stride - width * 3;
                    double red = 0;
                    double green = 0;
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            if ((Math.Abs(p[2] - p2[2]) > 50 || Math.Abs(p[1] - p2[1]) > 50) && (Math.Abs(p[2] - p2[2]) < 134 || Math.Abs(p[1] - p2[1]) < 134))//|| Math.Abs(p[3] - p2[3]) > threshold
                            {

                                //原来变白的差异点，现在用绿色
                                red = 27; green = 255;
                                chayi.Add(new Point(x - 15, y - 15));
                            }
                            else
                            {
                                //黑色
                                red = 0; green = 0;
                            }
                            p[0] = (byte)red;
                            p[1] = (byte)green;
                            p[2] = 0;
                            p += 3;
                            p2 += 3;
                        }
                        p += offset;
                        p2 += offset;
                    }
                }
                curBitmap.UnlockBits(data);//
                bit2.UnlockBits(data2);//解锁图2内存占用
                data = null;
                data2 = null;
                rgbvalues = null;
                return curBitmap;
            }
            else
            {
                MessageBox.Show("没有图片");
                return null;

            }
        }


    }
}
