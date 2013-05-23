using System.Web.Mvc;
using Aaron.Core;
using Aaron.Core.Caching;
using Aaron.Core.Services.Localization;
using Aaron.Core.Services.Topics;
using $rootnamespace$.Models.Topics;

namespace $rootnamespace$.Controllers
{
    public partial class TopicController : BaseController
    {
        public const string TOPIC_MODEL_KEY = "Aaron.client.topic.details-{0}-{1}";

        #region Fields

        private readonly ITopicService _topicService;
        private readonly ICurrentActivity _currentActivity;
        private readonly ILocalizationService _localizationService;
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Constructors

        public TopicController(ITopicService topicService,
            ILocalizationService localizationService,
            ICurrentActivity currentActivity, 
            ICacheManager cacheManager)
        {
            this._topicService = topicService;
            this._currentActivity = currentActivity;
            this._localizationService = localizationService;
            this._cacheManager = cacheManager;
        }

        #endregion

        #region Utilities

        [NonAction]
        protected TopicModel PrepareTopicModel(string systemName)
        {
            var topic = _topicService.GetTopicBySystemName(systemName);
            if (topic == null)
                return null;

            var model = new TopicModel()
            {
                Id = topic.Id,
                SystemName = topic.SystemName,
                IncludeInSitemap = topic.IncludeInSitemap,
                IsPasswordProtected = topic.IsPasswordProtected,
                Title = topic.IsPasswordProtected ? "" : topic.GetLocalized(x => x.Title),
                Body = topic.IsPasswordProtected ? "" : topic.GetLocalized(x => x.Body),
                MetaKeywords = topic.GetLocalized(x => x.MetaKeywords),
                MetaDescription = topic.GetLocalized(x => x.MetaDescription),
                MetaTitle = topic.GetLocalized(x => x.MetaTitle),
            };
            return model;
        }

        #endregion

        #region Methods

        public ActionResult TopicDetails(string systemName)
        {
            var cacheKey = string.Format(TOPIC_MODEL_KEY, systemName, _currentActivity.CurrentLanguage.Id);
            var cacheModel = _cacheManager.Get(cacheKey, () => PrepareTopicModel(systemName));

            if (cacheModel == null)
                return RedirectToRoute("HomePage");
            return View("TopicDetails", cacheModel);
        }

        public ActionResult TopicDetailsPopup(string systemName)
        {
            var cacheKey = string.Format(TOPIC_MODEL_KEY, systemName, _currentActivity.CurrentLanguage.Id);
            var cacheModel = _cacheManager.Get(cacheKey, () => PrepareTopicModel(systemName));

            if (cacheModel == null)
                return RedirectToRoute("HomePage");

            ViewBag.IsPopup = true;
            return View("TopicDetails", cacheModel);
        }

        [ChildActionOnly]
        //[OutputCache(Duration = 120, VaryByCustom = "CurrentLanguage")]
        public ActionResult TopicBlock(string systemName)
        {
            var cacheKey = string.Format(TOPIC_MODEL_KEY, systemName, _currentActivity.CurrentLanguage.Id);
            var cacheModel = _cacheManager.Get(cacheKey, () => PrepareTopicModel(systemName));

            if (cacheModel == null)
                return Content("");

            return PartialView(cacheModel);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Authenticate(int id, string password)
        {
            var authResult = false;
            var title = string.Empty;
            var body = string.Empty;
            var error = string.Empty;

            var topic = _topicService.GetTopicById(id);

            if (topic != null)
            {
                if (topic.Password != null && topic.Password.Equals(password))
                {
                    authResult = true;
                    title = topic.GetLocalized(x => x.Title);
                    body = topic.GetLocalized(x => x.Body);
                }
                else
                {
                    error = "Sai mật khẩu!";
                }
            }
            return Json(new { Authenticated = authResult, Title = title, Body = body, Error = error });
        }

        #endregion
    }
}
