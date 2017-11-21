using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace feature1
{
    public partial class Help : Form
    {
        public Help()
        {
            InitializeComponent();
        }

        private void Help_Load(object sender, EventArgs e)
        {
            richTextBox1.Text = "工具介绍：\r\n" +
                "相似度测试：选择文件夹1和选择文件夹2.然后点击相似度测试。会自动进行一一对应的文件夹内图片的相似度对比。最后生成图片报告到桌面\r\n" +
                "裁掉空白-图像文件1：选择图像文件1可多选。然后点击“裁掉空白-图像文件1”.会自动进行图像文件的裁空白，最后自动保存到文件夹外面新建的new文件夹里\r\n"
                +"输出特定字的所有字体：输入一些字，会自动生成所有字体到桌面字体文件夹\r\n"
                +""
                ;
        }
    }
}
