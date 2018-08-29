using System;
using System.ComponentModel;

namespace Rx.Net.Plus
{
    public class RxProperty<T> : RxVar<T>, IRxProperty<T>
    {
        private IPropertyChangedProxy _parent;
        private PropertyChangedEventArgs _eventArgs;

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        public RxProperty() : this(default (T))
        {
        }

        /// <inheritdoc />
        /// <summary>
        /// Create a new instance of RxProperty and set its initial value of a specific type
        /// </summary>
        /// <param name="v"> value </param>
        public RxProperty(T v) : base(v)
        {
        }
        
        /// <inheritdoc />
        /// <summary>
        /// Create a new instance of RxProperty and connect it to another source
        /// </summary>
        /// <param name="source"> RxVar source </param>
        public RxProperty(IObservable<T> source) : base (source)
        {
        }

        // implicit digit to byte conversion operator
        public static implicit operator T(RxProperty<T> v)
        {
            return v.Value; // implicit conversion
        }

        public void BindToView(IPropertyChangedProxy parent, string propertyName)
        {
            _parent = parent;
            _eventArgs = new PropertyChangedEventArgs(propertyName);
        }

        public override void OnNext(T value)
        {
            base.OnNext(value);
            _parent?.NotifyPropertyChanged(_eventArgs);
        }
    }
}
