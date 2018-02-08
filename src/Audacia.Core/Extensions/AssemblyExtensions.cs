using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Audacia.Core.Extensions
{
    public static class AssemblyExtensions
    {
        public static IEnumerable<TFilterType> GetNewInstancesOfInheritingTypesWithParameterlessConstructors<TFilterType>(this Assembly assembly) where TFilterType : class
        {
            var filterType = typeof(TFilterType);

            return from type in assembly.GetTypes()
                //Check if it is a FilterType
                where filterType.IsAssignableFrom(type)
                //Check that it is not FilterType itself
                where type != filterType
                //Check that is has a parameterless constructor
                where type.GetConstructor(Type.EmptyTypes) != null
                let instance = Activator.CreateInstance(type)
                select instance as TFilterType;
        }
    }
}