using System;

namespace Aaron.Core
{
    public static class EnumerableExtensions
    {
        public static void ForEach<T>(this T[] array, Action<T> action)
        {
            if (array.IsNull()) return;
            foreach (var obj in array)
            {
                action(obj);
            }
        }
    }
}
