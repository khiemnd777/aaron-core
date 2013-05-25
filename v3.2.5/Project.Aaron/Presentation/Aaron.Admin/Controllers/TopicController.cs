using System.Linq;
using System.Web.Mvc;
using Aaron.Admin.Models.Topics;
using Aaron.Core;
using Aaron.Core.Domain.Topics;
using Aaron.Core.Services.Localization;
using Aaron.Core.Services.Security;
using Aaron.Core.Services.Topics;
using Aaron.Core.Web.Controllers;
using Aaron.Core.Web.Security;
using Telerik.Web.Mvc;

namespace Aaron.Admin.Controllers
{
    [AdminAuthorize]
    public partial class TopicController : BaseController
    {
        #region Fields

        private readonly ITopicService _topicService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;

        #endregion Fields

        #region Constructors

        public TopicController(ITopicService topicService, ILanguageService languageService,
            ILocalizedEntityService localizedEntityService, ILocalizationService localizationService,
            IPermissionService permissionService)
        {
            this._topicService = topicService;
            this._languageService = languageService;
            this._localizedEntityService = localizedEntityService;
            this._localizationService = localizationService;
            this._permissionService = permissionService;
        }

        #endregion
        
        #region Utilities

        [NonAction]
        protected void UpdateLocales(Topic topic, TopicModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(topic,
                                                               x => x.Title,
                                                               localized.Title,
                                                               localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(topic,
                                                           x => x.Body,
                                                           localized.Body,
                                                           localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(topic,
                                                           x => x.MetaKeywords,
                                                           localized.MetaKeywords,
                                                           localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(topic,
                                                           x => x.MetaDescription,
                                                           localized.MetaDescription,
                                                           localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(topic,
                                                           x => x.MetaTitle,
                                                           localized.MetaTitle,
                                                           localized.LanguageId);
            }
        }
        
        #endregion
        
        #region List

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTopics))
                return AccessDeniedView();

            var topics = _topicService.GetAllTopics();
            var gridModel = new GridModel<TopicModel>
            {
                Data = topics.Select(x => 
                {
                    return new TopicModel() 
                    { 
                        Body = x.Body,
                        Id = x.Id,
                        IncludeInSitemap = x.IncludeInSitemap,
                        IsPasswordProtected = x.IsPasswordProtected,
                        MetaDescription = x.MetaDescription,
                        MetaKeywords = x.MetaKeywords,
                        MetaTitle = x.MetaTitle,
                        Password = x.Password,
                        SystemName = x.SystemName,
                        Title = x.Title
                    };
                }),
                Total = topics.Count
            };
            return View(gridModel);
        }

        [HttpPost, GridAction(EnableCustomBinding = true)]
        public ActionResult List(GridCommand command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTopics))
                return AccessDeniedView();

            var topics = _topicService.GetAllTopics();
            var gridModel = new GridModel<TopicModel>
            {
                Data = topics.Select(x => 
                {
                    return new TopicModel()
                    {
                        Body = x.Body,
                        Id = x.Id,
                        IncludeInSitemap = x.IncludeInSitemap,
                        IsPasswordProtected = x.IsPasswordProtected,
                        MetaDescription = x.MetaDescription,
                        MetaKeywords = x.MetaKeywords,
                        MetaTitle = x.MetaTitle,
                        Password = x.Password,
                        SystemName = x.SystemName,
                        Title = x.Title
                    };
                }),
                Total = topics.Count
            };
            return new JsonResult
            {
                Data = gridModel
            };
        }

        #endregion

        #region Create / Edit / Delete

        public ActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTopics))
                return AccessDeniedView();

            var model = new TopicModel();
            //locales
            AddLocales(_languageService, model.Locales);
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public ActionResult Create(TopicModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTopics))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                if (!model.IsPasswordProtected)
                {
                    model.Password = null;
                }

                var topic = new Topic()
                {
                    Body = model.Body,
                    IncludeInSitemap = model.IncludeInSitemap,
                    IsPasswordProtected = model.IsPasswordProtected,
                    MetaDescription = model.MetaDescription,
                    MetaKeywords = model.MetaKeywords,
                    MetaTitle = model.MetaTitle,
                    Password = model.Password,
                    SystemName = model.SystemName,
                    Title = model.Title
                };
                _topicService.InsertTopic(topic);
                //locales
                UpdateLocales(topic, model);

                SuccessNotification("Chủ đề đã được tạo.");
                return continueEditing ? RedirectToAction("Edit", new { id = topic.Id }) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTopics))
                return AccessDeniedView();

            var topic = _topicService.GetTopicById(id);
            if (topic == null)
                //No topic found with the specified id
                return RedirectToAction("List");

            var model = new TopicModel() 
            { 
                Body = topic.Body,
                Id = topic.Id,
                IncludeInSitemap = topic.IncludeInSitemap,
                IsPasswordProtected = topic.IsPasswordProtected,
                MetaDescription = topic.MetaDescription,
                MetaKeywords = topic.MetaKeywords,
                MetaTitle = topic.MetaTitle,
                Password = topic.Password,
                SystemName = topic.SystemName,
                Title = topic.Title
            };
            model.Url = Url.RouteUrl("Topic", new { SystemName = topic.SystemName }, "http");
            //locales
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.Title = topic.GetLocalized(x => x.Title, languageId, false, false);
                locale.Body = topic.GetLocalized(x => x.Body, languageId, false, false);
                locale.MetaKeywords = topic.GetLocalized(x => x.MetaKeywords, languageId, false, false);
                locale.MetaDescription = topic.GetLocalized(x => x.MetaDescription, languageId, false, false);
                locale.MetaTitle = topic.GetLocalized(x => x.MetaTitle, languageId, false, false);
            });

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormNameAttribute("save-continue", "continueEditing")]
        public ActionResult Edit(TopicModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTopics))
                return AccessDeniedView();

            var topic = _topicService.GetTopicById(model.Id);
            if (topic == null)
                //No topic found with the specified id
                return RedirectToAction("List");

            model.Url = Url.RouteUrl("Topic", new { SystemName = topic.SystemName }, "http");

            if (!model.IsPasswordProtected)
            {
                model.Password = null;
            }

            if (ModelState.IsValid)
            {
                topic.Body = model.Body;
                topic.IncludeInSitemap = model.IncludeInSitemap;
                topic.IsPasswordProtected = model.IsPasswordProtected;
                topic.MetaDescription = model.MetaDescription;
                topic.MetaKeywords = model.MetaKeywords;
                topic.MetaTitle = model.MetaTitle;
                topic.Password = model.Password;
                topic.SystemName = model.SystemName;
                topic.Title = model.Title;
                _topicService.UpdateTopic(topic);
                //locales
                UpdateLocales(topic, model);
                
                SuccessNotification(_localizationService.GetResource("Admin.ContentManagement.Topics.Updated"));
                return continueEditing ? RedirectToAction("Edit", topic.Id) : RedirectToAction("List");
            }


            //If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageTopics))
                return AccessDeniedView();

            var topic = _topicService.GetTopicById(id);
            if (topic == null)
                //No topic found with the specified id
                return RedirectToAction("List");

            _topicService.DeleteTopic(topic);

            SuccessNotification("Chủ đề đã được xóa.");
            return RedirectToAction("List");
        }
        
        #endregion
    }
}
