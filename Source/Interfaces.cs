using System;
using System.ComponentModel;
using System.Reactive.Subjects;
using System.Runtime.Serialization;
using System.Threading;

namespace Rx.Net.Plus
{
    // Design pattern for a base class.
    public interface IDisposableBaseClass : IDisposable
    {
        CancellationTokenSource CTS { get; }
        CancellationToken CancellationToken { get; }
        bool IsDisposed { get; }
    }

    public interface IRxVar<T> : ISubject<T>, IComparable<T>, IEquatable<T>, IConvertible, IDisposable, ISerializable
    {
        /// <summary>
        /// Distinct mode indicates that only when a distinct value is
        /// set to RxVar it is dispatched to observers
        /// Default value is true => Distinct mode is applied
        /// </summary>
        bool IsDistinctMode { get; set; }
        
        T Value { get; set; }
        T Set(T v);

        // Alias to Subscribe
        void ListenTo(IObservable<T> observable);
    }

    public interface IPropertyChangedProxy
    {
        void NotifyPropertyChanged(PropertyChangedEventArgs evtArgs);
    }

    public interface IPropertyToBind
    {
        void BindToView(IPropertyChangedProxy parent, string propertyName);
    }

    public interface IRxProperty<T> : IRxVar<T>, IPropertyToBind
    {
    }
}
