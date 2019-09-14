using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EPiServer.Framework.Initialization;

namespace EPiServer.Reference.Commerce.Site.modules._protected.SiteModules.Content.Services
{
    /// <summary>
    /// Helper class for reflection
    /// </summary>
    public static class ReflectionHelper
    {
        /// <summary>
        /// Get derived types from base type ignore abstract, interface, generic types
        /// </summary>
        /// <param name="baseType"></param>
        /// <returns>Derived types ignore abstract, interface, generic types </returns>
        public static IEnumerable<Type> GetConcreteDerivedTypes(Type baseType)
        {
            var domainAssembly = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.IsDynamicAssembly()) // ignore proxy classes of StructureMap, ...
                .SelectMany(a => GetTypesFromAssembly(a));

            // ignore abstract, interface, generic types
            return domainAssembly.Where(t => baseType.IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface && !t.IsGenericType)?.ToList();
        }

        /// <summary>
        /// Get all parent generic types of provided type including interfaces, abstract classes
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IEnumerable<Type> GetParentGenericTypes(Type type)
        {
            return GetParentTypes(type)?.Where(t => t.IsGenericType);
        }

        /// <summary>
        /// Get all parent types of provided type
        /// </summary>
        /// <param name="type"></param>
        /// <returns>parent types including interfaces, abstract classes</returns>
        public static IEnumerable<Type> GetParentTypes(Type type)
        {
            if (type == null)
            {
                yield break;
            }

            // return all implemented or inherited interfaces
            foreach (var i in type.GetInterfaces())
            {
                yield return i;
            }

            // return all inherited types
            var currentBaseType = type.BaseType;
            while (currentBaseType != null)
            {
                yield return currentBaseType;
                currentBaseType = currentBaseType.BaseType;
            }
        }

        /// <summary>
        /// Get all types from an assembly
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        private static IEnumerable<Type> GetTypesFromAssembly(Assembly assembly)
        {
            if (assembly == null)
            {
                return Enumerable.Empty<Type>();
            }
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                // when any classes of modules can not be loaded
                // Because assemblies can be loaded dynamically (e.g load all assemblies in a folder) and sometimes it fails to load (e.g loading an assembly which is not available on disk).
                // Fortunately, when it fails to load, the Exception still contains the information we need. 
                // For reference, please visit https://haacked.com/archive/2012/07/23/get-all-types-in-an-assembly.aspx/
                return ex.Types.Where(t => t != null);
            }
        }
    }
}