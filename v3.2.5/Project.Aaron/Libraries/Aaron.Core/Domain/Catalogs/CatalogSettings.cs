using Aaron.Core.SysConfiguration;

namespace Aaron.Core.Domain.Catalogs
{
    public class CatalogSettings : ISettings
    {
        public int DefaultSizeOnMenu { get; set; }
        public int DefaultGenericCatalogItemsOnPage { get; set; }
        public int DefaultCatalogItemsOnPage { get; set; }
        public string DefaultGenericCatalogIcon { get; set; }
        public string DefaultAvatar { get; set; }
        public string IconDimension { get; set; }
        public string AvatarDimension { get; set; }
        public bool ShowShareButton { get; set; }
        public string GenericCatalogIconPath { get; set; }
        public bool AllowAutoUpload { get; set; }
        public bool AllowUploadMultiFile { get; set; }
    }
}
