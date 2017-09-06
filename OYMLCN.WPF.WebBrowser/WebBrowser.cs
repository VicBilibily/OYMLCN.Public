﻿using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Threading;
using mshtml;
using static OYMLCN.WPF.WebBrowserDelegate;
using System.Runtime.InteropServices;
using System.Text;
using OYMLCN;

namespace OYMLCN.WPF
{
    /// <summary>
    /// WebBrowserExtension
    /// </summary>
    public static class WebBrowserExtension
    {
        /// <summary>
        /// 获取浏览器文档以进行操作（如getElementById获取html元素）
        /// </summary>
        /// <param name="wb"></param>
        /// <returns></returns>
        public static HTMLDocument GetDocument(this WebBrowser wb) => wb.Document as HTMLDocument;
        /// <summary>
        /// 获取主窗口以便于执行相关操作（如execScript执行js代码）
        /// </summary>
        /// <param name="wb"></param>
        /// <returns></returns>
        public static IHTMLWindow2 GetParentWindow(this WebBrowser wb) => wb.GetDocument().parentWindow;
        /// <summary>
        /// 获取网站Cookies
        /// </summary>
        /// <param name="wb"></param>
        /// <returns></returns>
        public static string GetCookies(this WebBrowser wb) => OYMLCN.WebBrowserExtension.GetCookies(wb.Source.GetHost());
        /// <summary>
        /// 执行Js代码（可有返回值）
        /// </summary>
        /// <param name="wb"></param>
        /// <param name="scriptStr"></param>
        /// <returns></returns>
        public static object ExecuteScript(this WebBrowser wb, string scriptStr) =>
            wb.GetParentWindow().execScript(scriptStr);
    }

    /// <summary>
    /// WebBrowserDelegate
    /// </summary>
    public class WebBrowserDelegate
    {
        /// <summary>
        /// 默认处理委托
        /// </summary>
        public delegate void DelfaultHandler();
        /// <summary>
        /// 变更处理委托
        /// </summary>
        /// <param name="text"></param>
        public delegate void TextChangeHandler(string text);
        /// <summary>
        /// 新窗口事件处理委托
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="e"></param>
        public delegate void NewWindowHandler(Uri uri, CancelEventArgs e);
        /// <summary>
        /// 导航处理委托
        /// </summary>
        /// <param name="url"></param>
        /// <param name="e"></param>
        public delegate void NavigateHandler(string url, CancelEventArgs e);
        /// <summary>
        /// 导航结束处理委托
        /// </summary>
        /// <param name="url"></param>
        public delegate void NavigatedHandler(string url);
        /// <summary>
        /// 加载进度处理委托
        /// </summary>
        /// <param name="current"></param>
        /// <param name="total"></param>
        public delegate void ProgressChangeHandler(int current, int total);
        /// <summary>
        /// 执行状态处理委托
        /// </summary>
        /// <param name="command"></param>
        /// <param name="enable"></param>
        public delegate void CommandStateChangeHandler(long command, bool enable);
        ///// <summary>
        ///// 文件下载处理委托
        ///// </summary>
        ///// <param name="e"></param>
        //public delegate void FileDownloadHandler(CancelEventArgs e);
        /// <summary>
        /// 页面加载结束处理委托
        /// </summary>
        /// <param name="url"></param>
        public delegate void LoadedHandler(string url);
    }

    /// <summary>
    /// WebBrowserHelper
    /// </summary>
    public partial class WebBrowserHelper : IDisposable
    {
        /// <summary>
        /// 设置浏览器不弹错误提示框
        /// </summary>
        /// <param name="silent">是否静默</param>
        public void SetSilent(bool silent = true)
        {
            FieldInfo fi = typeof(WebBrowser).GetField("_axIWebBrowser2", BindingFlags.Instance | BindingFlags.NonPublic);
            if (fi != null)
            {
                object browser = fi.GetValue(_webBrowser);
                if (browser != null)
                    browser.GetType().InvokeMember("Silent", BindingFlags.SetProperty, null, browser, new object[] { silent });
            }
        }

        private WebBrowser _webBrowser;
        private object _cookie;

        /// <summary>
        /// 辅助绑定浏览器事件
        /// </summary>
        /// <param name="webBrowser"></param>
        public WebBrowserHelper(WebBrowser webBrowser)
        {
            _webBrowser = webBrowser ?? throw new ArgumentNullException("webBrowser");
            _webBrowser.Dispatcher.BeginInvoke(new Action(Attach), DispatcherPriority.Loaded);
        }

        /// <summary>
        /// 程序关闭时释放所有反射钩子
        /// </summary>
        private void Disconnect()
        {
            if (_cookie != null)
            {
                _cookie.ReflectInvokeMethod("Disconnect", new Type[] { }, null);
                _cookie = null;
            }
        }
        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            Disconnect();
        }

        private void Attach()
        {
            var axIWebBrowser2 = _webBrowser.ReflectGetProperty("AxIWebBrowser2");
            var webBrowserEvent = new WebBrowserEvent(this);
            var cookieType = typeof(WebBrowser).Assembly.GetType("MS.Internal.Controls.ConnectionPointCookie");
            _cookie = Activator.CreateInstance(
                cookieType,
                ReflectionService.BindingFlags,
                null,
                new[] { axIWebBrowser2, webBrowserEvent, typeof(IDWebBrowserEvents2) },
                CultureInfo.CurrentUICulture);
        }


        /// <summary>
        /// 如果新窗口地址获取出错则使用最近的状态栏地址
        /// </summary>
        public bool IfNewWindowErrorUseStatusUrl { get; set; } = false;
        /// <summary>
        /// 新窗口事件处理
        /// </summary>
        public event NewWindowHandler NewWindow;
        private void OnNewWindow(ref bool cancel)
        {
            var eventArgs = new CancelEventArgs(cancel);
            if (NewWindow != null)
            {
                string url = string.Empty;
                try
                {
#if NET35
                    url = ((HTMLAnchorElementClass)((HTMLDocumentClass)_webBrowser.Document).activeElement).href;
#else
                    dynamic document = _webBrowser.Document;
                    url = document.activeElement.href;
#endif
                }
                catch
                {
                    if (IfNewWindowErrorUseStatusUrl && url.IsNullOrEmpty())
                        url = statusText;
                }
                try
                {
                    NewWindow(new Uri(url), eventArgs);
                    cancel = eventArgs.Cancel;
                }
                catch
                {
                    cancel = true;
                }
            }
        }
        /// <summary>
        /// 标题变更处理
        /// </summary>
        public event TextChangeHandler TitleChange;
        private void OnTitleChange(string text) => TitleChange?.Invoke(text);
        /// <summary>
        /// 页面加载结束处理
        /// </summary>
        public event LoadedHandler Loaded;
        private void OnLoad(ref object url)
        {
            if (url != null && Loaded != null)
                Loaded.Invoke(url as string);
        }
        /// <summary>
        /// 开始导航处理
        /// </summary>
        public event NavigateHandler BeforeNavigate;
        private void OnBeforeNavigate(string url, ref bool cancel)
        {
            var eventArgs = new CancelEventArgs(cancel);
            BeforeNavigate?.Invoke(url, eventArgs);
            cancel = eventArgs.Cancel;
        }
        /// <summary>
        /// 导航结束处理
        /// </summary>
        public event NavigatedHandler Navigated;
        private void OnNavigateComplete(string url)
        {
            Navigated?.Invoke(url);
            if (NewWindow != null)
                _webBrowser.ExecuteScript("window.open=function(url){location.href=url;}");
        }
        /// <summary>
        /// 导航错误处理
        /// </summary>
        public event NavigateHandler NavigateError;
        private void OnNavigateError(string url, ref bool cancel)
        {
            var eventArgs = new CancelEventArgs(cancel);
            NavigateError?.Invoke(url, eventArgs);
            cancel = eventArgs.Cancel;
        }
        private string statusText = string.Empty;
        /// <summary>
        /// 状态信息变更处理
        /// </summary>
        public event TextChangeHandler StatusTextChange;
        private void OnStatusTextChange(string text)
        {
            // 用于记录部分不可捕获的新窗口链接
            if (!text.IsNullOrEmpty())
                statusText = text;
            StatusTextChange?.Invoke(text);
        }
        /// <summary>
        /// 加载进度处理
        /// </summary>
        public event ProgressChangeHandler ProgressChange;
        private void OnProgressChange(int curr, int total) => ProgressChange?.Invoke(curr, total);
        /// <summary>
        /// 执行状态处理
        /// </summary>
        public event CommandStateChangeHandler CommandStateChange;
        private void OnCommandStateChange(long command, bool enable) => CommandStateChange?.Invoke(command, enable);


        ///// <summary>
        ///// 文件下载事件处理
        ///// </summary>
        //public event FileDownloadHandler FileDownload;
        //private void OnFileDownload(ref bool cancel)
        //{
        //    var eventArgs = new CancelEventArgs(cancel);
        //    FileDownload?.Invoke(eventArgs);
        //    cancel = eventArgs.Cancel;
        //}
    }

}
