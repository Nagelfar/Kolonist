﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kolonist.Contracts.Identities
{
    /// <summary>
    /// Strongly-typed identity class. Essentially just an ID with a 
    /// distinct type. It introduces strong-typing and speeds up development
    /// on larger projects. Idea by Jeremie, implementation by Rinat
    /// </summary>
    public interface IIdentity
    {
        /// <summary>
        /// Gets the id, converted to a string. Only alphanumerics and '-' are allowed.
        /// </summary>
        /// <returns></returns>
        string GetId();

        /// <summary>
        /// Unique tag (should be unique within the assembly) to distinguish
        /// between different identities, while deserializing.
        /// </summary>
        string GetTag();

        int GetStableHashCode();
    }

    public sealed class NullId : IIdentity
    {
        public const string TagValue = "";
        public static readonly IIdentity Instance = new NullId();

        public string GetId()
        {
            return "";
        }

        public string GetTag()
        {
            return "";
        }

        public int GetStableHashCode()
        {
            return 42;
        }
    }

    /// <summary>
    /// Base implementation of <see cref="IIdentity"/>, which implements
    /// equality and ToString once and for all.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    public abstract class AbstractIdentity<TKey> : IIdentity
    {
        public abstract TKey Id { get; protected set; }

        public string GetId()
        {
            return Id.ToString();            
        }

        public virtual string GetTag()
        {
            return this.GetType().Name
                .ToLowerInvariant()
                .Replace("id", "")
                .Replace("identity", "")
                ;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;

            var identity = obj as AbstractIdentity<TKey>;

            if (identity != null)
            {
                return Equals(identity);
            }

            return false;
        }

        public override string ToString()
        {
            return string.Format("{0}-{1}", GetType().Name.Replace("Id", ""), Id);
        }

        public override int GetHashCode()
        {
            return (Id.GetHashCode());
        }

        public int GetStableHashCode()
        {
            // same as hash code, but works across multiple architectures 
            var type = typeof(TKey);
            if (type == typeof(string))
            {
                return CalculateStringHash(Id.ToString());
            }
            return Id.GetHashCode();
        }

        static AbstractIdentity()
        {
            var type = typeof(TKey);
            if (type == typeof(int) || type == typeof(long) || type == typeof(uint) || type == typeof(ulong))
                return;
            if (type == typeof(Guid) || type == typeof(string))
                return;
            throw new InvalidOperationException("Abstract identity inheritors must provide stable hash. It is not supported for:  " + type);
        }

        static int CalculateStringHash(string value)
        {
            if (value == null) return 42;
            unchecked
            {
                var hash = 23;
                foreach (var c in value)
                {
                    hash = hash * 31 + c;
                }
                return hash;
            }
        }

        public bool Equals(AbstractIdentity<TKey> other)
        {
            if (other != null)
            {
                return other.Id.Equals(Id) && other.GetTag() == GetTag();
            }

            return false;
        }

        public static bool operator ==(AbstractIdentity<TKey> left, AbstractIdentity<TKey> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(AbstractIdentity<TKey> left, AbstractIdentity<TKey> right)
        {
            return !Equals(left, right);
        }
    }

    public static class IdentityConvert
    {
        public static string ToStream(IIdentity identity)
        {
            return identity.GetTag() + "-" + identity.GetId();
        }

        public static string ToTransportable(IIdentity identity)
        {
            return identity.GetTag() + "-" + identity.GetId();
        }

        private static readonly IEnumerable<Type> IdentityTypes =
            typeof(IIdentity)
                .Assembly
                .ExportedTypes
                .Where(x => !x.IsAbstract && !x.IsInterface)
                .Where(x => typeof(IIdentity).IsAssignableFrom(x))
                .ToList();

        private static Type FindType(string name)
        {
            return IdentityTypes.FirstOrDefault(x => x.Name.EndsWith(name, StringComparison.OrdinalIgnoreCase));
        }
        public static IIdentity FromTransportable(string transportable)
        {
            var index = transportable.IndexOf('-');
            var tag = transportable.Substring(0, index);
            var id = transportable.Substring(index + 1);

            var identityType = FindType(tag)
                ?? FindType(tag + "id")
                ?? FindType(tag + "identity");
            if (identityType == null)
                throw new InvalidOperationException("No class found with tag name " + tag);

            var idType = identityType.BaseType.GenericTypeArguments.First();
            var converter = TypeDescriptor.GetConverter(idType);
            var identityParam = converter.ConvertFromString(id);

            var instance = Activator.CreateInstance(identityType, identityParam);

            return instance as IIdentity;
        }
    }
}
