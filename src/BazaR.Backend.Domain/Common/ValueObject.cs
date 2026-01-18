using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace BazaR.Backend.Domain.Common
{
    
    public abstract class ValueObject
    {
        protected abstract IEnumerable<object?> GetEqualityComponents();

        public override bool Equals(object? obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;

            var other = (ValueObject)obj;

            return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
        }
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 1;
                foreach (var component in GetEqualityComponents())
                {
                    hash = (hash * 23) ^ (component?.GetHashCode() ?? 0);
                }
                return hash;
            }
        }
        public static bool operator ==(ValueObject? a, ValueObject? b)
        {
            if (a is null && b is null) return true;
            if (a is null || b is null) return false;
            return a.Equals(b);
        }
        public static bool operator !=(ValueObject? a, ValueObject? b) => !(a == b);
        protected static T? GetValue<T>(T? value) => value;
    }
}
