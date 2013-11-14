using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.IO;
using System.Web.WebPages;
using System.Web.Routing;
using Telerik.Web.Mvc.UI;
using Aaron.Core.Web;
using Aaron.Core.Infrastructure;
using Aaron.Core.Services.Localization;
using Aaron.Core;
using Aaron.Core.Web.Mvc;
using Aaron.Core.Web.Localization;
using Aaron.Core.Web.UI;
using System.Linq.Expressions;
using System.Globalization;

namespace Aaron.Core.Web
{
    public static class HtmlExtensions
    {
        public static MvcHtmlString ResolveUrl(this HtmlHelper htmlHelper, string url)
        {
            var urlHelper = new UrlHelper(htmlHelper.ViewContext.RequestContext);
            return MvcHtmlString.Create(urlHelper.Content(url));
        }

        public static MvcHtmlString Hint(this HtmlHelper helper, string value)
        {
            // Create tag builder
            var builder = new TagBuilder("img");

            // Add attributes
            builder.MergeAttribute("src", ResolveUrl(helper, "~/Administration/Content/images/ico-help.gif").ToHtmlString());
            builder.MergeAttribute("alt", value);
            builder.MergeAttribute("title", value);

            // Render tag
            return MvcHtmlString.Create(builder.ToString());
        }

        public static HelperResult LocalizedEditor<T, TLocalizedModelLocal>(this HtmlHelper<T> helper, string name,
             Func<int, HelperResult> localizedTemplate,
             Func<T, HelperResult> standardTemplate)
            where T : ILocalizedModel<TLocalizedModelLocal>
            where TLocalizedModelLocal : ILocalizedModelLocal
        {
            return new HelperResult(writer =>
            {
                if (helper.ViewData.Model.Locales.Count > 1)
                {
                    
                    var tabStrip = helper.Telerik().TabStrip().Name(name).Items(x =>
                    {
                        x.Add().Text("Standard").Content(standardTemplate(helper.ViewData.Model).ToHtmlString()).Selected(true);
                        for (int i = 0; i < helper.ViewData.Model.Locales.Count; i++)
                        {
                            var locale = helper.ViewData.Model.Locales[i];
                            var language = IoC.Resolve<ILanguageService>().GetLanguageById(locale.LanguageId);
                            x.Add().Text(language.Name)
                                .Content(localizedTemplate
                                    (i).
                                    ToHtmlString
                                    ())
                                .ImageUrl("~/Content/images/flags/" + language.FlagImageFileName);
                        }
                    }).ToHtmlString();
                    writer.Write(tabStrip);
                }
                else
                {
                    standardTemplate(helper.ViewData.Model).WriteTo(writer);
                }
            });
        }

        public static void MetaSEORecognition<T>(this HtmlHelper helper, T seoObject) where T : SEOEntityModel
        {
            var titleBuilder = IoC.Resolve<IPageTitleBuilder>();
            titleBuilder.AddMetaSeo(seoObject);
        }

        public static void MetaSEORecognition<T>(this HtmlHelper<T> helper) where T : SEOEntityModel
        {
            var model = helper.ViewData.Model;
            var titleBuilder = IoC.Resolve<IPageTitleBuilder>();
            titleBuilder.AddMetaSeo(model);
        }

        public static MvcHtmlString SimplifyHtmlContent(this HtmlHelper helper, string source, string selector = "")
        {
            var htmlEncoded = helper.Raw(source);
            selector = selector.ToSystemName();
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("<div id=\"simplifyHtmlContent-{0}\">{1}</div><div id=\"simplifyHtmlContentExt-{0}\"></div>", selector, htmlEncoded);
            //view more
            var expanding = "Xem thêm";
            var expanded = "Thu gọn";
            var v = "<span state='expanding' id='expand-{0}' style='cursor:pointer;text-decoration:underline;'>{1}</span>";
            var viewMore = string.Format(v, selector, expanding);
            var viewLess = string.Format(v, selector, expanded);

            //javascript creation.
            StringBuilder js = new StringBuilder();
            js.Append("<script type=\"text/javascript\">");
            js.Append("$(function(){");
            js.AppendFormat("/*SimplifyHtmlContent-{0} execute!*/ ", selector);
            js.AppendFormat("$('#simplifyHtmlContentExt-{0}').hide();", selector);
            js.AppendFormat("var f_{0} = $('#simplifyHtmlContent-{0} p:first');", selector);
            js.AppendFormat("var fext_{0} = $('#simplifyHtmlContent-{0} p:not(:first)');", selector);
            js.AppendFormat("if(f_{0}.length > 0){{", selector);
            js.AppendFormat("var val = f_{0};", selector);
            js.AppendFormat("$('#simplifyHtmlContentExt-{0}').html(fext_{0});", selector);
            js.AppendFormat("$('#simplifyHtmlContent-{0}').html(val);", selector);
            js.AppendFormat("$('#simplifyHtmlContentExt-{0}').after(\"{1}\");", selector, viewMore);
            js.AppendFormat("$('#expand-{0}').click(function(){{", selector);
            js.Append("var st = $(this).attr(\"state\");");
            js.Append("if(st == \"expanding\"){");
            js.Append("$(this).attr(\"state\", \"expanded\");");
            js.AppendFormat("$(this).text('{0}');", expanded);
            js.AppendFormat("$('#simplifyHtmlContentExt-{0}').fadeIn();", selector);
            js.Append("}");
            js.Append("else{");
            js.AppendFormat("$('#simplifyHtmlContentExt-{0}').fadeOut();", selector);
            js.Append("$(this).attr(\"state\", \"expanding\");");
            js.AppendFormat("$(this).text('{0}');", expanding);
            js.Append("}");
            js.Append("});");
            js.Append("}");
            js.Append("});");
            js.Append("</script>");

            sb.Append(js);
            var value = sb.ToString();
            return MvcHtmlString.Create(value);
        }

        public static MvcHtmlString DeleteConfirmation<T>(this HtmlHelper<T> helper, string buttonsSelector = null) where T : BaseEntityModel
        {
            return DeleteConfirmation<T>(helper, "", buttonsSelector);
        }

        // Adds an action name parameter for using other delete action names
        public static MvcHtmlString DeleteConfirmation<T>(this HtmlHelper<T> helper, string actionName, string buttonsSelector = null) where T : BaseEntityModel
        {
            if (String.IsNullOrEmpty(actionName))
                actionName = "Delete";

            var modalId = MvcHtmlString.Create(helper.ViewData.ModelMetadata.ModelType.Name.ToLower() + "-delete-confirmation").ToHtmlString();

            //there's an issue in Telerik (ScriptRegistrar.Current implemenation)
            //it's a little hack to ensure ScriptRegistrar.Current is loaded
            helper.Telerik();

            #region Write click events for button, if supplied

            if (!string.IsNullOrEmpty(buttonsSelector))
            {
                var textWriter = new StringWriter();
                IClientSideObjectWriter objectWriter = new ClientSideObjectWriterFactory().Create(buttonsSelector, "click", textWriter);
                objectWriter.Start();
                textWriter.Write("function(e){e.preventDefault();openModalWindow(\"" + modalId + "\");}");
                objectWriter.Complete();
                var value = textWriter.ToString();
                ScriptRegistrar.Current.OnDocumentReadyStatements.Add(value);
            }

            #endregion

            var deleteConfirmationModel = new DeleteConfirmationModel
            {
                Id = helper.ViewData.Model.Id,
                ControllerName = helper.ViewContext.RouteData.GetRequiredString("controller"),
                ActionName = actionName
            };

            var window = helper.Telerik().Window().Name(modalId)
                .Title("Bạn có muốn xóa?")
                .Modal(true)
                .Effects(x => x.Toggle())
                .Resizable(x => x.Enabled(false))
                .Buttons(x => x.Close())
                .Visible(false)
                .Content(helper.Partial("Delete", deleteConfirmationModel).ToHtmlString()).ToHtmlString();

            return MvcHtmlString.Create(window);
        }

        public static MvcHtmlString Widget(this HtmlHelper helper, string widgetZone)
        {
            return helper.Action("WidgetsByZone", "Widget", new { widgetZone = widgetZone });
        }

        public static MvcHtmlString AaronLabelFor<TModel, TValue>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression, bool displayHint = true)
        {
            var result = new StringBuilder();
            var metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            var hintResource = string.Empty;
            object value = null;
            if (metadata.AdditionalValues.TryGetValue("AaronResourceDisplayName", out value))
            {
                var resourceDisplayName = value as AaronResourceDisplayName;
                if (resourceDisplayName != null && displayHint)
                {
                    var langId = IoC.Resolve<ICurrentActivity>().CurrentLanguage.Id;
                    hintResource =
                        IoC.Resolve<ILocalizationService>()
                        .GetResource(resourceDisplayName.ResourceKey + ".Hint", langId);

                    result.Append(helper.Hint(hintResource).ToHtmlString());
                }
            }
            result.Append(helper.LabelFor(expression, new { title = hintResource }));
            return MvcHtmlString.Create(result.ToString());
        }

        public static MvcHtmlString RequiredHint(this HtmlHelper helper, string additionalText = null)
        {
            // Create tag builder
            var builder = new TagBuilder("span");
            builder.AddCssClass("required");
            var innerText = "*";
            //add additinal text if specified
            if (!String.IsNullOrEmpty(additionalText))
                innerText += " " + additionalText;
            builder.SetInnerText(innerText);
            // Render tag
            return MvcHtmlString.Create(builder.ToString());
        }

        public static string FieldNameFor<T, TResult>(this HtmlHelper<T> html, Expression<Func<T, TResult>> expression)
        {
            return html.ViewData.TemplateInfo.GetFullHtmlFieldName(ExpressionHelper.GetExpressionText(expression));
        }
        public static string FieldIdFor<T, TResult>(this HtmlHelper<T> html, Expression<Func<T, TResult>> expression)
        {
            var id = html.ViewData.TemplateInfo.GetFullHtmlFieldId(ExpressionHelper.GetExpressionText(expression));
            // because "[" and "]" aren't replaced with "_" in GetFullHtmlFieldId
            return id.Replace('[', '_').Replace(']', '_');
        }

        public static MvcHtmlString DatePickerDropDowns(this HtmlHelper html,
            string dayName, string monthName, string yearName,
            int? beginYear = null, int? endYear = null,
            int? selectedDay = null, int? selectedMonth = null, int? selectedYear = null, bool localizeLabels = true)
        {
            var daysList = new TagBuilder("select");
            var monthsList = new TagBuilder("select");
            var yearsList = new TagBuilder("select");

            daysList.Attributes.Add("name", dayName);
            monthsList.Attributes.Add("name", monthName);
            yearsList.Attributes.Add("name", yearName);

            var days = new StringBuilder();
            var months = new StringBuilder();
            var years = new StringBuilder();

            string dayLocale, monthLocale, yearLocale;
            if (localizeLabels)
            {
                var locService = IoC.Resolve<ILocalizationService>();
                dayLocale = locService.GetResource("Common.Day");
                monthLocale = locService.GetResource("Common.Month");
                yearLocale = locService.GetResource("Common.Year");
            }
            else
            {
                dayLocale = "Day";
                monthLocale = "Month";
                yearLocale = "Year";
            }

            days.AppendFormat("<option value='{0}'>{1}</option>", "0", dayLocale);
            for (int i = 1; i <= 31; i++)
                days.AppendFormat("<option value='{0}'{1}>{0}</option>", i,
                    (selectedDay.HasValue && selectedDay.Value == i) ? " selected=\"selected\"" : null);


            months.AppendFormat("<option value='{0}'>{1}</option>", "0", monthLocale);
            for (int i = 1; i <= 12; i++)
            {
                months.AppendFormat("<option value='{0}'{1}>{2}</option>",
                                    i,
                                    (selectedMonth.HasValue && selectedMonth.Value == i) ? " selected=\"selected\"" : null,
                                    CultureInfo.CurrentUICulture.DateTimeFormat.GetMonthName(i));
            }


            years.AppendFormat("<option value='{0}'>{1}</option>", "0", yearLocale);

            if (beginYear == null)
                beginYear = DateTime.UtcNow.Year - 100;
            if (endYear == null)
                endYear = DateTime.UtcNow.Year;

            for (int i = beginYear.Value; i <= endYear.Value; i++)
                years.AppendFormat("<option value='{0}'{1}>{0}</option>", i,
                    (selectedYear.HasValue && selectedYear.Value == i) ? " selected=\"selected\"" : null);

            daysList.InnerHtml = days.ToString();
            monthsList.InnerHtml = months.ToString();
            yearsList.InnerHtml = years.ToString();

            return MvcHtmlString.Create(string.Concat(daysList, monthsList, yearsList));
        }

        // Warning: it's very long and bulky
        public static MvcHtmlString Paging(this HtmlHelper helper, int currentPage, int totalPage, object routeValues = null)
        {
            #region Very long and bulk, need to fix at latest version.
            int total = totalPage;
            var page = currentPage;
            //var size = pageSize;

            var pageStart = page - 3;
            var pageEnd = page + 3;

            if (pageStart <= 0) { pageStart = 1; }
            if (pageStart == 1) { pageEnd = 5; }
            if (pageEnd >= total)
            {
                pageEnd = total;
                pageStart = total - 4;
            }
            if (pageStart <= 0) { pageStart = 1; }

            var urlHelper = IoC.Resolve<UrlHelper>();
            var controllerName = helper.ViewContext.RouteData.GetRequiredString("Controller");
            var actionName = helper.ViewContext.RouteData.GetRequiredString("Action");

            var result = new StringBuilder();

            if (routeValues != null)
            {
                var routingValues = new RouteValueDictionary(routeValues);
                var url = urlHelper.Action(actionName, controllerName, routingValues);

                result.Append("<div class=\"paging\">");

                if (page != 1)
                {
                    // Next Top

                    result.AppendFormat("<a href=\"{0}\">Đầu</a>", urlHelper.Content(url + "&pageIndex=1"));
                    result.AppendFormat(" <a href=\"{0}\">Trước</a>", urlHelper.Content(url + "&pageIndex=" + (page - 1)));

                }
                if (pageStart > 1)
                {
                    result.AppendFormat("<a href=\"{0}\">1</a>", urlHelper.Content(url + "&pageIndex=1"));
                }
                if (pageStart > 2)
                {
                    result.Append("<text>...</text>");
                }

                for (var i = pageStart; i <= pageEnd; i++)
                {
                    if (i == page)
                    {
                        result.AppendFormat("<span><b>{0}</b></span>", i);
                    }
                    else
                    {
                        result.AppendFormat("<a href=\"{0}\">{1}</a>", urlHelper.Content(url + "&pageIndex=" + i), i);
                    }
                }

                if (pageEnd < (total - 1))
                {
                    result.Append("<text>...</text>");
                }

                if (pageEnd < total)
                {
                    result.AppendFormat("<a class=\"page-end\" href=\"{0}\">{1}</a>", urlHelper.Content(url + "&pageIndex=" + total), total);
                }

                if (page != total)
                {
                    result.AppendFormat("<a href=\"{0}\">Tiếp</a>", urlHelper.Content(url + "&pageIndex=" + (page + 1)));
                    result.AppendFormat("<a href=\"{0}\">Cuối</a>", urlHelper.Content(url + "&pageIndex=" + total));
                }

                result.Append("</div>");
            }
            else
            {

                var url = urlHelper.Action(actionName, controllerName);

                result.Append("<div class=\"paging\">");

                if (page != 1)
                {
                    // Next Top

                    result.AppendFormat("<a href=\"{0}\">Đầu</a>", urlHelper.Content(url + "?pageIndex=1"));
                    result.AppendFormat(" <a href=\"{0}\">Trước</a>", urlHelper.Content(url + "?pageIndex=" + (page - 1)));

                }
                if (pageStart > 1)
                {
                    result.AppendFormat("<a href=\"{0}\">1</a>", urlHelper.Content(url + "?pageIndex=1"));
                }
                if (pageStart > 2)
                {
                    result.Append("<text>...</text>");
                }

                for (var i = pageStart; i <= pageEnd; i++)
                {
                    if (i == page)
                    {
                        result.AppendFormat("<span><b>{0}</b></span>", i);
                    }
                    else
                    {
                        result.AppendFormat("<a href=\"{0}\">{1}</a>", urlHelper.Content(url + "?pageIndex=" + i), i);
                    }
                }

                if (pageEnd < (total - 1))
                {
                    result.Append("<text>...</text>");
                }

                if (pageEnd < total)
                {
                    result.AppendFormat("<a class=\"page-end\" href=\"{0}\">{1}</a>", urlHelper.Content(url + "?pageIndex=" + total), total);
                }

                if (page != total)
                {
                    result.AppendFormat("<a href=\"{0}\">Tiếp</a>", urlHelper.Content(url + "?pageIndex=" + (page + 1)));
                    result.AppendFormat("<a href=\"{0}\">Cuối</a>", urlHelper.Content(url + "?pageIndex=" + total));
                }

                result.Append("</div>");

            }

            return MvcHtmlString.Create(result.ToString()); 
            #endregion

        }
    }
}
