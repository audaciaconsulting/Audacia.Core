using System.Collections.Generic;

namespace Audacia.Core
{
    /// <summary>
    /// Indicates the success or failure of a an operation with data or a collection for errors.
    /// </summary>
    /// <typeparam name="TOutput">The type of data.</typeparam>
    public interface IResult<out TOutput> : IResult
    {
        /// <summary>
        /// Gets the data result of a operation.
        /// </summary>
        TOutput Output { get; }
    }

    /// <summary>
    /// Indicates the success or failure of a an operation with a collection for errors (if applicable).
    /// </summary>
    public interface IResult
    {
        /// <summary>
        /// Gets a value indicating whether gets a flag indicating whether the operation succeeded or failed.
        /// </summary>
        bool IsSuccess { get; }

        /// <summary>
        /// Gets a collection of errors; this collection should only be populated if the result represents a failure.
        /// </summary>
        IEnumerable<string> Errors { get; }
    }
}