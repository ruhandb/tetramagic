using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Firebase.Database;

public class OnlineMatchManager : MatchManager
{

    public override void PlayerMove(string cardName, string spotName)
    {
        cardName = cardName.Replace("P1", "");
        var moveKey = Gm.fbManager.DB.GetReference(FirebasePaths.MATCHES).Child(OwnerId).Child("Moves").Push().Key;
        Dictionary<string, object> move = new Dictionary<string, object>
        {
            { "card", cardName },
            { "user", Gm.fbManager.Auth.CurrentUser.UserId },
            { "spot",  spotName },
            { "ts", ServerValue.Timestamp }
        };
        Gm.fbManager.DB.GetReference(FirebasePaths.MATCHES).Child(OwnerId).Child("Moves").Child(moveKey).SetValueAsync(move);
        ChangeTurn(isHost ? OpponentId : OwnerId);
    }

    public override void ChangeTurn(string player)
    {
        Debug.Log("ChangeTurn: " + player);
        Gm.fbManager.DB.GetReference(FirebasePaths.MATCHES).Child(OwnerId).Child("PlayerTurn").SetValueAsync(player);
    }

    public override async Task EndPlaceBlocks(Dictionary<string, object> blocks)
    {
        await Gm.fbManager.DB.GetReference(FirebasePaths.MATCHES).Child(OwnerId).Child("blocks").SetValueAsync(blocks);
    }

    public override void Listeners()
    {
        Gm.fbManager.DB.GetReference(FirebasePaths.MATCHES).Child(OwnerId).Child("PlayerTurn").ValueChanged += (object sender, ValueChangedEventArgs e) =>
        {
            if (e.Snapshot.Exists)
            {
                Gm.ChangeTurn(e.Snapshot.Value.ToString());                
            }
        };

        Gm.fbManager.DB.GetReference(FirebasePaths.MATCHES).Child(OwnerId).Child("Moves").ChildAdded += (object sender, ChildChangedEventArgs e) =>
        {
            if (e.Snapshot.Exists && NotInMoves(e.Snapshot.Key))
            {
                IDictionary value = (IDictionary)e.Snapshot.Value;
                if (value["user"].ToString() != Gm.fbManager.Auth.CurrentUser.UserId)
                {
                    Gm.OpponentMove($"P2{value["card"]}", value["spot"].ToString());
                }
            }
        };
    }

    private bool NotInMoves(string key)
    {
        // TODO Resolver na Query
        foreach (var move in moveList)
        {
            if (move.Key == key)
            {
                return false;
            }
        }
        return true;
    }

    public override void NotHostListeners()
    {
        Gm.fbManager.DB.GetReference(FirebasePaths.MATCHES).Child(OwnerId).Child("cards").ChildAdded += (object sender, ChildChangedEventArgs e) =>
        {
            Gm.Sequence.append(() =>
            {
                Gm.BlockMove();
            });
            foreach (var item in e.Snapshot.Children)
            {
                IDictionary value = (IDictionary)item.Value;
                Player pOwner = (Player)Int32.Parse(value["owner"].ToString());
                if (isHost)
                {
                    pOwner = pOwner == Player.P1 ? Player.P2 : Player.P1;
                }
                Debug.Log("draw: " + item.Key);
                Gm.GiveCard(item.Key, ToCard(value["card"]), pOwner);
            }
            Gm.Sequence.append(() =>
            {
                Gm.UnBlockMove();
            });
        };
        Gm.fbManager.DB.GetReference(FirebasePaths.MATCHES).Child(OwnerId).Child("blocks").ValueChanged += (object sender, ValueChangedEventArgs e) => {
            Debug.Log("!isHost:" + e.Snapshot.Key);
            foreach (var item in e.Snapshot.Children)
            {
                SpotController spot = Gm.gameObject.transform.Find(item.Key).GetComponent<SpotController>();
                spot.BlockSpot();
            }

        };
    }

    List<Move> moveList = new List<Move>();
    public override async Task OnMatchExists()
    {
        var blocks = await Gm.fbManager.DB.GetReference(FirebasePaths.MATCHES).Child(OwnerId).Child("blocks").GetValueAsync();
        if (blocks != null && blocks.Exists)
        {
            foreach (var block in blocks.Children)
            {
                SpotController spot = Gm.gameObject.transform.Find(block.Key).GetComponent<SpotController>();
                spot.BlockSpot();
            }
        }

        List<Draw> drawList = new List<Draw>();
        
        var draws = await Gm.fbManager.DB.GetReference(FirebasePaths.MATCHES).Child(OwnerId).Child("cards").Child("sorted").GetValueAsync();
        // var moves = (DataSnapshot)fbManager.PeristentData["Moves"];
        if (draws != null && draws.Exists)
        {
            foreach (var draw in draws.Children)
            {
                IDictionary value = (IDictionary)draw.Value;
                Player pOwner = (Player)Int32.Parse(value["owner"].ToString());
                if (isHost)
                {
                    pOwner = pOwner == Player.P1 ? Player.P2 : Player.P1;
                }
                Debug.Log("draw: " + draw.Key);
                drawList.Add(new Draw
                {
                    CardPosition = draw.Key,
                    Card = ToCard(value["card"]),
                    Player = pOwner
                });
            }
        }

        var moves = await Gm.fbManager.DB.GetReference(FirebasePaths.MATCHES).Child(OwnerId).Child("Moves").OrderByChild("ts").GetValueAsync();
        if (moves != null && moves.Exists)
        {
            foreach (var move in moves.Children)
            {
                IDictionary value = (IDictionary)move.Value;
                var cardName = (value["user"].ToString() == Gm.fbManager.Auth.CurrentUser.UserId ? "P1" : "P2") + value["card"].ToString();
                Debug.Log("move: " + cardName);
                SpotController spot = Gm.gameObject.transform.Find(value["spot"].ToString()).GetComponent<SpotController>();
                GameObject goCard = Gm.gameObject.transform.Find(cardName).gameObject;
                moveList.Add(new Move
                {
                    Spot = spot,
                    GOCard = goCard,
                    Key = move.Key
                });
            }
        }

        Gm.Sequence.append(() =>
        {
            Gm.BlockMove();
        });
        foreach (var draw in drawList)
        {
            Gm.GiveCard(draw.CardPosition, draw.Card, draw.Player);
        }
        foreach (var move in moveList)
        {
            move.Spot.DropCard(move.GOCard);
        }
        Gm.Sequence.append(() =>
        {
            Gm.UnBlockMove();
        });
    }

    public override async Task OnSortedCards(List<Dictionary<string, object>> sortedCards)
    {
        var cardsDict = new Dictionary<string, object>();
        foreach (var item in sortedCards)
        {
            cardsDict.Add(item["cardPositionToSend"].ToString(), new Dictionary<string, object> {
                { "card", item["card"] },
                { "owner", (int)((Player)item["owner"] == Player.P1 ? Player.P2 : Player.P1) },
            });
        }
        await Gm.fbManager.DB.GetReference(FirebasePaths.MATCHES).Child(OwnerId).Child("cards").Child("sorted").SetValueAsync(cardsDict);
    }

    
}
