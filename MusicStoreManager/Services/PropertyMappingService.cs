using MusicStoreManager.Models;
using MusicStoreManager.ViewModels;
using MusicStoreManager.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MusicStoreManager.Entities;

namespace MusicStoreManager.Services
{
    public class PropertyMappingService : IPropertyMappingService
    {
        private Dictionary<string, PropertyMappingValue> _albumPropertyMapping =
            new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
            {
                {"Artist", new PropertyMappingValue(new List<string>() {"Artist.Name"}) },
                {"Title", new PropertyMappingValue(new List<string>() {"Title"}) },
                {"Genre", new PropertyMappingValue(new List<string>() {"Genre.Name" }) }

            };

        private Dictionary<string, PropertyMappingValue> _artistPropertyMapping =
            new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
            {
                {"Artist", new PropertyMappingValue(new List<string>() {"Name"}) },

            };

        private IList<IPropertyMapping> propertyMappings = new List<IPropertyMapping>();

        public PropertyMappingService()
        {
            propertyMappings.Add(new PropertyMapping<Album, AlbumDto>(_albumPropertyMapping));
            propertyMappings.Add(new PropertyMapping<Artist, ArtistDto>(_artistPropertyMapping));
        }

        public Dictionary<string, PropertyMappingValue> GetPropertyMapping<TSource, TDestination>()
        {
            // get matching map
            var matchingMap = propertyMappings.OfType<PropertyMapping<TSource, TDestination>>();
            
            if (matchingMap.Count() == 1)
            {
                return matchingMap.First()._mappingDictionary;
            }

            throw new Exception($"Cannot find exact property mapping for <{typeof(TSource)},{typeof(TDestination)}> ");
        }

        public bool ValidMappingExistsFor<TSource, TDestination>(string fields)
        {
            var propertyMapping = GetPropertyMapping<TSource, TDestination>();

            if(string.IsNullOrWhiteSpace(fields))
            {
                return true;
            }

            var fieldsAfterSplit = fields.Split(',');

            foreach (var field in fieldsAfterSplit)
            {
                var trimmedField = field.Trim();

                var indexOfFirstSpace = trimmedField.IndexOf(" ");
                var propertyName = indexOfFirstSpace == -1 ?
                    trimmedField : trimmedField.Remove(indexOfFirstSpace);

                if (!propertyMapping.ContainsKey(propertyName))
                {
                    return false;
                }
            }
            return true;
        }

    }
}
