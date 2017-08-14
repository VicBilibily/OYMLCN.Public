﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace OYMLCN
{
    /// <summary>
    /// StringExtension
    /// </summary>
    public static partial class StringExtension
    {
        /// <summary>
        /// 生成随机字符
        /// </summary>
        /// <param name="length">字符串长度</param>
        /// <param name="blur">是否包含特殊符号</param>
        /// <returns></returns>
        public static string RandCode(int length = 6, bool blur = false)
        {
            if (length <= 0)
                return "";
            if (blur)
            {
                const string letters = "ABCDEFGHIJKLMNOPQRSTWXYZ";
                const string numbers = "0123456789";
                const string symbols = "~!@#$%^&*()_-+=[{]}|><,.?/";
                var hash = string.Empty;
                var rand = new Random();

                for (var cont = 0; cont < length; cont++)
                    switch (rand.Next(0, 3))
                    {
                        case 1:
                            hash = hash + numbers[rand.Next(0, 10)];
                            break;
                        case 2:
                            hash = hash + symbols[rand.Next(0, 26)];
                            break;
                        default:
                            hash = hash + ((cont % 3 == 0)
                                ? letters[rand.Next(0, 24)].ToString()
                                : (letters[rand.Next(0, 24)]).ToString().ToLower());
                            break;
                    }
                return hash;
            }
            else
            {
                string sCode = "";
                string codeSerial = "0,1,2,3,4,5,6,7,8,9,a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,x,y,z,A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z";
                string[] arr = codeSerial.Split(',');
                int randValue = -1;
                Random rand = new Random(unchecked((int)DateTime.Now.Ticks));
                for (int i = 0; i < length; i++)
                {
                    randValue = rand.Next(0, arr.Length - 1);
                    sCode += arr[randValue];
                }
                return sCode;
            }
        }
        /// <summary>
        /// 生成随机字符
        /// </summary>
        /// <param name="str"></param>
        /// <param name="length">字符串长度</param>
        /// <returns></returns>
        public static string RandCode(this string str, int length = 6) => RandCode(length);
        /// <summary>
        /// 生成带特殊符号的随机字符串
        /// </summary>
        /// <param name="str"></param>
        /// <param name="length">字符串长度</param>
        /// <returns></returns>
        public static string RandBlurCode(this string str, int length) => RandCode(length, true);


        /// <summary>
        /// 判断字符串是否为空
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this string str) => string.IsNullOrEmpty(str);
        /// <summary>
        /// 判断字符串是否为空/空格
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsNullOrWhiteSpace(this string str) => string.IsNullOrWhiteSpace(str);
        /// <summary>
        /// 判断字符串是否为空格类型
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsWhiteSpace(this string value)
        {
            foreach (var c in value)
            {
                if (char.IsWhiteSpace(c)) continue;
                return false;
            }
            return true;
        }

        /// <summary>
        /// 对比两个字符串是否相等
        /// </summary>
        /// <param name="str"></param>
        /// <param name="value"></param>
        /// <param name="comparison"></param>
        /// <returns></returns>
        public static bool IsEqual(this string str, string value, StringComparison comparison = StringComparison.Ordinal) => (str.IsNull() || value.IsNull()) ? false : str.Equals(value, comparison);

        /// <summary>
        /// 判断字符串是否是邮箱地址
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsEmail(this string str) =>
            str.IsNullOrEmpty() ? false : new Regex(@"[\w!#$%&'*+/=?^_`{|}~-]+(?:\.[\w!#$%&'*+/=?^_`{|}~-]+)*@(?:[\w](?:[\w-]*[\w])?\.)+[\w](?:[\w-]*[\w])?").IsMatch(str.Trim());


        /// <summary>
        /// 截取字符串
        /// </summary>
        /// <param name="str"></param>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <returns></returns>
        public static string Sub(this string str, int startIndex, int endIndex) =>
            str.Substring(startIndex, endIndex - startIndex);

        /// <summary>
        /// 截取字符串
        /// </summary>
        /// <param name="str"></param>
        /// <param name="startIndex"></param>
        /// <param name="subLength"></param>
        /// <returns></returns>
        public static string SubString(this string str, int startIndex, int subLength = int.MaxValue) =>
            new string(str.Skip(startIndex).Take(subLength).ToArray());


        /// <summary>
        /// 根据占位符紧接多个字符串 即string.Format
        /// </summary>
        /// <param name="str"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static string StringFormat(this string str, params string[] param) => string.Format(str, param);
        /// <summary>
        /// 分割字符串未每一个单字
        /// </summary>
        /// <param name="str">待分割字符串</param>
        /// <returns></returns>
        public static string[] StringToArray(this string str) => str.Select(x => x.ToString()).ToArray();
        /// <summary>
        /// 判断字符是不是汉字
        /// </summary>
        /// <param name="text">待判断字符或字符串</param>
        /// <returns>true是 false不是</returns>
        public static bool IsChineseRegString(this string text) => Regex.IsMatch(text, @"[\u4e00-\u9fbb]+$");


        /// <summary>
        /// 将Boolean转换为Yes是或No否
        /// </summary>
        /// <param name="boolean"></param>
        /// <param name="cnString">是否返回中文是/否</param>
        /// <returns></returns>
        public static string ToYesNo(this bool boolean, bool cnString = true) => cnString ? boolean ? "是" : "否" : boolean ? "Yes" : "No";


        /// <summary>
        /// 根据标志分割字符串
        /// </summary>
        /// <param name="str"></param>
        /// <param name="sign">分割标识</param>
        /// <param name="option">分割结果去重方式</param>
        /// <returns></returns>
        public static string[] SplitBySign(this string str, string sign, StringSplitOptions option = StringSplitOptions.None) => str?.Split(new string[] { sign }, option);
        /// <summary>
        /// 根据标志分割字符串后获得第一个字符串
        /// </summary>
        /// <param name="str"></param>
        /// <param name="sign"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        public static string SplitThenGetFirst(this string str, string sign, StringSplitOptions option = StringSplitOptions.RemoveEmptyEntries) => str?.SplitBySign(sign, option).FirstOrDefault();
        /// <summary>
        /// 根据标志分割字符串后获得最后一个字符串
        /// </summary>
        /// <param name="str"></param>
        /// <param name="sign">分割标志</param>
        /// <param name="option">分割结果去重方式</param>
        /// <returns></returns>
        public static string SplitThenGetLast(this string str, string sign, StringSplitOptions option = StringSplitOptions.RemoveEmptyEntries) => str?.SplitBySign(sign, option).LastOrDefault();
        /// <summary>
        /// 根据标志分割字符串
        /// </summary>
        /// <param name="str"></param>
        /// <param name="signs">分割标识，多重标识</param>
        /// <returns></returns>
        public static string[] SplitByMultiSign(this string str, params string[] signs) => str?.Split(signs, StringSplitOptions.RemoveEmptyEntries);


        /// <summary>
        /// 根据 | \ / 、 ， , 空格 中文空格 制表符空格换行 分割字符串
        /// </summary>
        /// <param name="str"></param>
        /// <param name="option">分割结果去重方式</param>
        /// <returns></returns>
        public static string[] SplitAuto(this string str, StringSplitOptions option = StringSplitOptions.RemoveEmptyEntries) =>
            str?.SplitByMultiSign("|", "\\", "/", "、", ":", "：", "，", ",", "　", " ", "\t");

        /// <summary>
        /// 根据换行符拆分字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string[] SplitByLine(this string str) => str?.SplitByMultiSign("\r\n", "\r", "\n");



        /// <summary>
        /// 将所有换行及其前后多余的空格替换掉合并为一行
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string AllInOneLine(this string str) =>
            str.RemoveNormalWithRegex("\r\n", "\r", "\n");

        /// <summary>
        /// 去除过多的换行符
        /// </summary>
        /// <param name="html"></param>
        /// <param name="keep">文本中保留的换行符最大数量（文本末尾的换行将不会保留）</param>
        /// <returns></returns>
        public static string RemoveWrap(this string html, byte keep = 0)
        {
            string[] param = new string[] { "\r\n", "\r", "\n" };
            if (keep == 0)
                return html.ReplaceNormalWithRegex(string.Empty, param);

            var stop = "._RWHS_.";
            var word = html.ReplaceNormalWithRegex(stop, param);
            string wrap = string.Empty;
            for (var i = 0; i < keep; i++)
                wrap += "\r\n";
            return word.SplitBySign(stop, StringSplitOptions.RemoveEmptyEntries).Join(wrap);
        }



        /// <summary>
        /// 转化为半角字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToDBC(this string str)
        {
            var c = str.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                // 全角空格为12288，半角空格为32
                if (c[i] == 12288)
                {
                    c[i] = (char)32;
                    continue;
                }
                // 其他字符半角(33-126)与全角(65281-65374)的对应关系是：均相差65248
                if (c[i] > 65280 && c[i] < 65375)
                    c[i] = (char)(c[i] - 65248);
            }
            return new string(c);
        }
        /// <summary>
        /// 转化为全角字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToSBC(this string str)
        {
            var c = str.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] == 32)
                {
                    c[i] = (char)12288;
                    continue;
                }
                if (c[i] > 32 && c[i] < 127)
                    c[i] = (char)(c[i] + 65248);
            }
            return new string(c);
        }
        /// <summary>
        /// ASCII转小写
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToLowerForASCII(this string value)
        {
            if (value.IsNullOrWhiteSpace())
                return value;
            var sb = new StringBuilder(value.Length);
            foreach (var c in value)
                if (c < 'A' || c > 'Z')
                    sb.Append(c);
                else
                    sb.Append((char)(c + 0x20));
            return sb.ToString();
        }
        /// <summary>
        /// ASCII转大写
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns> 
        public static string ToUpperForASCII(this string value)
        {
            if (value.IsNullOrWhiteSpace())
                return value;
            var sb = new StringBuilder(value.Length);
            foreach (var c in value)
                if (c < 'a' || c > 'z')
                    sb.Append(c);
                else
                    sb.Append((char)(c - 0x20));
            return sb.ToString();
        }




        /// <summary>
        /// 转换网页地址为Uri
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static Uri ToUri(this string str) => new Uri(str);
        /// <summary>
        /// 转换网页地址为Uri(失败返回null)
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static Uri ToNullableUri(this string str)

        {
            try
            {
                return new Uri(str);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 获取url字符串的的协议域名地址
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string UrlGetHost(this string url) => url.ToUri().GetHost();
        /// <summary>
        /// 获取Uri的协议域名地址
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static string GetHost(this Uri uri) => uri.IsNull() ? null : $"{uri.Scheme}://{uri.Host}/";


        /// <summary>
        /// 将单个Cookie字符串转换为Cookie类型
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static Cookie ToCookie(this string str)
        {
            if (str.IsNullOrEmpty())
                return null;
            var index = str.IndexOf('=');
            return new Cookie(str.SubString(0, index).Trim(), str.SubString(++index, int.MaxValue));
        }
        /// <summary>
        /// 将Cookies字符串转换为CookieCollection集合
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static CookieCollection ToCookieCollection(this string str)
        {
            var result = new CookieCollection();
            foreach (var cookie in str?.SplitBySign(";") ?? new string[0])
                result.Add(cookie.ToCookie());
            return result;
        }

        /// <summary>
        /// 将字符串填充到Steam中
        /// </summary>
        /// <param name="str"></param>
        /// <param name="encoder">默认使用UTF-8进行编码</param>
        /// <returns></returns>
        public static Stream StringToStream(this string str, Encoding encoder = null) => new MemoryStream(str.StringToBytes(encoder));

        /// <summary>
        /// 将字符串填充到byte[]字节流中
        /// </summary>
        /// <param name="str"></param>
        /// <param name="encoder">默认使用UTF-8进行编码</param>
        /// <returns></returns>
        public static byte[] StringToBytes(this string str, Encoding encoder = null) => encoder?.GetBytes(str) ?? Encoding.UTF8.GetBytes(str);


        /// <summary>
        /// 正则匹配所有结果
        /// </summary>
        /// <param name="str"></param>
        /// <param name="pattern"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static string[] RegexMatches(this string str, string pattern, RegexOptions options = RegexOptions.None)
        {
            var result = new List<string>();
            if (!str.IsNullOrEmpty())
            {
                var data = Regex.Matches(str, pattern, options);
                foreach (var item in data)
                    result.Add(item.ToString());
            }
            return result.ToArray();
        }

        /// <summary>
        /// 将字符串数组拼接成字符串
        /// </summary>
        /// <param name="list"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string Join(this IEnumerable<string> list, string separator = "") => string.Join(separator, list);

        /// <summary>
        /// 替换字符串
        /// </summary>
        /// <param name="str"></param>
        /// <param name="newValue">新值</param>
        /// <param name="oldValue">旧值</param>
        /// <returns></returns>
        public static string ReplaceNormal(this string str, string newValue, params string[] oldValue)
        {
            foreach (var item in oldValue)
                str = str.Replace(item, newValue);
            return str;
        }

        /// <summary>
        /// 使用正则匹配替换字符串
        /// </summary>
        /// <param name="str"></param>
        /// <param name="newValue">新值</param>
        /// <param name="oldValue">旧值(不能包含正则占位符)</param>
        /// <returns></returns>
        public static string ReplaceNormalWithRegex(this string str, string newValue, params string[] oldValue) =>
            oldValue.Length == 0 ? str : Regex.Replace(str, $"({oldValue.Join("|")})", newValue);

        /// <summary>
        /// 使用正则匹配替换字符串（忽略大小写）
        /// </summary>
        /// <param name="str"></param>
        /// <param name="newValue">新值</param>
        /// <param name="oldValue">旧值(不能包含正则占位符)</param>
        public static string ReplaceIgnoreCaseWithRegex(this string str, string newValue, params string[] oldValue) =>
            oldValue.Length == 0 ? str : Regex.Replace(str, $"({oldValue.Join("|")})", newValue, RegexOptions.Compiled | RegexOptions.IgnoreCase);

        /// <summary>
        /// 移除字符串
        /// </summary>
        /// <param name="str"></param>
        /// <param name="word">需要移除的字符</param>
        /// <returns></returns>
        public static string RemoveNormal(this string str, params string[] word) =>
            ReplaceNormal(str, string.Empty, word);

        /// <summary>
        /// 使用正则匹配移除字符串
        /// </summary>
        /// <param name="str"></param>
        /// <param name="word">需要移除的字符(不能包含正则占位符)</param>
        /// <returns></returns>
        public static string RemoveNormalWithRegex(this string str, params string[] word) =>
            word.Length == 0 ? str : str.RegexMatches($"[^({word.Join("|")})]").Join();
        /// <summary>
        /// 使用正则匹配移除字符串
        /// </summary>
        /// <param name="str"></param>
        /// <param name="word">需要移除的字符(不能包含正则占位符)</param>
        /// <returns></returns>
        public static string RemoveIgnoreCaseWithRegex(this string str, params string[] word) =>
            word.Length == 0 ? str : str.RegexMatches($"[^({word.Join("|")})]", RegexOptions.Compiled | RegexOptions.IgnoreCase).Join();


        /// <summary>
        /// 判断字符串是否以指定字符开头
        /// </summary>
        /// <param name="s"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static bool IsBeginWith(this string s, char c) =>
            s.IsNullOrEmpty() ? false : s[0] == c;
        /// <summary>
        /// 判断字符串是否以指定字符开头
        /// </summary>
        /// <param name="s"></param>
        /// <param name="chars"></param>
        /// <returns></returns>
        public static bool IsBeginWithAny(this string s, IEnumerable<char> chars) =>
            s.IsNullOrEmpty() ? false : chars.Contains(s[0]);
        /// <summary>
        /// 判断字符串是否以指定字符开头
        /// </summary>
        /// <param name="s"></param>
        /// <param name="chars"></param>
        /// <returns></returns>
        public static bool IsBeginWithAny(this string s, params char[] chars) =>
            s.IsBeginWithAny(chars.AsEnumerable());
        /// <summary>
        /// 判断字符串是否以指定字符开头
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="comparisonType"></param>
        /// <returns></returns>
        public static bool IsBeginWith(this string a, string b, StringComparison comparisonType = StringComparison.Ordinal) =>
            (a.IsNull() || b.IsNull()) ? false : a.StartsWith(b, comparisonType);
#if NET452
        /// <summary>
        /// 判断字符串是否以指定字符开头
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="ignoreCase"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public static bool IsBeginWith(this string a, string b, bool ignoreCase, CultureInfo culture) =>
            (a.IsNull() || b.IsNull()) ? false : a.StartsWith(b, ignoreCase, culture);
#endif
        /// <summary>
        /// 判断字符串是否以指定字符结尾
        /// </summary>
        /// <param name="s"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static bool IsFinishWith(this string s, char c)
        {
            if (s.IsNullOrEmpty()) return false;
            return s.Last() == c;
        }
        /// <summary>
        /// 判断字符串是否以指定字符结尾
        /// </summary>
        /// <param name="s"></param>
        /// <param name="chars"></param>
        /// <returns></returns>
        public static bool IsFinishWithAny(this string s, IEnumerable<char> chars) =>
            s.IsNullOrEmpty() ? false : chars.Contains(s.Last());
        /// <summary>
        /// 判断字符串是否以指定字符结尾
        /// </summary>
        /// <param name="s"></param>
        /// <param name="chars"></param>
        /// <returns></returns>
        public static bool IsFinishWithAny(this string s, params char[] chars) =>
            s.IsFinishWithAny(chars.AsEnumerable());
        /// <summary>
        /// 判断字符串是否以指定字符结尾
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="comparisonType"></param>
        /// <returns></returns>
        public static bool IsFinishWith(this string a, string b, StringComparison comparisonType = StringComparison.Ordinal) =>
            (a.IsNull() || b.IsNull()) ? false : a.EndsWith(b, comparisonType);
#if NET452
        /// <summary>
        /// 判断字符串是否以指定字符结尾
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="ignoreCase"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public static bool IsFinishWith(this string a, string b, bool ignoreCase, CultureInfo culture) =>
            (a.IsNull() || b.IsNull()) ? false : a.EndsWith(b, ignoreCase, culture);
#endif

        /// <summary>
        /// 返回一个值，该值指示指定的子串是否出现在此字符串中。
        /// </summary>
        /// <param name="str"></param>
        /// <param name="words"></param>
        /// <returns></returns>
        public static bool Contains(this string str, params string[] words)
        {
            var contain = false;
            foreach (var word in words)
                if (contain = str.Contains(word))
                    break;
            return contain;
        }

    }
}
