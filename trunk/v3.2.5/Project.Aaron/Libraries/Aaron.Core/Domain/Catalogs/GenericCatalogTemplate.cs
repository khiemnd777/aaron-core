using System.Collections.Generic;
using Aaron.Core.Domain.Common;

namespace Aaron.Core.Domain.Catalogs
{
    public partial class GenericCatalogTemplate : Template
    {
        private ICollection<GenericCatalog> _genericCatalogTemplates;
        public virtual ICollection<GenericCatalog> GenericCatalogTemplates
        {
            get { return _genericCatalogTemplates ?? (_genericCatalogTemplates = new List<GenericCatalog>()); }
            protected set { _genericCatalogTemplates = value; }
        }
    }
}
