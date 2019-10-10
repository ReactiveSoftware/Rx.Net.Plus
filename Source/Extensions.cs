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

        #region ReadOnlyRxVar

        /// <summary>
        /// Transform a read-write RxVar to Read-only one
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="v"></param>
        /// <returns></returns>
        public static ReadOnlyRxVar<T> ToReadOnlyRxVar<T>(this RxVar<T> v)
            => new ReadOnlyRxVar<T>(v);

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
}
