using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Connector;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SesamBot.Models
{
    public interface ISesamErrorRenderer
    {
        Task RenderAsync(List<ISesamError> errors);
    }

    public class SesamErrorRenderer : ISesamErrorRenderer
    {
        private IBotToUser botToUser;
        public SesamErrorRenderer(IBotToUser botToUser)
        {
            this.botToUser = botToUser;
        }

        public async Task RenderAsync(List<ISesamError> errors)
        {
            var message = botToUser.MakeMessage();
            message = BuildMessageFromErrors(errors, message);
            await botToUser.PostAsync(message);
        }

        public static IMessageActivity BuildMessageFromErrors(List<ISesamError> errors, IMessageActivity message)
        {
            foreach (ISesamError error in errors)
            {
                List<CardAction> cardButtons = new List<CardAction>();

                CardAction jiraButton = new CardAction()
                {
                    Value = "https://jira.atlassian.com/secure/Dashboard.jspa",
                    Type = "openUrl",
                    Title = "Create Jira case"
                };
                CardAction googleButton = new CardAction()
                {
                    Value = $"https://google.com?q={error.SubTitle}",
                    Type = "openUrl",
                    Title = "Google error message"
                };
                cardButtons.Add(jiraButton);
                cardButtons.Add(googleButton);

                ThumbnailCard tnCard = new ThumbnailCard(error.Title, error.SubTitle, error.Text, null, cardButtons);

                message.Attachments.Add(tnCard.ToAttachment());

            }
            return message;
        }
    }
}