using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Internals.Fibers;
using System;

namespace SesamBot.Models
{
    public interface ISesamSubscribeService
    {
        void Subscribe();
        void Unsubscribe();
    }

    [Serializable]
    public class SesamSubscribeService : ISesamSubscribeService
    {
        private readonly IPollScheduler scheduler;
        private readonly ResumptionCookie cookie;

        public SesamSubscribeService(IPollScheduler scheduler, ResumptionCookie cookie)
        {
            SetField.NotNull(out this.scheduler, nameof(scheduler), scheduler);
            SetField.NotNull(out this.cookie, nameof(cookie), cookie);
        }

        public void Subscribe()
        {
            scheduler.Subscribe(this.cookie);
        }

        public void Unsubscribe()
        {
            scheduler.Unsubscribe(this.cookie);
        }
    }

}