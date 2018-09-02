using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;

namespace Rx.Net.Plus
{
    public static class Extensions
    {
        #region Rx Subscriptions

        public static IDisposable RedirectTo<T>(this IObservable<T> src, IObserver<T> observer) where T : IComparable<T>
            => src.Subscribe(observer);

        public static IDisposable Notify<T>(this IObservable<T> src, IObserver<T> observer) where T : IComparable<T>
            => src.Subscribe(observer);

        public static IDisposable Notify<T>(this IObservable<T> src, Action<T> onNext) where T : IComparable<T>
            => src.Subscribe(onNext);

        public static void RedirectTo<T>(this IObservable<T> src, IObserver<T> observer, CancellationToken ct) where T : IComparable<T>
            => src.Subscribe(observer, ct);

        public static void Notify<T>(this IObservable<T> src, IObserver<T> observer, CancellationToken ct) where T : IComparable<T>
            => src.Subscribe(observer, ct);

        public static void Notify<T>(this IObservable<T> src, Action<T> onNext, CancellationToken ct) where T : IComparable<T>
            => src.Subscribe(onNext, ct);

        #endregion

        #region Rx Filtering and Comparison

        public static IObservable<T> When<T>(this IObservable<T> src, T v) where T : IComparable<T>
            => src.Where(v1 => v1.Equals(v));

        public static IObservable<T> If<T>(this IObservable<T> src, T v) where T : IComparable<T>
            => src.Where(v1 => v1.Equals(v));

        public static IObservable<T> IfNot<T>(this IObservable<T> src, T v) where T : IComparable<T>
            => src.Where(v1 => !v1.Equals(v));

        #endregion

        #region RxVar Instantation from other types (including subscription)

        public static RxVar<T> ToRxVar<T>(this T v) 
            => new RxVar<T>(v);

        /// <summary>
        /// Create a new instance of RxVar from another one
        /// and connect the new to the source
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"> a source RxVar</param>
        /// <returns> A new connected RxVar </returns>
        public static RxVar<T> ToRxVarAndSubscribe<T>(this IObservable<T> source) 
            => new RxVar<T>(source);

        #endregion

        #region RxProperty Instantation from other types (including subscription)

        public static RxProperty<T> ToRxProperty<T>(this T v)
            => new RxProperty<T>(v) ;

        /// <summary>
        /// Create a new instance of RxProperty from another RxVar
        /// and connect the new to the source
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"> a source RxVar</param>
        /// <returns> A new connected RxVar </returns>
        public static RxProperty<T> ToRxPropertyAndSubscribe<T>(this IObservable<T> source)
            => new RxProperty<T>(source);

        #endregion
    }

    /// <summary>
    /// The purpose of this class is to:
    /// 1) allow deterministic destruction of objects
    /// 2) provide a built-in CancellationToken to allow Rx unsubscriptions (using CancellationToken)
    /// </summary>

    public abstract class DisposableBaseClass : IDisposableBaseClass
    {
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
       
        public CancellationTokenSource CTS => _cts;

        public CancellationToken CancellationToken => CTS.Token;

        private bool _isDisposed = false;
        public bool IsDisposed => _isDisposed;

        //Implement IDisposable.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void OnDisposing(bool isDisposing)
        {
            CTS.Cancel();
        }

        protected virtual void Dispose(bool isDisposing)
        {
            if (false == IsDisposed)
            {
                OnDisposing(isDisposing);
                _isDisposed = true;
            }
        }

        #region Serialization

        #endregion
    }
}
