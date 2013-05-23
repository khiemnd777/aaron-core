using Aaron.Core;
using Aaron.Core.Domain.Topics;

namespace Aaron.Data.Mapping.Topics
{
    public class TopicMap : SEOEntityTypeConfiguration<Topic, int>
    {
        public TopicMap()
            : base()
        {
            this.Property(t => t.SystemName);
            this.Property(t => t.Password);
            this.Property(t => t.Title);
            this.Property(t => t.Body);
        }
    }
}
