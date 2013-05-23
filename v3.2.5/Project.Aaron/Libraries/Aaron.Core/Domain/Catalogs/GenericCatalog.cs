using System.Collections.Generic;
using Aaron.Core;
using Aaron.Core.Domain.Common;

namespace Aaron.Core.Domain.Catalogs
{
    public partial class GenericCatalog : SEOEntity<int>
    {
        private ICollection<GenericCatalogAttribute> _attributes;
        private ICollection<Catalog> _catalogs;
        public virtual string Name { get; set; }
        public virtual string SystemName { get; set; }
        public virtual string Icon { get; set; }
        public virtual int ItemOnPage { get; set; }
        public virtual int SizeOnMenu { get; set; }
        public virtual bool Published { get; set; }
        public virtual int DisplayOrder { get; set; }
        public virtual int? TemplateId { get; set; }
        public virtual int BlockViewId { get; set; }
        public virtual GenericCatalogTemplate Template { get; set; }
        public virtual ICollection<GenericCatalogAttribute> Attributes
        {
            get { return _attributes ?? (_attributes = new List<GenericCatalogAttribute>()); }
            protected set { _attributes = value; }
        }
        public virtual ICollection<Catalog> Catalogs
        {
            get { return _catalogs ?? (_catalogs = new List<Catalog>()); }
            protected set { _catalogs = value; }
        }

        public virtual BlockViewType BlockView
        {
            get
            {
                return (BlockViewType)this.BlockViewId;
            }
            set
            {
                this.BlockViewId = (int)value;
            }
        }
    }
}
