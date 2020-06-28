using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class AuthManager : MonoBehaviour
{
    public UnityEventString LoadScene;
    public static AuthManager instance;
    private FirebaseManager fbManager;

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

        fbManager = FindObjectOfType<FirebaseManager>();

        fbManager.Auth.StateChanged += Auth_StateChanged;
    }

    private void Auth_StateChanged(object sender, System.EventArgs e)
    {
        if(fbManager.Auth.CurrentUser != null)
        {
            SceneManager.LoadScene("Lobby");
        }
        else
        {
            SceneManager.LoadScene("Login");
        }
    }
}

[Serializable]
public class UnityEventString: UnityEvent<string> { }
