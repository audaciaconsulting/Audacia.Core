using System;

namespace Audacia.Core
{
    public class DisposableSwitch : IDisposable
    {
        private readonly Action _onDispose;

        public DisposableSwitch(Action onInit, Action onDispose)
        {
            _onDispose = onDispose;
            onInit();
        }

        public void Dispose()
        {
            _onDispose();
        }
    }
}