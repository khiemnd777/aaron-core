using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aaron.Core;
using Aaron.Core.Domain.Catalogs;

namespace Aaron.Core.Services.Catalogs
{
    public interface IGenericCatalogService: IServices
    {
        void InsertGenericCatalog(GenericCatalog genericCatalog);
        void UpdateGenericCatalog(GenericCatalog genericCatalog);
        void DeleteGenericCatalog(GenericCatalog genericCatalog);
        void ChangePublished(GenericCatalog genericCatalog);

        GenericCatalog GetGenericCatalogById(int id);
        GenericCatalog GetGenericCatalogBySystemName(string systemName);
        IList<GenericCatalog> GetAllGenericCatalogs(bool showHidden = false);
        IPagedList<GenericCatalog> GetAllGenericCatalogs(int pageIndex, int pageSize, bool showHidden = false);
        IList<GenericCatalog> GetGenericCatalogByBlockView(int blockViewId);
        GenericCatalogAttribute GetGenericCatalogAttributeById(int id);
        void DeleteGenericCatalogAttribute(GenericCatalogAttribute genericCatalogAttribute);
        IList<GenericCatalogTemplate> GetGenericCatalogTemplates(bool showHidden = false);
        bool GenericCatalogTemplateExisted();

    }
}
