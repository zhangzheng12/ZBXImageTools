using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Windows;
using System.Drawing.Imaging;

namespace feature1.Util

{
    class ImageTZ
    {
        public string fileName1 = "";
        public string fileName2 = "";


        private System.Drawing.Color bjs = System.Drawing.Color.FromArgb(255, 255, 255);
        private int bjjdz = 10;
        private int jdz = 10;

        private int fkbc = 50;
        private int fkjj = 200;
        private int fkbc1 = 50;

        private int x1 = 0;
        private int y1 = 0;

        private int x2 = 0;
        private int y2 = 0;

        private int _xcy = 0;
        private int _ycy = 0;

        private String _tztp = "";

        bool _isBit1Bigger = false;

        public bool IsBit1Bigger
        {
            get { return _isBit1Bigger; }
            set { _isBit1Bigger = value; }
        }

        public int xcy
        {
            get { return _xcy; }
            set { _xcy = value; }
        }

        public int ycy
        {
            get { return _ycy; }
            set { _ycy = value; }
        }

        public String tztp
        {
            get { return _tztp; }
            set { _tztp = value; }
        }

        public ImageTZ(String tp1, String tp2)
        {
            this.fileName1 = tp1;
            this.fileName2 = tp2;
        }

        public ImageTZ()
        {
        }




        public void DetectMove()
        {
            using (Bitmap bit1 = getez(huiduhua(new Bitmap(fileName1))))
            {
                using (Bitmap bit2 = getez(huiduhua(new Bitmap(fileName2))))
                {                  
                    if (bit1.Width > bit2.Width)
                    {
                        _isBit1Bigger = true;
                    }
                    int x1 = 0, x2 = 0, y1 = 0, y2 = 0;
                    for (int i = 0; i < bit1.Height; i++)
                    {
                        for (int j = 0; j < bit1.Width; j++)
                        {
                            if (bit1.GetPixel(j, i).R == 0)
                            {
                                x1 = j;
                                y1 = i;
                                i = bit1.Height;
                                break;
                            }
                        }
                    }

                    for (int i = 0; i < bit2.Height; i++)
                    {
                        for (int j = 0; j < bit2.Width; j++)
                        {
                            if (bit2.GetPixel(j, i).R == 0)
                            {
                                x2 = j;
                                y2 = i;
                                i = bit2.Height;
                                break;
                            }
                        }
                    }

                    xcy = x1 - x2;
                    ycy = y1 - y2;
                    tztp = "图片2";

                }
            }

        }
        public void getchayi()
        {

            bool IsOk = false;
            Bitmap bm1 = huiduhua(new Bitmap(fileName1));
            Bitmap bm2 = huiduhua(new Bitmap(fileName2));
            Bitmap bm3 = getez(huiduhua(new Bitmap(fileName1)));
            int width1 = bm1.Width;
            int height1 = bm1.Height;
            int width2 = bm2.Width;
            int height2 = bm2.Height;
            int width = 0;
            int height = 0;

            if (width1 < width2)
            {
                width = width1;
            }
            else
            {
                width = width2;
            }

            if (height1 < height2)
            {
                height = height1;
            }
            else
            {
                height = height2;
            }

            BitmapData bd1 = bm1.LockBits(new Rectangle(0, 0, bm1.Width, bm1.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            System.IntPtr Scan1 = bd1.Scan0;
            int stride1 = bd1.Stride;

            BitmapData bd2 = bm2.LockBits(new Rectangle(0, 0, bm2.Width, bm2.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            System.IntPtr Scan2 = bd2.Scan0;
            int stride2 = bd2.Stride;

            BitmapData bd3 = bm3.LockBits(new Rectangle(0, 0, bm3.Width, bm3.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            System.IntPtr Scan3 = bd3.Scan0;
            int stride3 = bd3.Stride;

            unsafe
            {
                byte* p1 = (byte*)Scan1;
                int offset1 = stride1 - width1 * 3;

                byte* p2 = (byte*)Scan2;
                int offset2 = stride2 - width2 * 3;

                byte* p3 = (byte*)Scan3;
                int offset3 = stride3 - width1 * 3;


                List<PointColor> fk1 = new List<PointColor>();
                List<PointColor> fk2 = new List<PointColor>();



                //读取第一个不为白色的点
                System.Drawing.Point qd = new System.Drawing.Point(0, 0);
                Boolean qdok = false;
                for (int y = 0; y < height1; y++)
                {
                    for (int x = 0; x < width1; x++)
                    {
                        p3 = (byte*)Scan3;
                        p3 = p3 + (y * width1 * 3) + y * offset3;
                        p3 = p3 + x * 3;
                        if (ScanColor(p3[0], p3[1], p3[2], bjs.B, bjs.G, bjs.R) == false)
                        {
                            qd.X = x;
                            qd.Y = y;
                            qdok = true;
                            break;
                        }
                    }

                    if (qdok == true)
                    {
                        break;
                    }
                }

                if (qd.X + fkbc > width1 - 1)
                {
                    fkbc = width1 - 1 - qd.X;
                }

                if (qd.Y + fkbc > height1 - 1)
                {
                    fkbc = height1 - 1 - qd.Y;
                }

                //读取第二个不为白色的点
                System.Drawing.Point qd1 = new System.Drawing.Point(0, 0);
                Boolean qdok1 = false;

                if (qdok == true)
                {
                    if (qd.Y + fkjj < height1 - 1)
                    {
                        for (int y = qd.Y + fkjj; y < height1; y++)
                        {
                            for (int x = 0; x < width1; x++)
                            {
                                p3 = (byte*)Scan3;
                                p3 = p3 + (y * width1 * 3) + y * offset3;
                                p3 = p3 + x * 3;
                                if (ScanColor(p3[0], p3[1], p3[2], bjs.B, bjs.G, bjs.R) == false)
                                {
                                    qd1.X = x;
                                    qd1.Y = y;
                                    qdok1 = true;
                                    break;
                                }
                            }

                            if (qdok1 == true)
                            {
                                break;
                            }
                        }
                    }
                }

                if (qd1.X + fkbc1 > width1 - 1)
                {
                    fkbc1 = width1 - 1 - qd1.X;
                }

                if (qd1.Y + fkbc1 > height1 - 1)
                {
                    fkbc1 = height1 - 1 - qd1.Y;
                }

                int jjx = qd1.X - qd.X;
                int jjy = qd1.Y - qd.Y;

                //读取第一个比较方块
                if (qdok == true)
                {
                    for (int y = qd.Y; y < qd.Y + fkbc; y++)
                    {
                        for (int x = qd.X; x < qd.X + fkbc; x++)
                        {
                            p1 = (byte*)Scan1;
                            p1 = p1 + (y * width1 * 3) + y * offset1;
                            p1 = p1 + x * 3;
                            fk1.Add(new PointColor(x, y, p1[2], p1[1], p1[0]));
                        }
                    }
                }


                //读取第二个比较方块
                if (qdok1 == true)
                {
                    for (int y = qd1.Y; y < qd1.Y + fkbc1; y++)
                    {
                        for (int x = qd1.X; x < qd1.X + fkbc1; x++)
                        {
                            p1 = (byte*)Scan1;
                            p1 = p1 + (y * width1 * 3) + y * offset1;
                            p1 = p1 + x * 3;
                            fk2.Add(new PointColor(x, y, p1[2], p1[1], p1[0]));
                        }
                    }
                }

                if (qdok == true)
                {
                    //在图2中找到两个方块
                    while (IsOk == false)
                    {
                        jdz = jdz + 20;
                        if (jdz >= 255)
                        {
                            break;
                        }
                        for (int y = 0; y < height2; y++)
                        {
                            for (int x = 0; x < width2; x++)
                            {

                                //在图2中找第一个方块
                                for (int i = 0; i < fk1.Count; i++)
                                {
                                    if (y + i / fkbc >= height2 - 1)
                                    {
                                        IsOk = false;
                                        break;
                                    }

                                    if (x + i % fkbc >= width2 - 1)
                                    {
                                        IsOk = false;
                                        break;
                                    }
                                    p2 = (byte*)Scan2;
                                    p2 = p2 + ((y + i / fkbc) * width2 * 3) + (y + i / fkbc) * offset1;
                                    p2 = p2 + (x + i % fkbc) * 3;

                                    if (ScanColor(fk1[i].B, fk1[i].G, fk1[i].R, p2[0], p2[1], p2[2], jdz) == true)
                                    {
                                        //MessageBox.Show("P2[0]:" + p2[0].ToString() + ";P2[1]:" + p2[1].ToString() + ";P2[2]:" + p2[2].ToString() + ";fk1[" + i + "].B:" + fk1[i].B.ToString() + ";fk1[" + i + "].B:" + fk1[i].B.ToString() + ";fk1[" + i + "].B:" + fk1[i].B.ToString() + ";X:" + x.ToString() + ";Y:" + y.ToString());
                                        IsOk = true;
                                    }
                                    else
                                    {
                                        IsOk = false;
                                        break;
                                    }
                                }

                                //在图2中找第二个方块
                                if (IsOk == true && qdok1 == true)
                                {
                                    int qdy = y + jjy;  //第二个方块的起点纵坐标
                                    int qdx = x + jjx;  //第二个方块的起点横坐标

                                    if (qdy < 0 || qdy > height2 - 1 || qdx < 0 || qdx > width2 - 1)
                                    {
                                        IsOk = false;
                                    }
                                    else
                                    {
                                        for (int i = 0; i < fk2.Count; i++)
                                        {
                                            if (y + i / fkbc1 >= height2 - 1)
                                            {
                                                IsOk = false;
                                                break;
                                            }

                                            if (x + i % fkbc1 >= width2 - 1)
                                            {
                                                IsOk = false;
                                                break;
                                            }
                                            p2 = (byte*)Scan2;
                                            p2 = p2 + ((qdy + i / fkbc1) * width2 * 3) + (qdy + i / fkbc1) * offset1;
                                            p2 = p2 + (qdx + i % fkbc1) * 3;

                                            if (ScanColor(fk2[i].B, fk2[i].G, fk2[i].R, p2[0], p2[1], p2[2], jdz) == true)
                                            {
                                                //报错一次：写入内存保护.
                                                //MessageBox.Show("P2[0]:" + p2[0].ToString() + ";P2[1]:" + p2[1].ToString() + ";P2[2]:" + p2[2].ToString() + ";fk1[" + i + "].B:" + fk1[i].B.ToString() + ";fk1[" + i + "].B:" + fk1[i].B.ToString() + ";fk1[" + i + "].B:" + fk1[i].B.ToString() + ";X:" + x.ToString() + ";Y:" + y.ToString());
                                                IsOk = true;
                                            }
                                            else
                                            {
                                                IsOk = false;
                                                break;
                                            }
                                        }
                                    }
                                }

                                if (IsOk == true)
                                {
                                    x1 = fk1[0].X;
                                    y1 = fk1[0].Y;
                                    x2 = x;
                                    y2 = y;
                                    break;
                                }
                            }
                            if (IsOk == true)
                            {
                                break;
                            }
                        }

                    }
                }
                else
                {
                    x1 = 0;
                    y1 = 0;

                    x2 = 0;
                    y2 = 0;
                }
            }

            bm1.UnlockBits(bd1);
            bm2.UnlockBits(bd2);
            bm3.UnlockBits(bd3);

            bm1.Dispose();
            bm2.Dispose();
            bm3.Dispose();

            GC.Collect();
            GC.WaitForPendingFinalizers();
            if (IsOk == false)
            {
                tztp = "没找到差异";
            }
            else
            {
                if (y1 > y2)
                {
                    ycy = y1 - y2;
                    xcy = x1 - x2;
                    tztp = "图片1";
                }
                else if (y2 > y1)
                {
                    ycy = y2 - y1;
                    xcy = x2 - x1;
                    tztp = "图片2";
                }
                else if (x1 > x2)
                {
                    ycy = y1 - y2;
                    xcy = x1 - x2;
                    tztp = "图片1";
                }
                else if (x2 > x1)
                {
                    ycy = y2 - y1;
                    xcy = x2 - x1;
                    tztp = "图片2";
                }
                else
                {
                    ycy = 0;
                    xcy = 0;
                    tztp = "无";
                }

            }
            //    MessageBox.Show("调整图片:" + tztp + "；横向差异:" + xcy.ToString() + "；纵向差异:" + ycy.ToString());

        }


       public  bool lastPageIsWhite = false;
      
        public void GetDifferBaseContent()
        {
                using (Bitmap bit1 =new Bitmap(fileName1))
                {
                    using (Bitmap bit2 = new Bitmap(fileName2))
                    {
                        bool isbit14 = false; bool isbit24 = false;
                        isbit14 = isContent(bit1, 7, 7);
                        isbit24 = isContent(bit2, 7, 7);
                        if (isbit14==true &&isbit24==true )
                        {
                           lastPageIsWhite= FindPicAllCaise(bit1, out l1, out r1, out u1, out d1);
                            FindPicAllCaise(bit2, out l2, out r2, out u2, out d2);
                        }
                        else if (isbit14 == true && isbit24 == false)
                        {
                            lastPageIsWhite = FindPicAllCaise(bit1, out l1, out r1, out u1, out d1);
                            FindPicContentCaise(bit2, out l2, out r2, out u2, out d2);   
                        }
                        else if (isbit14 == false && isbit24 == true)
                        {
                            lastPageIsWhite = FindPicContentCaise(bit1, out l1, out r1, out u1, out d1);
                            FindPicAllCaise(bit2, out l2, out r2, out u2, out d2);
                        }
                        else
                        {
                            lastPageIsWhite = FindPicContentCaise(bit1, out l1, out r1, out u1, out d1);
                            FindPicContentCaise(bit2, out l2, out r2, out u2, out d2);
                        }
                      
                        xcy = Math.Abs(l2 - l1) < Math.Abs(r2 - r1) ? l2 - l1 : r2 - r1;
                        ycy = Math.Abs(u2 - u1) < Math.Abs(d2 - d1) ? u2 - u1 : d2 - d1;
                        if (bit1.Width > bit2.Width)
                        {
                            tztp = "图片1";
                            xcy = -xcy;
                            ycy = -ycy;
                        }
                        else
                        {
                            tztp = "图片2";
                        }
                    }
                }
        }
        public void GetDifferBaseContentErzhi()
        {
            using (Bitmap bit1 = getez(huiduhua(new Bitmap(fileName1))))
            {
                using (Bitmap bit2 = getez(huiduhua(new Bitmap(fileName2))))
                {
                    //bit1.Save(@"C:\Users\zhengbx\Desktop\1.tif");
                    //bit2.Save(@"C:\Users\zhengbx\Desktop\2.tif");
                    bool isbit14 = false; bool isbit24 = false;
                    isbit14 = isContent(bit1, 7, 7);
                    isbit24 = isContent(bit2, 7, 7);
                    if (isbit14 == true && isbit24 == true)
                    {
                         FindPicAll(bit1, out l1, out r1, out u1, out d1);
                        FindPicAll(bit2, out l2, out r2, out u2, out d2);
                    }
                    else if (isbit14 == true && isbit24 == false)
                    {
                         FindPicAll(bit1, out l1, out r1, out u1, out d1);
                        FindPicContent(bit2, out l2, out r2, out u2, out d2);
                    }
                    else if (isbit14 == false && isbit24 == true)
                    {
                        FindPicContent(bit1, out l1, out r1, out u1, out d1);
                        FindPicAll(bit2, out l2, out r2, out u2, out d2);
                    }
                    else
                    {
                        FindPicContent(bit1, out l1, out r1, out u1, out d1);
                        FindPicContent(bit2, out l2, out r2, out u2, out d2);

                    }
                    


                    xcy = Math.Abs(l2 - l1) < Math.Abs(r2 - r1) ? l2 - l1 : r2 - r1;
                
                    ycy = Math.Abs(u2 - u1) < Math.Abs(d2 - d1) ? u2 - u1 : d2 - d1;
                    if (bit1.Width > bit2.Width)
                    {
                        tztp = "图片1";
                        xcy = -xcy;
                        ycy = -ycy;
                    }
                    else
                    {
                        tztp = "图片2";
                    }
                }
            }
        }



        int l1 = 0, l2 = 0, r1 = 0, r2 = 0, u1 = 0, u2 = 0, d1 = 0, d2 = 0;
        /// <summary>
        /// 找出图片的内容版心。输入原图，输出上下左右四个边框距离。 35像素是3毫米裁切线
        /// </summary>
        public void FindPicContent(Bitmap bit, out int left, out int right, out int up, out int down)
        {
            left = 0; right = 0; up = 0; down = 0;
            //左边线： 从width=0，height++开始找到第一个点
            for (int i = 20; i < bit.Width - 20; i++)
            {
                for (int j = 20; j < bit.Height - 20; j++)
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
            for (int i = 20; i < bit.Height - 20; i++)
            {
                for (int j = 20; j < bit.Width - 20; j++)
                {
                    if (bit.GetPixel(j, i).R == 0)
                    {
                        up = i;
                        i = bit.Height;
                        break;
                    }
                }
            }
       
            for (int i = bit.Width - 20; i > 0; i--)
            {
                for (int j = 20; j < bit.Height - 20; j++)
                {
                    if (bit.GetPixel(i, j).R == 0)
                    {
                        right = i;
                        i = 0;
                        break;
                    }
                }
            }
            for (int i = bit.Height - 20; i > 0; i--)
            {
                for (int j = 20; j < bit.Width - 20; j++)
                {
                    if (bit.GetPixel(j, i).R == 0)
                    {
                        down = i;
                        i = 0;
                        break;
                    }
                }
            }
        }

        
        public void FindPicContent(Bitmap bit, out int left, out int right, out int up, out int down, out int upixel, out int dpixel, out int lpixel, out int rpixel)
        {
            left = 0; right = 0; up = 0; down = 0; upixel = 0; dpixel = 0; lpixel = 0; rpixel = 0;

            //左边线： 从width=0，height++开始找到第一个点
            for (int i = 20; i < bit.Width - 20; i++)
            {
                for (int j = 20; j < bit.Height - 20; j++)
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
            for (int i = 20; i < bit.Height - 20; i++)
            {
                for (int j = 20; j < bit.Width - 20; j++)
                {
                    if (bit.GetPixel(j, i).R == 0)
                    {
                        up = i;
                        i = bit.Height;
                        break;
                    }
                }
            }

            for (int i = bit.Width - 20; i > 0; i--)
            {
                for (int j = 20; j < bit.Height - 20; j++)
                {
                    if (bit.GetPixel(i, j).R == 0)
                    {
                        right = i;
                        i = 0;
                        break;
                    }
                }
            }
            for (int i = bit.Height - 20; i > 0; i--)
            {
                for (int j = 20; j < bit.Width - 20; j++)
                {
                    if (bit.GetPixel(j, i).R == 0)
                    {
                        down = i;
                        i = 0;
                        break;
                    }
                }
            }
        }


        public bool FindPicContentCaise(Bitmap bit, out int left, out int right, out int up, out int down)
        {
            bool isWhite = false;
            left = 0; right = 0; up = 0; down = 0;
            //左边线： 从width=0，height++开始找到第一个点
            for (int i = 20; i < bit.Width/2; i++)
            {
                for (int j = 20; j < bit.Height - 20; j++)
                {
   
                    if (bit.GetPixel(i, j).R + bit.GetPixel(i, j).G + bit.GetPixel(i, j).B <760
                        )
                    {


                        left = i;
                        i = bit.Width;
                        break;
                    }
                    if (i == bit.Width / 2 - 1 && j == bit.Height - 21)
                    {
                        isWhite = true;
                    }
                }
            }
            //上边线：从height=0，width++开始找到第一个点
            for (int i = 20; i < bit.Height/2; i++)
            {
                for (int j = 20; j < bit.Width - 20; j++)
                {
                    //bit.GetPixel(j, i).R + bit.GetPixel(j, i).G + bit.GetPixel(j, i).B <760
                    if (bit.GetPixel(j, i).R + bit.GetPixel(j, i).G + bit.GetPixel(j, i).B < 760
                )
                    {
                        up = i;
                        i = bit.Height;
                        break;
                    }
                }
            }
            //右边线：从width=max，height++开始找到第一个点
            for (int i = bit.Width - 20; i > bit.Width/2; i--)
            {
                for (int j = 20; j < bit.Height - 20; j++)
                {
                    if (bit.GetPixel(i, j).R + bit.GetPixel(i, j).G + bit.GetPixel(i, j).B < 760
                   )
                    {
                        right = i;
                        i = 0;
                        break;
                    }
                }
            }
            //下边线：从height=Max，width++开始找到第一个点
            for (int i = bit.Height - 20; i > bit.Height/2; i--)
            {
                for (int j = 20; j < bit.Width - 20; j++)
                {
                    if (bit.GetPixel(j, i).R + bit.GetPixel(j, i).G + bit.GetPixel(j, i).B < 760
                )
                    {
                        down = i;
                        i = 0;
                        break;
                    }
                }
            }
            return isWhite;
        }
        /// <summary>
        /// 从0，0开始检测位移。
        /// </summary>
        /// <param name="bit"></param>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="up"></param>
        /// <param name="down"></param>
        public void FindPicAll(Bitmap bit, out int left, out int right, out int up, out int down)
        {
            left = 0; right = 0; up = 0; down = 0;
            //左边线： 从width=0，height++开始找到第一个点
            for (int i = 0; i < bit.Width ; i++)
            {
                for (int j =0; j < bit.Height; j++)
                {
                    if (bit.GetPixel(i, j).R == 0)
                    {
                        left = i;
                        i = bit.Width;
                        break;
                    }
                }
            }
            //上边线：从height=0，width++开始找到第一个点
            for (int i =0; i < bit.Height ; i++)
            {
                for (int j = 0; j < bit.Width ; j++)
                {
                    if (bit.GetPixel(j, i).R == 0)
                    {
                        up = i;
                        i = bit.Height;
                        break;
                    }
                }
            }
            //右边线：从width=max，height++开始找到第一个点
            for (int i = bit.Width-1; i > 0; i--)
            {
                for (int j = 0; j < bit.Height ; j++)
                {
                    if (bit.GetPixel(i, j).R == 0)
                    {
                        right = i;
                        i = 0;
                        break;
                    }
                }
            }
            //下边线：从height=Max，width++开始找到第一个点
            for (int i = bit.Height-1 ; i > 0; i--)
            {
                for (int j = 0; j < bit.Width ; j++)
                {
                    if (bit.GetPixel(j, i).R == 0)
                    {
                        down = i;
                        i = 0;
                        break;
                    }
                }
            }
        }

        public bool FindPicAllCaise(Bitmap bit, out int left, out int right, out int up, out int down)
        {
            bool iswhite = false;//不是白页
            left = 0; right = 0; up = 0; down = 0;
            //左边线： 从width=0，height++开始找到第一个点
            for (int i = 0; i < bit.Width/2; i++)
            {
                for (int j = 0; j < bit.Height; j++)
                {
                    //bit.GetPixel(i, j).R + bit.GetPixel(i, j).G + bit.GetPixel(i, j).B < 760
                    if (bit.GetPixel(i, j).R + bit.GetPixel(i, j).G + bit.GetPixel(i, j).B < 750
                    )
                    {
                        left = i;
                        i = bit.Width;
                        break;
                    }
                    if (i==bit.Width/2-1 &&j==bit.Height-1)
                    {
                        iswhite = true;
                    }
                }
            }
            //上边线：从height=0，width++开始找到第一个点
            for (int i = 0; i < bit.Height/2; i++)
            {
                for (int j = 0; j < bit.Width; j++)
                {
                    //bit.GetPixel(j, i).R + bit.GetPixel(j, i).G + bit.GetPixel(j, i).B < 760
                    if (bit.GetPixel(j, i).R + bit.GetPixel(j, i).G + bit.GetPixel(j, i).B < 749
          )
                    {
                        up = i;
                        i = bit.Height;
                        break;
                    }
                }
            }
            //右边线：从width=max，height++开始找到第一个点
            for (int i = bit.Width - 1; i > bit.Width/2; i--)
            {
                for (int j = 0; j < bit.Height; j++)
                {
                    //bit.GetPixel(i, j).R + bit.GetPixel(i, j).G + bit.GetPixel(i, j).B < 760
                    if (bit.GetPixel(i, j).R + bit.GetPixel(i, j).G + bit.GetPixel(i, j).B < 760
              )
                    {
                        right = i;
                        i = 0;
                        break;
                    }
                }
            }
            //下边线：从height=Max，width++开始找到第一个点
            for (int i = bit.Height - 1; i > bit.Height/2; i--)
            {
                for (int j = 0; j < bit.Width; j++)
                {
                    //bit.GetPixel(j, i).R + bit.GetPixel(j, i).G + bit.GetPixel(j, i).B < 760
                    if (bit.GetPixel(j, i).R + bit.GetPixel(j, i).G + bit.GetPixel(j, i).B < 760
            )
                    {
                        down = i;
                        i = 0;
                        break;
                    }
                }
            } return iswhite;
        }
        //调整图片
        public Bitmap tiaozhengtp(String filepath, int x, int y)
        {
            Bitmap bm = new Bitmap(filepath);

            if (x != 0)
            {
                //横向移动
                bm = hengxiangyd(bm, x);
            }

            if (y != 0)
            {
                bm = zongxiangyd(bm, y);
            }
            GC.Collect();
            GC.WaitForPendingFinalizers();


            return bm;
        }

        //不是白 r+g+b 255 255 255
        public bool isContent(Bitmap bit,int x,int y)
        {
            bool iscontent=false;//没有内容
            if (bit.GetPixel(x, y).R + bit.GetPixel(x, y).G + bit.GetPixel(x, y).B < 760
     || bit.GetPixel(bit.Width - x, y).R + bit.GetPixel(bit.Width - x, y).G + bit.GetPixel(bit.Width - x, y).B < 760
     || bit.GetPixel(x, bit.Height - y).R + bit.GetPixel(x, bit.Height - y).G + bit.GetPixel(x, bit.Height - y).B < 760
     || bit.GetPixel(bit.Width - x, bit.Height - y).R + bit.GetPixel(bit.Width - x, bit.Height - y).G + bit.GetPixel(bit.Width - x, bit.Height - y).B < 760)
	{
		 //如果检测到四个角落有一点不是白色，就是有背景之类的东西
        iscontent = true;
	}
 
       return iscontent;
        }

        /// <summary>
        /// 输入要处理的图片路径，偏移量。要保存的宽和高。返回制作好的图片
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="saveWidth"></param>这个要保存图的宽度不等于原图-位移量
        /// <param name="saveHeight"></param>
        /// <returns></returns>
        public Bitmap tiaozhengtpNew(string fileName, int x, int y, int saveWidth, int saveHeight)
        {
            Bitmap bitYuanTif = new Bitmap(fileName);//要处理的图
            Bitmap bitSave = new Bitmap(saveWidth, saveHeight);//要保存的大小
            Graphics g = Graphics.FromImage(bitSave);

            //图片1 宽 大于 图片2 但可能高小于图片2
            if (saveWidth>bitYuanTif.Width )
            {
                
            }
            //位移可能为正可能为负。

            //图片1 宽 小于 图片2  但可能高大于图片2

            
            Rectangle rect1 = new Rectangle(-x, -y, saveWidth, saveHeight);//原图的哪个部分哪个大小拿来绘制
            Rectangle destRectangle2 = new Rectangle(0, 0, saveWidth, saveHeight);// 在新图上什么位置开始绘制什么大小
            g.DrawImage(bitYuanTif, destRectangle2, rect1, GraphicsUnit.Pixel);
            if (bitYuanTif.Height < saveHeight)//如果新图的高又比原图的高大
            {
                Rectangle rect3 = new Rectangle(-x, 2 * bitYuanTif.Height - saveWidth, saveWidth, saveHeight - bitYuanTif.Height);//原图的哪个部分哪个大小拿来绘制
                Rectangle rect4 = new Rectangle(0, saveHeight - bitYuanTif.Height, saveWidth, saveHeight - bitYuanTif.Height);// 在新图上什么位置开始绘制什么大小
                g.DrawImage(bitYuanTif, rect4, rect3, GraphicsUnit.Pixel);
            }
            bitSave.SetResolution(300, 300);
            bitYuanTif.Dispose();
            g.Dispose();
            return bitSave;
        }

        public Bitmap CutTiff(string fileName, int x, int y)
        {
            Bitmap bit;
            if (IsBit1Bigger == false)//图2大
            {
                bit = new Bitmap(fileName1);
            }
            else//图1大
            {
                bit = new Bitmap(fileName2);
            }
            Bitmap bitYuanTif = new Bitmap(fileName);
            Bitmap bitSave = new Bitmap(bit.Width, bit.Height);//要保存的大小

            Graphics g = Graphics.FromImage(bitSave);
            Rectangle rect1 = new Rectangle(-xcy, -ycy, bit.Width, bit.Height);//原图的哪个部分哪个大小拿来绘制
            Rectangle destRectangle2 = new Rectangle(0, 0, bit.Width, bit.Height);// 在新图上什么位置开始绘制什么大小
            g.DrawImage(bitYuanTif, destRectangle2, rect1, GraphicsUnit.Pixel);
            bitSave.SetResolution(300, 300);
            g.Dispose();
            bit.Dispose();
            return bitSave;
        }


        //将大图按小图大小保存,无位移.
        public Bitmap CutBigTiff(Bitmap littlebit, Bitmap bigbit)
        {
            Bitmap bitSave = new Bitmap(littlebit.Width, littlebit.Height);//要保存的大小
            Graphics g = Graphics.FromImage(bitSave);
            Rectangle rect1 = new Rectangle(0, 0, littlebit.Width, littlebit.Height);//原图的哪个部分哪个大小拿来绘制
            Rectangle destRectangle2 = new Rectangle(0, 0, littlebit.Width, littlebit.Height);// 在新图上什么位置开始绘制什么大小
            g.DrawImage(bigbit, destRectangle2, rect1, GraphicsUnit.Pixel);//新大图
            bitSave.SetResolution(300, 300);
            g.Dispose();
            littlebit.Dispose();
            bigbit.Dispose();
            return bitSave;
        }

        /// <summary>
        /// *将图片从左边起裁固定尺寸
        /// </summary>
        /// <param name="bit">要处理的图片</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        /// <returns></returns>
        public Bitmap CutStipulateSizeFromLeft(Bitmap bit,int width,int height)
        {
            Bitmap bitSave = new Bitmap(width,height);//要保存的大小
            Graphics g = Graphics.FromImage(bitSave);
            Rectangle rect1 = new Rectangle(0, 0, width, height);//原图的哪个部分哪个大小拿来绘制
            Rectangle destRectangle2 = new Rectangle(0, 0, width,height);// 在新图上什么位置开始绘制什么大小
            g.DrawImage(bit, destRectangle2, rect1, GraphicsUnit.Pixel);//新大图
           // bitSave.SetResolution(300, 300);
            g.Dispose();
            bit.Dispose();
            return bitSave;
        }



        public Bitmap Get1_(Bitmap bitSource,int i)
        {
            Bitmap bitSourceS = new Bitmap(bitSource.Width/1, bitSource.Height/i);
            Graphics g = Graphics.FromImage(bitSourceS);
            Rectangle rect1 = new Rectangle(0, 0, bitSource.Width/1, bitSource.Height/i);
            Rectangle destRectangle2 = new Rectangle(0, 0, bitSourceS.Width, bitSourceS.Height);
            g.DrawImage(bitSource, destRectangle2, rect1, GraphicsUnit.Pixel);
            bitSourceS.SetResolution(300, 300);
            g.Dispose();
            bitSource.Dispose();
            return bitSourceS;
        }



        private Bitmap hengxiangyd(Bitmap bm, int jl)
        {
            int width = bm.Width;
            int height = bm.Height;

            Bitmap bmjg = new Bitmap(width, height);//bm结果？
            BitmapData bdjg = bmjg.LockBits(new Rectangle(0, 0, bmjg.Width, bmjg.Height), ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            System.IntPtr Scanjg = bdjg.Scan0;
            int stridejg = bdjg.Stride;


            BitmapData bd = bm.LockBits(new Rectangle(0, 0, bm.Width, bm.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            System.IntPtr Scan = bd.Scan0;
            int stride = bd.Stride;

            unsafe
            {
                byte* p = (byte*)Scan;
                int offset = stride - width * 3;

                byte* pjg = (byte*)Scanjg;
                int offsetjg = stridejg - width * 3;


                //横向移动
                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        int px = i + jl;

                        if (px < 0)
                        {
                            px = 0;
                        }
                        else if (px > width - 1)
                        {
                            px = width - 1;
                        }

                        p = (byte*)Scan;
                        p = p + (j * width * 3) + j * offset;
                        p = p + px * 3;

                        pjg = (byte*)Scanjg;
                        pjg = pjg + (j * width * 3) + j * offsetjg;
                        pjg = pjg + i * 3;

                        pjg[0] = p[0];
                        pjg[1] = p[1];
                        pjg[2] = p[2];
                    }
                }



            }

            bm.UnlockBits(bd);
            bmjg.UnlockBits(bdjg);
            bmjg.SetResolution(150, 150);

            bm.Dispose();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            return bmjg;
        }


        private Bitmap zongxiangyd(Bitmap bm, int jl)
        {
            int width = bm.Width;
            int height = bm.Height;

            Bitmap bmjg = new Bitmap(width, height);
            BitmapData bdjg = bmjg.LockBits(new Rectangle(0, 0, bmjg.Width, bmjg.Height), ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            System.IntPtr Scanjg = bdjg.Scan0;
            int stridejg = bdjg.Stride;


            BitmapData bd = bm.LockBits(new Rectangle(0, 0, bm.Width, bm.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            System.IntPtr Scan = bd.Scan0;
            int stride = bd.Stride;

            unsafe
            {
                byte* p = (byte*)Scan;
                int offset = stride - width * 3;

                byte* pjg = (byte*)Scanjg;
                int offsetjg = stridejg - width * 3;

                //纵向移动
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        int py = i + jl;
                        if (py < 0)
                        {
                            py = 0;
                        }
                        else if (py > height - 1)
                        {
                            py = height - 1;
                        }

                        p = (byte*)Scan;
                        p = p + (py * width * 3) + py * offset;
                        p = p + j * 3;

                        pjg = (byte*)Scanjg;
                        pjg = pjg + (i * width * 3) + i * offsetjg;
                        pjg = pjg + j * 3;


                        pjg[0] = p[0];
                        pjg[1] = p[1];
                        pjg[2] = p[2];
                    }
                }



            }

            bm.UnlockBits(bd);
            bmjg.UnlockBits(bdjg);
            bmjg.SetResolution(150, 150);
            bm.Dispose();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            return bmjg;
        }

        #region 灰度化、二值化
        public Bitmap huiduhua(Bitmap bit)
        {
            int width = bit.Width;
            int height = bit.Height;

            BitmapData bd = bit.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            System.IntPtr Scan0 = bd.Scan0;
            int stride = bd.Stride;

            //int bytes = width * height * 3;
            //byte[] rgbvalues = new byte[bytes];
            //System.Runtime.InteropServices.Marshal.Copy(Scan0, rgbvalues, 0, bytes);

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
                        //colorTemp = (p[2] + p[1] + p[0]) / 3;
                        p[2] = p[1] = p[0] = (byte)colorTemp;
                        //MessageBox.Show(p[2].ToString() + "**" + p[1].ToString() + "**" + p[0].ToString() + "**" + colorTemp.ToString());
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

        //图像二值化
        public Bitmap getez(Bitmap bit)
        {
            int width = bit.Width;
            int height = bit.Height;

            BitmapData bd = bit.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            System.IntPtr Scan0 = bd.Scan0;
            int stride = bd.Stride;

            //int bytes = width * height * 3;
            //byte[] rgbvalues = new byte[bytes];
            //System.Runtime.InteropServices.Marshal.Copy(Scan0, rgbvalues, 0, bytes);

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

            //bit = KiResizeImage(bit, zzwidth, zzheight);
            //zhjc.SetResolution(zzjd, zzjd);
            GC.Collect();
            GC.WaitForPendingFinalizers();
            return bit;
        }
        #endregion

        #region 颜色比较
        private unsafe bool ScanColor(byte b1, byte g1, byte r1, byte b2, byte g2, byte r2, int similar)
        {
            if ((Math.Abs(b1 - b2)) > similar)
            {
                return false;
            } //B

            if ((Math.Abs(g1 - g2)) > similar)
            {
                return false;
            } //G

            if ((Math.Abs(r1 - r2)) > similar)
            {
                return false;
            } //R

            return true;
        }

        private unsafe bool ScanColor(byte b1, byte g1, byte r1, byte b2, byte g2, byte r2)
        {
            if ((Math.Abs(b1 - b2)) > bjjdz)
            {
                return false;
            } //B

            if ((Math.Abs(g1 - g2)) > bjjdz)
            {
                return false;
            } //G

            if ((Math.Abs(r1 - r2)) > bjjdz)
            {
                return false;
            } //R

            return true;
        }
        #endregion

    }


    public class PointColor
    {
        private int _X = 0;
        private int _Y = 0;

        private byte _R = 0;
        private byte _G = 0;
        private byte _B = 0;

        public int X
        {
            get { return _X; }
            set { _X = value; }
        }

        public int Y
        {
            get { return _Y; }
            set { _Y = value; }
        }


        public byte R
        {
            get { return _R; }
            set { _R = value; }
        }

        public byte G
        {
            get { return _G; }
            set { _G = value; }
        }

        public byte B
        {
            get { return _B; }
            set { _B = value; }
        }

        public PointColor()
        {
        }

        public PointColor(int X, int Y, byte R, byte G, byte B)
        {
            this.X = X;
            this.Y = Y;
            this.R = R;
            this.G = G;
            this.B = B;
        }
    }
}
