namespace Aaron.Core.Web.Localization
{
    public interface ILocalizedModelLocal
    {
        int LanguageId { get; set; }
    }

    public class SEOLocalizedModelLocal : SEOEntityModel, ILocalizedModelLocal
    {
        public int LanguageId { get; set; }
    }
}
