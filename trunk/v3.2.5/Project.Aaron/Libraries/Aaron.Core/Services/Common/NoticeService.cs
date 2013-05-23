using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aaron.Core.Domain.Common;
using Aaron.Core.Data;
using Aaron.Core;
using Aaron.Core.Services.Accounts;
using Aaron.Core.Caching;

namespace Aaron.Core.Services.Common
{
    public class NoticeService: INoticeService
    {
        private const string NOTICE_ALL = "Aaron.Core.notice.all-{0}";
        private const string NOTICE_BY_ID = "Aaron.Core.notice.id-{0}";

        private readonly IRepository<Notice> _noticeRepository;
        private readonly ICurrentActivity _currentActivity;
        private readonly ICacheManager _cacheManager;

        public NoticeService(IRepository<Notice> noticeRepository, 
            ICurrentActivity currentActivity,
            ICacheManager cacheManager)
        {
            _noticeRepository = noticeRepository;
            _currentActivity = currentActivity;
            _cacheManager = cacheManager;
        }

        public void InsertNotice(Notice notice)
        {
            if (notice == null)
                throw new ArgumentNullException("notice");

            _noticeRepository.Insert(notice);

            _cacheManager.RemoveByPattern(NOTICE_ALL);
            _cacheManager.RemoveByPattern(NOTICE_BY_ID);
        }

        public void UpdateNotice(Notice notice)
        {
            if (notice == null)
                throw new ArgumentNullException("notice");

            _noticeRepository.Update(notice);

            _cacheManager.RemoveByPattern(NOTICE_ALL);
            _cacheManager.RemoveByPattern(NOTICE_BY_ID);
        }

        public void DeleteNotice(Notice notice)
        {
            if (notice == null)
                throw new ArgumentNullException("notice");
            if(notice.IsSystem)
                throw new AaronException("Notice is system that be not deleted!");

            _noticeRepository.Delete(notice);

            _cacheManager.RemoveByPattern(NOTICE_ALL);
            _cacheManager.RemoveByPattern(NOTICE_BY_ID);
        }

        public void HideNotice()
        {
            var notice = _noticeRepository.Table
                .Where(n => n.Published)
                .SingleOrDefault();

            if (notice == null)
                throw new ArgumentNullException("notice");

            if (!CheckAccountExisted(notice.Id) && _currentActivity.CurrentAccount.IsRegistered())
                notice.Accounts.Add(_currentActivity.CurrentAccount);

            UpdateNotice(notice);
        }

        public IList<Notice> GetAllNotice(bool showHidden = true)
        {
            var key = string.Format(NOTICE_ALL, showHidden);
            return _cacheManager.Get(key, () => 
            {
                var list = _noticeRepository.Table;
                if (!showHidden)
                    list = list.Where(x => x.Published);

                return list.ToList();
            });
        }

        public IList<Notice> GetPublishedNotice()
        {
            var list = new List<Notice>();

            var publishednotice = _noticeRepository.Table
                .SingleOrDefault(n => n.Published);
            var systemNotice = _noticeRepository.Table
                .FirstOrDefault(n => n.IsSystem);
            if(!publishednotice.IsNull() && !CheckAccountExisted(publishednotice.Id))
                list.Add(publishednotice);
            if (!systemNotice.IsNull() && !_currentActivity.CurrentAccount.IsRegistered())
                list.Add(systemNotice);

            return list.ToList();
        }

        public bool CheckAccountExisted(int noticeId)
        {
            var notice = _noticeRepository.GetById(noticeId);

            return notice.Accounts
                .Contains(_currentActivity
                    .CurrentAccount);
        }

        public Notice GetNoticeById(int noticeId)
        {
            if(noticeId == 0)
                return null;

            var key = string.Format(NOTICE_BY_ID, noticeId);
            return _cacheManager.Get(key, () => 
            {
                return _noticeRepository.GetById(noticeId);
            });
        }

    }
}
