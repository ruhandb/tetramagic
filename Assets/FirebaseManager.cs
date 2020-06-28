using Firebase;
using Firebase.Auth;
using Firebase.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FirebaseManager : MonoBehaviour
{
    public Dictionary<string, object> PeristentData = new Dictionary<string, object>();

    public UnityEvent OnFirebaseInitialized;

    public static FirebaseManager _instance;
    public static FirebaseManager Instance
    {
        get
        {
            return _instance;
        }
    }

    private FirebaseAuth _auth;
    public FirebaseAuth Auth
    {
        get
        {
            if(_auth == null)
            {
                _auth = FirebaseAuth.GetAuth(App);
            }
            return _auth;
        }
    }

    private FirebaseDatabase _db;
    public FirebaseDatabase DB
    {
        get
        {
            if (_db == null)
            {
                _db = FirebaseDatabase.GetInstance(App);
            }
            return _db;
        }
    }

    private FirebaseApp _app;
    public FirebaseApp App
    {
        get
        {
            if (_app == null)
            {
                _app = GetAppSynchronous();
            }
            return _app;
        }
    }

    private FirebaseApp GetAppSynchronous()
    {
        return FirebaseApp.DefaultInstance;
    }

    async void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            var dr = await FirebaseApp.CheckAndFixDependenciesAsync();
            if (dr == DependencyStatus.Available)
            {
                _app = FirebaseApp.DefaultInstance;
                //OnFirebaseInitialized.Invoke();
                Debug.Log("Firebase Initialized!");
            }
            else
            {
                Debug.LogError($"Failed to initialize: {dr}");
            }
        }
    }

    private void OnDestroy()
    {
        _auth = null;
        _app = null;
        _db = null;
        if(_instance == this)
        {
            _instance = null;
        }
    }
}
