using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Aaron.Core.Web
{
    public static class ModelExtensions
    {
        public static TOutput MoveData<TInput, TOutput>(this TInput input, TOutput output)
            where TInput : class
            where TOutput : class
        {
            foreach (var prpInput in input.GetType().GetProperties())
            {
                if (prpInput.GetValue(input, null) == null) continue;
                var pi = output.GetType()
                    .GetProperties()
                    .SingleOrDefault(x => x.Name.Equals(prpInput.Name));
                if (pi == null) continue;
                pi.SetValue(output, prpInput.GetValue(input, null), null);
            }
            return output;
        }

        public static TSource Ignore<TSource, TProperty>(this TSource source, Expression<Func<TSource, TProperty>> expr) where TSource : class
        {
            MemberExpression mExpr = expr.Body as MemberExpression;
            var mName = mExpr.Member.Name;
            var sProps = source.GetType().GetProperties();
            foreach (var pi in sProps)
            {
                if (pi.Name == mName)
                    pi.SetValue(source, null, null);
            }

            return source;
        }

        public static bool IsList(this PropertyInfo prp)
        {
            foreach (var type in ArrayListType())
            {
                if (prp.GetType().IsGenericType && prp.GetType().GetGenericTypeDefinition() == type)
                {
                    return true;
                }
            }
            return false;
        }

        public static Type[] ArrayListType()
        {
            return new[]
                       {
                           typeof (IList<>),
                           typeof (IEnumerable<>),
                           typeof (IQueryable<>)
                       };
        }
    }
}
