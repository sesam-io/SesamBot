using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using SesamBot.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SesamBot.Dialogs
{
    [LuisModel("16b241a8-719e-4783-a522-e98904ca92e9", "55a87363534c4c7882b72425ff8c3a4b")]
    [Serializable]
    public class SesamLuisDialog : LuisDialog<object>
    {
        private ISesamSubscribeService service;

        public SesamLuisDialog(ISesamSubscribeService service, ILuisService luis) :base(luis)
        {
            SetField.NotNull(out this.service, nameof(service), service);
        }

        [LuisIntent("None")]
        public async Task NoneHandler(IDialogContext context, LuisResult result)
        {
            HeroCard card = new HeroCard
            {
                Title = "WTF!?",
                Subtitle = "......",
                Images = new List<CardImage> { new CardImage("https://media.giphy.com/media/12KiGLydHEdak8/giphy.gif") }
            };

            var message = context.MakeMessage();
            message.Attachments.Add(card.ToAttachment());
            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }

        [LuisIntent("GetNodeEntities")]
        public async Task GetNodeEntities(IDialogContext context, LuisResult result)
        {
            if (result.Entities.Count > 0)
            {
                string entity = result.Entities[0].Entity;

                await context.PostAsync(await SesamService.GetEntities(entity));
                context.Wait(MessageReceived);
            }
            else
            {
                await NoneHandler(context, result);
            }
        }

        [LuisIntent("GetErrors")]
        public async Task GetErrors(IDialogContext context, LuisResult result)
        {
            List<ISesamError> errors = await SesamService.GetErrors();
            var message = context.MakeMessage();
            message.AttachmentLayout = AttachmentLayoutTypes.Carousel;
            if (errors.Count > 0)
            {
                message = SesamErrorRenderer.BuildMessageFromErrors(errors, message);
            }
            else
            {
                message.Text = "No errors found";
            }
            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }

        [LuisIntent("AlertErrors")]
        public async Task AlertErrors(IDialogContext context, LuisResult result)
        {
            service.Subscribe();
            await context.PostAsync("You will now be alerted on errors");
            context.Wait(MessageReceived);
        }

        [LuisIntent("StopAlerting")]
        public async Task RemoveAlerts(IDialogContext context, LuisResult result)
        {
            service.Unsubscribe();
            await context.PostAsync("You are no longer alerted on errors");
            context.Wait(MessageReceived);
        }



    }
}