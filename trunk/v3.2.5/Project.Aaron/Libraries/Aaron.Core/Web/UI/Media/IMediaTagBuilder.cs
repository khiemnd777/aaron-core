using Aaron.Core.Web.Mvc;

namespace Aaron.Core.Web.UI.Media
{

    /// <summary>
    /// Use with Open Graph Protocol
    /// </summary>
    public interface IMediaTagBuilder
    {
        void AddInfo<T>(T source) where T : BaseMediaModel;
        //meta tag media with Open Graph
        string GenerateMediaTag();
        bool IsMediaExisted();
    }
}
