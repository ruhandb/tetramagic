using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public CardConfig cfg;
    public Image TopValue;
    public Image BottomValue;
    public Image LeftValue;
    public Image RightValue;


    public Card Card { get; set; }
    public bool isPlaced = false;
    public RectTransform hand;
    public Player Owner;
    public Vector3 initialPosition;
    public GameManager gm;
    private ImageManager imageManager;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Image moldure;
    private Image image;

    private void Awake()
    {
        gm = FindObjectOfType<GameManager>();
        imageManager = FindObjectOfType<ImageManager>();
        moldure = gameObject.transform.Find("moldure").GetComponent<Image>();
        image = gameObject.transform.Find("image").GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        initialPosition = new Vector3(rectTransform.localPosition.x, rectTransform.localPosition.y, rectTransform.localPosition.z);

        rectTransform.localPosition = hand.localPosition;
    }

    public void UpdateValues()
    {
        TopValue.gameObject.transform.Find("TopValue").GetComponent<Text>().text = GetValueText(Card.TopValue);
        SetBackSprite(TopValue, Card.TopValue, Direction.Top);

        BottomValue.gameObject.transform.Find("BottomValue").GetComponent<Text>().text = GetValueText(Card.BottomValue);
        SetBackSprite(BottomValue, Card.BottomValue, Direction.Bottom);

        LeftValue.gameObject.transform.Find("LeftValue").GetComponent<Text>().text = GetValueText(Card.LeftValue);
        SetBackSprite(LeftValue, Card.LeftValue, Direction.Left);

        RightValue.gameObject.transform.Find("RightValue").GetComponent<Text>().text = GetValueText(Card.RightValue);
        SetBackSprite(RightValue, Card.RightValue, Direction.Right);

        image.sprite = GetSprite();

        moldure.color = Owner == Player.P1 ? cfg.P1Color : cfg.P2Color;
    }

    private Sprite GetSprite()
    {
        Debug.Log($"GetSprite() {Card.Kind}");
        switch (Card.Kind)
        {
            case Kind.Shield:
                return imageManager.Get("Kind.Shield");
            case Kind.Sword:
                return imageManager.Get("Kind.Sword");
            case Kind.Bow:
                return imageManager.Get("Kind.Bow");
            case Kind.Spear:
                return imageManager.Get("Kind.Spear");
            case Kind.Fire:
                return imageManager.Get("Kind.Fire");
            case Kind.Water:
                return imageManager.Get("Kind.Water");
            case Kind.Nature:
                return imageManager.Get("Kind.Nature");
            case Kind.Wind:
                return imageManager.Get("Kind.Wind");
            default:
                return null;
        }
    }

    private void SetBackSprite(Image image,  int value, Direction dir)
    {
        if (value == -1)
        {
            image.sprite = imageManager.Get("backShield");
            image.gameObject.LeanRotateZ(0, 0);
        }
        else if (value == 0)
        {
            image.sprite = imageManager.Get("backWeakness");
            switch (dir)
            {
                case Direction.Top:
                    image.gameObject.LeanRotateZ(270, 0);
                    break;
                case Direction.Bottom:
                    image.gameObject.LeanRotateZ(90, 0);
                    break;
                case Direction.Left:
                    image.gameObject.LeanRotateZ(0, 0);
                    break;
                case Direction.Right:
                    image.gameObject.LeanRotateZ(180, 0);
                    break;
                default:
                    break;
            }
        }
        else
        {
            image.sprite = imageManager.Get("backValue");
            image.gameObject.LeanRotateZ(0, 0);
        }
    }

    private string GetValueText(int value)
    {
        return value > 0 ? value.ToString() : String.Empty;
    }    

    private bool CanDrag()
    {
        return !isPlaced && Owner == Player.P1 && !gm.IsMoveBlocked;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (CanDrag())
        {
            gm.audioManager.Play("TakeCard");            
            canvasGroup.blocksRaycasts = false;
            transform.SetAsLastSibling();
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (CanDrag())
        {
            rectTransform.position = Input.mousePosition;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        
        if (CanDrag())
        {
            canvasGroup.blocksRaycasts = true;
            rectTransform.localPosition = initialPosition;
        }
    }

    public void ChangeColor()
    {
        moldure.color = Owner == Player.P1 ? cfg.P1Color : cfg.P2Color;
    }

    public void Placed()
    {
        isPlaced = true;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (CanDrag())
        {
            LeanTween.scale(gameObject, new Vector3(1.2f, 1.2f), 0.05f);
            gm.ShowAdvantage(Card.Kind);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (CanDrag())
        {
            LeanTween.scale(gameObject, new Vector3(1.0f, 1.0f), 0.05f);
            gm.HideAdvantage(Card.Kind);
        }
    }

    public void ShowAdvantage()
    {
        ChangeValues(1, Color.red);
    }

    public void ShowDisadvantage()
    {
        ChangeValues(-1, Color.green);
    }

    public void ResetValues()
    {
        ChangeValues(0, Color.white);
    }

    private void ChangeValues(int advantage, Color color)
    {
        TopValue.gameObject.transform.Find("TopValue").GetComponent<Text>().text = (Card.TopValue + advantage).ToString();
        BottomValue.gameObject.transform.Find("BottomValue").GetComponent<Text>().text = (Card.BottomValue + advantage).ToString();
        LeftValue.gameObject.transform.Find("LeftValue").GetComponent<Text>().text = (Card.LeftValue + advantage).ToString();
        RightValue.gameObject.transform.Find("RightValue").GetComponent<Text>().text = (Card.RightValue + advantage).ToString();

        TopValue.gameObject.transform.Find("TopValue").GetComponent<Text>().color = color;
        BottomValue.gameObject.transform.Find("BottomValue").GetComponent<Text>().color = color;
        LeftValue.gameObject.transform.Find("LeftValue").GetComponent<Text>().color = color;
        RightValue.gameObject.transform.Find("RightValue").GetComponent<Text>().color = color;
    }
}

public enum Player
{
    P1,
    P2
}
