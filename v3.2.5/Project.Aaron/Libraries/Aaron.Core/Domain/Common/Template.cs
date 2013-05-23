using Aaron.Core;

namespace Aaron.Core.Domain.Common
{
    public partial class Template : BaseEntity<int>
    {
        public virtual string Name { get; set; }
        public virtual string ViewPath { get; set; }
        public virtual int DisplayOrder { get; set; }
        public virtual bool Published { get; set; }
    }
}
