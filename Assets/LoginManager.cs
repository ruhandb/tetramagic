using Firebase.Auth;
using Google;
using System;
using System.Collections;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour
{
    

    public TMP_InputField Email;
    public TMP_InputField Password;

    public UnityEventException OnLoginFailed = new UnityEventException();
    public UnityEventFirebaseUser OnLoginSucceeded = new UnityEventFirebaseUser();

    private Coroutine _loginCoroutine;
    private FirebaseManager fbManager;

    private void Awake()
    {
        fbManager = FindObjectOfType<FirebaseManager>();
    }

    public void Logar()
    {        
        if(_loginCoroutine == null)
        {
            _loginCoroutine = StartCoroutine(LoginCoroutune(fbManager.Auth.SignInWithEmailAndPasswordAsync(Email.text, Password.text)));
            UpdateButtonInteractable();
        }
    }

    
    private void UpdateButtonInteractable()
    {
        //throw new NotImplementedException();
    }

    private IEnumerator LoginCoroutune(Task<FirebaseUser> loginTask)
    {
        yield return new WaitUntil(() => loginTask.IsCompleted);

        if(loginTask.Exception != null)
        {
            Debug.LogWarning($"Login Fail: {loginTask.Exception}");
            OnLoginFailed.Invoke(loginTask.Exception);
        }
        else
        {
            Debug.LogWarning($"Login Success: {loginTask.Result.Email}");
            OnLoginSucceeded.Invoke(loginTask.Result);
        }

        _loginCoroutine = null;
        UpdateButtonInteractable();
    }
}

public class UnityEventException: UnityEvent<Exception>{}
public class UnityEventFirebaseUser : UnityEvent<FirebaseUser> { }