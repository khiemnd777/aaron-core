using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Aaron.Core.Web
{
    public class BaseModel
    {
        public virtual void BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext) { }
    }

    public class BaseEntityModel : BaseModel
    {
        public virtual int Id { get; set; }
    }
}
