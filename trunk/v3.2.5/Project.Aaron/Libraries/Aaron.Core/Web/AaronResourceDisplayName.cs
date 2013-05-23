using Aaron.Core;
using Aaron.Core.Infrastructure;
using Aaron.Core.Services.Localization;
using Aaron.Core.Web.Mvc;

namespace Aaron.Core.Web
{
    public class AaronResourceDisplayName : System.ComponentModel.DisplayNameAttribute, IModelAttribute
    {
        private string _resourceValue = string.Empty;
        //private bool _resourceValueRetrived;

        public AaronResourceDisplayName(string resourceKey)
            : base(resourceKey)
        {
            ResourceKey = resourceKey;
        }

        public string ResourceKey { get; set; }

        public override string DisplayName
        {
            get
            {
                //do not cache resources because it causes issues when you have multiple languages
                //if (!_resourceValueRetrived)
                //{
                var langId = IoC.Resolve<ICurrentActivity>().CurrentLanguage.Id;
                _resourceValue = IoC
                    .Resolve<ILocalizationService>()
                    .GetResource(ResourceKey, langId, true, ResourceKey);
                //    _resourceValueRetrived = true;
                //}
                return _resourceValue;
            }
        }

        public string Name
        {
            get { return "AaronResourceDisplayName"; }
        }
    }
}
