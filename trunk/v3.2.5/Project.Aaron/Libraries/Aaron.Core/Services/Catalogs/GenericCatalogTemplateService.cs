using System;
using System.Linq;
using Aaron.Core;
using Aaron.Core.Data;
using Aaron.Core.Domain.Catalogs;
using Aaron.Core.Caching;
using System.Collections.Generic;

namespace Aaron.Core.Services.Catalogs
{
    public class GenericCatalogTemplateService : IGenericCatalogTemplateService
    {
        private const string GENERICCATALOGTEMPLATE_BY_ID = "Aaron.Core.genericcatalogtemplate.id-{0}";
        private const string GENERICCATALOGTEMPLATE_BY_PAGE = "Aaron.Core.genericcatalogtemplate.page-{0}--{1}--{2}";
        private const string GENERICCATALOGTEMPLATE_ALL = "Aaron.Core.genericcatalogtemplate.all-{0}";

        private readonly IRepository<GenericCatalogTemplate> _genericCatalogTemplateRepository;
        private readonly ICacheManager _cacheManager;

        public GenericCatalogTemplateService(IRepository<GenericCatalogTemplate> genericCatalogTemplateRepository,
            ICacheManager cacheManager)
        {
            _genericCatalogTemplateRepository = genericCatalogTemplateRepository;
            _cacheManager = cacheManager;
        }

        public void InsertGenericCatalogTemplate(GenericCatalogTemplate genericCatalogTemplate)
        {
            if (genericCatalogTemplate.Equals(null))
                throw new ArgumentNullException("genericCatalogTemplate");

            _genericCatalogTemplateRepository.Insert(genericCatalogTemplate);

            _cacheManager.RemoveByPattern(GENERICCATALOGTEMPLATE_ALL);
            _cacheManager.RemoveByPattern(GENERICCATALOGTEMPLATE_BY_ID);
            _cacheManager.RemoveByPattern(GENERICCATALOGTEMPLATE_BY_PAGE);
        }

        public void UpdateGenericCatalogTemplate(GenericCatalogTemplate genericCatalogTemplate)
        {
            if (genericCatalogTemplate.Equals(null))
                throw new ArgumentNullException("genericCatalogTemplate");

            _genericCatalogTemplateRepository.Update(genericCatalogTemplate);

            _cacheManager.RemoveByPattern(GENERICCATALOGTEMPLATE_ALL);
            _cacheManager.RemoveByPattern(GENERICCATALOGTEMPLATE_BY_ID);
            _cacheManager.RemoveByPattern(GENERICCATALOGTEMPLATE_BY_PAGE);
        }

        public void DeleteGenericCatalogTemplate(GenericCatalogTemplate genericCatalogTemplate)
        {
            if (genericCatalogTemplate.Equals(null))
                throw new ArgumentNullException("genericCatalogTemplate");

            _genericCatalogTemplateRepository.Delete(genericCatalogTemplate);

            _cacheManager.RemoveByPattern(GENERICCATALOGTEMPLATE_ALL);
            _cacheManager.RemoveByPattern(GENERICCATALOGTEMPLATE_BY_ID);
            _cacheManager.RemoveByPattern(GENERICCATALOGTEMPLATE_BY_PAGE);
        }

        public void ChangePublished(GenericCatalogTemplate genericCatalogTemplate)
        {
            if (genericCatalogTemplate.Equals(null))
                throw new ArgumentNullException("genericCatalogTemplate");

            genericCatalogTemplate.Published = !genericCatalogTemplate.Published;
            this.UpdateGenericCatalogTemplate(genericCatalogTemplate);
        }

        public GenericCatalogTemplate GetGenericCatalogTemplateById(int id)
        {
            if (id == 0)
                return null;

            var key = string.Format(GENERICCATALOGTEMPLATE_BY_ID, id);
            return _cacheManager.Get(key, () => 
            {
                return _genericCatalogTemplateRepository.GetById(id); 
            });
        }

        public IList<GenericCatalogTemplate> GetAllGenericCatalogTemplates(bool showHidden = false)
        {
            var key = string.Format(GENERICCATALOGTEMPLATE_ALL, showHidden);
            return _cacheManager.Get(key, () => 
            {
                var list = _genericCatalogTemplateRepository.Table;
                if (!showHidden)
                    list = list.Where(gct => gct.Published);
                list = list.OrderByDescending(gct => gct.Id);

                return list.ToList();
            });
        }

        public IPagedList<GenericCatalogTemplate> GetAllGenericCatalogTemplates(int pageIndex, int pageSize, bool showHidden = false)
        {
            var key = string.Format(GENERICCATALOGTEMPLATE_BY_PAGE, showHidden, pageIndex, pageSize);
            return _cacheManager.Get(key, () => 
            {
                var list = _genericCatalogTemplateRepository.Table;
                if (!showHidden)
                    list = list.Where(gct => gct.Published);

                list = list.OrderByDescending(gct => gct.Id);
                return list
                    .ToPagedList(pageIndex, pageSize);
            });
        }
    }
}
