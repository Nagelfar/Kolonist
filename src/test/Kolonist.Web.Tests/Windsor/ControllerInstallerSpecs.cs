using Castle.MicroKernel;
using Castle.Core.Internal;
using Castle.Windsor;
using Kolonist.Web.App_Start.Installers;
using Kolonist.Web.Controllers;
using Machine.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Mvc;

namespace Kolonist.Web.Tests.Windsor
{
    [Subject(typeof(MvcWebApiControllerInstaller))]
    public abstract class ControllerInstallerSpecs
    {
        internal static IWindsorContainer windsorContainer;

        Establish context = () => windsorContainer = new WindsorContainer();

        Because of = () => windsorContainer.Install(new MvcWebApiControllerInstaller());




        internal static IHandler[] GetAllHandlers(IWindsorContainer container)
        {
            return GetHandlersFor(typeof(object), container);
        }

        internal static IHandler[] GetHandlersFor(Type type, IWindsorContainer container)
        {
            return container.Kernel.GetAssignableHandlers(type);
        }
    }

    public class when_registering_controllers : ControllerInstallerSpecs
    {
        It should_register_some_Controllers = () => GetAllHandlers(windsorContainer).ShouldNotBeEmpty();

        It should_register_all_Controllers_with_IController = () =>
            GetAllHandlers(windsorContainer).ShouldContain(GetHandlersFor(typeof(IController), windsorContainer));

        It should_register_all_Controllers_with_IHttpController = () =>
            GetAllHandlers(windsorContainer).ShouldContain(GetHandlersFor(typeof(IHttpController), windsorContainer));

        It should_register_all_MvcControllers_found_in_the_assembly = () =>
            GetPublicClassesFromApplicationAssembly(x => x.Is<IController>()).ShouldEqual(GetImplementationTypesFor(typeof(IController), windsorContainer));
        It should_register_all_HttpControllers_found_in_the_assembly = () =>
            GetPublicClassesFromApplicationAssembly(x => x.Is<IHttpController>()).ShouldEqual(GetImplementationTypesFor(typeof(IHttpController), windsorContainer));


        It should_have_all_concrete_MvcControllers_with_an_Controller_suffix = () =>
            GetPublicClassesFromApplicationAssembly(c => c.Is<IController>() && !c.IsAbstract && c.Name.EndsWith("Controller")).ShouldEqual(GetImplementationTypesFor(typeof(IController), windsorContainer));
        It should_have_all_concrete_HttpControllers_with_an_Controller_suffix = () =>
            GetPublicClassesFromApplicationAssembly(c => c.Is<IHttpController>() && !c.IsAbstract && c.Name.EndsWith("Controller")).ShouldEqual(GetImplementationTypesFor(typeof(IHttpController), windsorContainer));

        It should_verify_that_all_MvcControllers_live_in_a_controller_namespace =()=>
            GetPublicClassesFromApplicationAssembly(c=>c.Namespace.Contains("Controllers") && !c.IsAbstract && c.Is<IController>()).ShouldEqual(GetImplementationTypesFor(typeof(IController),windsorContainer));
        It should_verify_that_all_HttpControllers_live_in_a_controller_namespace = () =>
            GetPublicClassesFromApplicationAssembly(c => c.Namespace.Contains("Controllers") && !c.IsAbstract && c.Is<IHttpController>()).ShouldEqual(GetImplementationTypesFor(typeof(IHttpController), windsorContainer));

        It should_register_all_MvcControllers_as_transient = () =>
            GetHandlersFor(typeof(IController), windsorContainer).Where(x => x.ComponentModel.LifestyleType != Castle.Core.LifestyleType.Transient).ShouldBeEmpty();
        It should_register_all_HttpControllers_as_transient = () =>
            GetHandlersFor(typeof(IHttpController), windsorContainer).Where(x => x.ComponentModel.LifestyleType != Castle.Core.LifestyleType.Transient).ShouldBeEmpty();

        It should_register_every_MvcController_as_service = () =>
            GetHandlersFor(typeof(IController), windsorContainer).Where(controller => controller.ComponentModel.Services.Single() != controller.ComponentModel.Implementation).ShouldBeEmpty();
        It should_register_every_HttpController_as_service = () =>
            GetHandlersFor(typeof(IHttpController), windsorContainer).Where(controller => controller.ComponentModel.Services.Single() != controller.ComponentModel.Implementation).ShouldBeEmpty();

        static Type[] GetImplementationTypesFor(Type type, IWindsorContainer container)
        {
            return GetHandlersFor(type, container)
                .Select(h => h.ComponentModel.Implementation)
                .OrderBy(t => t.Name)
                .ToArray();
        }

        static Type[] GetPublicClassesFromApplicationAssembly(Predicate<Type> where)
        {
            return typeof(HomeController).Assembly.GetExportedTypes()
                .Where(t => t.IsClass)
                .Where(t => t.IsAbstract == false)
                .Where(where.Invoke)
                .OrderBy(t => t.Name)
                .ToArray();
        }
    }


}
