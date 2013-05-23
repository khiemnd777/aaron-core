using System.Collections.Generic;
using Aaron.Core;
using Aaron.Core.Domain;
using Aaron.Core.Domain.Common;

namespace Aaron.Core.Services.Common
{
    public interface INoticeService: IServices
    {
        void InsertNotice(Notice notice);
        void UpdateNotice(Notice notice);
        void DeleteNotice(Notice notice);
        void HideNotice();
        Notice GetNoticeById(int noticeId);
        IList<Notice> GetAllNotice(bool showHidden = true);
        IList<Notice> GetPublishedNotice();
        bool CheckAccountExisted(int noticeId);

    }
}
