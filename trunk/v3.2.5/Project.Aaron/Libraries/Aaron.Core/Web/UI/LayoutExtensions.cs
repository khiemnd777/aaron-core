using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Aaron.Core.Infrastructure;
using Aaron.Core.Web.Mvc;
using System.Web.Routing;
using Aaron.Core.Web.UI.Media;
using Aaron.Core.Web.UI.GenericCatalog;
using Aaron.Core.Web.UI.Analysis;

namespace Aaron.Core.Web.UI
{
    public static class LayoutExtensions
    {
        public static void AddTitleParts(this HtmlHelper html, params string[] parts)
        {
            var pageTitleBuilder = IoC.Resolve<IPageTitleBuilder>();
            pageTitleBuilder.AddTitleParts(parts);
        }
        public static void AppendTitleParts(this HtmlHelper html, params string[] parts)
        {
            var pageTitleBuilder = IoC.Resolve<IPageTitleBuilder>();
            pageTitleBuilder.AppendTitleParts(parts);
        }
        public static MvcHtmlString Title(this HtmlHelper html, bool addDefaultTitle, params string[] parts)
        {
            var pageTitleBuilder = IoC.Resolve<IPageTitleBuilder>();
            html.AppendTitleParts(parts);
            return MvcHtmlString.Create(html.Encode(pageTitleBuilder.GenerateTitle(addDefaultTitle)));
        }


        public static void AddMetaDescriptionParts(this HtmlHelper html, params string[] parts)
        {
            var pageTitleBuilder = IoC.Resolve<IPageTitleBuilder>();
            pageTitleBuilder.AddMetaDescriptionParts(parts);
        }
        public static void AppendMetaDescriptionParts(this HtmlHelper html, params string[] parts)
        {
            var pageTitleBuilder = IoC.Resolve<IPageTitleBuilder>();
            pageTitleBuilder.AppendMetaDescriptionParts(parts);
        }
        public static MvcHtmlString MetaDescription(this HtmlHelper html, params string[] parts)
        {
            var pageTitleBuilder = IoC.Resolve<IPageTitleBuilder>();
            html.AppendMetaDescriptionParts(parts);
            return MvcHtmlString.Create(html.Encode(pageTitleBuilder.GenerateMetaDescription()));
        }


        public static void AddMetaKeywordParts(this HtmlHelper html, params string[] parts)
        {
            var pageTitleBuilder = IoC.Resolve<IPageTitleBuilder>();
            pageTitleBuilder.AddMetaKeywordParts(parts);
        }
        public static void AppendMetaKeywordParts(this HtmlHelper html, params string[] parts)
        {
            var pageTitleBuilder = IoC.Resolve<IPageTitleBuilder>();
            pageTitleBuilder.AppendMetaKeywordParts(parts);
        }
        public static MvcHtmlString MetaKeywords(this HtmlHelper html, params string[] parts)
        {
            var pageTitleBuilder = IoC.Resolve<IPageTitleBuilder>();
            html.AppendMetaKeywordParts(parts);
            return MvcHtmlString.Create(html.Encode(pageTitleBuilder.GenerateMetaKeywords()));
        }



        public static void AddScriptParts(this HtmlHelper html, params string[] parts)
        {
            AddScriptParts(html, ResourceLocation.Head, parts);
        }
        public static void AddScriptParts(this HtmlHelper html, ResourceLocation location, params string[] parts)
        {
            var pageTitleBuilder = IoC.Resolve<IPageTitleBuilder>();
            pageTitleBuilder.AddScriptParts(location, parts);
        }
        public static void AppendScriptParts(this HtmlHelper html, params string[] parts)
        {
            AppendScriptParts(html, ResourceLocation.Head, parts);
        }
        public static void AppendScriptParts(this HtmlHelper html, ResourceLocation location, params string[] parts)
        {
            var pageTitleBuilder = IoC.Resolve<IPageTitleBuilder>();
            pageTitleBuilder.AppendScriptParts(location, parts);
        }
        public static void AppendLastScriptParts(this HtmlHelper html, params string[] parts)
        {
            AppendLastScriptParts(html, ResourceLocation.Head, parts);
        }
        public static void AppendLastScriptParts(this HtmlHelper html, ResourceLocation location, params string[] parts)
        {
            var pageTitleBuilder = IoC.Resolve<IPageTitleBuilder>();
            pageTitleBuilder.AppendLastScriptParts(location, parts);
        }
        public static MvcHtmlString Scripts(this HtmlHelper html, params string[] parts)
        {
            return Scripts(html, ResourceLocation.Head, parts);
        }
        public static MvcHtmlString Scripts(this HtmlHelper html, ResourceLocation location, params string[] parts)
        {
            var pageTitleBuilder = IoC.Resolve<IPageTitleBuilder>();
            html.AppendScriptParts(parts);
            return MvcHtmlString.Create(pageTitleBuilder.GenerateScripts(location));
        }

        public static void AddCssFileParts(this HtmlHelper html, params string[] parts)
        {
            AddCssFileParts(html, ResourceLocation.Head, parts);
        }
        public static void AddCssFileParts(this HtmlHelper html, ResourceLocation location, params string[] parts)
        {
            var pageTitleBuilder = IoC.Resolve<IPageTitleBuilder>();
            pageTitleBuilder.AddCssFileParts(location, parts);
        }
        public static void AppendCssFileParts(this HtmlHelper html, params string[] parts)
        {
            AppendCssFileParts(html, ResourceLocation.Head, parts);
        }
        public static void AppendCssFileParts(this HtmlHelper html, ResourceLocation location, params string[] parts)
        {
            var pageTitleBuilder = IoC.Resolve<IPageTitleBuilder>();
            pageTitleBuilder.AppendCssFileParts(location, parts);
        }
        public static MvcHtmlString CssFiles(this HtmlHelper html, params string[] parts)
        {
            return CssFiles(html, ResourceLocation.Head, parts);
        }
        public static MvcHtmlString CssFiles(this HtmlHelper html, ResourceLocation location, params string[] parts)
        {
            var pageTitleBuilder = IoC.Resolve<IPageTitleBuilder>();
            html.AppendCssFileParts(parts);
            return MvcHtmlString.Create(pageTitleBuilder.GenerateCssFiles(location));
        }
        public static void AddCanonicalUrlParts(this HtmlHelper html, params string[] parts)
        {
            var pageTitleBuilder = IoC.Resolve<IPageTitleBuilder>();
            pageTitleBuilder.AddCanonicalUrlParts(parts);
        }
        public static void AppendCanonicalUrlParts(this HtmlHelper html, params string[] parts)
        {
            var pageTitleBuilder = IoC.Resolve<IPageTitleBuilder>();
            pageTitleBuilder.AppendCanonicalUrlParts(parts);
        }
        public static MvcHtmlString CanonicalUrls(this HtmlHelper html, params string[] parts)
        {
            var pageTitleBuilder = IoC.Resolve<IPageTitleBuilder>();
            html.AppendCanonicalUrlParts(parts);
            return MvcHtmlString.Create(pageTitleBuilder.GenerateCanonicalUrls());
        }
        public static void AddMediaInfo<T>(this HtmlHelper<T> html) where T : BaseMediaModel
        {
            var mediaTagBuilder = IoC.Resolve<IMediaTagBuilder>();
            var model = html.ViewData.Model;
            mediaTagBuilder.AddInfo(model);
        }
        public static void AddMediaInfo<T>(this HtmlHelper html, T model) where T : BaseMediaModel
        {
            var mediaTagBuilder = IoC.Resolve<IMediaTagBuilder>();
            mediaTagBuilder.AddInfo(model);
        }
        public static MvcHtmlString MediaTag(this HtmlHelper html)
        {
            var mediaTagBuilder = IoC.Resolve<IMediaTagBuilder>();
            return MvcHtmlString.Create(mediaTagBuilder.GenerateMediaTag());
        }

        public static MvcHtmlString GenericCatalogs(this HtmlHelper htmlHelper, string blockView)
        {
            var genericCatalogList = IoC.Resolve<IGenericCatalogBuilder>();
            genericCatalogList.GetGenericCatalogList(blockView);
            return MvcHtmlString.Create(genericCatalogList.GenerateGenericCatalog());
        }

        /* Chưa tổng hợp, nên sử dụng tạm thời */
        public static MvcHtmlString GoogleInitCode(this HtmlHelper htmlHelper)
        {
            var m = IoC.Resolve<IAnalysisNSocialNetworkBuilder>();
            return MvcHtmlString.Create(m.GoogleInitCode());
        }

        public static MvcHtmlString FacebookInitCode(this HtmlHelper htmlHelper)
        {
            var m = IoC.Resolve<IAnalysisNSocialNetworkBuilder>();
            return MvcHtmlString.Create(m.FacebookInitCode());
        }
    }
}