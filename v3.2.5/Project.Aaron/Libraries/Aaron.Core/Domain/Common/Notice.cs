using Aaron.Core;
using Aaron.Core.Domain.Accounts;
using System.Collections.Generic;

namespace Aaron.Core.Domain.Common
{
    public partial class Notice : BaseEntity<int>
    {
        private ICollection<Account> _accounts;

        public virtual string Name { get; set; }
        public virtual string Content { get; set; }
        public virtual bool IsSystem { get; set; }
        public virtual bool Published { get; set; }
        public virtual ICollection<Account> Accounts
        {
            get { return _accounts ?? (_accounts = new List<Account>()); }
            protected set { _accounts = value; }
        }
    }
}
