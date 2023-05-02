using Autofac;
using Lakeshore.Order.Application;
using MediatR;

using IContainer = Autofac.IContainer;

namespace Lakeshore.Order.Infrastructure;
public static class CommandsExecutor
{
    internal static async Task Execute(ICommand command)
    {
        using (var scope = HarCompositionRoot.BeginLifetimeScope())
        {
            var mediator = scope.Resolve<IMediator>();
            await mediator.Send(command);
        }
    }

    internal static async Task<TResult> Execute<TResult>(ICommand<TResult> command)
    {
        using (var scope = HarCompositionRoot.BeginLifetimeScope())
        {
            var mediator = scope.Resolve<IMediator>();
            return await mediator.Send(command);
        }
    }
}

internal static class HarCompositionRoot
{
    private static IContainer _container;

    internal static void SetContainer(IContainer container)
    {
        _container = container;
    }

    internal static ILifetimeScope BeginLifetimeScope()
    {
        return _container.BeginLifetimeScope();
    }
}