using System;
using System.Collections.Concurrent;

namespace Audacia.Core.Extensions
{
    public static class FuncExtensions
    {
        /// <summary>
        /// Memoize the results of a function 
        /// </summary>
        /// <typeparam name="T1">The type of the argument</typeparam>
        /// <typeparam name="TResult">The type of the result</typeparam>
        /// <param name="sourceFunction">The function to memoize</param>
        /// <returns>The results of the function, or a cached version of it if it has run before with the same argument</returns>
        public static Func<T1, TResult> Memoize<T1, TResult>(this Func<T1, TResult> sourceFunction)
        {
            var cache = new ConcurrentDictionary<T1, TResult>();
            return arg => cache.GetOrAdd(arg, sourceFunction);
        }

        /// <summary>
        /// Memoize the results of a function 
        /// </summary>
        /// <typeparam name="T1">The type of the first argument</typeparam>
        /// <typeparam name="T2">The type of the second argument</typeparam>
        /// <typeparam name="TResult">The type of the result</typeparam>
        /// <param name="sourceFunction">The function to memoize</param>
        /// <returns>The results of the function, or a cached version of it if it has run before with the same arguments</returns>
        public static Func<T1, T2, TResult> Memoize<T1, T2, TResult>(this Func<T1, T2, TResult> sourceFunction)
        {
            var cache = new ConcurrentDictionary<Tuple<T1, T2>, TResult>();
            return
                (arg1, arg2) =>
                    cache.GetOrAdd(new Tuple<T1, T2>(arg1, arg2), tuple => sourceFunction(tuple.Item1, tuple.Item2));
        }


        /// <summary>
        /// Memoize the results of a function 
        /// </summary>
        /// <typeparam name="T1">The type of the argument</typeparam>
        /// <typeparam name="TResult">The type of the result</typeparam>
        /// <typeparam name="THash">The type of the hash</typeparam>
        /// <param name="sourceFunction">The function to memoize</param>
        /// <param name="hashingFunction">A custom hash for complex type equality matching</param>
        /// <returns>The results of the function, or a cached version of it if it has run before with the same argument</returns>
        public static Func<T1, TResult> Memoize<T1, TResult, THash>(this Func<T1, TResult> sourceFunction,
            Func<T1, THash> hashingFunction)
        {
            var cache = new ConcurrentDictionary<THash, TResult>();
            return arg => cache.GetOrAdd(hashingFunction(arg), _ => sourceFunction(arg));
        }

        /// <summary>
        /// Memoize the results of a function 
        /// </summary>
        /// <typeparam name="T1">The type of the first argument</typeparam>
        /// <typeparam name="T2">The type of the second argument</typeparam>
        /// <typeparam name="TResult">The type of the result</typeparam>
        /// <typeparam name="THash">The type of the hash</typeparam>
        /// <param name="sourceFunction">The function to memoize</param>
        /// <param name="hashingFunction">A custom hash for complex type equality matching</param>
        /// <returns>The results of the function, or a cached version of it if it has run before with the same arguments</returns>
        public static Func<T1, T2, TResult> Memoize<T1, T2, TResult, THash>(this Func<T1, T2, TResult> sourceFunction,
            Func<T1, T2, THash> hashingFunction)
        {
            var cache = new ConcurrentDictionary<THash, TResult>();
            return (arg1, arg2) => cache.GetOrAdd(hashingFunction(arg1, arg2), _ => sourceFunction(arg1, arg2));
        }
    }
}