using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.Util;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Emgu.CV.Features2D;
using System.Diagnostics;
using System.Drawing;
using Emgu.CV.XFeatures2D;

using System.Runtime.InteropServices;
using Emgu.CV.CvEnum;
#if !__IOS__
using Emgu.CV.Cuda;
#endif


namespace feature1.Util

{
    class Match
    {
          public Match()
        {
        }

        public static void FindMatch(Mat modelImage, Mat observedImage, out long matchTime, out VectorOfKeyPoint modelKeyPoints, out VectorOfKeyPoint observedKeyPoints, VectorOfVectorOfDMatch matches, out Mat mask, out Mat homography)
        {
            int k = 2;
            double uniquenessThreshold = 0.8;
            double hessianThresh = 300;
            Stopwatch watch;
            homography = null;
            modelKeyPoints = new VectorOfKeyPoint();
            observedKeyPoints = new VectorOfKeyPoint();
            using (UMat uModelImage = modelImage.GetUMat(AccessType.Read))
            using (UMat uObservedImage = observedImage.GetUMat(AccessType.Read))
            {
                SURF surfCPU = new SURF(hessianThresh);
                //extract features from the object image
                UMat modelDescriptors = new UMat();
                surfCPU.DetectAndCompute(uModelImage, null, modelKeyPoints, modelDescriptors, false);

                watch = Stopwatch.StartNew();

                // extract features from the observed image
                UMat observedDescriptors = new UMat();
                surfCPU.DetectAndCompute(uObservedImage, null, observedKeyPoints, observedDescriptors, false);
                BFMatcher matcher = new BFMatcher(DistanceType.L2);
                matcher.Add(modelDescriptors);

                matcher.KnnMatch(observedDescriptors, matches, k, null);
                mask = new Mat(matches.Size, 1, DepthType.Cv8U, 1);
                mask.SetTo(new MCvScalar(255));
                Features2DToolbox.VoteForUniqueness(matches, uniquenessThreshold, mask);

                int nonZeroCount = CvInvoke.CountNonZero(mask);
                if (nonZeroCount >= 4)
                {
                    nonZeroCount = Features2DToolbox.VoteForSizeAndOrientation(modelKeyPoints, observedKeyPoints,
                       matches, mask, 1.5, 20);
                    if (nonZeroCount >= 4)
                        homography = Features2DToolbox.GetHomographyMatrixFromMatchedFeatures(modelKeyPoints,
                           observedKeyPoints, matches, mask, 2);
                }

                watch.Stop();
            }
            matchTime = watch.ElapsedMilliseconds;
        }


        /// <summary>
        /// EmguCV缩放图像的双三次插值算法实现 速度1~3ms resize算法可直接使用更加快。
        /// </summary>
        /// <param name="bit">要缩放的图像</param>
        /// <param name="newWidth">宽度</param>
        /// <param name="newHeight">高度</param>
        /// <returns>新图像</returns>
        public static Bitmap ResizeUsingEmguCV(Bitmap bit,int newWidth,int newHeight)
        {
            try
            {
                Emgu.CV.Image<Emgu.CV.Structure.Gray, byte> image = new Image<Gray, byte>(bit);
                Image<Gray, byte> newImage = image.Resize(newWidth, newHeight, Emgu.CV.CvEnum.Inter.Cubic);
                return newImage.Bitmap;

            }
            catch             {

                return null;
            }

           
        }


        /// <summary>
        /// GDI+实现两次立方插值算法缩放图像。  10ms
        /// </summary>
        /// <param name="bit"></param>
        /// <param name="newWidth"></param>
        /// <param name="newHeight"></param>
        /// <returns></returns>
        public static Bitmap ResizeUsingGDIPlus(Bitmap bit,int newWidth,int newHeight)
        {
            try
            {
                Bitmap bitmap = new Bitmap(newWidth,newHeight);
                Graphics g = Graphics.FromImage(bitmap);
                //插值算法的质量
                g.InterpolationMode=System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.DrawImage(bit,new Rectangle(0,0,newWidth,newHeight),new Rectangle(0,0,bit.Width,bit.Height),GraphicsUnit.Pixel);
                g.Dispose();
                return bitmap;
            }
            catch 
            {
                return null;
            }
        }


        public void DrawReturn10multiple(Mat modelImage, Mat observedImage, out long matchTime, out float xCount, out float yCount)
        {

            Mat homography;
            VectorOfKeyPoint modelKeyPoints;
            VectorOfKeyPoint observedKeyPoints;
            using (VectorOfVectorOfDMatch matches = new VectorOfVectorOfDMatch())
            {
                Mat mask;
                FindMatch(modelImage, observedImage, out matchTime, out modelKeyPoints, out observedKeyPoints, matches,
                   out mask, out homography);

                //Draw the matched keypoints
                Mat result = new Mat();
                Features2DToolbox.DrawMatches(modelImage, modelKeyPoints, observedImage, observedKeyPoints,
                   matches, result, new MCvScalar(255, 255, 255), new MCvScalar(255, 255, 255), mask);

                #region draw the projected region on the image

                xCount = 0; yCount = 0;
                if (homography != null)
                {
                    //draw a rectangle along the projected model
                    Rectangle rect = new Rectangle(Point.Empty, modelImage.Size);
                    PointF[] pts = new PointF[]
                    {
                  new PointF(rect.Left, rect.Bottom),
                  new PointF(rect.Right, rect.Bottom),
                  new PointF(rect.Right, rect.Top),
                  new PointF(rect.Left, rect.Top)
                    };
                    pts = CvInvoke.PerspectiveTransform(pts, homography);

                    Point[] points = Array.ConvertAll<PointF, Point>(pts, Point.Round);
                    xCount = pts[3].X;
                    yCount = pts[3].Y;

                    xCount = floatToInt10Muilti(-xCount); yCount = floatToInt10Muilti(-yCount);
                }
                #endregion
            }
        }


        public int floatToInt10Muilti(float f)
        {
            int i = 0;
            f = f * 10;
            if (f > 0)      //正数  
            {

                i = (int)(f + 0.3);
            }
            else if (f < 0) //负数  
            {
                i = -(int)(Math.Abs(f) + 0.3);
            }
            else
            {
                i = 0;
            }
            return i;
        }


        #region 
        //public void FindMatch(Image<Gray, Byte> modelImage, Image<Gray, byte> modelimage, out long matchTime,  out HomographyMatrix homography)
        //{            
        //     VectorOfKeyPoint modelKeyPoints;
        //    VectorOfKeyPoint modelkeyPoints;
        //    Matrix<int> indices;
        //     Matrix<byte> mask;
        //    int k = 2;
        //    double uniquenessThreshold = 0.8;


        //    Stopwatch watch;
        //    homography = null;
        //    modelKeyPoints = new VectorOfKeyPoint();
        //    Matrix<float> modelDescriptors = surfCPU.DetectAndCompute(modelImage, null, modelKeyPoints);
        //    watch = Stopwatch.StartNew();
        //    modelkeyPoints = new VectorOfKeyPoint();
        //    Matrix<float> observedDescriptors = surfCPU.DetectAndCompute(modelimage, null, modelkeyPoints);
        //    if (modelDescriptors!=null && observedDescriptors!=null)
        //    {                            
        //        BruteForceMatcher<float> matcher = new BruteForceMatcher<float>(DistanceType.L2);
        //        matcher.Add(modelDescriptors);
        //        indices = new Matrix<int>(observedDescriptors.Rows, k);
        //        using (Matrix<float> dist = new Matrix<float>(observedDescriptors.Rows, k))
        //        {
        //            matcher.KnnMatch(observedDescriptors, indices, dist, k, null);
        //            mask = new Matrix<byte>(dist.Rows, 1);
        //            mask.SetValue(255);
        //            Features2DToolbox.VoteForUniqueness(dist, uniquenessThreshold, mask);
        //        }
        //        int nonZeroCount = CvInvoke.cvCountNonZero(mask);
        //        if (nonZeroCount >= 4)
        //        {
        //            nonZeroCount = Features2DToolbox.VoteForSizeAndOrientation(modelKeyPoints, modelkeyPoints, indices, mask, 1.5, 20);
        //            if (nonZeroCount >= 4)
        //                homography = Features2DToolbox.GetHomographyMatrixFromMatchedFeatures(modelKeyPoints, modelkeyPoints, indices, mask, 2);
        //        }
        //    }            
        //    watch.Stop();
        //    matchTime = watch.ElapsedMilliseconds;
        //    modelKeyPoints.Dispose();
        //    modelkeyPoints.Dispose();
        //    //modelImage.Dispose();
        //    //modelimage.Dispose();
        //}


        //public void Draw(Image<Gray, Byte> modelimage, Image<Gray, byte> modelImage, out long matchTime, out float xCount, out float yCount)
        //{
        //    HomographyMatrix homography;
        //    xCount = 0;
        //    yCount = 0;
        //    FindMatch(modelimage, modelImage, out matchTime, out homography);          
        //    if (homography != null )
        //    { 
        //        Rectangle rect = modelimage.ROI;
        //        PointF[] pts = new PointF[] {new PointF(rect.Left,rect.Bottom),new PointF(rect.Right,rect.Bottom),new PointF(rect.Right,rect.Top),new PointF(rect.Left,rect.Top)};
        //        homography.ProjectPoints(pts);
        //        xCount = pts[3].X;
        //        yCount = pts[3].Y;               
        //    }
        //    else
        //    {
        //        xCount = 0;
        //        yCount = 0;
        //    }
        //    modelimage.Dispose();
        //    modelImage.Dispose();
        //}
        #endregion



    }
}
