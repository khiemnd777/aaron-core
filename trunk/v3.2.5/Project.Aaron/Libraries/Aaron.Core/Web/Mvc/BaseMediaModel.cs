using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aaron.Core.Web;

namespace Aaron.Core.Web.Mvc
{
    public class BaseMediaModel : SEOEntityModel
    {
        public string MediaId { get; set; }
        public string Artist { get; set; }
        public int ArtistId { get; set; }
        public string Gene { get; set; }
        public int GeneId { get; set; }
        public string Category { get; set; }
        public int CategoryId { get; set; }
        public string MediaName { get; set; }
        public string Description { get; set; }
        public string Lyric { get; set; }
    }
}
