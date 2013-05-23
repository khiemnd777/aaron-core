using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Web;
using System.Web.Mvc;
using Telerik.Web.Mvc;
using Aaron.Core;
using Aaron.Core.Domain.Catalogs;
using Aaron.Admin.Models.Catalogs;
using Aaron.Core.Services.Catalogs;
using Aaron.Core.Utility;
using Aaron.Core.Domain.Common;
using Aaron.Core.Services.Security;
using Aaron.Core.Web.Security;

namespace Aaron.Admin.Controllers
{
    [AdminAuthorize]
    public class GenericCatalogController : BaseController
    {
        private AttributeControlType _controlType { get; set; }

        private readonly IGenericCatalogService _genericCatalogService;
        private readonly IPermissionService _permissionService;
        private readonly CatalogSettings _catalogSettings;

        public string DefaultIconUrl
        {
            get 
            {
                var defaultIcon = _catalogSettings.DefaultGenericCatalogIcon;
                return defaultIcon != null && defaultIcon != "" ? defaultIcon : "~/content/biblcross.jpg"; 
            }
        }
        
        public GenericCatalogController(IGenericCatalogService genericCatalogService,
            CatalogSettings catalogSettings,
            IPermissionService permissionService)
        {
            _genericCatalogService = genericCatalogService;
            _catalogSettings = catalogSettings;
            _permissionService = permissionService;
        }


        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        #region Functions

        //File after upload
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

        // Get Control Type List
        private void GetControlTypeList()
        {
            var entity = new GenericCatalogAttribute();

            var controlTypeList = entity.ControlType.ToEnumList();
            ViewData["ControlTypes"] = controlTypeList;

            //var controlTypeModelList = new List<ControlTypeModel>();
            //controlTypeModelList.AddRange(entity.ControlType.ToEnumList().Select(controlType => new ControlTypeModel()
            //{
            //    ControlTypeId = controlType.Key,
            //    ControlTypeName = controlType.Name
            //}));
    
        }

        //Get TemplateList For Generic Catalog
        private void GetTemplateList()
        {
            //Get Templete List
            var templateListModel = new List<TemplateModel>();
            if (_genericCatalogService.GenericCatalogTemplateExisted())
            {
                templateListModel.AddRange(_genericCatalogService.GetGenericCatalogTemplates().Select(genericCatalogTemplate => new TemplateModel()
                {
                    TemplateId = genericCatalogTemplate.Id,
                    TemplateName = genericCatalogTemplate.Name
                }));
            }

            ViewData["TemplateList"] = templateListModel;
        }

        //Get Block View List
        private void GetBlockViewList()
        {
            var entity = new GenericCatalog();

            var GetBlockViewList = entity.BlockView.ToEnumList();
            ViewData["BlockViews"] = GetBlockViewList;
        }

        
        #endregion

        #region Upload file

        // Save File Upload/
        public ActionResult SaveFileUpload(IEnumerable<HttpPostedFileBase> iconAttachments)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageGenericCatalogs))
                return AccessDeniedView();

            foreach(var file in iconAttachments)
            {
                var fileName =  Path.GetFileName(file.FileName);
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

            foreach(var file in fileNames)
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


        #region Generic Catalog

        [ChildActionOnly]
        public ActionResult EditableCatalogs()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageGenericCatalogs))
                return AccessDeniedView();
            var genericCatlog = _genericCatalogService.GetAllGenericCatalogs()
                .Select(gc => new GenericCatalogEditableModel
                {
                    Id = gc.Id,
                    Name = gc.Name,
                    Catalogs = gc.Catalogs
                        .Select(c => new CatalogEditableModel 
                        { 
                            Id = c.Id,
                            Name = c.Name
                        })
                        .ToList()
                });
            return PartialView(genericCatlog);
        }

        public ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageGenericCatalogs))
                return AccessDeniedView();

            var genericCatlog = _genericCatalogService.GetAllGenericCatalogs(0, 10, true);

            var gridModel = new GridModel<GenericCatalogGridModel>
            {
                Data = genericCatlog.Select(x =>
                {
                    var m = new GenericCatalogGridModel
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Icon = x.Icon,
                        BlockViewId = x.BlockView.ToString(),
                        Published = x.Published,
                        DisplayOrder = x.DisplayOrder
                    };

                    return m;
                }),
                Total = genericCatlog.TotalCount
            };

            return View(gridModel);
        }

        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult List(GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageGenericCatalogs))
                return AccessDeniedView();

            var genericCatlog = _genericCatalogService.GetAllGenericCatalogs(command.Page -1, command.PageSize, true);

            var gridModel = new GridModel<GenericCatalogGridModel>
            {
                Data = genericCatlog.Select(x =>
                {
                    var m = new GenericCatalogGridModel();
                    m.Id = x.Id;
                    m.Name = x.Name;
                    m.Icon = x.Icon;
                    m.Published = x.Published;
                    m.DisplayOrder = x.DisplayOrder;
                    m.BlockViewId = x.BlockView.ToString();
                    return m;
                }),
                Total = genericCatlog.TotalCount
            };

            return new JsonResult
            {
                Data = gridModel
            };
        }

        public ActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageGenericCatalogs))
                return AccessDeniedView();

            var model = new GenericCatalogModel();

            //Get Templete List
            GetTemplateList();

            // Get BlockView List
            GetBlockViewList();

            //default values
            model.Published = true;
            model.ItemOnPage = _catalogSettings.DefaultGenericCatalogItemsOnPage;
            model.SizeOnMenu = _catalogSettings.DefaultSizeOnMenu;
            //model.TemplateModels = templateListModel;

            ViewData["autoUpload"] = _catalogSettings.AllowAutoUpload;
            ViewData["allowMultiFile"] = _catalogSettings.AllowUploadMultiFile;
            return View(model);

        }

        [HttpPost]
        public ActionResult Create(GenericCatalogModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageGenericCatalogs))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var genericCatalog = new GenericCatalog();
                
                genericCatalog.Icon = this.FileAfterUpload();
                genericCatalog.Name = model.Name.ToTitle(TitleStyle.FirstCaps);
                genericCatalog.SystemName = model.Name.ToSystemName();
                genericCatalog.ItemOnPage = model.ItemOnPage;
                genericCatalog.SizeOnMenu = model.SizeOnMenu;
                genericCatalog.Published = model.Published;
                genericCatalog.DisplayOrder = model.DisplayOrder;
                genericCatalog.BlockViewId = Convert.ToInt32(model.BlockViewId.GetEnumValue<BlockViewType>());
                genericCatalog.TemplateId = model.TemplateId;
                genericCatalog.SEOUrlName = model.Name.ToSEName();

                _genericCatalogService.InsertGenericCatalog(genericCatalog);

                return RedirectToAction("List");
            }

            // Get Template List
            GetTemplateList();

            // Get blockView List
            GetBlockViewList();

            return View(model);
        }

        public ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageGenericCatalogs))
                return AccessDeniedView();

            var genericCatalog = _genericCatalogService.GetGenericCatalogById(id);
            if (genericCatalog == null)
                //No poll found with the specified id
                return RedirectToAction("List");
            
            // Get Template List
            GetTemplateList();
           
            // Get blockView List
            GetBlockViewList();

            //Get Control Type List For Attribute
            GetControlTypeList();

            var model = new GenericCatalogModel();

            model.Id = genericCatalog.Id;
            model.Icon = genericCatalog.Icon;
            model.Name = genericCatalog.Name;
            model.ItemOnPage = genericCatalog.ItemOnPage;
            model.SizeOnMenu = genericCatalog.SizeOnMenu;
            model.BlockViewId = genericCatalog.BlockView.ToString();
            model.Published = genericCatalog.Published;
            model.DisplayOrder = genericCatalog.DisplayOrder;
            model.TemplateId = (genericCatalog.TemplateId.HasValue) ? genericCatalog.TemplateId.Value : 0;
            //model.TemplateModels = templateListModel;

            //this old way: use TempData["OldFilePath"]
            //TempData["OlfFilePath"] = genericCatalog.Icon; 
            //this new way: use model prop named OldFilePath
            model.OldFilePath = genericCatalog.Icon;

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(GenericCatalogModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageGenericCatalogs))
                return AccessDeniedView();

            var genericCatalog = _genericCatalogService.GetGenericCatalogById(model.Id);
            if (genericCatalog == null)
                //No poll found with the specified id
                return RedirectToAction("List");

            if (ModelState.IsValid)
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

                genericCatalog.Icon = this.FileAfterUpload(genericCatalog.Icon);
                genericCatalog.Name = model.Name.ToTitle(TitleStyle.FirstCaps);
                genericCatalog.SystemName = model.Name.ToSystemName();
                genericCatalog.ItemOnPage = model.ItemOnPage;
                genericCatalog.SizeOnMenu = model.SizeOnMenu;
                genericCatalog.Published = model.Published;
                genericCatalog.DisplayOrder = model.DisplayOrder;
                genericCatalog.BlockViewId = Convert.ToInt32(model.BlockViewId.GetEnumValue<BlockViewType>());
                genericCatalog.TemplateId = model.TemplateId;
                genericCatalog.SEOUrlName = model.Name.ToSEName();

                _genericCatalogService.UpdateGenericCatalog(genericCatalog);

                return RedirectToAction("List");
            }

            // Get Template List
            GetTemplateList();

            // Get blockView List
            GetBlockViewList();

            //Get Control Type List For Attribute
            GetControlTypeList();

            return View(model);
        }

        public ActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageGenericCatalogs))
                return AccessDeniedView();

            var genericCatalog = _genericCatalogService.GetGenericCatalogById(id);
            if (genericCatalog == null)
                //No poll found with the specified id
                return RedirectToAction("List");

            _genericCatalogService.DeleteGenericCatalog(genericCatalog);

            return RedirectToAction("List");
        }

        #endregion

        #region Generic Catalog Attribute

        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult GenericCatalogAttributes(int genericCatalogId, GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageGenericCatalogs))
                return AccessDeniedView();

            var genericCatalog = _genericCatalogService.GetGenericCatalogById(genericCatalogId);
            if (genericCatalog == null)
                throw new ArgumentException("No generic catalog found with the specified id", "genericCatalogId");

            var attributes = genericCatalog.Attributes.ToList();
            
            var model = new GridModel<GenericCatalogAttributeModel>
            {
                Data = attributes.Select(x =>
                {
                    return new GenericCatalogAttributeModel()
                    {
                        Id = x.Id,
                        GenericCatalogId = x.GenericCatalogId,
                        Name = x.Name,
                        SystemName = x.SystemName,
                        Published = x.Published,
                        DisplayOrder = x.DisplayOrder,
                        ControlTypeId = x.ControlType.ToString()
                    };
                })
                .OrderBy(o => o.DisplayOrder),
                Total = attributes.Count
            };
            return new JsonResult
            {
                Data = model
            };
        }

        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult GenericCatalogAttributeAdd(int genericCatalogId, GenericCatalogAttributeModel model, GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageGenericCatalogs))
                return AccessDeniedView();
          
            if (!ModelState.IsValid)
            {
                //display the first model error
                var modelStateErrors = this.ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage);
                return Content(modelStateErrors.FirstOrDefault());
            }

            var genericCatalog = _genericCatalogService.GetGenericCatalogById(genericCatalogId);
            if (genericCatalog == null)
                throw new ArgumentException("No poll found with the specified id", "pollId");

            genericCatalog.Attributes.Add(new GenericCatalogAttribute
            {
                Name = model.Name.ToTitle(TitleStyle.FirstCaps),
                SystemName = model.Name.ToSystemName(),
                CreationDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
                Published = model.Published,
                DisplayOrder = (model.DisplayOrder < 0) ? 0 : model.DisplayOrder,
                ControlTypeId = Convert.ToInt32(model.ControlTypeId.GetEnumValue<AttributeControlType>())

            });
            _genericCatalogService.UpdateGenericCatalog(genericCatalog);

            return GenericCatalogAttributes(genericCatalog.Id, command);
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult GenericCatalogAttributeUpdate(GenericCatalogAttributeModel model, GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageGenericCatalogs))
                return AccessDeniedView();

            if (!ModelState.IsValid)
            {
                //display the first model error
                var modelStateErrors = this.ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage);
                return Content(modelStateErrors.FirstOrDefault());
            }

            var genericCatalogAttribute = _genericCatalogService.GetGenericCatalogAttributeById(model.Id);
            if (genericCatalogAttribute == null)
                throw new ArgumentException("No Generic Catalog Attribute found with the specified id", "id");

            genericCatalogAttribute.Name = model.Name.ToTitle(TitleStyle.FirstCaps);
            genericCatalogAttribute.SystemName = model.Name.ToSystemName();
            genericCatalogAttribute.Published = model.Published;
            genericCatalogAttribute.DisplayOrder = (model.DisplayOrder < 0) ? 0 : model.DisplayOrder;
            genericCatalogAttribute.ControlTypeId = (int)model.ControlTypeId.GetEnumValue<AttributeControlType>();

            _genericCatalogService.UpdateGenericCatalog(genericCatalogAttribute.GenericCatalog);
            
            return GenericCatalogAttributes(genericCatalogAttribute.GenericCatalogId, command);
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult GenericCatalogAttributeDelete(int id, GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageGenericCatalogs))
                return AccessDeniedView();

            var genericCatalogAttribute = _genericCatalogService.GetGenericCatalogAttributeById(id);
            if (genericCatalogAttribute == null)
                throw new ArgumentException("No  Generic Catalog Attribute found with the specified id", "id");

            int genericCatalogId = genericCatalogAttribute.GenericCatalogId;
            _genericCatalogService.DeleteGenericCatalogAttribute(genericCatalogAttribute);


            return GenericCatalogAttributes(genericCatalogId, command);
        }

        #endregion

    }
}
