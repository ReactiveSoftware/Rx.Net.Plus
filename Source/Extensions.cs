using System;
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

        /// <summary>
        /// Ignore null elements
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="src"></param>
        /// <returns></returns>
        public static IObservable<T> IgnoreNull<T>(this IObservable<T> src) where T : class
            => src.Where(v => v != null);

        public static IObservable<T> When<T>(this IObservable<T> src, T v) where T : IComparable<T>
            => src.Where(v1 => v1.Equals(v));

        public static IObservable<T> If<T>(this IObservable<T> src, T v) where T : IComparable<T>
            => src.Where(v1 => v1.Equals(v));

        public static IObservable<T> IfNot<T>(this IObservable<T> src, T v) where T : IComparable<T>
            => src.Where(v1 => !v1.Equals(v));

        public static IObservable<T> IfEqualTo<T>(this IObservable<T> src, T v) where T : IComparable<T>
            => src.Where(v1 => v1.Equals(v));

        public static IObservable<T> IfNotEqualTo<T>(this IObservable<T> src, T v) where T : IComparable<T>
            => src.Where(v1 => !v1.Equals(v));

        public static IObservable<T> IfLessThan<T>(this IObservable<T> src, T v) where T : IComparable<T>
            => src.Where(v1 => v1.CompareTo(v) < 0);

        public static IObservable<T> IfLessThanOrEqualTo<T>(this IObservable<T> src, T v) where T : IComparable<T>
            => src.Where(v1 => v1.CompareTo(v) <= 0);

        public static IObservable<T> IfGreaterThan<T>(this IObservable<T> src, T v) where T : IComparable<T>
            => src.Where(v1 => v1.CompareTo(v) > 0);

        public static IObservable<T> IfGreaterThanOrEqualTo<T>(this IObservable<T> src, T v) where T : IComparable<T>
            => src.Where(v1 => v1.CompareTo(v) >= 0);

        public static IObservable<T> IfInRange<T>(this IObservable<T> src, T min, T max) where T : IComparable<T>
            => src.Where(val => val.CompareTo(min) >= 0 && val.CompareTo(max) <= 0);

        public static IObservable<T> IfInStrictRange<T>(this IObservable<T> src, T min, T max) where T : IComparable<T>
            => src.Where(val => val.CompareTo(min) > 0 && val.CompareTo(max) < 0);

        public static IObservable<T> IfOutOfRange<T>(this IObservable<T> src, T min, T max) where T : IComparable<T>
            => src.Where(val => val.CompareTo(min) <= 0 || val.CompareTo(max) >= 0);

        public static IObservable<T> IfOutOfStrictRange<T>(this IObservable<T> src, T min, T max) where T : IComparable<T>
            => src.Where(val => val.CompareTo(min) < 0 || val.CompareTo(max) > 0);

        public static IObservable<T> Clip<T>(this IObservable<T> src, T min, T max) where T : IComparable<T>
            => src.Select(val =>
            {
                if (val.CompareTo(min) < 0)
                {
                    return min;
                }
                else if (val.CompareTo(max) > 0)
                {
                    return max;
                }
                else
                {
                    return val;
                }
            });

        #endregion

        #region RxVar Instantation from other types (including subscription)

        public static RxVar<T> ToRxVar<T>(this T v) 
            => new RxVar<T>(v);

        /// <summary>
        /// Create a new instance of RxVar from another one
        /// and connect the new to the src
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="src"> a src RxVar</param>
        /// <returns> A new connected RxVar </returns>
        public static RxVar<T> ToRxVarAndSubscribe<T>(this IObservable<T> source) 
            => new RxVar<T>(source);

        #endregion

        #region RxProperty Instantation from other types (including subscription)

        public static RxProperty<T> ToRxProperty<T>(this T v)
            => new RxProperty<T>(v) ;

        /// <summary>
        /// Create a new instance of RxProperty from another RxVar
        /// and connect the new to the src
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="src"> a src RxVar</param>
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
                CTS.Dispose();
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
