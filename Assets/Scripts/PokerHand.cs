using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PokerHand
{
    public bool RYL_FLUSH;
    public bool STRAIGHT_FLUSH;
    public bool FOUR_A_KIND;
    public bool FULL_HOUSE;
    public bool FLUSH;
    public bool STRAIGHT;
    public bool THREE_A_KIND;
    public bool TWO_PAIR;
    public bool PAIR;
    public bool HIGH_CARD;
    public List<Card> highCard;
    public int strength;
    public int earlyStrenght;


    public PokerHand()
    {
        this.RYL_FLUSH = false;
        this.STRAIGHT_FLUSH = false;
        this.FOUR_A_KIND = false;
        this.FULL_HOUSE = false;
        this.FLUSH = false;
        this.STRAIGHT = false;
        this.THREE_A_KIND = false;
        this.TWO_PAIR = false;
        this.PAIR = false;
        this.HIGH_CARD = false;
        this.highCard = new List<Card>();
        this.strength = 0;
        this.earlyStrenght = 0;

    }

    public void setPokerHand(Card[] cardArray)
    {
        List<Card> cards = new List<Card>();
        foreach (Card c in cardArray)
        {
            cards.Add(c);
        }

        if (isRoyal(cards)) return;
        if (isStraightFlush(cards)) return;
        if (isFourAKind(cards)) return;
        if (isFullHouse(cards)) return;
        if (isFlush(cards)) return;
        if (isStraight(cards)) return;
        if (isThreeAKind(cards)) return;
        if (isTwoPair(cards)) return;
        if (isPair(cards)) return;
        isHigh(cards);
    }
    public void BotAISetPokerHand(Card[] cardArray)
    {
        List<Card> cards = new List<Card>();
        foreach (Card c in cardArray)
        {
            cards.Add(c);
        }

        if (isRoyal(cards) || isStraightFlush(cards) || isFourAKind(cards) || isFullHouse(cards) ||
            isFlush(cards) || isStraight(cards) || isThreeAKind(cards) || isTwoPair(cards) || isPair(cards))
        {
            return;
        }

        isHigh(cards);
    }

    public String printResult()
    {
        if (this.RYL_FLUSH)
        {
            return "Royal Flush" + "- Strength:" + this.strength;
        }
        else if (this.STRAIGHT_FLUSH)
        {
            return "Straight Flush" + "- Strength:" + this.strength;
        }
        else if (this.FOUR_A_KIND)
        {
            return "Four of a kind" + "- Strength:" + this.strength;
        }
        else if (this.FULL_HOUSE)
        {
            return "Full house" + "- Strength:" + this.strength;
        }
        else if (this.FLUSH)
        {
            return "Flush" + "- Strength:" + this.strength;
        }
        else if (this.STRAIGHT)
        {
            return "Straight" + "- Strength:" + this.strength;
        }
        else if (this.THREE_A_KIND)
        {
            return "Three of a kind" + "- Strength:" + this.strength;
        }
        else if (this.TWO_PAIR)
        {
            return "Two Pair" + "- Strength:" + this.strength;
        }
        else if (this.PAIR)
        {
            return "Pair" + "- Strength:" + this.strength;
        }
        else if (this.HIGH_CARD)
        {
            return "High card" + "- Strength:" + this.strength;
        }
        else
            return "error setting hand.";
    }
    public String BotAIprintResult()
    {

        if (this.PAIR)
        {
            return "Pair" + "- Strength:" + this.strength;
        }
        else if (this.TWO_PAIR)
        {
            return "Two Pair" + "- Strength:" + this.strength;
        }
        else if (this.THREE_A_KIND)
        {
            return "Three of a kind" + "- Strength:" + this.strength;
        }
        else if (this.STRAIGHT)
        {
            return "Straight" + "- Strength:" + this.strength;
        }
        else if (this.FLUSH)
        {
            return "Flush" + "- Strength:" + this.strength;
        }
        else if (this.FULL_HOUSE)
        {
            return "Full house" + "- Strength:" + this.strength;
        }
        else if (this.FOUR_A_KIND)
        {
            return "Four of a kind" + "- Strength:" + this.strength;
        }
        else if (this.STRAIGHT_FLUSH)
        {
            return "Straight Flush" + "- Strength:" + this.strength;
        }
        else if (this.RYL_FLUSH)
        {
            return "Royal Flush" + "- Strength:" + this.strength;
        }
        else if (this.HIGH_CARD)
        {
            return "High card" + "- Strength:" + this.strength;
        }
        else
            return "error setting hand.";
    }

    private bool isRoyal(List<Card> cards)
    {
        this.highCard = new List<Card>();
        this.highCard.Add(cards.ElementAt(4));

        if (isFlush(cards) && isStraight(cards) && this.highCard.ElementAt(0).cardValue == CardValue.Ace)
        {
            this.RYL_FLUSH = true;
            this.strength = 9;
            this.earlyStrenght = 9;

        }

        return this.RYL_FLUSH;
    }

    private bool isStraightFlush(List<Card> cards)
    {
        this.highCard = new List<Card>();
        this.highCard.Add(cards.ElementAt(4));
        if (isFlush(cards) && isStraight(cards))
        {
            this.STRAIGHT_FLUSH = true;
            this.strength = 8;
            this.earlyStrenght = 8;

            return true;
        }

        return false;
    }

    private bool isFourAKind(List<Card> cards)
    {
        this.highCard = new List<Card>();
        int count;

        for (int i = 0; i < 5; i++)
        {
            count = 0;
            Card curentCard = cards.ElementAt(i);
            foreach (Card card in cards)
            {
                if (curentCard.cardValue == card.cardValue)
                    count++;

                if (count == 4)
                {
                    this.FOUR_A_KIND = true;
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

    private bool isFullHouse(List<Card> cards)
    {
        this.highCard = new List<Card>();
        int count;
        Card match1;
        Card match2 = null;

        for (int i = 0; i < 5; i++)
        {
            count = 0;
            Card curentCard = cards.ElementAt(i);
            foreach (Card card in cards)
            {
                if (curentCard.cardValue == card.cardValue)
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
                                    this.FULL_HOUSE = true;
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

    private bool isFlush(List<Card> cards)
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

        this.FLUSH = true;
        this.strength = 5;
        this.earlyStrenght = 5;

        return true;
    }

    private bool isStraight(List<Card> cards)
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
                this.STRAIGHT = true;
                this.strength = 4;

                for (int i = 4; i >= 0; i--)
                {
                    this.highCard.Add(cards.ElementAt(i));
                }
            }

            else if ((int)card1.cardValue == 2 && (int)card5.cardValue == 14 /* ACE */)
            {
                this.STRAIGHT = true;
                this.strength = 4;
                this.earlyStrenght = 4;

                for (int i = 4; i >= 0; i--)
                {
                    this.highCard.Add(cards.ElementAt(i));
                }
            }
            else
                return this.STRAIGHT;
        }

        return this.STRAIGHT;
    }

    private bool isThreeAKind(List<Card> cards)
    {
        this.highCard = new List<Card>();
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
                    this.highCard.Add(card);
                    this.THREE_A_KIND = true;
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

        return this.THREE_A_KIND;
    }

    private bool isTwoPair(List<Card> cards)
    {
        this.highCard = new List<Card>();
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
                            this.highCard.Add(card);
                            this.highCard.Add(match1);
                            this.TWO_PAIR = true;
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

        return this.TWO_PAIR;
    }

    private bool isPair(List<Card> cards)
    {
        this.highCard = new List<Card>();
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
                    this.PAIR = true;
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

        return this.PAIR;
    }

    private void isHigh(List<Card> cards)
    {
        this.highCard = new List<Card>();
        for (int j = 4; j >= 0; j--)
        {
            this.highCard.Add(cards.ElementAt(j));
        }

        this.HIGH_CARD = true;
    }

    public void EarlyTourBotHandControl(List<Card> cards, Player player)
    {

        this.highCard = new List<Card>();
        int count;
        for (int i = 0; i < player.hand.Count; i++)
        {
            count = 0;
            Card curentCard = cards.ElementAt(i);
            foreach (Card card in cards)
            {
                if (curentCard.cardValue == CardValue.King || curentCard.cardValue == CardValue.Queen ||
                    curentCard.cardValue == CardValue.Jack || curentCard.cardValue == CardValue.Ace)
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
                if (curentCard.cardValue == card.cardValue)
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
                if (curentCard.cardValue - card.cardValue == 1)
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