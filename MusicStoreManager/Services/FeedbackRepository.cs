using MusicStoreManager.Entities;
using MusicStoreManager.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicStoreManager.Models
{
    public class FeedbackRepository : IFeedbackRepository
    {
        private readonly mvcMusicStoreContext _mvcMusicStoreContext;

        public FeedbackRepository(mvcMusicStoreContext mvcMusicStoreContext)
        {
            _mvcMusicStoreContext = mvcMusicStoreContext;
        }

        public void AddFeedback(Feedback feedback)
        {
            _mvcMusicStoreContext.Feedbacks.Add(feedback);
            _mvcMusicStoreContext.SaveChanges();

        }
    }
}