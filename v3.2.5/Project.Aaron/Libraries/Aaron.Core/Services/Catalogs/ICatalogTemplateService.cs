using Aaron.Core;
using Aaron.Core.Domain.Catalogs;
using System.Collections.Generic;

namespace Aaron.Core.Services.Catalogs
{
    public interface ICatalogTemplateService : IServices
    {
        void InsertCatalogTemplate(CatalogTemplate catalogTemplate);
        void UpdateCatalogTemplate(CatalogTemplate catalogTemplate);
        void DeleteCatalogTemplate(CatalogTemplate catalogTemplate);
        void ChangePublished(CatalogTemplate catalogTemplate);
        
        CatalogTemplate GetCatalogTemplateById(int id);
        IList<CatalogTemplate> GetAllCatalogTemplates(bool showHidden = false);
        IPagedList<CatalogTemplate> GetAllCatalogTemplates(int pageIndex, int pageSize, bool showHidden = false); 
    }
}
