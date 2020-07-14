using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardFlipAnimation : MonoBehaviour
{
    public GameObject Front;
    public GameObject Back;

    public void ShowUp(float time)
    {
        ShowUp(time, LeanTween.sequence());
    }

    public void ShowUp(float time, LTSeq sequence)
    {
        sequence.append(Back.LeanRotateY(90, time / 2));//.setEase(LeanTweenType.easeOutCubic));
        sequence.append(Front.LeanRotateY(0, time / 2));//.setEase(LeanTweenType.easeInCubic));
    }
}
