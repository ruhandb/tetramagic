using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Firebase.Database;

public class IAMatchManager : MatchManager
{
    private char[] spotRows = { 'A', 'B', 'C', 'D' };
    private string[] iaCards = { "P2C1", "P2C2", "P2C3", "P2C4", "P2C5", "P2C6" };

    public override void PlayerMove(string cardName, string spotName)
    {
        ChangeTurn(OpponentId);

        // Lógica IA
        //Gm.OpponentMove(cardName, spotName);
    }

    private void IAPlay()
    {
        var move = Calculate();
        if (move != null)
        {
            Gm.OpponentMove(move.cardName, move.spotName);
            ChangeTurn(OwnerId);
        }
    }

    private MoveIA Calculate()
    {
        List<MoveIA> movesIA = new List<MoveIA>();

        foreach (var spot in Gm.GetAllSpotsWithoutCards())
        {
            foreach (var cardName in iaCards)
            {
                CardController cardIA = Gm.gameObject.transform.Find(cardName).GetComponent<CardController>();
                if (!cardIA.isPlaced)
                {
                    var points = CalculatePoints(cardIA.Card.TopValue, cardIA.Card.Kind, spot.TopSpot, Direction.Top);
                    points += CalculatePoints(cardIA.Card.BottomValue, cardIA.Card.Kind, spot.BottonSpot, Direction.Bottom);
                    points += CalculatePoints(cardIA.Card.LeftValue, cardIA.Card.Kind, spot.LeftSpot, Direction.Left);
                    points += CalculatePoints(cardIA.Card.RightValue, cardIA.Card.Kind, spot.RightSpot, Direction.Right);
                    points += CardUtil.CascadeKinds.Contains(cardIA.Card.Kind) ? 2 : 0;

                    movesIA.Add(new MoveIA
                    {
                        points = points,
                        cardName = cardName,
                        spotName = spot.name
                    });
                    Debug.Log($"pts: { points }, c: { cardName }, s: { spot.name }");
                }
            }
        }
        if (movesIA.Count > 0)
        {
            return movesIA.OrderByDescending(m => m.points).First();
        }
        else
        {
            return null;
        }
    }

    private int CalculatePoints(int iaValue, Kind iaKind, SpotController spot, Direction direction)
    {
        int iaCalculedValue = (iaValue == -1 ? 9 : iaValue);
        if (spot != null && !spot.isBlocked && spot.card != null && spot.card.Owner == Player.P1)
        {
            iaCalculedValue += CardUtil.CheckStrongWeakness(iaKind, spot.card.Card.Kind);
            iaValue += CardUtil.CheckStrongWeakness(iaKind, spot.card.Card.Kind);
            int playerValue = 0;
            switch (direction)
            {
                case Direction.Top:
                    playerValue = spot.card.Card.BottomValue;
                    break;
                case Direction.Bottom:
                    playerValue = spot.card.Card.TopValue;
                    break;
                case Direction.Left:
                    playerValue = spot.card.Card.RightValue;
                    break;
                case Direction.Right:
                    playerValue = spot.card.Card.LeftValue;
                    break;
            }
            if (playerValue == -1)
            {
                return 12 - iaCalculedValue;
            }
            if (iaValue > playerValue)
            {
                return 15 + iaCalculedValue - playerValue;
            }
            else
            {
                return 10 - iaCalculedValue;
            }

        }
        else if(spot == null || spot.isBlocked)
        {
            return 12 - iaCalculedValue;
        }
        else if (spot.card == null)
        {
            return iaCalculedValue;
        }
        else if (spot.card.Owner == Player.P2)
        {
            return 10 - iaCalculedValue;
        }
        return 0;
    }

    public override void ChangeTurn(string player)
    {
        Gm.ChangeTurn(player);
        if (player == "IA")
        {
            IAPlay();
        }
    }

    public override Task EndPlaceBlocks(Dictionary<string, object> blocks)
    {
        return Task.Delay(0);
    }

    public override void Listeners(){ }

    public override void NotHostListeners(){ }

    public override Task OnMatchExists()
    {
        return Task.Delay(0);
    }

    public override Task OnSortedCards(List<Dictionary<string, object>> sortedCards)
    {
        return Task.Delay(0);
    }
}

public enum Direction
{
    Top,
    Bottom,
    Left,
    Right
}

public class MoveIA
{
    public int points;
    public string cardName;
    public string spotName;
}
