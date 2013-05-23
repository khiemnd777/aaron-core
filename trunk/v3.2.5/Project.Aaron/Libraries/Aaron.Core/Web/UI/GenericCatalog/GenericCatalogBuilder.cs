using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Aaron.Core.Web;
using Aaron.Core.Services.Catalogs;
using Aaron.Core.Domain.Catalogs;
using Aaron.Core.Web.Mvc;
using System.Web.Mvc;
using System.Web;
using System.Web.Routing;

namespace Aaron.Core.Web.UI.GenericCatalog
{
    public class GenericCatalogBuilder: IGenericCatalogBuilder
    {
        private readonly List<GenericCatalogPartsModel> _modelList;
        private readonly IGenericCatalogService _genericCatalogService;
        private readonly CatalogSettings _catalogSettings;
        private readonly UrlHelper _urlHelper;

        public GenericCatalogBuilder(IGenericCatalogService genericCatalogService,
            IWebHelper webHelper,
            CatalogSettings catalogSettings,
            UrlHelper urlHelper)
        {
            _modelList = new List<GenericCatalogPartsModel>();
            _genericCatalogService = genericCatalogService;
            _catalogSettings = catalogSettings;
            _urlHelper = urlHelper;
        }

        public void GetGenericCatalogList(string blockView)
        {
            var count = _modelList.Count;
            if (string.IsNullOrEmpty(blockView))
                throw new ArgumentException("No Generic Catalog found.Because: 'BlockView' is Null or Empty");

            int blockViewId = 0;
            switch (blockView)
            {
                case "left":
                    blockViewId = (int)BlockViewType.Left;
                    break;
                case "right":
                    blockViewId = (int)BlockViewType.Right;
                    break;
                case "center":
                    blockViewId = (int)BlockViewType.Center;
                    break;
                case "bottom":
                    blockViewId = (int)BlockViewType.Bottom;
                    break;
            }

            var genericCatalogList = _genericCatalogService.GetGenericCatalogByBlockView(blockViewId);

            if (genericCatalogList.Any())
            {
                foreach (var genericCatalog in genericCatalogList)
                {
                    var genericCatalogModel = new GenericCatalogPartsModel()
                    {
                        Id = genericCatalog.Id,
                        Name = genericCatalog.Name,
                        Icon = genericCatalog.Icon,
                        SizeOnMenu = genericCatalog.SizeOnMenu,
                        Count = genericCatalog.Catalogs.Count
                    };

                    
                    if (genericCatalog.Catalogs.Any())
                    {
                        var catalogModelList = new List<CatalogsInGenericModel>();
                        catalogModelList.AddRange(genericCatalog.Catalogs
                            .Take(genericCatalogModel.SizeOnMenu)
                            .Select(catalog => new CatalogsInGenericModel()
                            {
                                Id = catalog.Id,
                                Name = catalog.Name
                            }));

                        genericCatalogModel.Catalogs = catalogModelList;
                    }

                    _modelList.Add(genericCatalogModel);
                }
            }
        }

        public string GenerateGenericCatalog()
        {
            var result = new StringBuilder();
            if(_modelList.Any())
            {
                foreach (var genericCatalogModel in _modelList)
                {
                    var catalogCount = genericCatalogModel.Count;
                    var sizeOnMenu = genericCatalogModel.SizeOnMenu;
                    
                    result.Append("<menu class=\"as-menu\">");
                        result.Append("<div class=\"menu-header\">");
                            result.AppendFormat("<h2>{0}</h2>", genericCatalogModel.Name);
                            if(catalogCount > sizeOnMenu)
                            {
                                result.AppendFormat("<a href=\"{0}{1}\">Xem tất cả</a>", _urlHelper.Content("~/GenericCatalog/Catalogs/"), genericCatalogModel.Id);
                            }
                        
                        result.Append("</div>");
                        result.Append("<ul>");
                            if(genericCatalogModel.Catalogs.Any())
                            {
                                var count = 1;
                                foreach (var catalogModel in genericCatalogModel.Catalogs)
                                {
                                    if (count > sizeOnMenu)
                                    {
                                        break;
                                    }
                                    result.Append("<li>");
                                    var imgDim = _catalogSettings.IconDimension.ToImageDimension();
                                    result.AppendFormat("<a href=\"{0}{1}\">", _urlHelper.Content("~/Catalog/Items/"), catalogModel.Id);
                                    result.AppendFormat("<img width=\"{0}px\" height=\"{1}px\" src=\"{2}\" alt=\"\"/>&nbsp;&nbsp;&nbsp;{3}</a>", imgDim.Width, imgDim.Height, _urlHelper.Content(genericCatalogModel.Icon ?? "~/"), catalogModel.Name.ToTitle(TitleStyle.FirstCaps));
                                    result.Append("</li>");

                                    count += 1;
                                }
                            }
                        result.Append("</ul>");
                    result.Append("</menu>");

                }
               
            }

            return result.ToString();
        }
    }
}