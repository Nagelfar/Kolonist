using Castle.MicroKernel;
using Castle.MicroKernel.Lifestyle;
using Castle.MicroKernel.Lifestyle.Scoped;
using Castle.MicroKernel.Registration;
using Kolonist.Contracts.Identities;
using Raven.Client;
using Raven.Client.Converters;
using Raven.Client.Embedded;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace Kolonist.Web.App_Start.Installers
{
    public class RavenDbInstaller : IWindsorInstaller
    {
        public void Install(Castle.Windsor.IWindsorContainer container, Castle.MicroKernel.SubSystems.Configuration.IConfigurationStore store)
        {
            var docStore = new EmbeddableDocumentStore
            {
                UseEmbeddedHttpServer = true,
                ConnectionStringName = "RavenDB"
            };
            docStore.Conventions.IdentityTypeConvertors.Add(new Converter());

            container.Register(
                Component.For<IDocumentStore>()
                    .Instance(docStore.Initialize())
                    .LifestyleSingleton(),

                Component.For<IDocumentSession>()
                    .UsingFactoryMethod(GetDocumentSession)
                    .LifestyleScoped<HybridScope>()
                );
        }

        private static IDocumentSession GetDocumentSession(IKernel kernel)
        {
            var store = kernel.Resolve<IDocumentStore>();
            return store.OpenSession();
        }

        private class HybridScope : IScopeAccessor
        {
            private readonly IScopeAccessor webRequestScopeAccessor = new WebRequestScopeAccessor();
            private readonly IScopeAccessor secondaryScopeAccessor = new ThreadScopeAccessor();

            private static FieldInfo initialized = typeof(PerWebRequestLifestyleModule).GetField("initialized", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.GetField);

            public ILifetimeScope GetScope(Castle.MicroKernel.Context.CreationContext context)
            {
                if (HttpContext.Current != null && (bool)initialized.GetValue(null))
                    return webRequestScopeAccessor.GetScope(context);
                return secondaryScopeAccessor.GetScope(context);
            }

            public void Dispose()
            {
                webRequestScopeAccessor.Dispose();
                secondaryScopeAccessor.Dispose();
            }
        }

        private class Converter : ITypeConverter
        {
            public bool CanConvertFrom(Type sourceType)
            {
                return typeof(Kolonist.Contracts.Identities.IIdentity).IsAssignableFrom(sourceType);
            }

            public string ConvertFrom(string tag, object value, bool allowNull)
            {
                return tag + IdentityConvert.ToTransportable(value as Kolonist.Contracts.Identities.IIdentity);
            }

            public object ConvertTo(string value)
            {
                return IdentityConvert.FromTransportable(value);
            }
        }
    }
}