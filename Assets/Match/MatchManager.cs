using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using System.Linq;

public abstract class MatchManager
{
    private char[] rows = { 'A', 'B', 'C', 'D' };

    public string OwnerId;
    public string OpponentId;
    private bool Exists;
    public GameManager Gm;
    public bool isHost;
    private List<Dictionary<string, object>> SortedCards = new List<Dictionary<string, object>>();

    public abstract Task EndPlaceBlocks(Dictionary<string, object> blocks);
    public abstract Task OnMatchExists();
    public abstract Task OnSortedCards(List<Dictionary<string, object>> sortedCards);
    public abstract void ChangeTurn(string player);
    public abstract void NotHostListeners();
    public abstract void Listeners();
    public abstract void PlayerMove(string cardName, string spotName);


    public async Task Start(GameManager gm)
    {
        Gm = gm;
        OwnerId = Gm.DataManager["OwnerId"].ToString();
        OpponentId = Gm.DataManager["OpponentId"].ToString();
        Exists = (bool)Gm.DataManager["Exists"];
        isHost = OwnerId == Gm.UserId;

        if (Exists)
        {
            await OnMatchExists();
        }
        else
        {
            if (isHost)
            {
                PLaceBlocks();
                await SortAndDrawCards();
                List<string> players = new List<string> { OwnerId, OpponentId };
                Gm.Sequence.append(() => {
                    ChangeTurn(players[UnityEngine.Random.Range(0, 2)]);
                });
            }
            else
            {
                NotHostListeners();
            }
        }
        Listeners();
    }

    private void PLaceBlocks() {
        int blocks = 0;
        Dictionary<string, object> spotsDict = new Dictionary<string, object>();
        while (blocks != Gm.totalBlocks)
        {
            int row = UnityEngine.Random.Range(0, 4);
            int col = UnityEngine.Random.Range(1, 5);

            string spotName = $"{rows[row]}{col}";
            SpotController spot = Gm.gameObject.transform.Find(spotName).GetComponent<SpotController>();
            if (spot.CanPlaceABlock())
            {
                spot.BlockSpot();
                blocks++;
                Debug.Log("isHost:" + spotName);
                spotsDict[spotName] = true;
            }
        }
        EndPlaceBlocks(spotsDict);
    }

    private async Task SortAndDrawCards()
    {
        //Dictionary<string, Card> allCards = new Dictionary<string, Card>();
        //var snapshot = await Gm.fbManager.DB.GetReference(FirebasePaths.CARDS).GetValueAsync();
        //Debug.Log(snapshot.GetRawJsonValue());
        //foreach (var childSnapshot in snapshot.Children)
        //{
        //    ValueWrapper<Card> card = childSnapshot.Value<Card>();
        //    allCards.Add(card.Key, card.Value);
        //}
        //DrawCard(1, Player.P1, allCards);
        DrawCard(1, Player.P1);

        await OnSortedCards(SortedCards);
        
        Gm.Sequence.append(() =>
        {
            Gm.BlockMove();
        });
        foreach (var item in SortedCards)
        {
            Gm.GiveCard(item["cardPosition"].ToString(), ToCard(item["card"]), (Player)item["owner"]);
        }
        Gm.Sequence.append(() =>
        {
            Gm.UnBlockMove();
        });
    }

    private void DrawCard(int c, Player p, Dictionary<string, Card> allCards=null)
    {
        if (c <= 6)
        {
            //int idx = UnityEngine.Random.Range(0, allCards.Count);            
            //var kv = allCards.ElementAt(idx);
            //Card card = kv.Value;
            //allCards.Remove(kv.Key);

            Card card = CardUtil.Generate(new List<Rarity> { Rarity._1Star });

            string cardPosition = $"P{(p == Player.P1 ? 1 : 2)}C{c}";
            Debug.Log(cardPosition);

            SortedCards.Add(new Dictionary<string, object>
            {
                { "card", ToDict(card) },
                { "owner", (int)p },
                { "cardPosition", cardPosition },
                { "cardPositionToSend", $"P{(p == Player.P1 ? 2 : 1)}C{c}" }
            });

            DrawCard(p == Player.P1 ? c : ++c, p == Player.P1 ? Player.P2 : Player.P1, allCards);
        }
    }
    
    public Card ToCard(object v)
    {
        IDictionary<string, object> dic = (IDictionary<string, object>)v;
        var card = new Card()
        {
            TopValue = Int32.Parse(dic["TopValue"].ToString()),
            BottomValue = Int32.Parse(dic["BottomValue"].ToString()),
            LeftValue = Int32.Parse(dic["LeftValue"].ToString()),
            RightValue = Int32.Parse(dic["RightValue"].ToString()),
            Kind = (Kind)Int32.Parse(dic["Kind"].ToString()),
            Rarity = (Rarity)Int32.Parse(dic["Rarity"].ToString()),
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
            { "Kind", (int)c.Kind },
            { "Rarity", (int)c.Rarity },
        };
        return dict;
    }
}

public class Move
{
    public SpotController Spot;
    public GameObject GOCard;
    public string Key;
}

public class Draw
{
    public string CardPosition;
    public Card Card;
    public Player Player;
}
