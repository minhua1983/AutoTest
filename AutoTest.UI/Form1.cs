using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using AutoTest.UI.Controls;
using AutoTest.UI.Models;
using Newtonsoft.Json;

namespace AutoTest.UI
{
    public partial class Form1 : Form
    {
        object lockHelper = new object();
        bool isWorking = false;
        MyBrowser myBrowser;
        MyTask myTask;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Width = 1920;
            this.Height = 1080;
            using (StreamReader reader = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + "/config.json"))
            {
                var data = reader.ReadToEnd();
                myTask = JsonConvert.DeserializeObject<MyTask>(data);

                myBrowser = new MyBrowser();
                myBrowser.ScriptErrorsSuppressed = true;
                myBrowser.Dock = DockStyle.Fill;
                this.Controls.Add(myBrowser);

                var url = myTask.Url;

                myBrowser.Navigate(url);
                myBrowser.PrintScreenFilePath = @"e:/printscreen.jpg";
                myBrowser.DocumentCompleted += MyBrowser_DocumentCompleted;
            }
        }



        private void MyBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (myBrowser.ReadyState == WebBrowserReadyState.Complete)
            {
                //myBrowser.PrintScreen();
                if (!isWorking)
                {
                    lock (lockHelper)
                    {
                        if (!isWorking)
                        {
                            isWorking = true;

                            if (myTask.CurrentActionIndex < myTask.ActionList.Count)
                            {
                                foreach (MyStep myStep in myTask.ActionList[myTask.CurrentActionIndex].StepList)
                                {
                                    HtmlDocument document = myBrowser.Document;
                                    HtmlElement htmlElement = null;

                                    //以Id找到对象
                                    if (myStep.Id != "")
                                    {
                                        htmlElement = document.GetElementById(myStep.Id);
                                    }
                                    else
                                    {
                                        htmlElement = document.Body;
                                    }

                                    //以Pattern找到对象
                                    if (myStep.Pattern != "")
                                    {
                                        /*
                                        foreach (HtmlElement tempHtmlElement in document.All)
                                        {
                                            if (tempHtmlElement.OuterHtml.IndexOf(myElement.Pattern) == 0) htmlElement = tempHtmlElement;
                                        }
                                        //*/
                                        htmlElement = GetHtmlElement(htmlElement, myStep.Pattern);
                                    }

                                    //设置Value
                                    if (myStep.Value != "")
                                    {
                                        htmlElement.SetAttribute("value", myStep.Value);
                                    }

                                    //执行javascript脚本
                                    if (myStep.Script != "")
                                    {
                                        HtmlElement script = document.CreateElement("script");
                                        script.InnerHtml = myStep.Script;
                                        document.Body.AppendChild(script);
                                    }

                                    /*快照
                                    myBrowser.DoPrintScreen(@"e:/printscreen_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".jpg");
                                    //*/

                                    //执行Invoke
                                    if (myStep.Invoke != "")
                                    {
                                        if (htmlElement.TagName.ToLower() == "a") htmlElement.SetAttribute("target", "_self");
                                        htmlElement.InvokeMember(myStep.Invoke);
                                    }

                                    var i = 0;
                                    var sleepTime = 50;
                                    while (i < 1000)
                                    {
                                        Application.DoEvents();
                                        Thread.Sleep(sleepTime);
                                        i = i + sleepTime;
                                    }
                                }
                                myTask.CurrentActionIndex++;
                            }

                            isWorking = false;
                        }
                    }
                }
            }
        }

        HtmlElement GetHtmlElement(HtmlElement htmlElement, string pattern)
        {
            var newPattern = "";
            var tagName = "";
            var position = 1;
            var tagPatterns = pattern.Split('/');

            tagName = tagPatterns[0].Split('[')[0];
            position = int.Parse(tagPatterns[0].Split('[')[1].Replace("]", ""));

            if (tagPatterns[0] != pattern) newPattern = pattern.Substring(tagPatterns[0].Length + 1);

            var tagCount = 1;
            foreach (HtmlElement element in htmlElement.Children)
            {
                //MessageBox.Show(element.TagName);
                if (element.TagName.ToLower() == tagName)
                {
                    if (tagCount == position)
                    {
                        if (newPattern == "")
                        {
                            return element;
                        }
                        else
                        {
                            //MessageBox.Show(newPattern);
                            return GetHtmlElement(element, newPattern);
                        }
                    }
                    tagCount++;
                }
            }

            return null;
        }
    }
}
