using System.Collections.Generic;

namespace Aaron.Core.Web.Localization
{
    public interface ILocalizedModel
    {

    }
    public interface ILocalizedModel<TLocalizedModel> : ILocalizedModel
    {
        #region Data Members (1)

        IList<TLocalizedModel> Locales { get; set; }

        #endregion Data Members
    }
}
