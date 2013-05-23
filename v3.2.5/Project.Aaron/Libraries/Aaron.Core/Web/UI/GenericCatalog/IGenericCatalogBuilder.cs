using System.Web.Mvc;
namespace Aaron.Core.Web.UI.GenericCatalog
{
    public interface IGenericCatalogBuilder
    {
        void GetGenericCatalogList(string blockView);
        string GenerateGenericCatalog();
    }
}
