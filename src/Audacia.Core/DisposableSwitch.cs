using System;

namespace Audacia.Core
{
    /// <summary>
    /// A disposable class with events to handle IDisposable. 
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("IDisposableAnalyzers.Correctness", "IDISP025:Class with no virtual dispose method should be sealed", Justification = "Class needs to be inherited.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1063:Implement IDisposable Correctly", Justification = "No managed resources inherited.")]
    public class DisposableSwitch : IDisposable
    {
        private readonly Action _onDispose;

        /// <summary>
        /// Initialize a new instance of <see cref="DisposableSwitch"/>.
        /// </summary>
        /// <param name="onInit">Action to invoke when class is initialized.</param>
        /// <param name="onDispose">Action to invoke when class is disposed.</param>
        public DisposableSwitch(Action onInit, Action onDispose)
        {
            _onDispose = onDispose;
            onInit?.Invoke();
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            _onDispose();
            GC.SuppressFinalize(this);
        }
    }
}