using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System;
using System.Dynamic;

public static class ObjectExtensions
{
    public static T ToObject<T>(this IDictionary<string, object> dictionary) where T : class, new()
    {
        return DictionaryToObject<T>(dictionary) as T;
    }

    public static dynamic DictionaryToObject<T>(IDictionary<string, object> dictionary) where T : class, new()
    {
        var expandoObj = new ExpandoObject();
        var expandoObjCollection = (ICollection<KeyValuePair<string, object>>)expandoObj;

        foreach (var keyValuePair in dictionary)
        {
            Debug.Log(GetProp((new T()).GetType(), keyValuePair.Key));
            Debug.Log(keyValuePair.Value);
            expandoObjCollection.Add(new KeyValuePair<string, object>(GetProp((new T()).GetType(), keyValuePair.Key), keyValuePair.Value));
        }
        dynamic eoDynamic = expandoObj;
        return eoDynamic;
    }

    private static String GetProp(Type type, string key)
    {
        //foreach (var prop in type.GetProperties())
        //{
        //    foreach (var att in prop.GetCustomAttributes<FirebaseKeyAttribute>(false))
        //    {
        //        if(att.Name == key)
        //        {
        //            return prop.Name;
        //        }
        //    }            
        //}
        //return null;
        //iterate the properties
        return (from property in type.GetProperties()
                    // iterate it's attributes
                from attrib in property.GetCustomAttributes(typeof(FirebaseKeyAttribute), false).Cast<FirebaseKeyAttribute>()
                    // filter on the name
                where attrib.Name == key
                // select the propertyInfo
                select property).FirstOrDefault().Name;
    }

    public static IDictionary<string, object> AsDictionary(this object source, BindingFlags bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance)
    {
        return source.GetType().GetProperties(bindingAttr).ToDictionary
        (
            propInfo => propInfo.GetCustomAttributes(typeof(FirebaseKeyAttribute), false).Cast<FirebaseKeyAttribute>().FirstOrDefault().Name,
            propInfo => propInfo.GetValue(source, null)
        );

    }
}

[AttributeUsage(AttributeTargets.All)]
public class FirebaseKeyAttribute : Attribute
{
    /// <summary>
    /// This constructor takes name of attribute
    /// </summary>
    /// <param name="name"></param>
    public FirebaseKeyAttribute(string name)
    {
        Name = name;
    }

    public virtual string Name { get; }
}
