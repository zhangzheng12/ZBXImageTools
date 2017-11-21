using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace feature1.Util
{
    class ImageDealClass
    {
        /// <summary>
        /// 传入两个图片，输出hash差异数
        /// </summary>
        /// <param name="img1">旧图片</param>
        /// <param name="img2">新图片</param>
        /// <returns>hash差异数</returns>
        public static int getHashDiff(Image<Bgr, byte> img1, Image<Bgr, Byte> img2)
        {
            //Umat转Image缩略+灰度
            Image<Bgr, Byte> iO = img1.Resize(8, 8, Emgu.CV.CvEnum.Inter.Cubic);
            Image<Gray, Single> iOG = iO.Convert<Gray, Single>();
            Image<Bgr, Byte> iN = img2.Resize(8, 8, Emgu.CV.CvEnum.Inter.Cubic);
            Image<Gray, Single> iNG = iN.Convert<Gray, Single>();
            Bitmap bitO = iOG.ToBitmap();
            Bitmap bitN = iNG.ToBitmap();

            //计算平均灰度 
            int iAvg1 = 0, iAvg2 = 0;
            List<int> list1 = new List<int>();
            List<int> list2 = new List<int>();

            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    list1.Add(bitO.GetPixel(x, y).R);
                    iAvg1 += bitO.GetPixel(x, y).R;

                    list2.Add(bitN.GetPixel(x, y).R);

                    iAvg2 += bitN.GetPixel(x, y).R;
                }
            }
            iAvg1 /= 64;
            iAvg2 /= 64;
            for (int ooo = 0; ooo < 64; ooo++)
            {
                list1[ooo] = (list1[ooo] > iAvg1) ? 0 : 1;
                list2[ooo] = (list2[ooo] > iAvg2) ? 0 : 1;
            }

            int diffNum = 0;
            for (int j = 0; j < 64; j++)
            {
                if (list1[j] != list2[j])
                {
                    diffNum++;
                }
            }

            return diffNum;
        }//public

        #region 灰度化、二值化
        /// <summary>
        /// 输入图片，进行灰度化返回。
        /// </summary>
        /// <param name="bit"></param>
        /// <returns></returns>
        public static Bitmap huiduhua(Bitmap bit)
        {
            int width = bit.Width;
            int height = bit.Height;

            BitmapData bd = bit.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            System.IntPtr Scan0 = bd.Scan0;
            int stride = bd.Stride;

            unsafe
            {
                byte* p = (byte*)Scan0;
                int offset = stride - width * 3;
                Double colorTemp = 0;
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        colorTemp = p[2] * 0.299 + p[1] * 0.587 + p[0] * 0.114;                        
                        p[2] = p[1] = p[0] = (byte)colorTemp;                      
                        p = p + 3;
                    }
                    p = p + offset;
                }
            }

            bit.UnlockBits(bd);

            GC.Collect();
            GC.WaitForPendingFinalizers();

            return bit;
        }

        /// <summary>
        /// 输入图片，进行c#算法二值化操作
        /// </summary>
        /// <param name="bit"></param>
        /// <returns></returns>
        public static Bitmap getez(Bitmap bit)
        {
            int width = bit.Width;
            int height = bit.Height;

            BitmapData bd = bit.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            System.IntPtr Scan0 = bd.Scan0;
            int stride = bd.Stride;
            unsafe
            {
                byte* p = (byte*)Scan0;
                int offset = stride - width * 3;
                int[] histogram = new int[256];
                int mingrayvalue = 255;
                int maxgrayvalue = 0;
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        histogram[p[2]]++;
                        if (p[2] > maxgrayvalue)
                        {
                            maxgrayvalue = p[2];
                        }

                        if (p[2] < mingrayvalue)
                        {
                            mingrayvalue = p[2];
                        }

                        p = p + 3;
                    }
                    p = p + offset;
                }

                int threshold = -1;
                int newThreshold = (mingrayvalue + maxgrayvalue) / 2;

                for (int iterationTimes = 0; threshold != newThreshold && iterationTimes < 100; iterationTimes++)
                {
                    threshold = newThreshold;
                    int lP1 = 0;
                    int lP2 = 0;
                    int lS1 = 0;
                    int lS2 = 0;
                    //求两个区域的灰度的平均值
                    for (int i = mingrayvalue; i < threshold; i++)
                    {
                        lP1 += histogram[i] * i;
                        lS1 += histogram[i];
                    }
                    int mean1GrayValue = 0;
                    try
                    {
                        mean1GrayValue = Math.Abs((lP1 / lS1));
                    }
                    catch
                    {
                        mean1GrayValue = lP1;
                    }
                    for (int i = threshold + 1; i < maxgrayvalue; i++)
                    {
                        lP2 += histogram[i] * i;
                        lS2 += histogram[i];
                    }
                    int mean2GrayValue = 0;
                    try
                    {
                        mean2GrayValue = Math.Abs((lP2 / lS2));
                    }
                    catch
                    {
                        mean2GrayValue = lP2;
                    }
                    newThreshold = (mean1GrayValue + mean2GrayValue) / 2;
                }

                p = (byte*)Scan0;
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        if (p[2] > threshold)
                        {
                            p[0] = p[1] = p[2] = 255;
                        }
                        else
                        {
                            p[0] = p[1] = p[2] = 0;
                        }

                        p = p + 3;
                    }
                    p = p + offset;
                }
            }

            bit.UnlockBits(bd);

            GC.Collect();
            GC.WaitForPendingFinalizers();
            return bit;
        }
        #endregion

        /// <summary>
        /// 找出图片的内容版心。输入原图，输出横坐标，纵坐标，宽度和高度
        /// </summary>
        public static void FindPicContent(Bitmap bit, out int x, out int y, out int width, out int height)
        {
            int left = 0; int right = 0; int up = 0; int down = 0;
            //左边线： 从width=0，height++开始找到第一个点
            for (int i = 0; i < bit.Width; i++)
            {
                for (int j = 0; j < bit.Height; j++)
                {
                    if (bit.GetPixel(i, j).R == 0)
                    {
                        left = i;
                        i = bit.Width;
                        break;
                    }
                }
            }
            //上边线
            for (int i = 0; i < bit.Height; i++)
            {
                for (int j = 0; j < bit.Width; j++)
                {
                    if (bit.GetPixel(j, i).R == 0)
                    {
                        up = i;
                        i = bit.Height;
                        break;
                    }
                }
            }

            for (int i = bit.Width - 1; i > 0; i--)
            {
                for (int j = 0; j < bit.Height; j++)
                {
                    if (bit.GetPixel(i, j).R == 0)
                    {
                        right = i;
                        i = 0;
                        break;
                    }
                }
            }
            for (int i = bit.Height - 1; i > 0; i--)
            {
                for (int j = 0; j < bit.Width; j++)
                {
                    if (bit.GetPixel(j, i).R == 0)
                    {
                        down = i;
                        i = 0;
                        break;
                    }
                }
            }

            x = left;
            y = up;
            width = right - left;
            height = down - up;

        }


    }//class
}//namespace
