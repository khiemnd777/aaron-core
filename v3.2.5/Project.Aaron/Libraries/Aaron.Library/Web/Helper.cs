using System.Linq.Expressions;
using System.Web.Routing;

namespace System.Web.Mvc
{
    public static class HtmlHelpers
    {
        //HTML.Image
        //public static MvcHtmlString Image(this HtmlHelper helper, string url, string altText)
        //{
        //    var builder = new TagBuilder("img");
        //    builder.MergeAttribute("src", url);
        //    builder.MergeAttribute("alt", altText);
        //    return MvcHtmlString.Create(builder.ToString(TagRenderMode.SelfClosing));
        //}

        public static MvcHtmlString Image(this HtmlHelper helper, string url)
        {
            var builder = new TagBuilder("img");
            builder.MergeAttribute("src", url);
            return MvcHtmlString.Create(builder.ToString(TagRenderMode.SelfClosing));
        }

        public static MvcHtmlString Image(this HtmlHelper helper, string url, object htmlAttributes)
        {
            var builder = new TagBuilder("img");
            builder.MergeAttribute("src", url);
            if (htmlAttributes != null)
                builder.MergeAttributes(new RouteValueDictionary(htmlAttributes));
            return MvcHtmlString.Create(builder.ToString(TagRenderMode.SelfClosing));
        }


        //HTML.ImageFor(Model)
        public static MvcHtmlString ImageFor<TModel, TProperty>(
            this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression)
        {
            var imgUrl = expression.Compile()(htmlHelper.ViewData.Model);
            return BuildImageTag(imgUrl.ToString(), null);
        }
        //HTML.ImageFor(Model, html Attributes)
        public static MvcHtmlString ImageFor<TModel, TProperty>(
            this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression,
            object htmlAttributes)
        {
            var imgUrl = expression.Compile()(htmlHelper.ViewData.Model);
            return BuildImageTag(imgUrl.ToString(), htmlAttributes);
        }

        private static MvcHtmlString BuildImageTag(string imgUrl, object htmlAttributes)
        {
            TagBuilder tag = new TagBuilder("img");

            tag.Attributes.Add("src", imgUrl);
            if (htmlAttributes != null)
                tag.MergeAttributes(new RouteValueDictionary(htmlAttributes));

            return MvcHtmlString.Create(tag.ToString(TagRenderMode.Normal));
        }
    }
}