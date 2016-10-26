using Autofac;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Builder.Luis;
using SesamBot.Dialogs;
using System.Collections.Generic;

namespace SesamBot.Models
{
    public class SesamModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.Register(c => new LuisModelAttribute("16b241a8-719e-4783-a522-e98904ca92e9", "55a87363534c4c7882b72425ff8c3a4b")).AsSelf().AsImplementedInterfaces().SingleInstance();

            // register the top level dialog
            builder.RegisterType<SesamLuisDialog>().As<IDialog<object>>().InstancePerDependency();

            // register other dialogs we use
            builder.Register((c, p) => new SesamAlertDialog(p.TypedAs<List<ISesamError>>(), c.Resolve<ISesamErrorRenderer>())).AsSelf().InstancePerDependency();

            // register some singleton services
            builder.RegisterType<LuisService>().Keyed<ILuisService>(FiberModule.Key_DoNotSerialize).AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<PollScheduler>().Keyed<IPollScheduler>(FiberModule.Key_DoNotSerialize).AsImplementedInterfaces().SingleInstance();

            // register some objects dependent on the incoming message
            builder.Register(c => new SesamErrorRenderer(c.Resolve<IBotToUser>())).Keyed<ISesamErrorRenderer>(FiberModule.Key_DoNotSerialize).AsImplementedInterfaces().InstancePerMatchingLifetimeScope(DialogModule.LifetimeScopeTag);
            builder.Register(c => new SesamSubscribeService(c.Resolve<IPollScheduler>(), c.Resolve<ResumptionCookie>())).Keyed<ISesamSubscribeService>(FiberModule.Key_DoNotSerialize).AsImplementedInterfaces().InstancePerMatchingLifetimeScope(DialogModule.LifetimeScopeTag);

        }
    }
}