using Aaron.Core;

namespace Aaron.Core.Domain.Common
{
    public partial class Attribute : BaseEntity<int>
    {
        public virtual string Name { get; set; }
        public virtual string SystemName { get; set; }
        public virtual int ControlTypeId { get; set; }
        public virtual bool Published { get; set; }
        public virtual int DisplayOrder { get; set; }

        public virtual AttributeControlType ControlType
        {
            get 
            {
                return (AttributeControlType)this.ControlTypeId;
            }
            set
            {
                this.ControlTypeId = (int)value;
            }
        }
    }
}
