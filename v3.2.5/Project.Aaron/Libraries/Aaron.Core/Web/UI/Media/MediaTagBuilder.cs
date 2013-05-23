using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aaron.Core.Web.Mvc;
using Aaron.Core.Web;
using Aaron.Core.Domain.Common;

namespace Aaron.Core.Web.UI.Media
{
    internal class MediaModel
    {
        public string MediaId { get; set; }
        public string Artist { get; set; }
        public string MediaName { get; set; }
        public string Description { get; set; }
        public bool Existed { get; set; }
    }

    public class MediaTagBuilder : IMediaTagBuilder
    {
        private readonly MediaModel _model;
        private readonly IWebHelper _webHelper;
        private readonly AnalysisNSocialNetworkSettings _snSettings;

        public MediaTagBuilder(IWebHelper webHelper,
            AnalysisNSocialNetworkSettings snSettings)
        {
            _model = new MediaModel();
            _webHelper = webHelper;
            _snSettings = snSettings;
        }

        public void AddInfo<T>(T source) where T : BaseMediaModel
        {
            if (source == null)
                _model.Existed = false;

            _model.Artist = source.Artist;
            _model.MediaId = source.MediaId;
            _model.MediaName = source.MediaName;
            _model.Description = source.Description;
            _model.Existed = !string.IsNullOrEmpty(source.MediaId);
        }

        public string GenerateMediaTag()
        {
            var result = new StringBuilder();
            if (_model.Existed)
            {
                result.AppendFormat("<meta property=\"og:title\" content=\"{0}\" />", _model.MediaName);
                result.Append(Environment.NewLine);
                result.AppendFormat("<meta property=\"og:url\" content=\"{0}\" />", _webHelper.GetThisPageUrl(true));
                result.Append(Environment.NewLine);
                result.Append("<meta property=\"og:type\" content=\"video.movie\" />");
                result.Append(Environment.NewLine);
                result.AppendFormat("<meta property=\"og:image\" content=\"http://img.youtube.com/vi/{0}/1.jpg\" />", _model.MediaId);
                result.Append(Environment.NewLine);
                result.AppendFormat("<meta property=\"og:video\" content=\"http://www.youtube.com/v/{0}?version=3&amp;autohide=1\" />", _model.MediaId);
                result.Append(Environment.NewLine);
                result.Append("<meta property=\"og:video:type\" content=\"application/x-shockwave-flash\" />");
                result.Append(Environment.NewLine);
                result.AppendFormat("<meta property=\"fb:admins\" content=\"{0}\" />", _snSettings.FacebookID);
                result.Append(Environment.NewLine);
            }
            return result.ToString();
        }


        public bool IsMediaExisted()
        {
            return _model.Existed;
        }
    }
}
