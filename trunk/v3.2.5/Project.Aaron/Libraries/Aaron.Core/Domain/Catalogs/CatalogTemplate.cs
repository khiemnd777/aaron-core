using System;
using System.Collections.Generic;
using Aaron.Core;
using Aaron.Core.Domain.Common;

namespace Aaron.Core.Domain.Catalogs
{
    public partial class CatalogTemplate : Template
    {
        private ICollection<Catalog> _catalogs;
        public virtual ICollection<Catalog> Catalogs
        {
            get { return _catalogs ?? (_catalogs = new List<Catalog>()); }
            protected set { _catalogs = value; }
        }
    }
}
