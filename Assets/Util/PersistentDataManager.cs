using UnityEngine;
using System.Collections.Generic;

public class PersistentDataManager : MonoBehaviour
{
    public static PersistentDataManager instance;
    private Dictionary<string, object> _peristentData = new Dictionary<string, object>();

    public object this[string key]
    {
        get
        {
            return _peristentData[key];
        }
        set
        {
            _peristentData[key] = value;
        }
    }

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}
