using System;
using System.Web;
using System.Linq;
using System.Collections.Generic;
using System.Web.Mvc;
using System.IO;
using Aaron.Core;
using Aaron.Core.Services.Catalogs;
using Aaron.Core.Domain.Catalogs;
using Aaron.Core.Web.Security;
using Aaron.Core.Services.Security;
using Aaron.Core.Utility;
using Aaron.Core.Web.Controllers;
using Aaron.Admin.Models.Catalogs;
using Aaron.Admin.Models.Common;

namespace Aaron.Admin.Controllers
{
    [AdminAuthorize]
    public class CatalogController : BaseController
    {
        private readonly ICatalogService _catalogService;
        private readonly IGenericCatalogService _genericCatalogService;
        private readonly ICatalogTemplateService _catalogTemplateService;
        private readonly IPermissionService _permissionService;
        private readonly CatalogSettings _catalogSettings;

        public string DefaultIconUrl
        {
            get
            {
                var defaultIcon = _catalogSettings.DefaultGenericCatalogIcon;
                return defaultIcon != null && defaultIcon != "" ? defaultIcon : "~/content/default.png";
            }
        }

        public CatalogController(ICatalogService catalogService, 
            IGenericCatalogService genericCatalogService,
            ICatalogTemplateService catalogTemplateService,
            IPermissionService permissionService,
            CatalogSettings catalogSettings)
        {
            _catalogService = catalogService;
            _genericCatalogService = genericCatalogService;
            _catalogTemplateService = catalogTemplateService;
            _permissionService = permissionService;
            _catalogSettings = catalogSettings;
        }

        #region Upload file
        private string FileAfterUpload(string oldFile = null)
        {
            var iconAbsolutePath = oldFile ?? DefaultIconUrl;

            if (TempData["fileUploaded"] != null)
            {
                var iconPath = (string)TempData["fileUploaded"];
                var iconName = iconPath.Substring(iconPath.LastIndexOf(@"\") + 1);
                iconName = Guid.NewGuid() + "-" + CommonHelper.RemoveMarks(iconName);
                iconAbsolutePath = (_catalogSettings.GenericCatalogIconPath ?? "~/Content/") + iconName;

                if (System.IO.File.Exists(iconPath))
                {
                    System.IO.File.Move(iconPath, Server.MapPath(iconAbsolutePath));
                }
                TempData["fileUploaded"] = null;
            }

            return iconAbsolutePath;
        }

        // Save File Upload/
        public ActionResult SaveFileUpload(IEnumerable<HttpPostedFileBase> iconAttachments)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageGenericCatalogs))
                return AccessDeniedView();

            foreach (var file in iconAttachments)
            {
                var fileName = Path.GetFileName(file.FileName);
                if (fileName != null)
                {
                    var physicalPath = Path.Combine(Server.MapPath(_catalogSettings.GenericCatalogIconPath ?? "~/Content"), fileName);
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
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageGenericCatalogs))
                return AccessDeniedView();

            foreach (var file in fileNames)
            {
                var fileName = Path.GetFileName(file);
                if (fileName != null)
                {
                    var physicalPath = Path.Combine(Server.MapPath(_catalogSettings.GenericCatalogIconPath ?? "~/Content"), fileName);

                    if (System.IO.File.Exists(physicalPath))
                        System.IO.File.Delete(physicalPath);
                }
            }
            TempData["fileUploaded"] = null;
            return Content("");
        }

        #endregion

        //
        // GET: /CatalogManager/
        public ActionResult Index(int genericCatalogId = 0)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalogs))
                return AccessDeniedView();

            GenericCatalogDDL();

            var catalogs = _catalogService.GetCatalogsByGenericCatalogId(genericCatalogId, true, true);
            var catalogModel = catalogs.Select((entity) => 
            {
                var model = new CatalogModel(entity);
                return model;
            });
            return View(catalogModel);
        }

        private void TemplateDDL()
        {
            var templateDDL = new List<DropDownListModel<int, string>>();

            templateDDL.Add(new DropDownListModel<int, string>()
            {
                Name = "- Chọn template -",
                Value = 0
            });

            templateDDL.AddRange(_catalogTemplateService
                .GetAllCatalogTemplates()
                .Select(entity => new DropDownListModel<int, string>()
            {
                Value = entity.Id,
                Name = entity.Name
            }));
            ViewData["TemplateDDL"] = templateDDL;
        }

        private void GenericCatalogDDL()
        {
            var genericCatalogDDL = new List<DropDownListModel<int, string>>();
            genericCatalogDDL.Add(new DropDownListModel<int, string>()
            {
                Name = "- Chọn danh mục động -",
                Value = 0
            });
            genericCatalogDDL.AddRange(_genericCatalogService.GetAllGenericCatalogs().Select(entity => new DropDownListModel<int, string>()
            {
                Value = entity.Id,
                Name = entity.Name
            }));
            ViewData["GenericCatalogDDL"] = genericCatalogDDL;
        }

        private IList<Catalog> PreparedParentCatalogs(IList<Catalog> source, int duplicateCatalogId = 0)
        {
            var result = source.AsQueryable() as IQueryable<Catalog>;
            if (duplicateCatalogId != 0)
            {
                result = result.Where(c => c.Id != duplicateCatalogId && 
                    c.GenericCatalog.Catalogs
                    .Any(cx => cx.Id == duplicateCatalogId));
            }

            result = result.Where(c => !c.ParentCatalogId.HasValue || 
                c.ParentCatalogId.HasValue && 
                c.ParentCatalogId != duplicateCatalogId);

            return result.ToList();
        }

        private void ParentCatalogDDL(int duplicateCatalogId = 0)
        {
            var parentCatalogDDL = new List<DropDownListModel<int, string>>();
            parentCatalogDDL.Add(new DropDownListModel<int, string>() 
            { 
                Name = "- Chọn danh mục cha -",
                Value = 0
            });
            parentCatalogDDL.AddRange(PreparedParentCatalogs(_catalogService.GetAllCatalog(), duplicateCatalogId)
                .Select(entity => new DropDownListModel<int, string>() 
                {  
                    Value = entity.Id,
                    Name = entity.Name
                }));
            ViewData["ParentCatalogDDL"] = parentCatalogDDL;
        }

        public JsonResult _GetDropDownListParentCatalogs(int? GenericCatalogId, int duplicateCatalogId)
        {
            var parentCatalogDDL = new List<DropDownListModel<int, string>>();
            parentCatalogDDL.Add(new DropDownListModel<int, string>()
            {
                Name = "- Chọn danh mục cha -",
                Value = 0
            });
            parentCatalogDDL.AddRange(PreparedParentCatalogs(_catalogService.GetCatalogsByGenericCatalogId(GenericCatalogId.Value), duplicateCatalogId)
                .Select(entity => new DropDownListModel<int, string>()
                {
                    Value = entity.Id,
                    Name = entity.Name
                }));

            return Json(new SelectList(parentCatalogDDL, "Value", "Name"), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalogs))
                return AccessDeniedView();

            TemplateDDL();
            GenericCatalogDDL();
            ParentCatalogDDL();

            return View(new CatalogModel {Published = true});
        }

        [HttpPost]
        public ActionResult Create(CatalogModel model, FormCollection formCollection)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalogs))
                return AccessDeniedView();

            if(ModelState.IsValid)
            {
                var catalog = new Catalog
                {
                    Avatar = this.FileAfterUpload(),
                    Name = model.Name,
                    MetaTitle = model.MetaTitle,
                    MetaKeywords = model.MetaKeywords,
                    MetaDescription = model.MetaDescription,
                    Published = model.Published,
                    DisplayOrder = model.DisplayOrder,
                    SEOUrlName = (!string.IsNullOrEmpty(model.SEOUrlName)) ? model.SEOUrlName : model.Name.ToSEName(),
                };
                if (model.TemplateId != 0)
                    catalog.TemplateId = model.TemplateId;

                if (model.GenericCatalogId != 0)
                    catalog.GenericCatalogId = model.GenericCatalogId;

                if (model.ParentCatalogId != 0)
                    catalog.ParentCatalogId = model.ParentCatalogId;

            _catalogService.InsertCatalog(catalog);
            return RedirectToAction("Edit", new {id = catalog.Id});
            }
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalogs))
                return AccessDeniedView();

            TemplateDDL();
            GenericCatalogDDL();
            ParentCatalogDDL(id);

            var catalog = _catalogService.GetCatalogById(id);
            _catalogService.CreateRecordForCatalog(catalog);
            var model = new CatalogModel(catalog);
            model.CatalogRecordModel
                .AddRange(catalog.CatalogAttributeRecord
                    .Select(x => new CatalogRecordModel
                    {
                        InputName = x.Attribute.SystemName,
                        Name = x.Attribute.Name,
                        Value = x.Value,
                        Type = x.Attribute.ControlType
                    }));
            return View(model);
        }

        [HttpPost, ValidateInput(false), ParameterBasedOnFormNameAttribute("save-and-back-list", "saveAndBackList")]
        [FormValueRequired("save", "save-and-back-list")]
        public ActionResult Edit(CatalogModel model, int id, FormCollection formCollection, bool saveAndBackList)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalogs))
                return AccessDeniedView();

            if(ModelState.IsValid)
            {
                if (TempData["fileUploaded"] != null && model.OldFilePath != null && !string.Empty.Equals(model.OldFilePath))
                {
                    var oldIconPath = model.OldFilePath;
                    var dirOldIconPath = Server.MapPath(oldIconPath);
                    if (System.IO.File.Exists(dirOldIconPath) && oldIconPath != DefaultIconUrl)
                    {
                        System.IO.File.Delete(dirOldIconPath);
                    }
                }

            var catalog = _catalogService.GetCatalogById(model.Id);
                catalog.Name = model.Name;
                catalog.Avatar = this.FileAfterUpload(catalog.Avatar);
                catalog.MetaTitle = model.MetaTitle;
                catalog.MetaKeywords = model.MetaKeywords;
                catalog.MetaDescription = model.MetaDescription;
                catalog.Published = model.Published;
                catalog.DisplayOrder = model.DisplayOrder;
                catalog.SEOUrlName = (!string.IsNullOrEmpty(model.SEOUrlName)) ? model.SEOUrlName : model.Name.ToSEName();
                catalog.TemplateId = model.TemplateId;

                if (model.TemplateId != 0)
                    catalog.TemplateId = model.TemplateId;
                else catalog.TemplateId = null;

                if (model.ParentCatalogId != 0)
                    catalog.ParentCatalogId = model.ParentCatalogId;
                else catalog.ParentCatalogId = null;

                if (model.GenericCatalogId != 0)
                    catalog.GenericCatalogId = model.GenericCatalogId;
                else catalog.GenericCatalogId = null;

                foreach (var record in catalog.CatalogAttributeRecord)
                {
                    if (String.IsNullOrWhiteSpace(formCollection[record.Attribute.SystemName]))
                        ModelState.AddModelError("","Kiểm tra có để trống không");
                    record.Value = formCollection[record.Attribute.SystemName];
                }
                _catalogService.UpdateCatalog(catalog);

                SuccessNotification("Danh mục đã chỉnh sửa thành công!");
                return saveAndBackList ? RedirectToAction("Index", new { genericCatalogId = catalog.GenericCatalogId }) : RedirectToAction("Index");
            }
            ViewBag.TemplateId = new SelectList(_catalogTemplateService.GetAllCatalogTemplates(), "Id", "Name", model.TemplateId);

            return View(model);
        }

        public ActionResult Remove(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalogs))
                return AccessDeniedView();

            var model = new CatalogModel(_catalogService.GetCatalogById(id));
            return View(model);
        }

        [HttpPost, ActionName("Remove")]
        public ActionResult RemoveConfirmed(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCatalogs))
                return AccessDeniedView();

            _catalogService.DeleteCatalogAttributeRecordByCatalogId(id);
            _catalogService.DeleteCatalog(id);

            return RedirectToAction("Index");
        }
    }
}
