using System.Collections.Generic;
using System.Linq;

namespace Snifter.Filter
{
    public class Filters<T>
    {
        public FilterOperator FilterOperator { get; set; }
        public IList<PropertyFilter<T>> PropertyFilters { get; set; }

        public Filters(FilterOperator filterFilterOperator)
        {
            this.FilterOperator = filterFilterOperator;
            this.PropertyFilters = new List<PropertyFilter<T>>();
        }

        public bool IsMatch(T obj)
        {
            return this.FilterOperator == FilterOperator.AND 
                ? this.PropertyFilters.All(x => x.IsMatch(obj)) 
                : this.PropertyFilters.Any(x => x.IsMatch(obj));
        }
    }
}
