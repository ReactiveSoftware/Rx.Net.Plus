using System;

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
}