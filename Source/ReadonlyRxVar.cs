using System;

namespace Rx.Net.Plus
{
    /// <summary>
    /// The purpose of this class is to mirror a RxVar
    /// and expose it as a read-only value
    /// It implies that ReadOnlyRxVar cannot be an observer
    /// as its value cannot be changed
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class ReadOnlyRxVar<T> : IReadOnlyRxVar<T>
    {
        #region Fields

        private readonly RxVar<T> _source;

        #endregion

        #region Constructors

        public ReadOnlyRxVar(RxVar<T> source)
        {
            _source = source;
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

        public static implicit operator T(ReadOnlyRxVar<T> v) // implicit digit to byte conversion operator
        {
            return v.Value;
        }

        #endregion

        #region IReadOnlyRxVar Interface

        public bool IsDistinctMode => _source.IsDistinctMode;

        public T Value => _source.Value;

        #endregion

        #region Equatable

        private static bool EqualsInternal(ReadOnlyRxVar<T> left, ReadOnlyRxVar<T> right)
        {
            return left.Value.Equals(right.Value);
        }

        private bool Equals(ReadOnlyRxVar<T> other)
        {
            return Equals((object)other);
        }

        public bool Equals(T other)
        {
            return Value.Equals(other);
        }

        public override bool Equals(object other)
        {
            switch (other)
            {
                case null:
                    return base.Equals(null);
                case IReadOnlyRxVar<T> rxvar:
                    return EqualsInternal(this, rxvar as ReadOnlyRxVar<T>);
                case T val:
                    return Value.Equals(val);
                default:
                    return false;
            }
        }

        public override int GetHashCode()
        {
            return _source.GetHashCode();
        }

        public static bool operator== (ReadOnlyRxVar<T> left, ReadOnlyRxVar<T> right)
        {
            return ReferenceEquals(left, null)
                ? ReferenceEquals(right, null)
                : left.Equals(right);
        }

        public static bool operator!= (ReadOnlyRxVar<T> left, ReadOnlyRxVar<T> right)
        {
            return !(left == right);
        }


        #endregion

        #region Comparison operators

        public static bool operator==(ReadOnlyRxVar<T> left, T right)
        {
            return ReferenceEquals(left, null)
                ? ReferenceEquals(right, null)
                : left.Value.Equals(right);
        }

        public static bool operator !=(ReadOnlyRxVar<T> left, T right)
        {
            return !(left == right);
        }

        public static bool operator >(ReadOnlyRxVar<T> left, T right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator >=(ReadOnlyRxVar<T> left, T right)
        {
            return left.CompareTo(right) >= 0;
        }

        public static bool operator <(ReadOnlyRxVar<T> left, T right)
        {

            return left.CompareTo(right) < 0;
        }

        public static bool operator <=(ReadOnlyRxVar<T> left, T right)
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
            return _source.Subscribe(observer);
        }

        #endregion

        #region Comparable Interface

        public int CompareTo(T other)
        {
            return _source.CompareTo(other);
        }

        #endregion

        #region IAsObject interface

        public object AsObject => (object)Value;

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
    }
}