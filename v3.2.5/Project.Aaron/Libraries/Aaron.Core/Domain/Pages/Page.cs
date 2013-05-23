using System.Collections.Generic;

namespace Aaron.Core.Domain.Pages
{
    public partial class Page : SEOEntity<int>
    {
        private ICollection<PageAttribute> _pageAttributes;
        public virtual string Name { get; set; }
        public virtual string SystemName { get; set; }
        public virtual ICollection<PageAttribute> PageAttributes
        {
            get { return _pageAttributes ?? (_pageAttributes = new List<PageAttribute>()); }
            protected set { _pageAttributes = value; }
        }
    }
}
