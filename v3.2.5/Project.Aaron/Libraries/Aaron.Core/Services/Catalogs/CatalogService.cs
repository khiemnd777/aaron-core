using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aaron.Core;
using Aaron.Core.Data;
using Aaron.Core.Caching;
using Aaron.Core.Domain.Catalogs;

namespace Aaron.Core.Services.Catalogs
{
    public class CatalogService : ICatalogService
    {
        private const string CATALOG_BY_ID = "Aaron.catalog.id-{0}";
        private const string CATALOG_BY_PAGE = "Aaron.catalog.page-{0}--{1}--{2}";
        private const string CATALOG_ALL = "Aaron.catalog.all-{0}";
        private const string CATALOG_BY_GENERICCATALOGID = "Aaron.catalog.by.genericcatalogid-{0}--{1}--{2}";
        private const string CATALOGATTRIBUTERECORD_BY_ID = "Aaron.catalogattributerecord.id-{0}";
        private const string CATALOGATTRIBUTERECORD_ALL = "Aaron.catalogattributerecord.all";

        private readonly IRepository<Catalog> _catalogRepository;
        private readonly IRepository<CatalogAttributeRecord> _catalogAttributeRecordRepository;
        private readonly ICacheManager _cacheManager;

        public CatalogService(IRepository<Catalog> catalogRepository,
            IRepository<CatalogAttributeRecord> catalogAttributeRecordRepository,
            ICacheManager cacheManager)
        {
            _catalogRepository = catalogRepository;
            _catalogAttributeRecordRepository = catalogAttributeRecordRepository;
            _cacheManager = cacheManager;
        }

        #region Catalog

        public void InsertCatalog(Catalog catalog)
        {
            if (catalog == null)
                throw new Exception("catalog");
            _catalogRepository.Insert(catalog);

            _cacheManager.RemoveByPattern(CATALOG_ALL);
            _cacheManager.RemoveByPattern(CATALOG_BY_PAGE);
            _cacheManager.RemoveByPattern(CATALOG_BY_ID);
            _cacheManager.RemoveByPattern(CATALOG_BY_GENERICCATALOGID);
        }

        public void UpdateCatalog(Catalog catalog)
        {
            if (catalog == null)
                throw new Exception("catalog");
            _catalogRepository.Update(catalog);

            _cacheManager.RemoveByPattern(CATALOG_ALL);
            _cacheManager.RemoveByPattern(CATALOG_BY_PAGE);
            _cacheManager.RemoveByPattern(CATALOG_BY_ID);
            _cacheManager.RemoveByPattern(CATALOG_BY_GENERICCATALOGID);
        }

        public void DeleteCatalog(Catalog catalog)
        {
            if (catalog == null)
                throw new Exception("catalog");

            if (catalog.Catalogs.Any())
                throw new AaronException("Do not delete parent catalog has more childs");

            _catalogRepository.Delete(catalog);

            _cacheManager.RemoveByPattern(CATALOG_ALL);
            _cacheManager.RemoveByPattern(CATALOG_BY_PAGE);
            _cacheManager.RemoveByPattern(CATALOG_BY_ID);
            _cacheManager.RemoveByPattern(CATALOG_BY_GENERICCATALOGID);
        }

        public void DeleteCatalog(int id)
        {
            var catalog = this.GetCatalogById(id);
            this.DeleteCatalog(catalog);
        }

        public Catalog GetCatalogById(int id)
        {
            if (id == 0)
                return null;

            var key = string.Format(CATALOG_BY_ID, id);
            return _cacheManager.Get(key, () =>
            {
                return _catalogRepository.GetById(id);
            });
        }

        public Catalog GetCatalogBySEName(string seName)
        {
            if (string.IsNullOrEmpty(seName))
                return null;
            return _catalogRepository.Table
                    .Where(c => seName.Equals(c.SEOUrlName))
                    .SingleOrDefault();
        }

        public IList<Catalog> GetAllCatalog(bool sortParent = false, bool showHidden = false)
        {
            var key = string.Format(CATALOG_ALL, showHidden);
            return _cacheManager.Get(key, () => 
            {
                var list = _catalogRepository.Table;
                if (!showHidden)
                    list = list.Where(c => c.Published);
                list = list.OrderByDescending(c => c.Id);

                if(sortParent)
                    return list
                        .ToList()
                        .SortCatalogsForParent("", 0);

                return list.ToList();
            });
        }

        public IPagedList<Catalog> GetAllCatalog(int page, int pageSize, bool showHidden = false)
        {
            var key = string.Format(CATALOG_BY_PAGE, showHidden, page, pageSize);
            return _cacheManager.Get(key, () =>
            {
                var list = _catalogRepository.Table;
                if (!showHidden)
                    list = list.Where(c => c.Published);

                list = list.OrderByDescending(c => c.Id);

                return list.ToPagedList(page, pageSize);
            });
        }

        public IList<Catalog> GetCatalogsByGenericCatalogId(int genericCatalogId, bool sortParent = false, bool showHidden = false)
        {
            if(genericCatalogId == 0)
                return GetAllCatalog(sortParent, showHidden);

            var key = string.Format(CATALOG_BY_GENERICCATALOGID, genericCatalogId, sortParent, showHidden);
            return _cacheManager.Get(key, () => 
            {
                var list = _catalogRepository.Table
                    .Where(c => c.GenericCatalogId == genericCatalogId);
                if (!showHidden)
                    list = list.Where(c => c.Published);

                list = list.OrderBy(c => c.DisplayOrder);

                if (sortParent)
                    return list
                        .ToList()
                        .SortCatalogsForParent("", 0);

                return list.ToList();
            });
        }
        #endregion

        #region Catalog Attribute Record

        public void CreateRecordForCatalog(Catalog catalog)
        {
            if (catalog.GenericCatalog == null) return;
            foreach (var attribute in catalog.GenericCatalog.Attributes)
            {
                if (!catalog.CatalogAttributeRecord.Any(x => x.AttributeId == attribute.Id) || catalog.CatalogAttributeRecord == null)
                {
                    var record = new CatalogAttributeRecord
                    {
                        Id = Guid.NewGuid(),
                        CatalogId = catalog.Id,
                        AttributeId = attribute.Id,
                        Value = ""
                    };
                    InsertCatalogAttributeRecord(record);
                }

            }
        }

        public void InsertCatalogAttributeRecord(CatalogAttributeRecord catalogAttributeRecord)
        {
            if (catalogAttributeRecord == null)
                throw new ArgumentNullException("catalogAttributeRecord");

            _catalogAttributeRecordRepository.Insert(catalogAttributeRecord);

            _cacheManager.RemoveByPattern(CATALOGATTRIBUTERECORD_BY_ID);
            _cacheManager.RemoveByPattern(CATALOGATTRIBUTERECORD_ALL);
        }

        public void UpdateCatalogAttributeRecord(CatalogAttributeRecord catalogAttributeRecord)
        {
            if (catalogAttributeRecord == null)
                throw new ArgumentNullException("catalogAttributeRecord");

            _catalogAttributeRecordRepository.Update(catalogAttributeRecord);

            _cacheManager.RemoveByPattern(CATALOGATTRIBUTERECORD_BY_ID);
            _cacheManager.RemoveByPattern(CATALOGATTRIBUTERECORD_ALL);
        }

        public void DeleteCatalogAttributeRecord(CatalogAttributeRecord catalogAttributeRecord)
        {
            if (catalogAttributeRecord == null)
                throw new ArgumentNullException("catalogAttributeRecord");

            _catalogAttributeRecordRepository.Delete(catalogAttributeRecord);

            _cacheManager.RemoveByPattern(CATALOGATTRIBUTERECORD_BY_ID);
            _cacheManager.RemoveByPattern(CATALOGATTRIBUTERECORD_ALL);
        }

        public void DeleteCatalogAttributeRecord(Guid id)
        {
            var catalogAttributeRecord = GetCatalogAttributeRecordById(id);
            DeleteCatalogAttributeRecord(catalogAttributeRecord);
        }

        public void DeleteCatalogAttributeRecordByCatalogId(int catalogId)
        {
            List<Guid> recordIds = _catalogAttributeRecordRepository
                .Get(f => f.CatalogId == catalogId)
                .Select(i => i.Id)
                .ToList();

            foreach (var recordId in recordIds)
            {
                try
                {
                    DeleteCatalogAttributeRecord(recordId);
                }
                catch { continue; }
            }

        }

        public CatalogAttributeRecord GetCatalogAttributeRecordById(Guid id)
        {
            if (id == null)
                return null;
            var key = string.Format(CATALOGATTRIBUTERECORD_BY_ID, id);
            return _cacheManager.Get(key, () => 
            {
                return _catalogAttributeRecordRepository.GetById(id); 
            });
        }

        public IList<CatalogAttributeRecord> GetAllCatalogAttributeRecord()
        {
            return _cacheManager.Get(CATALOGATTRIBUTERECORD_ALL, () => 
            {
                var list = _catalogAttributeRecordRepository.Table;
                return list.ToList(); 
            });
        } 
        #endregion
    }
}