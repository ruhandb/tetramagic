using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{

    public Animator animator;
    public float SecondsToWait = 1f;
    
    public void LoadScene(string scene)
    {
        StartCoroutine(LoadSceneAfterTime(scene));        
    }

    private IEnumerator LoadSceneAfterTime(string scene)
    {
        animator.SetTrigger("Start");
        yield return new WaitForSeconds(SecondsToWait);
        StartCoroutine(Loading(scene));
    }

    private IEnumerator Loading(string scene)
    {
        var ao = SceneManager.LoadSceneAsync(scene);
        while (!ao.isDone)
        {
            yield return new WaitForEndOfFrame();
        }
    }
}
