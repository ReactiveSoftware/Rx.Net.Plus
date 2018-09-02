using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.Serialization;

namespace Rx.Net.Plus
{
    internal static class ConversionHelper
    {
        public static T ConvertTo<T>(this object value)
        {
            // Get the type that was made nullable.
            var valueType = Nullable.GetUnderlyingType(typeof(T));

            if (valueType != null)
            {
                // Nullable type.
                if (value == null)
                {
                    return default(T);
                }
                else
                {
                    return (T) Convert.ChangeType(value, valueType);
                }
            }
            else
            {
                // Not nullable.
                return (T) Convert.ChangeType(value, typeof(T));
            }
        }
    }


    /// <summary>
    /// RxVar is basically intended to add Reactive capabilities to
    /// basic types like int, bool...
    /// It supports two main functionalities:
    ///  Observing: It is capable to observe other sources
    ///     RxVar<bool> isConnected = false;
    ///     Observable<bool> source = ....;
    ///     source.Subscribe (isConnected);
    ///  Emitting data to observers:
    ///     RxVar<bool> isConnected = false;
    ///     isConnected.Subscribe (val => Console.WriteLine ($" Value is {val} "));     // Outputs false to screen
    ///     isConnected.Value = true;            // Outputs true to screen
    ///  </summary>
    /// <example>
    /// RxVar<bool> isConnected = false;
    /// </example>
    /// <typeparam name="T"></typeparam>
    ///
#pragma warning disable CS0660, CS0661

    [Serializable]
    public class RxVar<T> : DisposableBaseClass, IRxVar<T>
    {
        #region Fields

        private readonly BehaviorSubject<T> _subject;
        private IObservable<T> _observable;
        private IComparer<T> _comparer = Comparer<T>.Default;

        #endregion

        #region Constructors

        public RxVar() : this(default(T))
        {
        }

        public RxVar(object value) : this(value.ConvertTo<T>())
        {
        }

        /// <summary>
        /// Create a new instance of RxVar and set its initial value of a specific type
        /// </summary>
        /// <param name="v"> value </param>
        public RxVar(T v)
        {
            _subject = new BehaviorSubject<T>(v);
            _observable = _subject.Synchronize();
            IsDistinctMode = true;  
        }

        /// <inheritdoc />
        /// <summary>
        /// Create a new instance of RxVar and connect it to another source
        /// </summary>
        /// <param name="source"> RxVar source </param>
        public RxVar(IObservable<T> source) : this(default (T))
        {
            source.Subscribe(this, CancellationToken);
        }

        #endregion

        #region Boolean 'Operators'

        // This pseudo operators are nice for syntax like that
        // if (isVisible.Not) ...
        //
        public bool True => Value.Equals(true);
        public bool False => Value.Equals(false);
        public bool Not => Value.Equals(false);

        #endregion

        #region Casting operator

        public static implicit operator T(RxVar<T> v) // implicit digit to byte conversion operator
        {
            return v._subject.Value;
        }

        #endregion

        #region IRxVar Interface

        public bool IsDistinctMode { get; set; }

        public T Value
        {
            get => IsDisposed 
                    ?
                        default(T)
                    :
                        _subject.Value;

            set => OnNext(value);
        }

        public virtual T Set(T v) => Value = v;

        public void ListenTo(IObservable<T> observable)
        {
            observable.Subscribe(this, CancellationToken);
        }
        public void RedirectTo (IObserver<T> observer)
            => _observable.Subscribe (observer, CancellationToken);

        public void Notify(IObserver<T> observer)
            => _observable.Subscribe(observer, CancellationToken);

        public void Notify (Action<T> onNext)
            => _observable.Subscribe(onNext, CancellationToken);

        #endregion

        #region Equatable

        private static bool EqualsInternal(RxVar<T> left, RxVar<T> right)
        {
            return left.Value.Equals(right.Value);
        }

        public virtual bool Equals(RxVar<T> other)
        {
            return Equals((object) other);
        }

        public virtual bool Equals(T other)
        {
            return this.Value.Equals(other);
        }

        public override bool Equals(object obj)
        {
            if (obj is IRxVar<T>)
            {
                return EqualsInternal(this, (RxVar<T>) obj);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return (this.Value.GetHashCode() * 397 ^ this.IsDisposed.GetHashCode());
        }

        public static bool operator ==(RxVar<T> left, RxVar<T> right)
        {
            return ReferenceEquals(left, null)
                ? 
                    ReferenceEquals(right, null)
                : 
                    left.Equals(right);
        }

        public static bool operator !=(RxVar<T> left, RxVar<T> right)
        {
            return !(left == right);
        }


        #endregion

        #region Comparison operators

        public static bool operator ==(RxVar<T> left, T right)
        {
            return ReferenceEquals(left, null)
                ?
                    ReferenceEquals(right, null)
                :
                    left.Value.Equals (right);
        }

        public static bool operator !=(RxVar<T> left, T right)
        {
            return ! (left == right);
        }

        public static bool operator >(RxVar<T> left, T right)
        {

            return left.CompareTo(right) > 0;
        }

        public static bool operator >=(RxVar<T> left, T right)
        {
            return left.CompareTo(right) >= 0;
        }

        public static bool operator <(RxVar<T> left, T right)
        {

            return left.CompareTo(right) < 0;
        }
        
        public static bool operator <=(RxVar<T> left, T right)
        {

            return left.CompareTo(right) <= 0;
        }

        #endregion

        #region Object class

        public override string ToString()
        {
            return Value.ToString();
        }

        #endregion

        #region IObservable Interface

        public IDisposable Subscribe(IObserver<T> observer)
        {
            return _observable.Subscribe(observer);
        }

        #endregion

        #region IObserver interface

        public virtual void OnNext(T value)
        {
            bool publishValue = true;

            if (IsDistinctMode)
            {
                if (value.Equals(Value))
                {
                    publishValue = false;
                }
            }

            if (publishValue)
            {
                _subject.OnNext(value);
            }
        }

        public void OnError(Exception error)
        {
            _subject.OnError(error);
        }

        public void OnCompleted()
        {
            _subject.OnCompleted();
        }

        #endregion

        #region Comparable Interface

        public void SetComparer(IComparer<T> comparer)
        {
            _comparer = comparer;
        }

        public int CompareTo(T other)
        {
            return _comparer.Compare (Value, other);
        }

        #endregion
        
        #region IConvertible

        public TypeCode GetTypeCode()
        {
            return TypeCode.Object;
        }

        bool IConvertible.ToBoolean(IFormatProvider provider)
        {
            return Convert.ToBoolean(Value);
        }

        byte IConvertible.ToByte(IFormatProvider provider)
        {
            return Convert.ToByte(Value);
        }

        char IConvertible.ToChar(IFormatProvider provider)
        {
            return Convert.ToChar(Value);
        }

        DateTime IConvertible.ToDateTime(IFormatProvider provider)
        {
            return Convert.ToDateTime(Value);
        }

        decimal IConvertible.ToDecimal(IFormatProvider provider)
        {
            return Convert.ToDecimal(Value);
        }

        double IConvertible.ToDouble(IFormatProvider provider)
        {
            return Convert.ToDouble(Value);
        }

        short IConvertible.ToInt16(IFormatProvider provider)
        {
            return Convert.ToInt16(Value);
        }

        int IConvertible.ToInt32(IFormatProvider provider)
        {
            return Convert.ToInt32(Value);
        }

        long IConvertible.ToInt64(IFormatProvider provider)
        {
            return Convert.ToInt64(Value);
        }

        sbyte IConvertible.ToSByte(IFormatProvider provider)
        {
            return Convert.ToSByte(Value);
        }

        float IConvertible.ToSingle(IFormatProvider provider)
        {
            return Convert.ToSingle(Value);
        }

        string IConvertible.ToString(IFormatProvider provider)
        {
            return Value.ToString();
        }

        object IConvertible.ToType(Type conversionType, IFormatProvider provider)
        {
            return Convert.ChangeType(Value, conversionType);
        }

        ushort IConvertible.ToUInt16(IFormatProvider provider)
        {
            return Convert.ToUInt16(Value);
        }

        uint IConvertible.ToUInt32(IFormatProvider provider)
        {
            return Convert.ToUInt32(Value);
        }

        ulong IConvertible.ToUInt64(IFormatProvider provider)
        {
            return Convert.ToUInt64(Value);
        }
        #endregion

        #region IDisposable Support

        protected override void OnDisposing(bool isDisposing)
        {
            base.OnDisposing(isDisposing);
            _subject.Dispose();
        }
        
        #endregion

        #region ISerializable

        public RxVar (SerializationInfo info, StreamingContext context) : this(default(T))
        {
            IsDistinctMode = (bool) info.GetValue ("IsDistinctMode", typeof(bool));
            Value = (T) info.GetValue ("Value", typeof(T));
        }

        public void GetObjectData (SerializationInfo info, StreamingContext context)
        {
            info.AddValue ("IsDistinctMode", IsDistinctMode);
            info.AddValue ("Value", Value);
        }

        #endregion
    }
}
