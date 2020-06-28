using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Card : IFirebaseData
{
    public int TopValue;
    public int BottomValue;
    public int LeftValue;
    public int RightValue;
    public string Name;
    public Kind Kind;
    public CardGroup CardGroup;
    public Rarity Rarity;
}



public enum CardGroup
{
    Weapon,
    Magic
}

public enum Kind
{
    Shield,
    Sword,
    Bow,
    Spear,

    Fire,
    Water,
    Nature,
    Wind
}

public enum Rarity
{
    _1Star,
    _2Star,
    _3Star,
    _4Star,
    _5Star
}