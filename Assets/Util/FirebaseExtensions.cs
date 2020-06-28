using UnityEngine;
using System.Collections;
using Firebase.Database;

public static class FirebaseExtensions 
{
    public static ValueWrapper<T> Value<T>(this DataSnapshot dataSnapshot)
    {
        ValueWrapper<T> wrapper = new ValueWrapper<T>
        {
            Value = JsonUtility.FromJson<T>(dataSnapshot.GetRawJsonValue()),
            Key = dataSnapshot.Key
        };
        return wrapper;
    }

    public static string Json(this IFirebaseData obj)
    {
        return JsonUtility.ToJson(obj);
    }
}
