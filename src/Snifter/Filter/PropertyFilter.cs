using System;

namespace Snifter.Filter
{
    public class PropertyFilter<T>
    {
        public Func<T, dynamic> Property { get; set; }
        public dynamic Value { get; set; }

        public PropertyFilter(Func<T, dynamic> property, dynamic value)
        {
            this.Property = property;
            this.Value = value;
        }

        public bool IsMatch(T obj)
        {
            return this.Property(obj).Equals(this.Value);
        }
    }
}
