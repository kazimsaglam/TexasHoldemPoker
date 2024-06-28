using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PokerHand
{
    public bool royalFlush;
    public bool straightFlush;
    public bool fourAKing;
    public bool fullHouse;
    public bool flush;
    public bool straight;
    public bool threeAKind;
    public bool twoPair;
    public bool pair;
    public bool highCards;
    public List<Card> highCard;
    public int strength;
    public int earlyStrenght;

    public static PokerHand instance;

    private void Awake()
    {
        instance = this;

    }

    public PokerHand()
    {
        royalFlush = false;
        straightFlush = false;
        fourAKing = false;
        fullHouse = false;
        flush = false;
        straight = false;
        threeAKind = false;
        twoPair = false;
        pair = false;
        highCards = false;
        highCard = new List<Card>();
        strength = 0;
        earlyStrenght = 0;

    }

    public void SetPokerHand(Card[] cardArray)
    {
        List<Card> cards = new List<Card>();
        foreach (Card c in cardArray)
        {
            cards.Add(c);
        }

        //if (isRoyal(cards)) return;
        if (IsStraightFlush(cards)) return;
        if (IsFourAKind(cards)) return;
        if (IsFullHouse(cards)) return;
        if (IsFlush(cards)) return;
        if (IsStraight(cards)) return;
        if (IsThreeAKind(cards)) return;
        if (IsTwoPair(cards)) return;
        if (IsPair(cards)) return;
        IsHigh(cards);
    }
    public void BotAISetPokerHand(Card[] cardArray)
    {
        List<Card> cards = new List<Card>();
        foreach (Card c in cardArray)
        {
            cards.Add(c);
        }

        if (IsRoyal(cards) || IsStraightFlush(cards) || IsFourAKind(cards) || IsFullHouse(cards) ||
            IsFlush(cards) || IsStraight(cards) || IsThreeAKind(cards) || IsTwoPair(cards) || IsPair(cards))
        {
            return;
        }

        IsHigh(cards);
    }

    public void WinnerHandControl()
    {

    }


    public String PrintResult()
    {
        if (this.royalFlush)
        {
            return "ROYAL FLUSH"/* + "- Strength:" + this.strength*/;
        }
        else if (this.straightFlush)
        {
            return "STRAIGHT FLUSH"/* + "- Strength:" + this.strength*/;
        }
        else if (this.fourAKing)
        {
            return "FOUR OF O KIND" /*+ "- Strength:" + this.strength*/;
        }
        else if (this.fullHouse)
        {
            return "FULL HOUSE" /*+ "- Strength:" + this.strength*/;
        }
        else if (this.flush)
        {
            return "FLUSH" /*+ "- Strength:" + this.strength*/;
        }
        else if (this.straight)
        {
            return "STRAIGHT" /*+ "- Strength:" + this.strength*/;
        }
        else if (this.threeAKind)
        {
            return "THREE OF A KIND" /*+ "- Strength:" + this.strength*/;
        }
        else if (this.twoPair)
        {
            return "TWO PAIR"/* + "- Strength:" + this.strength*/;
        }
        else if (this.pair)
        {
            return "PAIR" /*+ "- Strength:" + this.strength*/;
        }
        else if (this.highCards)
        {
            return "HIGH CARD"/* + "- Strength:" + this.strength*/;
        }
        else
            return "error setting hand.";
    }
    public String BotAIprintResult()
    {

        if (this.pair)
        {
            return "Pair" + "- Strength:" + this.strength;
        }
        else if (this.twoPair)
        {
            return "Two Pair" + "- Strength:" + this.strength;
        }
        else if (this.threeAKind)
        {
            return "Three of a kind" + "- Strength:" + this.strength;
        }
        else if (this.straight)
        {
            return "Straight" + "- Strength:" + this.strength;
        }
        else if (this.flush)
        {
            return "Flush" + "- Strength:" + this.strength;
        }
        else if (this.fullHouse)
        {
            return "Full house" + "- Strength:" + this.strength;
        }
        else if (this.fourAKing)
        {
            return "Four of a kind" + "- Strength:" + this.strength;
        }
        else if (this.straightFlush)
        {
            return "Straight Flush" + "- Strength:" + this.strength;
        }
        else if (this.royalFlush)
        {
            return "Royal Flush" + "- Strength:" + this.strength;
        }
        else if (this.highCards)
        {
            return "High card" + "- Strength:" + this.strength;
        }
        else
            return "error setting hand.";
    }

    private bool IsRoyal(List<Card> cards)
    {
        this.highCard = new List<Card>();
        this.highCard.Add(cards.ElementAt(4));

        if (IsFlush(cards) && IsStraight(cards) && this.highCard.ElementAt(0).cardValue == CardValue.Ace)
        {
            this.royalFlush = true;
            this.strength = 9;
            this.earlyStrenght = 9;

        }

        return this.royalFlush;
    }

    private bool IsStraightFlush(List<Card> cards)
    {
        this.highCard = new List<Card>();
        this.highCard.Add(cards.ElementAt(4));
        if (IsFlush(cards) && IsStraight(cards))
        {
            this.straightFlush = true;
            this.strength = 8;
            this.earlyStrenght = 8;

            return true;
        }

        return false;
    }

    private bool IsFourAKind(List<Card> cards)
    {
        this.highCard = new List<Card>();
        int count;

        for (int i = 0; i < 5; i++)
        {
            count = 0;
            Card _curentCard = cards.ElementAt(i);
            foreach (Card card in cards)
            {
                if (_curentCard.cardValue == card.cardValue)
                    count++;

                if (count == 4)
                {
                    this.fourAKing = true;
                    this.highCard.Add(card);
                    this.strength = 7;
                    this.earlyStrenght = 7;

                    // find the kicker card
                    foreach (Card c in cards)
                    {
                        if ((int)c.cardValue != (int)highCard.ElementAt(0).cardValue)
                            this.highCard.Add(c);
                    }

                    return true;
                }
            }
        }

        return false;
    }

    private bool IsFullHouse(List<Card> cards)
    {
        this.highCard = new List<Card>();
        int count;
        Card match1;
        Card match2 = null;

        for (int i = 0; i < 5; i++)
        {
            count = 0;
            Card _curentCard = cards.ElementAt(i);
            foreach (Card card in cards)
            {
                if (_curentCard.cardValue == card.cardValue)
                    count++;

                // found three of a kind
                if (count == 3)
                {
                    match1 = card;

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
                                    this.highCard.Add(match1);
                                    this.highCard.Add(match2);
                                    this.fullHouse = true;
                                    this.strength = 6;
                                    this.earlyStrenght = 6;

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

    private bool IsFlush(List<Card> cards)
    {
        // check to see what suite the playersAndBots card is and see if all cards
        // share that suite.
        this.highCard = new List<Card>();
        int playerSuite;
        Card card = cards.ElementAt(0);
        playerSuite = (int)card.cardColor;

        foreach (Card c in cards)
        {
            if ((int)c.cardColor != playerSuite)
                return false;
        }

        for (int i = 4; i >= 0; i--)
        {
            this.highCard.Add(cards.ElementAt(i));
        }

        this.flush = true;
        this.strength = 5;
        this.earlyStrenght = 5;

        return true;
    }

    private bool IsStraight(List<Card> cards)
    {
        this.highCard = new List<Card>();
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
                this.straight = true;
                this.strength = 4;

                for (int i = 4; i >= 0; i--)
                {
                    this.highCard.Add(cards.ElementAt(i));
                }
            }

            else if ((int)card1.cardValue == 2 && (int)card5.cardValue == 14 /* ACE */)
            {
                this.straight = true;
                this.strength = 4;
                this.earlyStrenght = 4;

                for (int i = 4; i >= 0; i--)
                {
                    this.highCard.Add(cards.ElementAt(i));
                }
            }
            else
                return this.straight;
        }

        return this.straight;
    }

    private bool IsThreeAKind(List<Card> cards)
    {
        this.highCard = new List<Card>();
        int count;

        for (int i = 0; i < 5; i++)
        {
            count = 0;
            Card _curentCard = cards.ElementAt(i);
            foreach (Card card in cards)
            {
                if (_curentCard.cardValue == card.cardValue)
                    count++;

                if (count == 3)
                {
                    this.highCard.Add(card);
                    this.threeAKind = true;
                    this.strength = 3;
                    this.earlyStrenght = 3;

                    // find the two kickers
                    for (int j = 4; j >= 0; j--)
                    {
                        if (cards.ElementAt(j).cardValue != this.highCard.ElementAt(0).cardValue)
                            this.highCard.Add(cards.ElementAt(j));
                    }
                }
            }
        }

        return this.threeAKind;
    }

    private bool IsTwoPair(List<Card> cards)
    {
        this.highCard = new List<Card>();
        int count;
        Card match1 = null;

        for (int i = 0; i < 5; i++)
        {
            count = 0;
            Card _curentCard = cards.ElementAt(i);

            foreach (Card card in cards)
            {
                if (_curentCard.cardValue == card.cardValue)
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
                Card _curentCard = cards.ElementAt(i);

                if (_curentCard.cardValue != match1.cardValue)
                {
                    foreach (Card card in cards)
                    {
                        if (_curentCard.cardValue == card.cardValue)
                            count++;

                        // found second pair
                        if (count == 2)
                        {
                            this.highCard.Add(card);
                            this.highCard.Add(match1);
                            this.twoPair = true;
                            this.strength = 2;
                            this.earlyStrenght = 2;

                            // get the kicker card
                            for (int j = 4; j >= 0; j--)
                            {
                                if (cards.ElementAt(j).cardValue != this.highCard.ElementAt(0).cardValue
                                    && cards.ElementAt(j).cardValue != this.highCard.ElementAt(1).cardValue)
                                    this.highCard.Add(cards.ElementAt(j));
                            }
                        }
                    }
                }
            }
        }
        return this.twoPair;
    }

    public bool IsPair(List<Card> cards)
    {
        this.highCard = new List<Card>();
        int count;

        for (int i = 0; i < 5; i++)
        {
            count = 0;
            Card _curentCard = cards.ElementAt(i);
            foreach (Card card in cards)
            {
                if (_curentCard.cardValue == card.cardValue)
                    count++;

                if (count == 2)
                {
                    this.pair = true;
                    this.highCard.Add(card);
                    this.strength = 1;
                    this.earlyStrenght = 1;

                    for (int j = 4; j >= 0; j--)
                    {
                        if (this.highCard.ElementAt(0).cardValue != cards.ElementAt(j).cardValue)
                            this.highCard.Add(cards.ElementAt(j));
                    }
                }
            }
        }

        return this.pair;
    }

    private void IsHigh(List<Card> cards)
    {
        this.highCard = new List<Card>();
        for (int j = 4; j >= 0; j--)
        {
            this.highCard.Add(cards.ElementAt(j));
        }

        this.highCards = true;
    }

    public void EarlyTourBotHandControl(List<Card> cards, Player player)
    {

        this.highCard = new List<Card>();
        int count;
        for (int i = 0; i < player.hand.Count; i++)
        {
            count = 0;
            Card _curentCard = cards.ElementAt(i);
            foreach (Card card in cards)
            {
                if (_curentCard.cardValue == CardValue.King || _curentCard.cardValue == CardValue.Queen ||
                    _curentCard.cardValue == CardValue.Jack || _curentCard.cardValue == CardValue.Ace)
                    count++;

                if (count == 2)
                {
                    this.earlyStrenght = 1;
                    Debug.Log("Player; " + player.name + " / İlk tur kart gücü yüksek.");

                }
            }
            count = 0;
            foreach (Card card in cards)
            {
                if (_curentCard.cardValue == card.cardValue)
                    count++;

                if (count == 2)
                {

                    this.earlyStrenght = 1;
                    Debug.Log("Player; " + player.name + " / İlk tur kart gücü pair.");

                }
            }
            count = 0;
            foreach (Card card in cards)
            {
                if (_curentCard.cardValue - card.cardValue == 1)
                    count++;

                if (count == 2)
                {

                    this.earlyStrenght = 1;
                    Debug.Log("Player; " + player.name + " / İlk tur el sıralı.");

                }
            }

        }
    }
}