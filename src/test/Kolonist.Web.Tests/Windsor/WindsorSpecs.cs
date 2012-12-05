using Castle.MicroKernel;
using Castle.Windsor;
using Machine.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kolonist.Web.Tests.Windsor
{
    [Subject("Windsor")]
    public abstract class WindsorSpecs
    {
        internal static IWindsorContainer windsorContainer;

        Establish context = () => windsorContainer = new WindsorContainer();

        internal static IHandler[] GetAllHandlers(IWindsorContainer container)
        {
            return GetHandlersFor(typeof(object), container);
        }

        internal static IHandler[] GetHandlersFor<T>(IWindsorContainer container = null)
        {
            return GetHandlersFor(typeof(T), container ?? windsorContainer);
        }
        internal static IHandler[] GetHandlersFor(Type type, IWindsorContainer container)
        {
            return container.Kernel.GetAssignableHandlers(type);
        }
    }
}
