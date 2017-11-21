using System.Windows.Forms; 

namespace feature1.Util
{
    class DialogHelper
    {
        private const string IMAGE_FILTER = @"影像文件 (*.tif;*.jpg;*.png;*.bmp;*.pdf)|*.jpg;*.png;*.tif;*.bmp;*.pdf";
        private const string PDF_FILTER = @"文档文件 (*.pdf)|*.pdf";


        private static int LastFiterIndex = 0;

        public static string ShowOpenImageFile()
        {
            using (var dlg = new OpenFileDialog())
            {
                dlg.InitialDirectory = Application.StartupPath;
                dlg.Title = "选择影像文件";
                dlg.AddExtension = true; 
                dlg.Filter = IMAGE_FILTER;
                dlg.FilterIndex = LastFiterIndex;
                dlg.Multiselect = false;
                dlg.FileName = "";
            
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    LastFiterIndex = dlg.FilterIndex;
                    return dlg.FileName;
                }
            }
            return "";
        }

        public static string[] ShowOpenImageFiles()
        {
            using (var dlg = new OpenFileDialog())
            {
                dlg.InitialDirectory = System.Windows.Forms.Application.StartupPath;
                dlg.Title = "选择影像文件";
                dlg.AddExtension = true;
                dlg.DefaultExt = "tif";
                dlg.Filter = IMAGE_FILTER;
                dlg.FilterIndex = LastFiterIndex;
                dlg.Multiselect = true;
                dlg.FileName = "";

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    LastFiterIndex = dlg.FilterIndex;
                    return dlg.FileNames;
                }
            }
            return new string[0];
        }

        /// <summary>
        /// 打开pdf文件
        /// </summary>
        /// <returns>返回文件路径</returns>
        public static string ShowOpenPdfFile()
        {
            using (var dlg = new OpenFileDialog())
            {
                dlg.Title = "选择文件";
                dlg.AddExtension = true;
                dlg.DefaultExt = "pdf";
                dlg.Filter = PDF_FILTER;
                //dlg.FilterIndex = LastFiterIndex;
                dlg.Multiselect = false;
                dlg.FileName = "";

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    LastFiterIndex = dlg.FilterIndex;
                    return dlg.FileName;
                }
            }
            return string.Empty;
        }

        private static string _lastSelectPath = "";

        //获取默认文件打开路径
        public static string ShowOpenFolder(string defaultFolder="")
        {
            using (var dlg = new System.Windows.Forms.FolderBrowserDialog())
            {
                if (string.IsNullOrEmpty(defaultFolder))
                {
                    defaultFolder = _lastSelectPath;
                }

                dlg.SelectedPath = defaultFolder;
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    _lastSelectPath = dlg.SelectedPath;
                    return dlg.SelectedPath;
                }
            }
            return "";
        }

        //public static string[] ShowScanImageFiles()
        //{
        //    using (var scanDlg = new ScanImageDlg())
        //    {
        //        if (scanDlg.ShowDialog() == DialogResult.OK)
        //        {
        //            return scanDlg.ScanFiles;
        //        }
        //        else
        //        {
        //            scanDlg.Close();
        //        }
        //    }
        //    return new string[0];
        //}

       
    }
}
