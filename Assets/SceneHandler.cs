using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneHandler : MonoBehaviour
{
    public GameObject overlay;
    public Image bar;

    [SerializeField]
    public UnityEventFloat ProgressChange;
    public UnityEvent StartLoading;

    public void Load(string scene)
    {
        overlay.SetActive(true);
        StartLoading?.Invoke();
        StartCoroutine(Loading(scene));
    }

    IEnumerator Loading(string scene)
    {
        var ao = SceneManager.LoadSceneAsync(scene);
        while (!ao.isDone)
        {
            bar.fillAmount = ao.progress;
            ProgressChange?.Invoke(ao.progress);
            yield return new WaitForEndOfFrame();
        }
    }
}

[Serializable]
public class UnityEventFloat: UnityEvent<float> { }
