using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using System.Dynamic;

namespace MusicStoreManager.Helpers
{
    public static class IEnumerableExtensions
    {

        public static IEnumerable<ExpandoObject> ShapeData<TSource>(
            this IEnumerable<TSource> source,
            string fields)
        {
            if(source == null)
            {
                throw new ArgumentNullException("source");
            }

            //make a list to hold the ExpandoObjects
            var expandoObjectList = new List<ExpandoObject>();

            //make a list with PropertyInfo objects on TSource

            var propertyInfoList = new List<PropertyInfo>();

            if (string.IsNullOrWhiteSpace(fields))
            {
                //all public properties should be in the ExpandoObject
                var propertyInfos = typeof(TSource)
                    .GetProperties(BindingFlags.Public | BindingFlags.Instance);

                propertyInfoList.AddRange(propertyInfos);   
            }
            else
            {
                //only the public properties that match the fields should be in the 
                //the fields are seperated by "," so we split them.
                var fieldsAfterSplit = fields.Split(',');

                foreach (var field in fieldsAfterSplit)
                {
                    //Trim to remove leading/trailing spaces
                    var propName = field.Trim();

                    //use reflection to get the property on the source object
                    //we need to include public and instance because specifying a binding flag overwrites the already existing binding flags.
                    var propertyInfo = typeof(TSource)
                        .GetProperty(propName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                    if (propertyInfo == null)
                    {
                        throw new Exception($"Property {propName} was not found on {typeof(TSource)}");
                    }

                    propertyInfoList.Add(propertyInfo);
                }
            }

            // run through the source objects
            foreach( TSource sourceObject in source)
            {
                //Create an ExpandoObject that will hold the selected props & values
                var dataShapedObject = new ExpandoObject();

                //get the value of each prop we have to return
                foreach (var propertyInfo in propertyInfoList)
                {
                    //GetValue returns the value of the property
                    var propValue = propertyInfo.GetValue(sourceObject);

                    //Add the field to the ExpandoObject
                    ((IDictionary<string, object>)dataShapedObject).Add(propertyInfo.Name, propValue);
                }

                //add ExpandoObject to the list
                expandoObjectList.Add(dataShapedObject);
            }

            return expandoObjectList;
        }

    }
}
