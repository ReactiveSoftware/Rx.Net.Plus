using System;
using System.Threading;

namespace Rx.Net.Plus
{
    /// <summary>
    /// The purpose of this class is to:
    /// 1) allow deterministic destruction of objects
    /// 2) provide a built-in CancellationToken to allow Rx un-subscriptions (using CancellationToken)
    /// </summary>

    public abstract class DisposableBaseClass : IDisposableBaseClass
    {
        private CancellationTokenSource _cts = new CancellationTokenSource();
        private bool _isDisposed = false;

        public CancellationTokenSource CTS => _cts;
        public CancellationToken CancellationToken => CTS.Token;
        public bool IsDisposed => _isDisposed;

        //Implement IDisposable.
        public void Dispose()
        {
            if (false == IsDisposed)
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }
        }

        protected virtual void OnDisposing(bool isDisposing)
        {
            if (null != _cts)
            {
                CTS.Cancel();
                _cts = null;
            }
        }

        private void Dispose(bool isDisposing)
        {
            if (false == IsDisposed)
            {
                OnDisposing(isDisposing);
                _isDisposed = true;
            }
        }

        ~DisposableBaseClass()
        {
            Dispose(false);
        }
    }
}