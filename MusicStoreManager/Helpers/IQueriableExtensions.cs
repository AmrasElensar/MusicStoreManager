using MusicStoreManager.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;

namespace MusicStoreManager.Helpers
{
    public static class IQueryableExtensions
    {
        public static IQueryable<T> ApplySort<T>(this IQueryable<T> source, string orderBy,
            Dictionary<string, PropertyMappingValue> mappingDictionary)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (mappingDictionary == null)
            {
                throw new ArgumentNullException("mappingDictionary");
            }

            if (string.IsNullOrWhiteSpace(orderBy))
            {
                return source;
            }

            //order string is separated by"," so we use split
            var orderBySplitted = orderBy.Split(',');

            //apply orderby clauses in reverse, if not the IQeuriable will be ordered the wrong way
            foreach (var orderByClause in orderBySplitted.Reverse())
            {
                // remove leading/trailing spaces
                var orderByClauseTrimmed = orderByClause.Trim();

                // check if sort option is 'desc' otherwise sort 'asc'
                var orderDescending = orderByClauseTrimmed.EndsWith(" desc");

                // remove sorting direction from orderByClause, so we can get the property to look for in de mapping.
                var indexOfFirstSpace = orderByClauseTrimmed.IndexOf(" ");
                var propertyName = indexOfFirstSpace == -1 ?
                    orderByClauseTrimmed : orderByClauseTrimmed.Remove(indexOfFirstSpace);

                // check if property mapping exists
                if(!mappingDictionary.ContainsKey(propertyName))
                {
                    throw new ArgumentException($"Key mapping for {propertyName} is missing.");
                }

                // get the mapping
                var propertyMappingValue = mappingDictionary[propertyName];

                if (propertyMappingValue == null)
                {
                    throw new ArgumentNullException("propertyMappingValue");
                }

                //iterate through properties in reverse so sorting is applied in correct order
                foreach (var destinationProperty in propertyMappingValue.DestinationProperties.Reverse())
                {
                    //revert if needed (eg. for age, not used here but tried to be as generic as possible)
                    if (propertyMappingValue.Revert)
                    {
                        orderDescending = !orderDescending;
                    }
                    source = source.OrderBy(destinationProperty + (orderDescending ? " descending" : " ascending"));
                }
            }
            return source;
        }
    }
}
