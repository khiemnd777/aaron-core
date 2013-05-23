using Aaron.Core;
using Aaron.Core.Domain.Catalogs;
using System.Collections.Generic;

namespace Aaron.Core.Services.Catalogs
{
    public interface IGenericCatalogTemplateService : IServices
    {
        void InsertGenericCatalogTemplate(GenericCatalogTemplate genericCatalogTemplate);
        void UpdateGenericCatalogTemplate(GenericCatalogTemplate genericCatalogTemplate);
        void DeleteGenericCatalogTemplate(GenericCatalogTemplate genericCatalogTemplate);
        void ChangePublished(GenericCatalogTemplate genericCatalogTemplate);

        GenericCatalogTemplate GetGenericCatalogTemplateById(int id);
        IList<GenericCatalogTemplate> GetAllGenericCatalogTemplates(bool showHidden = false);
        IPagedList<GenericCatalogTemplate> GetAllGenericCatalogTemplates(int pageIndex, int pageSize, bool showHidden = false);
    }
}
