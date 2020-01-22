using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace VideoApi.Domain.Ddd
{
    // This base class comes from Jimmy Bogard, but with support of inheritance
    // http://grabbagoft.blogspot.com/2007/06/generic-value-object-equality.html

    public abstract class ValueObject<T> : IEquatable<T>
      where T : ValueObject<T>
    {
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            var other = obj as T;

            return Equals(other);
        }

        public virtual bool Equals(T other)
        {
            if (other == null)
                return false;

            var t = GetType();
            var otherType = other.GetType();

            if (t != otherType)
                return false;

            var fields = GetFields(this);

            foreach (var field in fields)
            {
                var value1 = field.GetValue(other);
                var value2 = field.GetValue(this);

                if (value1 == null)
                {
                    if (value2 != null)
                        return false;
                }
                else if (!value1.Equals(value2))
                    return false;
            }

            return true;
        }
        
        public override int GetHashCode()
        {
            var fields = GetFields(this);

            var startValue = 17;
            var multiplier = 59;

            return fields
                .Select(field => field.GetValue(this))
                .Where(value => value != null)
                .Aggregate(
                    startValue,
                        (current, value) => current * multiplier + value.GetHashCode());
        }

        private static IEnumerable<FieldInfo> GetFields(object obj)
        {
            var t = obj.GetType();

            var fields = new List<FieldInfo>();

            while (t != typeof(object))
            {
                if (t == null) continue;
                fields.AddRange(t.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public));

                t = t.BaseType;
            }

            return fields;
        }

        public static bool operator ==(ValueObject<T> x, ValueObject<T> y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            if (((object)x == null) || ((object)y == null))
            {
                return false;
            }

            return x.Equals(y);
        }

        public static bool operator !=(ValueObject<T> x, ValueObject<T> y)
        {
            return !(x == y);
        }
    }

}