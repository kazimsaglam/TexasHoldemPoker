using System;
using UnityEngine;

public enum CardValue
{
  Two,
  Three,
  Four,
  Five,
  Six,
  Seven,
  Eight,
  Nine,
  Ten,
  Jack,
  Queen,
  King,
  Ace
};

public enum CardColor
{
  Hearts,
  Diamonds,
  Spades,
  Clubs
}

[CreateAssetMenu(fileName = "New Card", menuName = "Cards/CardData")]
[Serializable]
public class CardData : ScriptableObject
{
  public Sprite cardSprite;
  public CardValue cardValue;
  public CardColor cardColor;
}