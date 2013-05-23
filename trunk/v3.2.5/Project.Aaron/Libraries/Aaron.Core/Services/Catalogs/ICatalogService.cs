using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aaron.Core;
using Aaron.Core.Domain.Catalogs;

namespace Aaron.Core.Services.Catalogs
{
    public interface ICatalogService : IServices
    {
        #region Catalog
        void InsertCatalog(Catalog catalog);
        void UpdateCatalog(Catalog catalog);
        void DeleteCatalog(Catalog catalog);
        void DeleteCatalog(int id);
        Catalog GetCatalogById(int id);
        Catalog GetCatalogBySEName(string seName);
        IList<Catalog> GetAllCatalog(bool sortParent = false, bool showHidden = false);
        IPagedList<Catalog> GetAllCatalog(int page, int pageSize, bool showHidden = false);
        IList<Catalog> GetCatalogsByGenericCatalogId(int genericCatalogId, bool sortParent = false, bool showHidden = false);
        #endregion

        #region Catalog Attribute
        void CreateRecordForCatalog(Catalog catalog);
        void InsertCatalogAttributeRecord(CatalogAttributeRecord CatalogAttributeRecord);
        void UpdateCatalogAttributeRecord(CatalogAttributeRecord CatalogAttributeRecord);
        void DeleteCatalogAttributeRecord(CatalogAttributeRecord CatalogAttributeRecord);
        void DeleteCatalogAttributeRecord(Guid id);
        void DeleteCatalogAttributeRecordByCatalogId(int catalogId);
        CatalogAttributeRecord GetCatalogAttributeRecordById(Guid id);
        IList<CatalogAttributeRecord> GetAllCatalogAttributeRecord(); 
        #endregion
    }
}
