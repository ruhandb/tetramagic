using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RegisterManager : MonoBehaviour
{
    public TMP_InputField Email;
    public TMP_InputField Password;
    public TMP_InputField Confirm;

    public UnityEventException OnLoginFailed = new UnityEventException();
    public UnityEventFirebaseUser OnLoginSucceeded = new UnityEventFirebaseUser();

    private Coroutine _registerCoroutine;
    private FirebaseManager fbManager;

    private void Awake()
    {
        fbManager = FindObjectOfType<FirebaseManager>();
        Debug.Log("Register Awake");
    }

    public void Register()
    {
        _registerCoroutine = StartCoroutine(RegisterUser(Email.text, Password.text));
        UpdateInteractible();
    }

    private IEnumerator RegisterUser(string email, string password)
    {
        var task = fbManager.Auth.CreateUserWithEmailAndPasswordAsync(email, password);
        yield return new WaitUntil(() => task.IsCompleted);

        if (task.Exception != null)
        {
            Debug.LogWarning($"Register Fail: {task.Exception}");
            OnLoginFailed.Invoke(task.Exception);
        }
        else
        {
            Debug.LogWarning($"Register Success: {task.Result.Email}");
            OnLoginSucceeded.Invoke(task.Result);
        }

        _registerCoroutine = null;
        UpdateInteractible();
    }

    private void UpdateInteractible()
    {
        //throw new NotImplementedException();
    }
}
