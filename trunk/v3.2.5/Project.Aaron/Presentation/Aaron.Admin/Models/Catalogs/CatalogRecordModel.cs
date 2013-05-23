using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Aaron.Core.Domain.Common;

namespace Aaron.Admin.Models.Catalogs
{
    public class CatalogRecordModel
    {
        public string InputName { get; set; }

        public string Name { get; set; }

        public string Value { get; set; }

        public AttributeControlType Type { get; set; }
    }
}