﻿using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OYMLCN
{
    /// <summary>
    /// HtmlAgilityPackExtension
    /// </summary>
    public static class HtmlAgilityPackExtension
    {
        /// <summary>
        /// 获取精简后的HTML(空的div会被移除)
        /// </summary>
        /// <param name="hn"></param>
        /// <param name="removeDataAttribute">移除data-属性</param>
        /// <param name="removeMeta">移除head的meta标签</param>
        /// <param name="removeInlineStyle">移除内联样式</param>
        /// <param name="removeEventAttribute">移除on事件属性</param>
        /// <param name="allInOneLine">标记是否合并为一行</param>
        /// <returns></returns>
        public static string GetCleanHtml(this HtmlNode hn, bool removeDataAttribute = false, bool removeMeta = true, bool removeInlineStyle = true, bool removeEventAttribute = true, bool allInOneLine = true)
        {
            if (removeInlineStyle)
            {
                var nodeStyles = hn.SelectNodes("//@style");
                if (nodeStyles != null)
                    foreach (var style in nodeStyles)
                        style.Attributes["style"]?.Remove();
            }
            if (removeDataAttribute || removeEventAttribute)
            {
                var nodeData = hn.SelectNodes("//*").Where(d => d.Attributes.Count > 0).Select(d => d.Attributes).ToArray();
                foreach (var attributes in nodeData)
                {
                    if (removeDataAttribute)
                        foreach (var data in attributes.Where(d => d.Name.StartsWith("data-", StringComparison.OrdinalIgnoreCase)).ToArray())
                            data.Remove();
                    if (removeEventAttribute)
                        foreach (var data in attributes.Where(d => d.Name.StartsWith("on", StringComparison.OrdinalIgnoreCase)).ToArray())
                            data.Remove();
                }
            }
            if (removeMeta)
                hn.RemoveNodes("//meta");

            hn.RemoveEmptyNodes("//div");
            if (allInOneLine)
                return hn.OwnerDocument.DocumentNode.InnerHtml.AllInOneLine().RemoveSpace().Replace("> <","><");
            else
                return hn.OwnerDocument.DocumentNode.InnerHtml.RemoveSpace().SplitByLine().Select(d => d.Trim()).ToArray().Join("\r\n");
        }

        /// <summary>
        /// 获取去除HTML标签后的文本（block元素会自动换行）
        /// </summary>
        /// <param name="hn"></param>
        /// <returns></returns>
        public static string GetCleanText(this HtmlNode hn)
        {
            hn.RemoveNodes("//img", "//map", "//audio", "//canvas");
            string html = hn.InnerHtml;

            var block = new string[] {
                "address","article","aside",
                "blockquote",
                "caption","code","cite",
                "div","dl","details",
                "footer",
                @"h(\d)","header",
                "label","ul","ol","li",
                "nav",
                "p","pre",
                "q",
                "table","tr","textarea",
                "select","section",
            };

            return html
                .ReplaceHtmlBr()
                .ReplaceIgnoreCaseWithRegex("\r\n", block.Select(d => $"<{d}>").ToArray())
                .RemoveSpace()
                .HtmlDecode()
                .RemoveHtml()
                .SplitByLine()
                .Select(d => d.Trim()).ToArray()
                .Join("\r\n");
        }

        /// <summary>
        /// 将字符串转换为Html便捷操作模式
        /// </summary>
        /// <param name="html"></param>
        /// <param name="removeComment">移除注释</param>
        /// <param name="removeScriptLinkStyle">移除脚本样式相关区块</param>
        /// <returns></returns>
        public static HtmlNode AsAgilityHtml(this string html, bool removeComment = true, bool removeScriptLinkStyle = true)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            if (removeComment)
            {
                var comments = doc.DocumentNode.SelectNodes("//comment()");
                if (comments != null)
                    foreach (var comment in comments)
                        comment.ParentNode.RemoveChild(comment);
            }
            if (removeScriptLinkStyle)
            {
                foreach (var script in doc.DocumentNode.Descendants("script").ToArray())
                    script.Remove();
                foreach (var style in doc.DocumentNode.Descendants("style").ToArray())
                    style.Remove();
                foreach (var link in doc.DocumentNode.Descendants("link").ToArray())
                    link.Remove();
            }
            return doc.DocumentNode;
        }

        /// <summary>
        /// 从Dom中移除符合条件的元素
        /// </summary>
        /// <param name="hn"></param>
        /// <param name="xpath"></param>
        /// <returns></returns>
        public static HtmlNode RemoveNodes(this HtmlNode hn, params string[] xpath)
        {
            foreach (var item in xpath)
            {
                var nodes = hn.SelectNodes(item);
                if (nodes != null)
                    foreach (var ele in nodes)
                        ele.Remove();
            }
            return hn;
        }

        /// <summary>
        /// 移除空节点
        /// </summary>
        /// <param name="hn"></param>
        /// <param name="xpath"></param>
        /// <returns></returns>
        public static HtmlNode RemoveEmptyNodes(this HtmlNode hn, params string[] xpath)
        {
            foreach (var item in xpath)
            {
                var empties = hn.SelectNodes(item)?.Where(d => d.ChildNodes.Count == 0).ToArray();
                do
                {
                    if (empties != null)
                        foreach (var empty in empties)
                            empty.Remove();
                    empties = hn.SelectNodes(item)?.Where(d => d.InnerHtml.Trim().IsNullOrEmpty()).ToArray();
                }
                while (empties?.Length > 0);
            }
            return hn;
        }

        /// <summary>
        /// 获取指定路径内元素的Html
        /// </summary>
        /// <param name="hn"></param>
        /// <param name="xpath"></param>
        /// <returns></returns>
        public static string GetInnerHtml(this HtmlNode hn, string xpath) => hn?.SelectSingleNode(xpath)?.InnerHtml?.Trim();
        /// <summary>
        /// 获取指定路径内元素的文本
        /// </summary>
        /// <param name="hn"></param>
        /// <param name="xpath"></param>
        /// <returns></returns>
        public static string GetInnerText(this HtmlNode hn, string xpath) => hn?.SelectSingleNode(xpath)?.InnerText?.HtmlDecode().Trim();

        /// <summary>
        /// 获取指定路径元素的属性值
        /// </summary>
        /// <param name="gn"></param>
        /// <param name="xpath"></param>
        /// <param name="attr">属性名称</param>
        /// <param name="def">默认返回值</param>
        /// <returns></returns>
        public static string GetAttributeValue(this HtmlNode gn, string xpath, string attr, string def) =>
            gn?.SelectSingleNode(xpath)?.GetAttributeValue(attr, def);



        /// <summary>
        /// 获取具有指定属性名的元素
        /// </summary>
        /// <param name="hnc"></param>
        /// <param name="attrName"></param>
        /// <returns></returns>
        public static HtmlNode GetAttributeNode(this HtmlNode hnc, string attrName) =>
            hnc.GetAttributeNodes(attrName)?.FirstOrDefault();
        /// <summary>
        /// 获取具有指定属性名以及包含指定值的元素
        /// </summary>
        /// <param name="hnc"></param>
        /// <param name="attrName"></param>
        /// <param name="attrValue"></param>
        /// <returns></returns>
        public static HtmlNode GetAttributeNode(this HtmlNode hnc, string attrName, string attrValue) =>
            hnc.GetAttributeNodes(attrName, attrValue)?.FirstOrDefault();


        /// <summary>
        /// 获取指定标签名称的所有元素
        /// </summary>
        /// <param name="hn"></param>
        /// <param name="tagName">标签名</param>
        /// <returns></returns>
        public static IEnumerable<HtmlNode> GetDescendants(this HtmlNode hn, string tagName) => hn.Descendants(tagName);

        /// <summary>
        /// 获取具有指定属性名的元素集合
        /// </summary>
        /// <param name="hnc"></param>
        /// <param name="attrName"></param>
        /// <returns></returns>
        public static IEnumerable<HtmlNode> GetAttributeNodes(this HtmlNode hnc, string attrName) =>
            hnc.ChildNodes.GetAttributeNodes(attrName);
        /// <summary>
        /// 获取具有指定属性名以及包含指定值的元素集合
        /// </summary>
        /// <param name="hnc"></param>
        /// <param name="attrName"></param>
        /// <param name="attrValue"></param>
        /// <returns></returns>
        public static IEnumerable<HtmlNode> GetAttributeNodes(this HtmlNode hnc, string attrName, string attrValue) =>
            hnc.ChildNodes.GetAttributeNodes(attrName, attrValue);

        /// <summary>
        /// 获取具有指定属性名的元素
        /// </summary>
        /// <param name="hnc"></param>
        /// <param name="attrName"></param>
        /// <returns></returns>
        public static HtmlNode GetAttributeNode(this HtmlNodeCollection hnc, string attrName) =>
            hnc.GetAttributeNodes(attrName)?.FirstOrDefault();
        /// <summary>
        /// 获取具有指定属性名以及包含指定值的元素
        /// </summary>
        /// <param name="hnc"></param>
        /// <param name="attrName"></param>
        /// <param name="attrValue"></param>
        /// <returns></returns>
        public static HtmlNode GetAttributeNode(this HtmlNodeCollection hnc, string attrName, string attrValue) =>
            hnc.GetAttributeNodes(attrName, attrValue)?.FirstOrDefault();

        /// <summary>
        /// 获取具有指定属性名的元素集合
        /// </summary>
        /// <param name="hnc"></param>
        /// <param name="attrName"></param>
        /// <returns></returns>
        public static IEnumerable<HtmlNode> GetAttributeNodes(this HtmlNodeCollection hnc, string attrName) =>
            hnc.Where(d => d.Attributes.Where(i => i.Name == attrName).Any());
        /// <summary>
        /// 获取具有指定属性名以及包含指定值的元素集合
        /// </summary>
        /// <param name="hnc"></param>
        /// <param name="attrName"></param>
        /// <param name="attrValue"></param>
        /// <returns></returns>
        public static IEnumerable<HtmlNode> GetAttributeNodes(this HtmlNodeCollection hnc, string attrName, string attrValue) =>
            hnc.Where(d => d.Attributes.Where(i => i.Name == attrName && i.Value.Contains(attrValue)).Any());



        /// <summary>
        /// 获取具有指定Id名称的元素
        /// </summary>
        /// <param name="hnc"></param>
        /// <param name="idName"></param>
        /// <returns></returns>
        public static HtmlNode GetIdNode(this HtmlNode hnc, string idName) =>
            hnc.ChildNodes.Where(d => d.Id == idName).FirstOrDefault();
        /// <summary>
        /// 获取具有指定Id名称的元素
        /// </summary>
        /// <param name="hnc"></param>
        /// <param name="idName"></param>
        /// <returns></returns>
        public static HtmlNode GetIdNode(this HtmlNodeCollection hnc, string idName) =>
            hnc.Where(d => d.Id == idName).FirstOrDefault();

        /// <summary>
        /// 获取包含指定样式名称的元素
        /// </summary>
        /// <param name="hnc"></param>
        /// <param name="className"></param>
        /// <returns></returns>
        public static HtmlNode GetClassNameNode(this HtmlNode hnc, string className) =>
            hnc.GetAttributeNode("class", className);
        /// <summary>
        /// 获取包含指定样式名称的元素集合
        /// </summary>
        /// <param name="hnc"></param>
        /// <param name="className"></param>
        /// <returns></returns>
        public static IEnumerable<HtmlNode> GetClassNameNodes(this HtmlNode hnc, string className) =>
            hnc.GetAttributeNodes("class", className);
        /// <summary>
        /// 获取包含指定样式名称的元素
        /// </summary>
        /// <param name="hnc"></param>
        /// <param name="className"></param>
        /// <returns></returns>
        public static HtmlNode GetClassNameNode(this HtmlNodeCollection hnc, string className) =>
            hnc.GetAttributeNode("class", className);
        /// <summary>
        /// 获取包含指定样式名称的元素集合
        /// </summary>
        /// <param name="hnc"></param>
        /// <param name="className"></param>
        /// <returns></returns>
        public static IEnumerable<HtmlNode> GetClassNameNodes(this HtmlNodeCollection hnc, string className) =>
            hnc.GetAttributeNodes("class", className);

    }
}
