using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoosterManager : MonoBehaviour
{

    public GameObject TopBooster;

    public GameObject Card1;
    public GameObject Card2;
    public GameObject Card3;

    private AudioManager audioManager;

    private void Awake()
    {
        audioManager = FindObjectOfType<AudioManager>();
    }

    public void OpenBooster()
    {
        var seq = LeanTween.sequence();
        var pos = new Vector2(TopBooster.transform.position.x + 1000, TopBooster.transform.position.y + 100);
        seq.append(LeanTween.move(TopBooster, pos, 2));
        LeanTween.move(TopBooster, pos, 2);
        audioManager.Play("RipPaper ");
    }
}
