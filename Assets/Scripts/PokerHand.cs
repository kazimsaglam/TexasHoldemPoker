using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class PokerHand
{
  public bool royalFlush;
  public bool straightFlush;
  public bool fourAKind;
  public bool fullHouse;
  public bool flush;
  public bool straight;
  public bool threeAKind;
  public bool twoPair;
  public bool pair;
  public bool highCard;
  public List<Card> highCards;
  public int strength;

  public PokerHand()
  {
    this.royalFlush = false;
    this.straightFlush = false;
    this.fourAKind = false;
    this.fullHouse = false;
    this.flush = false;
    this.straight = false;
    this.threeAKind = false;
    this.twoPair = false;
    this.pair = false;
    this.highCard = false;
    this.highCards = new List<Card>();
    this.strength = 0;
  }

  public void SetPokerHand(Card[] cardArray)
  {
    List<Card> cards = new List<Card>();
    foreach (Card c in cardArray)
    {
      cards.Add(c);
    }

    if (IsRoyal(cards)) return;
    if (IsStraightFlush(cards)) return;
    if (isFourAKind(cards)) return;
    if (IsFullHouse(cards)) return;
    if (isFlush(cards)) return;
    if (IsStraight(cards)) return;
    if (isThreeAKind(cards)) return;
    if (isTwoPair(cards)) return;
    if (isPair(cards)) return;
    isHigh(cards);
  }

  public String PrintResult()
  {
    if (this.royalFlush)
    {
      return "Royal Flush" + "- Strength:" + this.strength;
    }
    else if (this.straightFlush)
    {
      return "Straight Flush" + "- Strength:" + this.strength;
    }
    else if (this.fourAKind)
    {
      return "Four of a kind" + "- Strength:" + this.strength;
    }
    else if (this.fullHouse)
    {
      return "Full house" + "- Strength:" + this.strength;
    }
    else if (this.flush)
    {
      return "Flush" + "- Strength:" + this.strength;
    }
    else if (this.straight)
    {
      return "Straight" + "- Strength:" + this.strength;
    }
    else if (this.threeAKind)
    {
      return "Three of a kind" + "- Strength:" + this.strength;
    }
    else if (this.twoPair)
    {
      return "Two Pair" + "- Strength:" + this.strength;
    }
    else if (this.pair)
    {
      return "Pair" + "- Strength:" + this.strength;
    }
    else if (this.highCard)
    {
      return "High card" + "- Strength:" + this.strength;
    }
    else
      return "error setting hand.";
  }

  private bool IsRoyal(List<Card> cards)
  {
    highCards = new List<Card> {cards.ElementAt(4)};

    if (isFlush(cards) && IsStraight(cards) && this.highCards.ElementAt(0).cardValue == CardValue.Ace)
    {
      royalFlush = true;
      strength = 9;
    }

    return royalFlush;
  }

  private bool IsStraightFlush(List<Card> cards)
  {
    highCards = new List<Card> {cards.ElementAt(4)};
    if (isFlush(cards) && IsStraight(cards))
    {
      this.straightFlush = true;
      this.strength = 8;
      return true;
    }

    return false;
  }

  private bool isFourAKind(List<Card> cards)
  {
    this.highCards = new List<Card>();
    int count;

    for (int i = 0; i < 5; i++)
    {
      count = 0;
      Card currentCard = cards.ElementAt(i);
      foreach (Card card in cards)
      {
        if (currentCard.cardValue == card.cardValue)
          count++;

        if (count == 4)
        {
          fourAKind = true;
          highCards.Add(card);
          this.strength = 7;

          // find the kicker card
          foreach (Card c in cards)
          {
            if ((int)c.cardValue != (int)highCards.ElementAt(0).cardValue)
              this.highCards.Add(c);
          }

          return true;
        }
      }
    }

    return false;
  }

  private bool IsFullHouse(List<Card> cards)
  {
    this.highCards = new List<Card>();
    Card match2 = null;

    for (int i = 0; i < 5; i++)
    {
      int count = 0;
      Card currentCard = cards.ElementAt(i);
      foreach (Card card in cards)
      {
        if (currentCard.cardValue == card.cardValue)
          count++;

        // found three of a kind
        if (count == 3)
        {
          Card match1 = card;

          // find the second match for the full house
          foreach (Card c2 in cards)
          {
            if (match1.cardValue != c2.cardValue)
            {
              match2 = c2;
            }
          }

          if (match2 != null)
          {
            count = 0;
            foreach (Card c3 in cards)
            {
              if (match2.cardValue == c3.cardValue)
              {
                count++;
                if (count == 2)
                {
                  highCards.Add(match1);
                  highCards.Add(match2);
                  fullHouse = true;
                  strength = 6;
                  return true;
                }
              }
            }
          }
        }
      }
    }

    return false;
  }

  private bool isFlush(List<Card> cards)
  {
    // check to see what suite the playersAndBots card is and see if all cards
    // share that suite.
    highCards = new List<Card>();
    Card card = cards.ElementAt(0);
    int playerSuite = (int)card.cardColor;

    foreach (Card c in cards)
    {
      if ((int)c.cardColor != playerSuite)
        return false;
    }

    for (int i = 4; i >= 0; i--)
    {
      highCards.Add(cards.ElementAt(i));
    }

    flush = true;
    strength = 5;
    return true;
  }

  private bool IsStraight(List<Card> cards)
  {
    highCards = new List<Card>();
    Card card1 = cards.ElementAt(0);
    Card card2 = cards.ElementAt(1);
    Card card3 = cards.ElementAt(2);
    Card card4 = cards.ElementAt(3);
    Card card5 = cards.ElementAt(4);

    // make sure the cards rank is in order
    if (((int)card3.cardValue - (int)card4.cardValue == -1) && ((int)card2.cardValue - (int)card3.cardValue == -1) && ((int)card1.cardValue - (int)card2.cardValue == -1))
    {
      if ((card4.cardValue - card5.cardValue == -1))
      {
        straight = true;
        strength = 4;

        for (int i = 4; i >= 0; i--)
        {
          this.highCards.Add(cards.ElementAt(i));
        }
      }

      else if ((int)card1.cardValue == 2 && (int)card5.cardValue == 14 /* ACE */)
      {
        straight = true;
        strength = 4;

        for (int i = 4; i >= 0; i--)
        {
          highCards.Add(cards.ElementAt(i));
        }
      }
      else
        return straight;
    }

    return straight;
  }

  private bool isThreeAKind(List<Card> cards)
  {
    this.highCards = new List<Card>();
    int count;

    for (int i = 0; i < 5; i++)
    {
      count = 0;
      Card curentCard = cards.ElementAt(i);
      foreach (Card card in cards)
      {
        if (curentCard.cardValue == card.cardValue)
          count++;

        if (count == 3)
        {
          this.highCards.Add(card);
          this.threeAKind = true;
          this.strength = 3;

          // find the two kickers
          for (int j = 4; j >= 0; j--)
          {
            if (cards.ElementAt(j).cardValue != this.highCards.ElementAt(0).cardValue)
              this.highCards.Add(cards.ElementAt(j));
          }
        }
      }
    }

    return this.threeAKind;
  }

  private bool isTwoPair(List<Card> cards)
  {
    this.highCards = new List<Card>();
    int count;
    Card match1 = null;

    for (int i = 0; i < 5; i++)
    {
      count = 0;
      Card curentCard = cards.ElementAt(i);

      foreach (Card card in cards)
      {
        if (curentCard.cardValue == card.cardValue)
          count++;

        // found a pair
        if (count == 2)
        {
          match1 = card;
          break;
        }
      }

      if (match1 != null) break;
    }

    // found one match
    if (match1 != null)
    {
      for (int i = 0; i < 5; i++)
      {
        count = 0;
        Card curentCard = cards.ElementAt(i);

        if (curentCard.cardValue != match1.cardValue)
        {
          foreach (Card card in cards)
          {
            if (curentCard.cardValue == card.cardValue)
              count++;

            // found second pair
            if (count == 2)
            {
              this.highCards.Add(card);
              this.highCards.Add(match1);
              this.twoPair = true;
              this.strength = 2;

              // get the kicker card
              for (int j = 4; j >= 0; j--)
              {
                if (cards.ElementAt(j).cardValue != this.highCards.ElementAt(0).cardValue
                    && cards.ElementAt(j).cardValue != this.highCards.ElementAt(1).cardValue)
                  this.highCards.Add(cards.ElementAt(j));
              }
            }
          }
        }
      }
    }

    return this.twoPair;
  }

  private bool isPair(List<Card> cards)
  {
    this.highCards = new List<Card>();
    int count;

    for (int i = 0; i < 5; i++)
    {
      count = 0;
      Card curentCard = cards.ElementAt(i);
      foreach (Card card in cards)
      {
        if (curentCard.cardValue == card.cardValue)
          count++;

        if (count == 2)
        {
          this.pair = true;
          this.highCards.Add(card);
          this.strength = 1;

          for (int j = 4; j >= 0; j--)
          {
            if (this.highCards.ElementAt(0).cardValue != cards.ElementAt(j).cardValue)
              this.highCards.Add(cards.ElementAt(j));
          }
        }
      }
    }

    return this.pair;
  }

  private void isHigh(List<Card> cards)
  {
    this.highCards = new List<Card>();
    for (int j = 4; j >= 0; j--)
    {
      this.highCards.Add(cards.ElementAt(j));
    }

    this.highCard = true;
  }
}