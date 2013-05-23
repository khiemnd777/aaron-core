using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using Telerik.Web.Mvc;
using Aaron.Core.Domain.Catalogs;
using Aaron.Core.Services.Catalogs;
using Aaron.Admin.Models.Catalogs;
using Aaron.Core.Web.Security;
using Aaron.Core.Services.Security;
using Aaron.Core;

namespace Aaron.Admin.Controllers
{
    [AdminAuthorize]
    public class TemplatesController : BaseController
    {
        private readonly ICatalogTemplateService _catalogTemplateService;
        private readonly IGenericCatalogTemplateService _genericCatalogTemplateService;
        private readonly IPermissionService _permissionService;

        private const string DefaultTemplateUrl = "~/Views/Shared/Templates/";

        public TemplatesController(ICatalogTemplateService catalogTemplateService,
            IGenericCatalogTemplateService genericCatalogTemplateService,
            IPermissionService permissionService)
        {
            _catalogTemplateService = catalogTemplateService;
            _genericCatalogTemplateService = genericCatalogTemplateService;
            _permissionService = permissionService;
        }

        #region Upload file

        // Save File Upload/
        public ActionResult SaveFileUpload(IEnumerable<HttpPostedFileBase> templateAttachments)
        {
            foreach (var file in templateAttachments)
            {
                var fileName = Path.GetFileName(file.FileName);
                if (fileName != null)
                {
                    var physicalPath = Path.Combine(Server.MapPath(DefaultTemplateUrl), fileName);
                    //System.IO.File.Create(physicalPath);
                    file.SaveAs(physicalPath);

                    TempData["fileUploaded"] = physicalPath;
                }
            }


            return Content("");
        }

        //Remove file Uploaded
        public ActionResult RemoveFileUpload(string[] fileNames)
        {
            foreach (var file in fileNames)
            {
                var fileName = Path.GetFileName(file);
                if (fileName != null)
                {
                    var physicalPath = Path.Combine(Server.MapPath(DefaultTemplateUrl), fileName);

                    if (System.IO.File.Exists(physicalPath))
                        System.IO.File.Delete(physicalPath);
                }
            }
            return Content("");
        }

        //File after upload
        private string FileAfterUpload(string oldTemplatePath = null)
        {
            var templateAbsolutePath = (!string.IsNullOrEmpty(oldTemplatePath)) ? oldTemplatePath : "";

            if (TempData["fileUploaded"] != null)
            {
                var templatePath = (string)TempData["fileUploaded"];
                var templateName = templatePath.Substring(templatePath.LastIndexOf(@"\") + 1);
                //templateName = Guid.NewGuid() + "-" + CommonHelper.RemoveMarks(templateName);
                templateAbsolutePath = DefaultTemplateUrl + templateName;

                if (System.IO.File.Exists(templatePath))
                {
                    System.IO.File.Move(templatePath, Server.MapPath(templateAbsolutePath));
                }

                TempData["fileUploaded"] = null;
            }

            return templateAbsolutePath;
        }

        #endregion

        #region Catalog Template
        //
        // GET: /CatalogTemplateManager/

        public ActionResult Catalog()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult GetCatalogTemplates(GridCommand command)
        {
            var catalogTemplateList = _catalogTemplateService.GetAllCatalogTemplates(true).OrderBy(x => x.DisplayOrder);

            var gridModel = new GridModel<CatalogTemplateModel>
            {
                Data = catalogTemplateList.Select(template =>
                {
                    var m = new CatalogTemplateModel();

                    m.Id = template.Id;
                    m.Name = template.Name;
                    m.ViewPath = template.ViewPath;
                    m.DisplayOrder = template.DisplayOrder;
                    m.Published = template.Published;

                    return m;
                }),

                Total = catalogTemplateList.Count()
            };

            return new JsonResult()
            {
                Data = gridModel
            };
        }

        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult AddCatalogTemplate(CatalogTemplateModel model, GridCommand command)
        {
            if (!ModelState.IsValid)
            {
                //display the first model error
                var modelStateErrors = this.ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage);
                return Content(modelStateErrors.FirstOrDefault());
            }

            var template = new CatalogTemplate
            {
                Name = model.Name,
                ViewPath = this.FileAfterUpload(),
                DisplayOrder = model.DisplayOrder,
                Published = model.Published
            };

            _catalogTemplateService.InsertCatalogTemplate(template);

            return GetCatalogTemplates(command);
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult UpdateCatalogTemplate(CatalogTemplateModel model, GridCommand command)
        {

            if (!ModelState.IsValid)
            {
                //display the first model error
                var modelStateErrors = this.ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage);
                return Content(modelStateErrors.FirstOrDefault());
            }

            var catalogTemplate = _catalogTemplateService.GetCatalogTemplateById(model.Id);
            if (catalogTemplate == null)
                throw new ArgumentException("No Catalog Template found with the specified id", "id");

            catalogTemplate.Name = model.Name.ToTitle(TitleStyle.FirstCaps);
            catalogTemplate.Published = model.Published;
            catalogTemplate.DisplayOrder = model.DisplayOrder;

            catalogTemplate.ViewPath = this.FileAfterUpload(model.OldViewPath);

            _catalogTemplateService.UpdateCatalogTemplate(catalogTemplate);

            return GetCatalogTemplates(command);
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult DeleteCatalogTemplate(int id, GridCommand command)
        {

            var catalogTemplate = _catalogTemplateService.GetCatalogTemplateById(id);
            if (catalogTemplate == null)
                throw new ArgumentException("No Catalog Template found with the specified id", "id");

            _catalogTemplateService.DeleteCatalogTemplate(catalogTemplate);

            return GetCatalogTemplates(command);
        }

        #endregion

        #region Generic Catalog Template
        //
        // GET: /GenericCatalogTemplatesManager/

        public ActionResult GenericCatalog()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult GetGenericCatalogTemplates(GridCommand command)
        {
            var genericCatalogTemplateList = _genericCatalogTemplateService.GetAllGenericCatalogTemplates(true);

            var gridModel = new GridModel<GenericCatalogTemplateModel>
            {
                Data = genericCatalogTemplateList.Select(template =>
                {
                    var m = new GenericCatalogTemplateModel();

                    m.Id = template.Id;
                    m.Name = template.Name;
                    m.ViewPath = template.ViewPath;
                    m.DisplayOrder = template.DisplayOrder;
                    m.Published = template.Published;

                    return m;
                }),

                Total = genericCatalogTemplateList.Count
            };

            return new JsonResult()
            {
                Data = gridModel
            };
        }

        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult AddGenericCatalogTemplate(GenericCatalogTemplateModel model, GridCommand command)
        {
            if (!ModelState.IsValid)
            {
                //display the first model error
                var modelStateErrors = this.ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage);
                return Content(modelStateErrors.FirstOrDefault());
            }

            var template = new GenericCatalogTemplate
            {
                Name = model.Name,
                ViewPath = this.FileAfterUpload(),
                DisplayOrder = model.DisplayOrder,
                Published = model.Published
            };

            _genericCatalogTemplateService.InsertGenericCatalogTemplate(template);

            return GetGenericCatalogTemplates(command);
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult UpdateGenericCatalogTemplate(GenericCatalogTemplateModel model, GridCommand command)
        {

            if (!ModelState.IsValid)
            {
                //display the first model error
                var modelStateErrors = this.ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage);
                return Content(modelStateErrors.FirstOrDefault());
            }

            var genericCatalogTemplate = _genericCatalogTemplateService.GetGenericCatalogTemplateById(model.Id);
            if (genericCatalogTemplate == null)
                throw new ArgumentException("No Generic Catalog Template found with the specified id", "id");

            genericCatalogTemplate.Name = model.Name.ToTitle(TitleStyle.FirstCaps);
            genericCatalogTemplate.Published = model.Published;
            genericCatalogTemplate.DisplayOrder = model.DisplayOrder;

            genericCatalogTemplate.ViewPath = this.FileAfterUpload(model.OldViewPath);

            _genericCatalogTemplateService.UpdateGenericCatalogTemplate(genericCatalogTemplate);

            return GetGenericCatalogTemplates(command);
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult DeleteGenericCatalogTemplate(int id, GridCommand command)
        {

            var catalogTemplate = _genericCatalogTemplateService.GetGenericCatalogTemplateById(id);
            if (catalogTemplate == null)
                throw new ArgumentException("No Generic Catalog Template found with the specified id", "id");

            _genericCatalogTemplateService.DeleteGenericCatalogTemplate(catalogTemplate);

            return GetGenericCatalogTemplates(command);
        }

        #endregion

        #region Ajax way, not recommended
        /*
        #region Catalog Template
        public ActionResult Catalog()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTemplates))
                return AccessDeniedView();

            return View();
        }

        [GridAction]
        public ActionResult _SelectCatalogBatchEditing()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTemplates))
                return AccessDeniedView();

            var catalogTemplateList = _catalogTemplateService.GetAllCatalogTemplates(true);

            var gridModel = new GridModel<CatalogTemplateModel>
            {
                Data = catalogTemplateList.Select(template =>
                {
                    var m = new CatalogTemplateModel();

                    m.Id = template.Id;
                    m.Name = template.Name;
                    m.ViewPath = template.ViewPath;
                    m.DisplayOrder = template.DisplayOrder;
                    m.Published = template.Published;

                    return m;
                })
            };

            return View(gridModel);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _SaveCatalogBatchEditing([Bind(Prefix = "inserted")]IEnumerable<CatalogTemplateModel> insertedCatalogTemplateModels,
            [Bind(Prefix = "updated")]IEnumerable<CatalogTemplateModel> updatedCatalogTemplateModels,
            [Bind(Prefix = "deleted")]IEnumerable<CatalogTemplateModel> deletedCatalogTemplateModels)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTemplates))
                return AccessDeniedView();

            if (insertedCatalogTemplateModels != null)
            {
                foreach (var model in insertedCatalogTemplateModels)
                {
                    var template = new CatalogTemplate
                    {
                        Name = model.Name,
                        ViewPath = model.ViewPath,
                        DisplayOrder = model.DisplayOrder,
                        Published = model.Published
                    };

                    _catalogTemplateService.InsertCatalogTemplate(template);
                }
            }

            if (updatedCatalogTemplateModels != null)
            {
                foreach (var model in updatedCatalogTemplateModels)
                {
                    var target = _catalogTemplateService.GetCatalogTemplateById(model.Id);
                    if (target != null)
                    {
                        target.Name = model.Name;
                        target.ViewPath = model.ViewPath;
                        target.DisplayOrder = model.DisplayOrder;
                        target.Published = model.Published;

                        _catalogTemplateService.UpdateCatalogTemplate(target);
                    }
                }
            }

            if (deletedCatalogTemplateModels != null)
            {
                foreach (var model in deletedCatalogTemplateModels)
                {
                    var template = _catalogTemplateService.GetCatalogTemplateById(model.Id);
                    _catalogTemplateService.DeleteCatalogTemplate(template);
                }
            }


            var catalogTemplateList = _catalogTemplateService.GetAllCatalogTemplates(true);

            var gridModel = new GridModel<CatalogTemplateModel>
            {
                Data = catalogTemplateList.Select(template =>
                {
                    var m = new CatalogTemplateModel();

                    m.Id = template.Id;
                    m.Name = template.Name;
                    m.ViewPath = template.ViewPath;
                    m.DisplayOrder = template.DisplayOrder;
                    m.Published = template.Published;

                    return m;
                })
            };

            return View(gridModel);
        } 
        #endregion

        #region Generic Catalog Template
        public ActionResult GenericCatalog()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTemplates))
                return AccessDeniedView();

            return View();
        }

        [GridAction]
        public ActionResult _SelectGenericCatalogBatchEditing()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTemplates))
                return AccessDeniedView();

            var genericCatalogTemplateList = _genericCatalogTemplateService.GetAllGenericCatalogTemplates(true);

            var gridModel = new GridModel<GenericCatalogTemplateModel>
            {
                Data = genericCatalogTemplateList.Select(template =>
                {
                    var m = new GenericCatalogTemplateModel();

                    m.Id = template.Id;
                    m.Name = template.Name;
                    m.ViewPath = template.ViewPath;
                    m.DisplayOrder = template.DisplayOrder;
                    m.Published = template.Published;

                    return m;
                })
            };

            return View(gridModel);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [GridAction]
        public ActionResult _SaveGenericCatalogBatchEditing([Bind(Prefix = "inserted")]IEnumerable<GenericCatalogTemplateModel> insertedGenericCatalogTemplateModels,
            [Bind(Prefix = "updated")]IEnumerable<GenericCatalogTemplateModel> updatedGenericCatalogTemplateModels,
            [Bind(Prefix = "deleted")]IEnumerable<GenericCatalogTemplateModel> deletedGenericCatalogTemplateModels)
        {
            if (insertedGenericCatalogTemplateModels != null)
            {
                foreach (var model in insertedGenericCatalogTemplateModels)
                {
                    var template = new GenericCatalogTemplate
                    {
                        Name = model.Name,
                        ViewPath = model.ViewPath,
                        DisplayOrder = model.DisplayOrder,
                        Published = model.Published
                    };

                    _genericCatalogTemplateService.InsertGenericCatalogTemplate(template);
                }
            }

            if (updatedGenericCatalogTemplateModels != null)
            {
                foreach (var model in updatedGenericCatalogTemplateModels)
                {
                    var target = _genericCatalogTemplateService.GetGenericCatalogTemplateById(model.Id);
                    if (target != null)
                    {
                        target.Name = model.Name;
                        target.ViewPath = model.ViewPath;
                        target.DisplayOrder = model.DisplayOrder;
                        target.Published = model.Published;

                        _genericCatalogTemplateService.UpdateGenericCatalogTemplate(target);
                    }
                }
            }

            if (deletedGenericCatalogTemplateModels != null)
            {
                foreach (var model in deletedGenericCatalogTemplateModels)
                {
                    var template = _genericCatalogTemplateService.GetGenericCatalogTemplateById(model.Id);
                    _genericCatalogTemplateService.DeleteGenericCatalogTemplate(template);
                }
            }


            var genericCatalogTemplateList = _genericCatalogTemplateService.GetAllGenericCatalogTemplates(true);

            var gridModel = new GridModel<GenericCatalogTemplateModel>
            {
                Data = genericCatalogTemplateList.Select(template =>
                {
                    var m = new GenericCatalogTemplateModel();

                    m.Id = template.Id;
                    m.Name = template.Name;
                    m.ViewPath = template.ViewPath;
                    m.DisplayOrder = template.DisplayOrder;
                    m.Published = template.Published;

                    return m;
                })
            };

            return View(gridModel);
        } 
        #endregion
         */
        
        #endregion
    }
}
