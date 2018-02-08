using System.Collections.Generic;

namespace Audacia.Core
{
    public interface IResult<out TOutput> : IResult
    {
        TOutput Output { get; }
    }

    public interface IResult
    {
        /// <summary>
        /// Gets a flag indicating whether the operation succeeded or failed.
        /// </summary>
        bool IsSuccess { get; }

        /// <summary>
        /// Gets a collection of errors; this collection should only be populated if the result represents a failure.
        /// </summary>
        IEnumerable<string> Errors { get; }
    }
}