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
    }
}
