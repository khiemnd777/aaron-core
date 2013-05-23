using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aaron.Core.Domain.Catalogs;
using Aaron.Core.Infrastructure;
using System.Web.Mvc;

namespace Aaron.Core.Services.Catalogs
{
    public static class CatalogExtensions
    {
        public static Catalog PrepareCatalogsByGCSystemNameForList(this ICollection<Catalog> source, string gCSystemName)
        {
            var proc_source = (!string.IsNullOrEmpty(gCSystemName)) ? source
                .Where(s => gCSystemName.Equals(s.GenericCatalog.SystemName)) : source;

            return proc_source.FirstOrDefault();
        }

        public static string PrepareCatalogsByForString(this ICollection<Catalog> source, string separator = ",")
        {
            return PrepareCatalogsByGCSystemNameForString(source, null, null, null, separator);
        }

        public static string PrepareCatalogsByGCSystemNameForString(this ICollection<Catalog> source, string gCSystemName, string separator = ",")
        {
            return PrepareCatalogsByGCSystemNameForString(source, gCSystemName, "Catalog", "Items", separator);
        }

        public static string PrepareCatalogsByGCSystemNameForString(this ICollection<Catalog> source, string gCSystemName, string controller, string action,  string separator=",")
        {
            var urlHelper = IoC.Resolve<UrlHelper>();
            var sb = new StringBuilder();
            var proc_source = (!string.IsNullOrEmpty(gCSystemName)) ? source
                .Where(s => gCSystemName.Equals(s.GenericCatalog.SystemName)) : source;

            for (int i = 0; i < proc_source.Count(); i++)
            {
                var obj = proc_source.ElementAt(i);
                if (string.IsNullOrEmpty(controller) || string.IsNullOrEmpty(action))
                    sb.Append(obj.Name);
                else
                {
                    var urlContent = urlHelper.Content(string.Format("~/{0}/{1}/{2}", controller, action, obj.Id));
                    sb.AppendFormat("<a href=\"{0}\">{1}</a>", new object[] { urlContent, obj.Name });
                }
                if (i != proc_source.Count() - 1)
                {
                    sb.Append(separator);
                    sb.Append(" ");
                }
            }
            return sb.Length == 0 ? "Không biết" : sb.ToString();
        }

        public static string DisplayDefaultIcon(this string source)
        {
            var defaultIcon = IoC.Resolve<CatalogSettings>().DefaultGenericCatalogIcon;
            return !string.IsNullOrEmpty(source) ? source : 
                (!string.IsNullOrEmpty(defaultIcon) ? defaultIcon :
                    "~/content/defaulticon.png");
        }

        public static string DisplayDefaultAvatar(this string source)
        {
            var defaultIcon = IoC.Resolve<CatalogSettings>().DefaultAvatar;
            return !string.IsNullOrEmpty(source) ? source :
                (!string.IsNullOrEmpty(defaultIcon) ? defaultIcon :
                    "~/content/defaultavatar.png");
        }

        public static IList<Catalog> SortCatalogsForParent(this List<Catalog> source, string name, int parentId)
        {
            var result = new List<Catalog>();
            var list = new List<Catalog>();

            if (parentId == 0) list = source.FindAll(c => !c.ParentCatalogId.HasValue);
            else list = source.FindAll(c => c.ParentCatalogId == parentId);

            foreach (var item in list)
            {
                if (name != "")
                    item.Name = name + " >> " + item.Name;
                result.Add(item);
                result.AddRange(SortCatalogsForParent(source, item.Name, item.Id));
            }
            return result;
        }
    }
}
