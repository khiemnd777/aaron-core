using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aaron.Core
{
    public class KeyValuePair
    {
        public string Key { get; set; }
        public string Name { get; set; }
    }

    public static class EnumExtensions
    {
        public static List<KeyValuePair> ToEnumList<TEnum>(this TEnum source)
        {
            var array = (TEnum[])(Enum.GetValues(typeof(TEnum)).Cast<TEnum>());
            return array
                .Select(kpv => new KeyValuePair 
                { 
                    Key = kpv.ToString(),
                    Name = kpv.ToString().SplitCapitalizedWords()
                })
                .OrderBy(kpv => kpv.Name)
                .ToList();
        }

        public static object GetEnumValue<TEnum>(this string source)
        {
            return Enum.Parse(typeof(TEnum), source);
        }
    }
}
