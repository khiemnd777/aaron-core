using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Aaron.Core
{
    public enum TitleStyle
    {
        UpperCase,
        FirstCaps
    }

    public class ImageDimension
    {
        public int Width{get;set;}
        public int Height{get;set;}
    }

    public static class TextExtensions
    {
        public static string ToAscii(this string text)
        {
            var unicodeString = text.ToLower();
            unicodeString = Regex.Replace(unicodeString, "[áàảãạâấầẩẫậăắằẳẵặ]", "a");
            unicodeString = Regex.Replace(unicodeString, "[éèẻẽẹêếềểễệ]", "e");
            unicodeString = Regex.Replace(unicodeString, "[iíìỉĩị]", "i");
            unicodeString = Regex.Replace(unicodeString, "[óòỏõọơớờởỡợôốồổỗộ]", "o");
            unicodeString = Regex.Replace(unicodeString, "[úùủũụưứừửữự]", "u");
            unicodeString = Regex.Replace(unicodeString, "[yýỳỷỹỵ]", "y");
            unicodeString = Regex.Replace(unicodeString, "[đ]", "d");

            return unicodeString;
        }

        public static string ToSystemName(this string text)
        {
            var unicodeString = ToAscii(text);
            unicodeString = Regex.Replace(unicodeString, "-", "");
            unicodeString = Regex.Replace(unicodeString, @"\b[a-z]\w+", delegate (Match m)
            {
                string v = m.ToString();
                if (m.Index == 0) return v;
                return char.ToUpper(v[0]) + v.Substring(1);
            });

            unicodeString = Regex.Replace(unicodeString, @"\s", "");

            return unicodeString;
        }

        public static string ToSEName(this string text)
        {
            text = ToAscii(text);
            text = Regex.Replace(text, @"\W", delegate(Match m)
            {
                string v = m.ToString();
                if (m.Index == text.Length - 1 || m.Index == 0) 
                    return "";
                return '-' + v.Substring(1);
            });
            return text;
        }

        public static string ToTitle(this string text, TitleStyle style)
        {
            var unicode = String.Empty;
            switch (style)
            {
                case TitleStyle.FirstCaps:
                    unicode = text.ToLower();
                    unicode = Regex.Replace(unicode, @"\b[a-z]\w+", delegate(Match m)
                    {
                        string v = m.ToString();
                        return char.ToUpper(v[0]) + v.Substring(1);
                    });
                    break;

                case TitleStyle.UpperCase:
                    unicode = text.ToUpper();
                    break;
            }

            return unicode;
        }

        public static string ToFileName(this string text)
        {
            var result = text.Substring(text.LastIndexOf("/") + 1);
            return result;
        }

        public static string SplitCapitalizedWords(this string source)
        {
            if (string.IsNullOrEmpty(source)) return String.Empty;
            var newText = new StringBuilder(source.Length * 2);
            newText.Append(source[0]);
            for (int i = 1; i < source.Length; i++)
            {
                if (char.IsUpper(source[i]))
                    newText.Append(' ');
                newText.Append(source[i]);
            }
            return newText.ToString();
        }

        public static ImageDimension ToImageDimension(this string dimension)
        {
            var arr = dimension.Split('x');
            return new ImageDimension 
            { 
                Width = Int32.Parse(arr[0].Trim()),
                Height = Int32.Parse(arr[1].Trim())
            };
        }

        public static string ToTemplateView(this string source)
        {
            var result = source.Substring(source.LastIndexOf("/") + 1);
            return result.Split('.')[0];
        }

        public static string ToSeparateString(this int source)
        {
            string strInput = string.Empty;
            string strOutput = string.Empty;
            List<string> list = new List<string>();
            strInput = source.ToString();
            char[] proc_I = strInput.ToCharArray();
            string proc_II = string.Empty;
            int pos = 0;
            if (proc_I.Length > 3)
            {
                for (int i = (proc_I.Length - 1); i >= 0; i--)
                {
                    if (pos == 2 || pos == 5 || pos == 8 || pos == 11 || pos == 14 || pos == 17 || pos == 20)
                    {
                        list.Add(proc_I[i].ToString());
                        if (i == 0)
                        {
                            break;
                        }
                        else
                        {
                            list.Add(".");
                        }
                    }
                    else
                    {
                        list.Add(proc_I[i].ToString());
                    }
                    pos++;
                }
            }
            else
            {
                strOutput = strInput;
            }
            foreach (string s in list)
            {
                proc_II += s;
            }
            char[] proc_III = proc_II.ToCharArray();
            for (int i = (proc_III.Length - 1); i >= 0; i--)
            {
                strOutput += proc_III[i];
            }
            return strOutput;
        }

        public static int ToOriginalNumber(this string source)
        {
            string[] _price = source.Split(new string[] { ",", "." }, StringSplitOptions.RemoveEmptyEntries);
            string priceOut = string.Empty;
            foreach (string p in _price)
            {
                priceOut += p;
            }
            return Convert.ToInt32(priceOut);
        }
    }
}
