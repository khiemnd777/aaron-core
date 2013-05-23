using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aaron.Core;
using Aaron.Core.Data;
using Aaron.Core.Domain.Catalogs;
using Aaron.Core.Caching;

namespace Aaron.Core.Services.Catalogs
{
    public class GenericCatalogService: IGenericCatalogService
    {
        private const string GENERICCATALOG_BY_ID = "Aaron.Core.genericcatalog.id-{0}";
        private const string GENERICCATALOG_BY_SYSTEMNAME = "Aaron.Core.genericcatalog.sn-{0}";
        private const string GENERICCATALOG_BY_PAGE = "Aaron.Core.allgenericcatalog-{0}--{1}--{2}";
        private const string GENERICCATALOG_ALL = "Aaron.Core.allgenericcatalog-{0}";
        private const string GENERICCATALOGATTRIBUTES_BY_ID = "Aaron.Core.genericcatalogattribute.id-{0}";
        private const string GENERICCATALOG_BY_BLOCKVIEW = "Aaron.Core.genericcatalog.blockview-{0}";

        private readonly IRepository<GenericCatalog> _genericCatalogRepository;
        private readonly IRepository<GenericCatalogAttribute> _genericCatalogAttributeRepository;
        private readonly IRepository<GenericCatalogTemplate> _genericCatalogTemplateRepository;
        private readonly ICacheManager _cacheManager;

        public GenericCatalogService(IRepository<GenericCatalog> genericCatalogRepository, 
            IRepository<GenericCatalogAttribute> genericCatalogAttributeRepository, 
            IRepository<GenericCatalogTemplate> genericCatalogTemplateRepository,
            ICacheManager cacheManager)
        {
            _genericCatalogRepository = genericCatalogRepository;
            _genericCatalogAttributeRepository = genericCatalogAttributeRepository;
            _genericCatalogTemplateRepository = genericCatalogTemplateRepository;
            _cacheManager = cacheManager;
        }

        #region Generic Catalog Services

        public void InsertGenericCatalog(GenericCatalog genericCatalog)
        {
            if(genericCatalog == null)
                throw new ArgumentNullException("genericCatalog");

            genericCatalog.CreationDate = DateTime.Now;
            genericCatalog.ModifiedDate = DateTime.Now;

            _genericCatalogRepository.Insert(genericCatalog);

            //cache
            _cacheManager.RemoveByPattern(GENERICCATALOG_BY_PAGE);
            _cacheManager.RemoveByPattern(GENERICCATALOG_ALL);
            _cacheManager.RemoveByPattern(GENERICCATALOG_BY_ID);
            _cacheManager.RemoveByPattern(GENERICCATALOG_BY_BLOCKVIEW);
            _cacheManager.RemoveByPattern(GENERICCATALOG_BY_SYSTEMNAME);
        }

        public void UpdateGenericCatalog(GenericCatalog genericCatalog)
        {
            if (genericCatalog == null)
                throw new ArgumentNullException("genericCatalog");

            genericCatalog.ModifiedDate = DateTime.Now;
            _genericCatalogRepository.Update(genericCatalog);

            //cache
            _cacheManager.RemoveByPattern(GENERICCATALOG_BY_PAGE);
            _cacheManager.RemoveByPattern(GENERICCATALOG_ALL);
            _cacheManager.RemoveByPattern(GENERICCATALOG_BY_ID);
            _cacheManager.RemoveByPattern(GENERICCATALOGATTRIBUTES_BY_ID);
            _cacheManager.RemoveByPattern(GENERICCATALOG_BY_BLOCKVIEW);
            _cacheManager.RemoveByPattern(GENERICCATALOG_BY_SYSTEMNAME);
        }

        public void DeleteGenericCatalog(GenericCatalog genericCatalog)
        {
            if (genericCatalog == null)
                throw new ArgumentNullException("genericCatalog");

            if(GenericCatalogAttributeExisted(genericCatalog.Id))
            {
                var attributeList = GetGenericCatalogById(genericCatalog.Id).Attributes.ToList();
                foreach (var attribute in attributeList)
                {
                    DeleteGenericCatalogAttribute(attribute);
                }

                //_genericCatalogRepository.Delete(_genericCatalogRepository
                //    .GetById(genericCatalog.Id)
                //    .Attributes);

                //cache
                _cacheManager.RemoveByPattern(GENERICCATALOG_BY_PAGE);
                _cacheManager.RemoveByPattern(GENERICCATALOG_ALL);
                _cacheManager.RemoveByPattern(GENERICCATALOG_BY_ID);
                _cacheManager.RemoveByPattern(GENERICCATALOGATTRIBUTES_BY_ID);
                _cacheManager.RemoveByPattern(GENERICCATALOG_BY_BLOCKVIEW);
                _cacheManager.RemoveByPattern(GENERICCATALOG_BY_SYSTEMNAME);
            }
    
            _genericCatalogRepository.Delete(genericCatalog);
        }

        public GenericCatalog GetGenericCatalogById(int id)
        {
            if (id == 0)
                return null;

            var key = string.Format(GENERICCATALOG_BY_ID, id);
            return _cacheManager.Get(key, () => 
            {
                return _genericCatalogRepository.GetById(id); 
            });
        }

        public GenericCatalog GetGenericCatalogBySystemName(string systemName)
        {
            if (string.IsNullOrEmpty(systemName))
                return null;

            var key = string.Format(GENERICCATALOG_BY_SYSTEMNAME, systemName);
            return _cacheManager.Get(key, () => 
            {
                return _genericCatalogRepository.Get(filter: x => systemName.Equals(x.SystemName))
                    .FirstOrDefault();
            });
        }

        public void ChangePublished(GenericCatalog genericCatalog)
        {
            if (genericCatalog == null)
                throw new ArgumentNullException("genericCatalog");

            genericCatalog.Published = !genericCatalog.Published;
            UpdateGenericCatalog(genericCatalog);
        }

        public IList<GenericCatalog> GetAllGenericCatalogs(bool showHidden = false)
        {
            var key = string.Format(GENERICCATALOG_ALL, showHidden);
            return _cacheManager.Get(key, () =>
            {
                var list = _genericCatalogRepository
                .Get(orderBy: (x) => x.OrderByDescending(y => y.Id));
                if (!showHidden)
                    list = list.Where(gc => gc.Published);

                return list.ToList();
            });
        }

        public IPagedList<GenericCatalog> GetAllGenericCatalogs(int pageIndex, int pageSize, bool showHidden = false)
        {
            var key = string.Format(GENERICCATALOG_BY_PAGE, showHidden, pageIndex, pageSize);
            return _cacheManager.Get(key, () => 
            {
                var list = _genericCatalogRepository
                .Get(orderBy: (x) => x.OrderByDescending(y => y.Id));
                if (!showHidden)
                    list = list.Where(gc => gc.Published);

                return list.ToPagedList(pageIndex, pageSize);
            });
        }

        public IList<GenericCatalogTemplate> GetGenericCatalogTemplates(bool showHidden = false)
        {
            var list = _genericCatalogTemplateRepository.Table;
            if (!showHidden)
                list = list.Where(gct => gct.Published);

            return list.ToList();
        }

        public bool GenericCatalogTemplateExisted()
        {
            return _genericCatalogTemplateRepository.Table.Any(x => x.Published);
        }

        public IList<GenericCatalog> GetGenericCatalogByBlockView(int blockViewId)
        {
            if (blockViewId == 0)
                return null;
            var key = string.Format(GENERICCATALOG_BY_BLOCKVIEW, blockViewId);
            return _cacheManager.Get(key, () => 
            {
                return _genericCatalogRepository
                    .Get(x => x.BlockViewId == blockViewId && x.Published)
                    .OrderBy(x => x.DisplayOrder)
                    .ToList();
            });
        }

        #endregion 

        #region Generic Catalog Attribute Services

        public bool GenericCatalogAttributeExisted(int GenericCatalogId)
        {
            return _genericCatalogRepository.GetById(GenericCatalogId).Attributes.Count() > 0;
        }

        public GenericCatalogAttribute GetGenericCatalogAttributeById(int id)
        {
            if (id == 0)
                return null;

            var key = string.Format(GENERICCATALOGATTRIBUTES_BY_ID, id);
            return _cacheManager.Get(key, () =>
            {
                return _genericCatalogAttributeRepository.GetById(id);    
            });
        }

        public void DeleteGenericCatalogAttribute(GenericCatalogAttribute genericCatalogAttribute)
        {
            if (genericCatalogAttribute == null)
                throw new ArgumentNullException("genericCatalogAttribute");

            _genericCatalogAttributeRepository.Delete(genericCatalogAttribute);

            _cacheManager.RemoveByPattern(GENERICCATALOGATTRIBUTES_BY_ID);
        }

        #endregion 
    }
}
