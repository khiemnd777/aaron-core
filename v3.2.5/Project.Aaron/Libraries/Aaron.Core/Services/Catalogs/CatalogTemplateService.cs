using System;
using System.Linq;
using Aaron.Core;
using Aaron.Core.Data;
using Aaron.Core.Domain.Catalogs;
using Aaron.Core.Caching;
using System.Collections.Generic;

namespace Aaron.Core.Services.Catalogs
{
    public class CatalogTemplateService : ICatalogTemplateService
    {
        private const string CATALOGTEMPLATE_BY_ID = "Aaron.Core.catalogtemplate.id-{0}";
        private const string CATALOGTEMPLATE_BY_PAGE = "Aaron.Core.catalogtemplate.page-{0}--{1}--{2}";
        private const string CATALOGTEMPLATE_ALL = "Aaron.Core.catalogtemplate.all-{0}";

        private readonly IRepository<CatalogTemplate> _catalogTemplatesRepository;
        private readonly ICacheManager _cacheManager;

        public CatalogTemplateService(IRepository<CatalogTemplate> catalogTemplatesRepository,
            ICacheManager cacheManager)
        {
            _catalogTemplatesRepository = catalogTemplatesRepository;
            _cacheManager = cacheManager;
        }

        public void InsertCatalogTemplate(CatalogTemplate catalogTemplate)
        {
            if (catalogTemplate.Equals(null))
                throw new ArgumentNullException("catalogTemplate");

            _catalogTemplatesRepository.Insert(catalogTemplate);

            _cacheManager.RemoveByPattern(CATALOGTEMPLATE_ALL);
            _cacheManager.RemoveByPattern(CATALOGTEMPLATE_BY_PAGE);
            _cacheManager.RemoveByPattern(CATALOGTEMPLATE_BY_ID);
        }

        public void UpdateCatalogTemplate(CatalogTemplate catalogTemplate)
        {
            if (catalogTemplate.Equals(null))
                throw new ArgumentNullException("catalogTemplate");

            _catalogTemplatesRepository.Update(catalogTemplate);

            _cacheManager.RemoveByPattern(CATALOGTEMPLATE_ALL);
            _cacheManager.RemoveByPattern(CATALOGTEMPLATE_BY_PAGE);
            _cacheManager.RemoveByPattern(CATALOGTEMPLATE_BY_ID);
        }

        public void DeleteCatalogTemplate(CatalogTemplate catalogTemplate)
        {
            if (catalogTemplate.Equals(null))
                throw new ArgumentNullException("catalogTemplate");

            _catalogTemplatesRepository.Delete(catalogTemplate);

            _cacheManager.RemoveByPattern(CATALOGTEMPLATE_ALL);
            _cacheManager.RemoveByPattern(CATALOGTEMPLATE_BY_PAGE);
            _cacheManager.RemoveByPattern(CATALOGTEMPLATE_BY_ID);
        }

        public void ChangePublished(CatalogTemplate catalogTemplate)
        {
            if (catalogTemplate.Equals(null))
                throw new ArgumentNullException("catalogTemplate");

            catalogTemplate.Published = !catalogTemplate.Published;
            this.UpdateCatalogTemplate(catalogTemplate);
        }

        public CatalogTemplate GetCatalogTemplateById(int id)
        {
            if (id == 0)
                return null;

            var key = string.Format(CATALOGTEMPLATE_BY_ID, id);
            return _cacheManager.Get(key, () => 
            {
                return _catalogTemplatesRepository.GetById(id);     
            });
        }

        public IList<CatalogTemplate> GetAllCatalogTemplates(bool showHidden = false)
        {
            var key = string.Format(CATALOGTEMPLATE_ALL, showHidden);

            return _cacheManager.Get(key, () =>
            {
                var list = _catalogTemplatesRepository.Table;
                if (!showHidden)
                    list = list.Where(ct => ct.Published);
                list = list.OrderByDescending(ct => ct.Id);

                return list.ToList();
            });
        }

        public IPagedList<CatalogTemplate> GetAllCatalogTemplates(int pageIndex, int pageSize, bool showHidden = false)
        {
            var key = string.Format(CATALOGTEMPLATE_BY_PAGE, showHidden, pageIndex, pageSize);

            return _cacheManager.Get(key, () => 
            {
                var list = _catalogTemplatesRepository.Table;
                if (!showHidden)
                    list = list.Where(ct => ct.Published);
                list = list.OrderByDescending(ct => ct.Id);

                return list
                    .ToPagedList(pageIndex, pageSize); 
            });
        }
    }
}
