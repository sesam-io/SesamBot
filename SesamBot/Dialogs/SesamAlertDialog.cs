using Microsoft.Bot.Builder.Dialogs;
using SesamBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Builder.Dialogs.Internals;

namespace SesamBot.Dialogs
{
    [Serializable]
    public sealed class SesamAlertDialog : IDialog<object>
    {
        private readonly List<ISesamError> errors;
        private readonly ISesamErrorRenderer renderer;
        public SesamAlertDialog(List<ISesamError> errors, ISesamErrorRenderer renderer)
        {
            SetField.NotNull(out this.renderer, nameof(renderer), renderer);
            SetField.NotNull(out this.errors, nameof(errors), errors);
        }

        async Task IDialog<object>.StartAsync(IDialogContext context)
        {
            await renderer.RenderAsync(errors);
            context.Done<object>(null);
        }
    }
}