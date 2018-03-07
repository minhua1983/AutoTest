# 自动化处理
这是一个自动化处理的程序，可以用于在web页面执行预先定义好的过程。
如，可以完成如下过程的过程。
* 来到后台登陆界面。
* 自动填充预先配置好的账号和密码，并自行登陆。
* 访问某个后台页面，进行表单提交。
* 自动退出登陆。

## 项目环境
* .net framework 4.5.2

## 项目结构
* AutoTest，winform主程序

## 其他说明
* AutoTest.UI\Controls\MyWebBrowser.cs
这是一个继承于WebBrowser的自定义控件，主要实现了基于windows底层API的截图功能。
