using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Runtime.Serialization;

namespace Rx.Net.Plus
{
#pragma warning disable CS0660, CS0661


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
    ///  RxVar are thread safe (based on BehaviorSubject)
    ///  </summary>
    /// <example>
    /// RxVar<bool> isConnected = false;
    /// </example>
    /// <typeparam name="T"></typeparam>
    ///
#pragma warning disable CS0660, CS0661

    [Serializable]
    public class RxVar<T> :
        DisposableBaseClass, 
        IRxVar<T>,
        IReadOnlyRxVar<T>
    {
        #region Fields

        private BehaviorSubject<T> _subject;
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
            IsDistinctMode = true;
        }

        /// <inheritdoc />
        /// <summary>
        /// Create a new instance of RxVar and connect it to another source
        /// </summary>
        /// <param name="source"> RxVar source </param>
        public RxVar(IObservable<T> source) : this(default(T))
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
            get
            {
                T retValue = default(T);

                if (!IsDisposed)
                {
                    _subject.TryGetValue(out retValue);
                }

                return retValue;
            }

            set => OnNext(value);
        }

        public virtual T Set(T v) => Value = v;

        public void ListenTo(IObservable<T> observable)
        {
            observable.Subscribe(this, CancellationToken);
        }

        #endregion

        #region Equatable

        private static bool EqualsInternal(RxVar<T> left, RxVar<T> right)
        {
            return left.Value.Equals(right.Value);
        }

        public bool Equals(RxVar<T> other)
        {
            return Equals((object) other);
        }

        public bool Equals(T other)
        {
            return Value?.Equals(other) ?? other == null;
        }

        public override bool Equals(object other)
        {
            switch (other)
            {
                case null:
                    return base.Equals(other);
                case IRxVar<T> rxvar:
                    return EqualsInternal(this, rxvar as RxVar<T>);
                case T val:
                    return Value.Equals(val);
                default:
                    return false;
            }
        }

        public override int GetHashCode()
        {
            return (Value.GetHashCode() * 397 ^ IsDisposed.GetHashCode());
        }

        public static bool operator ==(RxVar<T> left, RxVar<T> right)
        {
            return ReferenceEquals(left, null)
                ? ReferenceEquals(right, null)
                : left.Equals(right);
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
                ? ReferenceEquals(right, null)
                : left.Value.Equals(right);
        }

        public static bool operator !=(RxVar<T> left, T right)
        {
            return !(left == right);
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
            return Value?.ToString() ?? "null";
        }

        #endregion

        #region IObservable Interface

        public IDisposable Subscribe(IObserver<T> observer)
        {
            return _subject.Subscribe(observer);
        }

        #endregion

        #region IObserver interface

        public virtual void OnNext(T value)
        {
            bool publishValue = true;

            if (IsDistinctMode)
            {
                if ( Equals(value))
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
            return _comparer.Compare(Value, other);
        }

        #endregion

        #region IAsObject interface

        public object AsObject => (object) Value;

        #endregion

        #region IConvertible

        public TypeCode GetTypeCode()
        {
            return TypeCode.Object;
        }

        public bool ToBoolean(IFormatProvider provider)
        {
            return Convert.ToBoolean(Value);
        }

		public byte ToByte(IFormatProvider provider)
        {
            return Convert.ToByte(Value);
        }

		public char ToChar(IFormatProvider provider)
        {
            return Convert.ToChar(Value);
        }

		public DateTime ToDateTime(IFormatProvider provider)
        {
            return Convert.ToDateTime(Value);
        }

		public decimal ToDecimal(IFormatProvider provider)
        {
            return Convert.ToDecimal(Value);
        }

		public double ToDouble(IFormatProvider provider)
        {
            return Convert.ToDouble(Value);
        }

		public short ToInt16(IFormatProvider provider)
        {
            return Convert.ToInt16(Value);
        }

		public int ToInt32(IFormatProvider provider)
        {
            return Convert.ToInt32(Value);
        }

		public long ToInt64(IFormatProvider provider)
        {
            return Convert.ToInt64(Value);
        }

		public sbyte ToSByte(IFormatProvider provider)
        {
            return Convert.ToSByte(Value);
        }

		public float ToSingle(IFormatProvider provider)
        {
            return Convert.ToSingle(Value);
        }

		public string ToString(IFormatProvider provider)
        {
            return Value?.ToString() ?? String.Empty;
        }

		public object ToType(Type conversionType, IFormatProvider provider)
        {
            return Convert.ChangeType(Value, conversionType);
        }

		public ushort ToUInt16(IFormatProvider provider)
        {
            return Convert.ToUInt16(Value);
        }

		public uint ToUInt32(IFormatProvider provider)
        {
            return Convert.ToUInt32(Value);
        }

		public ulong ToUInt64(IFormatProvider provider)
        {
            return Convert.ToUInt64(Value);
        }

        #endregion

        #region IDisposable Support

        protected override void OnDisposing(bool isDisposing)
        {
            base.OnDisposing(isDisposing);

            if (_subject != null)
            {
                _subject.Dispose();
                _subject = null;
            }
        }

        #endregion

        #region ISerializable

        protected RxVar(SerializationInfo info, StreamingContext context) : this(default(T))
        {
            IsDistinctMode = (bool) info.GetValue("IsDistinctMode", typeof(bool));
            Value = (T) info.GetValue("Value", typeof(T));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("IsDistinctMode", IsDistinctMode);
            info.AddValue("Value", Value);
        }

        #endregion
    }
}