using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.InteropServices;

namespace AutoTest.UI.Controls
{
    /// <summary>
    /// 自定义MyBrowser控件，直接继承自WebBrowser来扩展
    /// </summary>
    public class MyBrowser : WebBrowser
    {
        public event EventHandler<MyBrowserPrintScreenEventArgs> PrintScreen;
        public string PrintScreenFilePath { get; set; }

        protected virtual void OnPrintScreen(MyBrowserPrintScreenEventArgs e)
        {
            // 获取网页高度和宽度,也可以自己设置
            var height = this.Document.Body.ScrollRectangle.Height;
            var width = this.Document.Body.ScrollRectangle.Width;

            var screenHeight = this.Size.Height;
            //var screenHeight = this.FindForm().Height;
            //MessageBox.Show(height.ToString());
            //MessageBox.Show(this.FindForm().Height.ToString());

            var pages = (height - 1) / screenHeight + 1;

            /*WebBrowser自带截图方法无法使用
            var bitmap = new Bitmap(width, height);
            this.DrawToBitmap(bitmap, new Rectangle(0, 0, width, height));
            bitmap.Save(e.PrintScreenFilePath);
            //*/

            //MessageBox.Show(pages.ToString());


            //*强制只截屏首页
            pages = 1;
            //*/

            for (int i = 0; i < pages; i++)
            {
                this.Document.Window.ScrollTo(0, i * screenHeight);

                //*网上找到的调用windows底层系统api抓屏
                IntPtr myIntptr = this.Handle;
                int hwndInt = myIntptr.ToInt32();
                IntPtr hwnd = myIntptr;

                // Set hdc to the bitmap

                Bitmap bm = new Bitmap(this.Size.Width, this.Size.Height);
                //Bitmap bm = new Bitmap(width, height);
                Graphics g = Graphics.FromImage(bm);
                IntPtr hdc = g.GetHdc();

                // Snapshot the WebBrowser

                bool result = PrintWindow(hwnd, hdc, 0);
                g.ReleaseHdc(hdc);
                g.Flush();

                bm.Save(e.PrintScreenFilePath.Replace(".jpg", "_" + i + ".jpg"));
            }

            if (PrintScreen != null)
            {
                PrintScreen(this, e);
            }
        }

        public void DoPrintScreen(string filePath = null)
        {
            filePath = filePath ?? PrintScreenFilePath;
            var e = new MyBrowserPrintScreenEventArgs() { PrintScreenFilePath = filePath };
            OnPrintScreen(e);
        }

        [DllImport("user32.dll")]
        private static extern bool PrintWindow(IntPtr hwnd, IntPtr hdcBlt, uint nFlags);
    }

    public class MyBrowserPrintScreenEventArgs : EventArgs
    {
        public string PrintScreenFilePath { get; set; }
    }
}
