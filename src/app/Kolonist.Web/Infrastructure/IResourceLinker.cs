using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace Kolonist.Web.Infrastructure
{
    public interface IResourceLinker
    {
        Uri GetContent<T>(Expression<Action<T>> method);
        Uri GetUri<T>(Expression<Action<T>> method);
    }
}