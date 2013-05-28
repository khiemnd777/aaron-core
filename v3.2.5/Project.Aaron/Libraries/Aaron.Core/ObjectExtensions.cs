using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aaron.Core
{
    public static class ObjectExtensions
    {
        public static bool IsNull<T>(this T source)
        {
            return source == null;
        }

        public static bool IsNullOrDefault<T>(this T? value) where T : struct
        {
            return default(T).Equals(value.GetValueOrDefault());
        }

        public static bool ToBoolean(this int source)
        {
            return (source == 1) ? true : false;
        }

        public static int ToInt32(this bool source)
        {
            return (source) ? 1 : 0;
        }
    }
}
