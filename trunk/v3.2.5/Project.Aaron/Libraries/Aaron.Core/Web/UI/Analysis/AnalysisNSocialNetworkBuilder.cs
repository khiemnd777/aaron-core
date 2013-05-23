namespace Aaron.Core.Web.UI.Analysis
{
    using Aaron.Core.Domain.Common;

    public class AnalysisNSocialNetworkBuilder : IAnalysisNSocialNetworkBuilder
    {
        private const string _googleInitCode = @"<script type='text/javascript'>" +
              "var _gaq = _gaq || [];" +
              @"_gaq.push(['_setAccount', '{0}']);" +
              "_gaq.push(['_trackPageview']);" +
              "(function() {{" +
                "var ga = document.createElement('script'); ga.type = 'text/javascript'; ga.async = true;" +
                "ga.src = ('https:' == document.location.protocol ? 'https://ssl' : 'http://www') + '.google-analytics.com/ga.js';" +
                "var s = document.getElementsByTagName('script')[0]; s.parentNode.insertBefore(ga, s);" +
              "}})();" +
            "</script>";

        private const string _facebookInitCode = @"<div id='fb-root'></div>" +
                "<script>(function(d, s, id) {{" +
                "var js, fjs = d.getElementsByTagName(s)[0];" +
                "if (d.getElementById(id)) return;" +
                "js = d.createElement(s); js.id = id;" +
            @"js.src = '//connect.facebook.net/vi_VN/all.js#xfbml=1&appId={0}';" +
                "fjs.parentNode.insertBefore(js, fjs);" +
                "}}(document, 'script', 'facebook-jssdk'));" +
                "</script>";


        private readonly AnalysisNSocialNetworkSettings _settings;

        public AnalysisNSocialNetworkBuilder(AnalysisNSocialNetworkSettings settings)
        {
            _settings = settings;
        }

        public string GoogleInitCode()
        {
            var format = string.Format((!string.IsNullOrEmpty(_settings.GoogleInitCode)) ? _settings.GoogleInitCode : _googleInitCode, _settings.GoogleAnalysis);
            return format;
        }

        public string FacebookInitCode()
        {
            var format = string.Format((!string.IsNullOrEmpty(_settings.FacebookInitCode)) ? _settings.FacebookInitCode : _facebookInitCode, _settings.FacebookID);
            return format;
        }
    }
}
