using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class WaitAndRun : MonoBehaviour
{
    public float TimeToWait = 6f;

    public UnityEvent RunAfterWait;

    void Start()
    {
        StartCoroutine(WaitIntro());
    }

    private IEnumerator WaitIntro()
    {
        yield return new WaitForSeconds(TimeToWait);
        RunAfterWait?.Invoke();
    }
}
