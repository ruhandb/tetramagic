
using Firebase.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public int totalBlocks = 6;
    public ModalEndGame modalEndGame;

    public FirebaseManager fbManager;
    public AudioManager audioManager;
    public PersistentDataManager DataManager;
    public ImageManager imageManager;

    private MatchManager matchManager;

    public LTSeq Sequence;

    public BoardSpotMap BoardSpotMap = new BoardSpotMap();
    private Dictionary<Player, int> score = new Dictionary<Player, int>();
    public Text P1Score;
    public Text P2Score;
    public Text YourTurn;
    public Text OpponentTurn;
    public string UserId;

    public string PlayerTurn;
    private bool isBlockedUI = true;
    public bool IsMoveBlocked
    {
        get
        {
            return isBlockedUI || PlayerTurn != UserId;
        }
    }

    private char[] spotRows = { 'A', 'B', 'C', 'D' };

    private List<SpotController> AllSpots;

    private  void Awake()
    {
        Input.multiTouchEnabled = false;
        score.Add(Player.P1, 6);
        score.Add(Player.P2, 6);

        //fbManager = FindObjectOfType<FirebaseManager>();
        audioManager = FindObjectOfType<AudioManager>();
        DataManager = FindObjectOfType<PersistentDataManager>();
        imageManager = FindObjectOfType<ImageManager>();
        UserId = DataManager["UserId"].ToString();

        LoadSpots();

        Sequence = LeanTween.sequence();
        audioManager.Play("BgMusic");

        if (DataManager["OpponentId"].ToString() == "IA")
        {
            matchManager = new IAMatchManager();
        }
        else
        {
            matchManager = new OnlineMatchManager();
        }
    }

    private void LoadSpots()
    {
        AllSpots = new List<SpotController>();
        foreach (var row in spotRows)
        {
            for (int col = 1; col <= 4; col++)
            {
                var spotName = $"{row}{col}";
                AllSpots.Add(gameObject.transform.Find(spotName).GetComponent<SpotController>());                
            }
        }
    }

    public List<SpotController> GetAllSpots()
    {
        return AllSpots;
    }

    public List<SpotController> GetAllAvaiableSpots()
    {
        return GetAllSpots().Where(spot => !spot.isBlocked).ToList();
    }

    public List<SpotController> GetAllSpotsWithCards()
    {
        return GetAllAvaiableSpots().Where(spot => spot.card != null).ToList();
    }

    public List<SpotController> GetAllSpotsWithoutCards()
    {
        return GetAllAvaiableSpots().Where(spot => spot.card == null).ToList();
    }

    public List<SpotController> GetAllSpotsWithCardsWeakTo(Kind kind)
    {
        return GetAllSpotsWithCards()
            .Where(spot => CardUtil.StrongsWeaks.ContainsKey(kind) && spot.card.Card.Kind == CardUtil.StrongsWeaks[kind].Weak).ToList();
    }

    public List<SpotController> GetAllSpotsWithCardsStrongTo(Kind kind)
    {
        return GetAllSpotsWithCards()
            .Where(spot => CardUtil.StrongsWeaks.ContainsKey(kind) && spot.card.Card.Kind == CardUtil.StrongsWeaks[kind].Strong).ToList();
    }


    public void BlockMove()
    {
        isBlockedUI = true;
    }

    public void UnBlockMove()
    {
        isBlockedUI = false;
    }
    
    public void PlayerMove(string cardName, string spotName)
    {
        matchManager.PlayerMove(cardName, spotName);
        CheckEndGame();
    }

    private int moves = 0;
    private void CheckEndGame()
    {
        moves++;
        if (moves == 11)
        {
            modalEndGame.EndGame(score[Player.P1], score[Player.P2]);
        }
    }

    public void OpponentMove(string cardName, string spotName)
    {
        SpotController spot = gameObject.transform.Find(spotName).GetComponent<SpotController>();
        GameObject goCard = gameObject.transform.Find(cardName).gameObject;
        goCard.transform.SetAsLastSibling();
        LeanTween.scale(goCard.gameObject, new Vector3(1.2f, 1.2f), 0.05f);
        spot.DropCard(goCard);
        Sequence.append(() =>
        {
            CheckEndGame();
        });
    }
        

    private async void Start()
    {
        await matchManager.Start(this);
    }

    public void GiveCard(string cardPosition, Card card, Player p)
    {
        cardPosition = (p == Player.P1 ? "P1" : "P2") + cardPosition.Remove(0, 2);
        CardController cardController = gameObject.transform.Find(cardPosition).GetComponent<CardController>();
        cardController.Card = card;
        cardController.Owner = p;
        cardController.UpdateValues();
        Sequence.append(() => { audioManager.Play("DrawCard");  });
        Sequence.append(LeanTween.moveLocal(cardController.gameObject, cardController.initialPosition, 0.3f));
    }
            

    public void IncScore(Player p)
    {
        ChangeScore(p, 1);
    }
    public void DecScore(Player p)
    {
        ChangeScore(p, -1);
    }

    public void ChangeTurn(string playerTurn)
    {
        PlayerTurn = playerTurn;
        YourTurn.gameObject.SetActive(PlayerTurn == UserId);
        OpponentTurn.gameObject.SetActive(PlayerTurn != UserId);
        Debug.Log("PlayerTurn:" + PlayerTurn);
    }

    private void ChangeScore(Player p, int amount)
    {
        Text pTxt = p == Player.P1 ? P1Score : P2Score;
        score[p]+= amount;
        
        var seq = LeanTween.sequence();
        seq.append(LeanTween.scale(pTxt.gameObject, new Vector3(1.1f, 1.1f), 0.2f));//.setEase(LeanTweenType.));
        seq.append((object val) =>
        {
            pTxt.text = val.ToString();
        }, score[p]);
        seq.append(LeanTween.scale(pTxt.gameObject, new Vector3(1.0f, 1.0f), 0.2f));//.setEase(LeanTweenType.easeIn));
        
    }

    internal void HideAdvantage(Kind kind)
    {
        foreach (var spot in GetAllSpotsWithCardsStrongTo(kind).Where(s => s.card.Owner == Player.P2))
        {
            spot.card.ResetValues();
        }
        foreach (var spot in GetAllSpotsWithCardsWeakTo(kind).Where(s => s.card.Owner == Player.P2))
        {
            spot.card.ResetValues();
        }
    }

    internal void ShowAdvantage(Kind kind)
    {
        foreach (var spot in GetAllSpotsWithCardsStrongTo(kind).Where(s => s.card.Owner == Player.P2))
        {
            spot.card.ShowDisadvantage();
        }
        foreach (var spot in GetAllSpotsWithCardsWeakTo(kind).Where(s => s.card.Owner == Player.P2))
        {
            spot.card.ShowAdvantage();
        }
    }

    private Card ToCard(object v)
    {
        IDictionary<string, object> dic = (IDictionary<string, object>)v;
        var card = new Card()
        {
            TopValue = Int32.Parse(dic["TopValue"].ToString()),
            BottomValue = Int32.Parse(dic["BottomValue"].ToString()),
            LeftValue = Int32.Parse(dic["LeftValue"].ToString()),
            RightValue = Int32.Parse(dic["RightValue"].ToString()),
            Name = dic["Name"].ToString()
        };
        return card;
    }

    private IDictionary<string, object> ToDict(Card c)
    {
        var dict = new Dictionary<string, object>
        {
            { "TopValue", c.TopValue },
            { "BottomValue", c.BottomValue },
            { "LeftValue", c.LeftValue },
            { "RightValue", c.RightValue },
            { "Name", c.Name }
        };
        return dict;
    }
}




