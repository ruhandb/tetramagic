using UnityEngine;
using System.Collections.Generic;

public class CardUtil
{
    private static Dictionary<Kind, List<ValueType>> _kindDefinition = new Dictionary<Kind, List<ValueType>>
    {
        { Kind.Sword, new List<ValueType> { ValueType.Shield, ValueType.Value, ValueType.Value, ValueType.Value } },
        { Kind.Shield, new List<ValueType> { ValueType.Shield, ValueType.Shield, ValueType.Value, ValueType.Value } },
        { Kind.Bow, new List<ValueType> { ValueType.Weakness, ValueType.Value, ValueType.Value, ValueType.Value } },
        { Kind.Spear, new List<ValueType> { ValueType.Weakness, ValueType.Shield, ValueType.Value, ValueType.Value } },

        { Kind.Fire, new List<ValueType> { ValueType.Value, ValueType.Value, ValueType.Value, ValueType.Value } },
        { Kind.Nature, new List<ValueType> { ValueType.Value, ValueType.Value, ValueType.Value, ValueType.Value } },
        { Kind.Water, new List<ValueType> { ValueType.Value, ValueType.Value, ValueType.Value, ValueType.Value } },
        { Kind.Wind, new List<ValueType> { ValueType.Value, ValueType.Value, ValueType.Value, ValueType.Value } },
    };

    private static Dictionary<Rarity, int> _pointsByRarity = new Dictionary<Rarity, int> {
        { Rarity._1Star, 12 },
        { Rarity._2Star, 14 },
        { Rarity._3Star, 16 },
        { Rarity._4Star, 18 },
        { Rarity._5Star, 20 }
    };

    public static Dictionary<Kind, StrongWeak> StrongsWeaks = new Dictionary<Kind, StrongWeak> {
        { Kind.Fire, new StrongWeak { Strong = Kind.Nature, Weak = Kind.Water } },
        { Kind.Nature, new StrongWeak { Strong = Kind.Wind, Weak = Kind.Fire } },
        { Kind.Wind, new StrongWeak { Strong = Kind.Water, Weak = Kind.Nature } },
        { Kind.Water, new StrongWeak { Strong = Kind.Fire, Weak = Kind.Wind } }
    };

    public static List<Kind> CascadeKinds = new List<Kind> { Kind.Fire, Kind.Nature, Kind.Water, Kind.Wind };

    private static List<Kind> _kinds = new List<Kind>
    {
        Kind.Sword, Kind.Shield, Kind.Bow, Kind.Spear,
        Kind.Fire, Kind.Nature, Kind.Water, Kind.Wind
    };

    public static Card Generate(List<Rarity> rarities, List<Kind> kinds = null)
    {
        var rarity = GetRandom(rarities);
        var totalPoints = _pointsByRarity[rarity];

        var kind = GetRandom(kinds == null ? _kinds : kinds);

        if (CascadeKinds.Contains(kind))
        {
            totalPoints -= 2;
        }

        List<Direction> _directions = new List<Direction>
        {
            Direction.Top, Direction.Bottom, Direction.Left, Direction.Right
        };
        
        Card card = new Card();
        card.Kind = kind;
        card.Rarity = rarity;
        Debug.Log($"RandomCard() {card.Kind}");
        foreach (ValueType vt in _kindDefinition[kind])
        {
            var iDir = _directions.Count;
            var dir = ExtractRandom(_directions);

            var value = totalPoints;
            if (vt == ValueType.Weakness)
            {
                value = 0;
            }
            else if (vt == ValueType.Shield)
            {
                value = 5;
            }
            else if (iDir > 1)
            {
                var maxValue = totalPoints - iDir < 10 ? totalPoints - iDir : 9;

                var virtualMin = totalPoints - (9 * (iDir - 1));
                var minValue = virtualMin > 1 ? virtualMin : 1;
                Debug.Log($"{dir} min: {minValue}, max: {maxValue}");
                value = Random.Range(minValue, maxValue + 1);
            }

            totalPoints -= value;

            value = vt == ValueType.Shield ? -1 : value;

            switch (dir)
            {
                case Direction.Top:
                    card.TopValue = value;
                    break;
                case Direction.Bottom:
                    card.BottomValue = value;
                    break;
                case Direction.Left:
                    card.LeftValue = value;
                    break;
                case Direction.Right:
                    card.RightValue = value;
                    break;
                default:
                    break;
            }

        }
        return card;
    }

    private static T GetRandom<T>(List<T> list)
    {
        return list[Random.Range(0, list.Count)];
    }

    private static T ExtractRandom<T>(List<T> list)
    {
        var item = list[Random.Range(0, list.Count)];
        list.Remove(item);
        return item;
    }

    public static int CheckStrongWeakness(Kind kindAtk, Kind kindDef)
    {

        if (StrongsWeaks.ContainsKey(kindAtk))
        {
            var sw = StrongsWeaks[kindAtk];
            if (sw.Strong == kindDef)
            {
                return 1;
            }
            else if (sw.Weak == kindDef)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }
        else
        {
            return 0;
        }
    }
}

public enum ValueType
{
    Shield,
    Weakness,
    Value
}

public class StrongWeak
{
    public Kind Strong;
    public Kind Weak;
}
