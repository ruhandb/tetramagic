using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoosterManager : MonoBehaviour
{

    public GameObject TopBooster;
    public GameObject DropPlace;

    public GameObject DropPlace2;
    public GameObject DropPlace3;

    public GameObject Card1;
    public GameObject Card2;
    public GameObject Card3;

    private bool IsOpened = false;

    private AudioManager audioManager;

    private void Awake()
    {
        audioManager = FindObjectOfType<AudioManager>();
    }

    public void OpenBooster()
    {
        if (IsOpened)
            return;

        IsOpened = true;

        Card1.transform.Find("Front").GetComponent<CardController>().Card = CardUtil.Generate(new List<Rarity> { Rarity._1Star });
        Card1.transform.Find("Front").GetComponent<CardController>().UpdateValues();

        Card2.transform.Find("Front").GetComponent<CardController>().Card = CardUtil.Generate(new List<Rarity> { Rarity._1Star });
        Card2.transform.Find("Front").GetComponent<CardController>().UpdateValues();

        Card3.transform.Find("Front").GetComponent<CardController>().Card = CardUtil.Generate(new List<Rarity> { Rarity._1Star });
        Card3.transform.Find("Front").GetComponent<CardController>().UpdateValues();

        var seq = LeanTween.sequence();
        var pos = new Vector2(TopBooster.transform.position.x + 220, TopBooster.transform.position.y + 30);
        seq.append(()=> {
            LeanTween.move(TopBooster, pos, 0.3f).setEase(LeanTweenType.easeInSine);
            audioManager.Play("RipPaper");
        });
        seq.append(0.6f);

        seq.append(() =>
        {
            //Card3.transform.SetAsLastSibling();
            var pos3 = new Vector2(DropPlace.transform.position.x, DropPlace.transform.position.y + 6);
            LeanTween.move(Card3, pos3, 0.3f).setEase(LeanTweenType.easeOutQuart);
            audioManager.Play("DrawCard");
        });
        seq.append(0.4f);
        seq.append(() =>
        {
            //Card2.transform.SetAsLastSibling();
            var pos2 = new Vector2(DropPlace.transform.position.x, DropPlace.transform.position.y + 3);
            LeanTween.move(Card2, pos2, 0.3f).setEase(LeanTweenType.easeOutQuart); ;
            audioManager.Play("DrawCard");
        });
        seq.append(0.4f);
        seq.append(() =>
        {
            //Card1.transform.SetAsLastSibling();
            var pos1 = new Vector2(DropPlace.transform.position.x, DropPlace.transform.position.y);
            LeanTween.move(Card1, pos1, 0.3f).setEase(LeanTweenType.easeOutQuart); ;
            audioManager.Play("DrawCard");
        });
        seq.append(0.4f);

        seq.append(() =>
        {
            //Card3.transform.SetAsLastSibling();
            var pos3 = new Vector2(DropPlace3.transform.position.x, DropPlace3.transform.position.y);
            LeanTween.move(Card1, pos3, 0.3f);
            LeanTween.scale(Card1, Card1.transform.localScale * 1.2f, 0.3f);
            audioManager.Play("TakeCard");
            
        });
        seq.append(0.4f);
        seq.append(() =>
        {
            //Card2.transform.SetAsLastSibling();
            var pos2 = new Vector2(DropPlace2.transform.position.x, DropPlace2.transform.position.y);
            LeanTween.move(Card2, pos2, 0.3f);
            LeanTween.scale(Card2, Card2.transform.localScale * 1.2f, 0.3f);
            audioManager.Play("TakeCard");
        });
        seq.append(0.4f);
        seq.append(() =>
        {
            //Card1.transform.SetAsLastSibling();
            var pos2 = new Vector2(DropPlace.transform.position.x, DropPlace2.transform.position.y);
            LeanTween.move(Card3, pos2, 0.3f);
            LeanTween.scale(Card3, Card3.transform.localScale * 1.2f, 0.3f);
            audioManager.Play("TakeCard");
        });
        seq.append(0.4f);

        seq.append(() => {
            Card1.GetComponent<CardFlipAnimation>().ShowUp(0.5f);
            audioManager.Play("DrawCard");
        });
        seq.append(0.6f);
        seq.append(() => {
            Card3.GetComponent<CardFlipAnimation>().ShowUp(0.5f);
            audioManager.Play("DrawCard");
        });
        seq.append(0.6f);
        seq.append(() => {
            Card2.GetComponent<CardFlipAnimation>().ShowUp(0.5f);
            audioManager.Play("DrawCard");
        });
        seq.append(0.6f);
    }
}
