using Autofac;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Connector;
using SesamBot.Dialogs;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SesamBot.Models
{
    public static class SesamErrorHandler
    {
        public static async Task HandleSesamErrors(List<ISesamError> errors, ResumptionCookie cookie, CancellationToken token)
        {
            var container = WebApiApplication.FindContainer();
            await HandleSesamErrors(container, errors, cookie, token);
        }

        public static async Task HandleSesamErrors(ILifetimeScope container, List<ISesamError> errors, ResumptionCookie cookie, CancellationToken token)
        {
            
            var message = cookie.GetMessage();

            using (var scope = DialogModule.BeginLifetimeScope(container, message))
            {
                // find the bot data interface and load up the conversation dialog state
                var botData = scope.Resolve<IBotData>();
                await botData.LoadAsync(token);

                errors = removeAlreadyHandledErrors(errors, botData.UserData);
                if (errors.Count > 0)
                {
                    // resolve the dialog stack
                    var stack = scope.Resolve<IDialogStack>();
                    // make a dialog to push on the top of the stack
                    var child = scope.Resolve<SesamAlertDialog>(TypedParameter.From(errors));
                    // wrap it with an additional dialog that will restart the wait for
                    // messages from the user once the child dialog has finished
                    var interruption = child.Void<object, IMessageActivity>();

                    try
                    {
                        // put the interrupting dialog on the stack
                        stack.Call(interruption, null);
                        // start running the interrupting dialog
                        await stack.PollAsync(token);
                    }
                    finally
                    {
                        //update which errors already has been handled
                        foreach (ISesamError error in errors)
                        {
                            botData.UserData.SetValue(error.Hash, string.Empty);
                        }

                        // save out the conversation dialog state
                        await botData.FlushAsync(token);
                    }
                }
            }
        }

        private static List<ISesamError> removeAlreadyHandledErrors(List<ISesamError> errors, IBotDataBag userData)
        {
            // removes all the errors that the user already has been notified about
            ISesamError[] copy = errors.ToArray();
            string hash;
            foreach(ISesamError error in copy)
            {
                if(userData.TryGetValue(error.Hash, out hash))
                {
                    errors.Remove(error);
                }
            }
            return errors;
        }
    }
}