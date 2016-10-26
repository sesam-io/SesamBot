using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Hosting;

namespace SesamBot.Models
{

    public interface IPollScheduler
    {
        void Subscribe(ResumptionCookie cookie);
        void Unsubscribe(ResumptionCookie cookie);
    }

    public class PollScheduler : IPollScheduler
    {
        const int pollInterval = 5;
        private readonly List<ResumptionCookie> subscribers = new List<ResumptionCookie>();

        public PollScheduler()
        {
            
            ResumptionCookie[] subscriberCookies = subscribers.ToArray<ResumptionCookie>();

            HostingEnvironment.QueueBackgroundWorkItem(async token =>
            {
                while (true)
                {
                    token.ThrowIfCancellationRequested();
                    subscriberCookies = subscribers.ToArray<ResumptionCookie>();

                    //Only bother to check if somebody is subscribing
                    if (subscriberCookies.Length > 0)
                    {
                        List<ISesamError> errors = await SesamService.GetErrors();
                        
                        //only take action if theres anything to report
                        if (errors.Count > 0)
                        {
                            foreach (ResumptionCookie cookie in subscriberCookies)
                            {
                                await SesamErrorHandler.HandleSesamErrors(errors, cookie, token);
                            }
                        }
                    }
                    await Task.Delay(TimeSpan.FromSeconds(pollInterval));
                }
            });
        }

        public void Subscribe(ResumptionCookie cookie)
        {
            this.subscribers.Add(cookie);
        }

        public void Unsubscribe(ResumptionCookie cookie)
        {
            if(this.subscribers.Contains(cookie))
            {
                this.subscribers.Remove(cookie);
            }
        }
    }
}