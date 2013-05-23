using Aaron.Core.Domain.Common;

namespace Aaron.Core.Domain.Pages
{
    public partial class PageAttribute : Attribute
    {
        public virtual int PageId { get; set; }
        public virtual Page Page { get; set; }
    }
}
