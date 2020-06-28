using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SpotController : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public SpotController TopSpot;
    public SpotController BottonSpot;
    public SpotController LeftSpot;
    public SpotController RightSpot;
    public SpotsConfig cfg;

    private Image marker;
    private GameManager gm;

    public CardController card;
    public GameObject GOcard;

    public bool isBlocked = false;

    private Guid lastMoveId = Guid.NewGuid();
    private ImageManager imageManager;

    private List<Kind> cascadeKinds = new List<Kind> { Kind.Fire, Kind.Nature, Kind.Water, Kind.Wind };

    private void Awake()
    {
        gm = FindObjectOfType<GameManager>();
        imageManager = FindObjectOfType<ImageManager>();

        SetSpots();

        marker = gameObject.transform.Find("marker").GetComponent<Image>();
        marker.color = cfg.NormalColor;
        if (isBlocked)
        {
            marker.sprite = imageManager.Get("blockSlot");
            marker.color = cfg.BlockColor;
        }
    }

    private void SetSpots()
    {
        var neighbors = gm.BoardSpotMap[gameObject.name];
        if (neighbors.t != null)
        {
            TopSpot = gm.gameObject.transform.Find(neighbors.t).GetComponent<SpotController>();
        }
        if (neighbors.b != null)
        {
            BottonSpot = gm.gameObject.transform.Find(neighbors.b).GetComponent<SpotController>();
        }
        if (neighbors.l != null)
        {
            LeftSpot = gm.gameObject.transform.Find(neighbors.l).GetComponent<SpotController>();
        }
        if (neighbors.r != null)
        {
            RightSpot = gm.gameObject.transform.Find(neighbors.r).GetComponent<SpotController>();
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (IsDraggingCard(eventData.pointerDrag) && card == null)
        {
            gm.Sequence = LeanTween.sequence();
            DropCard(eventData.pointerDrag);
            var cardName = eventData.pointerDrag.name;
            var spotName = name;
            gm.Sequence.append(() => {
                gm.PlayerMove(cardName, spotName);
            });
        }
    }

    public void DropCard(GameObject goCard)
    {
        
        GOcard = goCard;
        gm.BlockMove();
        gm.Sequence.append(() =>
        {
        });
        var distance = Vector3.Distance(GOcard.GetComponent<RectTransform>().localPosition, GetComponent<RectTransform>().localPosition);
        LeanTween.moveLocal(GOcard, GetComponent<RectTransform>().localPosition, distance / 1000).setOnComplete(() => {
            gm.audioManager.Play("DropCard");
            LeanTween.scale(GOcard, new Vector3(1.0f, 1.0f), 0.05f);
            marker.color = cfg.NormalColor;
        });
        //gm.Sequence.append(LeanTween.moveLocal(GOcard, GetComponent<RectTransform>().localPosition, distance / 1000));
        gm.Sequence.append(distance / 1000 + 0.1f);
        lastMoveId = Guid.NewGuid();
        gm.Sequence.append(() =>
        {
            //SetCard(goCard.GetComponent<CardController>());
            card = goCard.GetComponent<CardController>();
            card.Placed();
            //gm.IncScore(card.Owner);
        });
        Compare(lastMoveId, goCard.GetComponent<CardController>());
        gm.Sequence.append(() =>
        {
            gm.UnBlockMove();
        });
    }

    public void SetCard(CardController selectedCard)
    {        
        lastMoveId = Guid.NewGuid();
        card = selectedCard;
        card.Placed();
        //Compare(lastMoveId);
    }

    public void BlockSpot()
    {
        isBlocked = true;  
        if(marker != null)
        {
            marker.sprite = imageManager.Get("blockSlot");
            marker.color = cfg.BlockColor;
        }
    }

    public void UnblockSpot()
    {
        isBlocked = false;
        marker.sprite = imageManager.Get("marker");
        marker.color = cfg.NormalColor;
    }

    public bool CanPlaceABlock()
    {
        if (isBlocked)
        {
            return false;
        }

        if(IsFree(TopSpot) 
            && IsBlocked(TopSpot.TopSpot)
            && IsBlocked(TopSpot.LeftSpot)
            && IsBlocked(TopSpot.RightSpot))
        {
            return false;
        }
        if (IsFree(BottonSpot)
            && IsBlocked(BottonSpot.BottonSpot)
            && IsBlocked(BottonSpot.LeftSpot)
            && IsBlocked(BottonSpot.RightSpot))
        {
            return false;
        }
        if (IsFree(LeftSpot)
            && IsBlocked(LeftSpot.BottonSpot)
            && IsBlocked(LeftSpot.LeftSpot)
            && IsBlocked(LeftSpot.TopSpot))
        {
            return false;
        }
        if (IsFree(RightSpot)
            && IsBlocked(RightSpot.BottonSpot)
            && IsBlocked(RightSpot.RightSpot)
            && IsBlocked(RightSpot.TopSpot))
        {
            return false;
        }
        return true;
    }

    private bool IsFree(SpotController spot)
    {
        return spot != null && !spot.isBlocked;
    }

    private bool IsBlocked(SpotController spot)
    {
        return !IsFree(spot);
    }

    public void Compare(Guid moveId, CardController cardController)
    {
        bool isCascade = cascadeKinds.Contains(cardController.Card.Kind);

        

        if (TopSpot != null)
        {
            TopSpot.CompareWithBotton(cardController, moveId, isCascade);
        }
        if (BottonSpot != null)
        {
            BottonSpot.CompareWithTop(cardController, moveId, isCascade);
        }
        if (LeftSpot != null)
        {
            LeftSpot.CompareWithRight(cardController, moveId, isCascade);
        }
        if (RightSpot != null)
        {
            RightSpot.CompareWithLeft(cardController, moveId, isCascade);
        }
    }

    private void CompareValues(int opponentValue, int myValue, Player player, Guid moveId, bool isCascade)
    {
        
        if (myValue >= 0 && opponentValue > myValue && lastMoveId.CompareTo(moveId) != 0 && player != card.Owner)
        {
            lastMoveId = moveId;
            var loser = card.Owner;
            card.Owner = player;
            gm.Sequence.append(() => {
                card.transform.SetAsLastSibling();
            });
            gm.Sequence.append(() => {
                gm.audioManager.Play("TurnCard");                
            });
            gm.Sequence.append(LeanTween.scale(GOcard, new Vector3(1.1f, 1.1f), 0.2f));
            gm.Sequence.append(LeanTween.scale(GOcard, new Vector3(1.0f, 1.0f), 0.2f));
            gm.Sequence.append(() => {
                card.ChangeColor();
                gm.IncScore(player);
                gm.DecScore(loser);
            });

            if(isCascade)
            {
                Compare(moveId, card);
            }
        }
    }

    private void CompareWithLeft(CardController opponentCard, Guid moveId, bool isCascade)
    {
        if(card != null)
        {
            int advantage = CardUtil.CheckStrongWeakness(opponentCard.Card.Kind, card.Card.Kind);
            CompareValues(opponentCard.Card.RightValue + advantage, card.Card.LeftValue, opponentCard.Owner, moveId, isCascade);
        }
    }

    private void CompareWithRight(CardController opponentCard, Guid moveId, bool isCascade)
    {
        if (card != null)
        {
            int advantage = CardUtil.CheckStrongWeakness(opponentCard.Card.Kind, card.Card.Kind);
            CompareValues(opponentCard.Card.LeftValue + advantage, card.Card.RightValue, opponentCard.Owner, moveId, isCascade);
        }
    }

    private void CompareWithTop(CardController opponentCard, Guid moveId, bool isCascade)
    {
        if (card != null)
        {
            int advantage = CardUtil.CheckStrongWeakness(opponentCard.Card.Kind, card.Card.Kind);
            CompareValues(opponentCard.Card.BottomValue + advantage, card.Card.TopValue, opponentCard.Owner, moveId, isCascade);
        }
    }

    private void CompareWithBotton(CardController opponentCard, Guid moveId, bool isCascade)
    {
        if (card != null)
        {
            int advantage = CardUtil.CheckStrongWeakness(opponentCard.Card.Kind, card.Card.Kind);
            CompareValues(opponentCard.Card.TopValue + advantage, card.Card.BottomValue, opponentCard.Owner, moveId, isCascade);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (IsDraggingCard(eventData.pointerDrag))
        {
            marker.color = cfg.SelectedColor;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (IsDraggingCard(eventData.pointerDrag))
        {
            marker.color = cfg.NormalColor;
        }
    }

    private bool IsDraggingCard(GameObject pointerDrag)
    {
        return pointerDrag != null && pointerDrag.GetComponent<CardController>() != null 
            && !pointerDrag.GetComponent<CardController>().isPlaced 
            && pointerDrag.GetComponent<CardController>().Owner == Player.P1
            && !isBlocked && !gm.IsMoveBlocked;
    }
}
