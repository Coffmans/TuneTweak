using Akka.Actor;
using Akka.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using TuneTweak.Actors;
using TuneTweak.Interfaces;

namespace TuneTweak
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            // Setup DI container
            var services = new ServiceCollection();
            services.AddSingleton<IUIUpdater, TuneTweakMainWindow>(); // Form implements IUIUpdater
            var serviceProvider = services.BuildServiceProvider();

            var bootstrap = BootstrapSetup.Create();
            // Use DependencyResolverSetup instead of ServiceProviderSetup
            var diSetup = DependencyResolverSetup.Create(new ServiceProviderDependencyResolver(serviceProvider));
            var actorSystemSetup = bootstrap.And(diSetup);
            using var system = ActorSystem.Create("TuneTweakSystem", actorSystemSetup);

            var resolver = DependencyResolver.For(system);
            var coordinatorActor = system.ActorOf(resolver.Props<CoordinatorActor>(), "coordinator");

            // Launch the UI
            var form = serviceProvider.GetRequiredService<IUIUpdater>() as TuneTweakMainWindow;
            if (form != null)
            {
                form.SetCoordinator(coordinatorActor); // Add this method to form
                Application.Run(form);
            }

            // Clean shutdown
            system.Terminate().Wait();
        }
    }
}
