using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Audacia.Core.Extensions;

/// <summary>
/// Extension methods for the type <see cref="Assembly"/>.
/// </summary>
public static class AssemblyExtensions
{
    /// <summary>
    /// Get a collection of types from an assembly that match the type <typeparamref name="TFilterType"/>.
    /// </summary>
    /// <typeparam name="TFilterType">A type to filter the assembly parameters.</typeparam>
    /// <param name="assembly">The source assembly.</param>
    /// <returns>A collection of <typeparamref name="TFilterType"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="assembly"/> is null.</exception>
    public static IEnumerable<TFilterType> GetNewInstancesOfInheritingTypesWithParameterlessConstructors<TFilterType>(this Assembly assembly) where TFilterType : class
    {
        if (assembly == null)
        {
            throw new ArgumentNullException(nameof(assembly), "Assembly cannot be null");
        }

        var filterType = typeof(TFilterType);

        var result = from type in assembly.GetTypes()
                     where filterType.IsAssignableFrom(type) //Check if it is a FilterType
                     where type != filterType //Check that it is not FilterType itself                       
                     where type.GetConstructor(Type.EmptyTypes) != null //Check that is has a parameterless constructor
                     let instance = Activator.CreateInstance(type)
                     select instance as TFilterType;

        return result.ToList() ?? default!;
    }
}