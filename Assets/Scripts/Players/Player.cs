using Database;
using Game;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum PlayerType
{
    None,
    Player,
    Bot,
}

public class Player : MonoBehaviour
{
    public static Player instance;
    public string playerName;
    public int money;


    public List<Card> hand;
    public int handValue;
    public string handValueString;
    public int earlyTourBotHandValue;
    public string earlyTourBotHandValueString;
    public bool isFolded = false;

    public int betAmount;
    public int betRoundIndex;
    public PlayerType playerType;
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI betText;
    public GameObject playerActionTextPrefab;
    public Transform playerActionTextContainer;
    GameState _gameState;



    private void Awake()
    {
        instance = this;

    }
    public void SetPlayer(string fullName, int startingMoney, PlayerType player, int botPower)
    {
        gameObject.name = fullName;
        playerName = fullName;
        playerType = player;
        money = startingMoney;
        hand = new List<Card>();
        betAmount = 0;
        isFolded = false;
    }

    public void MakeBet(int amount)
    {
        betAmount += amount;
        money -= amount;
        UIManager.instance.UpdatePot(amount);
        GameController.instance.AddToCurrentBet(amount);

        if (playerType == PlayerType.Player)
        {
            PlayerManager.Instance.playerMoney -= GameController.instance.minimumBet;
            Debug.Log(PlayerManager.Instance.playerMoney);
            FirebaseAuthManager.Instance.UpdateMoney(PlayerManager.Instance.playerMoney);
        }

        SoundManager.instance.PlayCallAndRaiseSound();
    }


    public void ClearBets()
    {
        betAmount = 0;
    }
    public void ClearHand()
    {
        hand.Clear();
    }


    public void ShowPlayerAction(string action)
    {
        GameObject textObj = Instantiate(playerActionTextPrefab, playerActionTextContainer);
        textObj.GetComponent<TextMeshProUGUI>().text = $"- {action}";
    }


    // Checks what kind of match you have on your hand and writes it to the UI
    public void CompareHand(List<Card> boardCards)
    {
        List<Card> handToCompare = new List<Card>();

        //cards on the board
        for (int i = 0; i < boardCards.Count; i++)
        {
            if (boardCards[i] != null)
            {
                handToCompare.Add(boardCards[i]);
            }
        }

        //cards on the hand of player
        for (int i = 0; i < hand.Count; i++)
        {
            handToCompare.Add(hand[i]);
        }

        PokerHand pk = new PokerHand();

        //compare them out
        pk.SetPokerHand(handToCompare.ToArray());

        handValue = pk.strength;
        handValueString = pk.PrintResult();
    }

    public int ComparePairCard(Player otherPlayer, List<Card> boardCards)
    {

        List<Card> combinedHand = GameController.instance.tiedPlayers[0].hand;
        combinedHand.AddRange(boardCards);
        combinedHand.Sort((x, y) => y.cardValue.CompareTo(x.cardValue));

        List<Card> otherCombinedHand = GameController.instance.tiedPlayers[1].hand;
        otherCombinedHand.AddRange(boardCards);
        otherCombinedHand.Sort((x, y) => y.cardValue.CompareTo(x.cardValue));

        // Pair kartlarını bul
        Card thisPairCard = GetPairCard(combinedHand);
        Card otherPairCard = GetPairCard(otherCombinedHand);

        if (thisPairCard == null || otherPairCard == null)
        {
            return 0;
        }

        if (thisPairCard.cardValue > otherPairCard.cardValue)
        {
            return 1;
        }
        else if (thisPairCard.cardValue < otherPairCard.cardValue)
        {
            return -1;
        }
        else
        {
            return 0;
        }

    }
    public int CompareTwoPairCard(Player otherPlayer, List<Card> boardCards)
    {
        List<Card> combinedTwoPairHand = GameController.instance.tiedPlayers[0].hand;
        combinedTwoPairHand.AddRange(boardCards);
        combinedTwoPairHand.Sort((x, y) => y.cardValue.CompareTo(x.cardValue));

        List<Card> otherCombinedTwoPairHand = GameController.instance.tiedPlayers[1].hand;
        otherCombinedTwoPairHand.AddRange(boardCards);
        otherCombinedTwoPairHand.Sort((x, y) => y.cardValue.CompareTo(x.cardValue));


        GetTwoPairCard(combinedTwoPairHand);
        List<Card> thisTwoPairCard = pairCards;
        thisTwoPairCard[0] = pairCards[0];
        thisTwoPairCard[1] = pairCards[1];

        GetTwoPairCard(otherCombinedTwoPairHand);
        List<Card> otherTwoPairCard = pairCards;
        otherTwoPairCard[0] = pairCards[2];
        otherTwoPairCard[1] = pairCards[3];

        if (thisTwoPairCard == null || otherTwoPairCard == null)
        {
            return 0;

        }
        if (thisTwoPairCard[0].cardValue > otherTwoPairCard[0].cardValue || thisTwoPairCard[1].cardValue > otherTwoPairCard[1].cardValue)
        {
            return 1;
        }
        else if (thisTwoPairCard[0].cardValue < otherTwoPairCard[0].cardValue || thisTwoPairCard[1].cardValue < otherTwoPairCard[1].cardValue)
        {

            return -1;
        }
        else
        {
            return 0;
        }
    }
    public int CompareHighestCard(Player otherPlayer, List<Card> boardCards)
    {
        List<Card> combinedHighestHand = GameController.instance.tiedPlayers[0].hand;
        if (GameController.instance.tiedPlayers[0].hand.Count < 7)
        {
            combinedHighestHand.AddRange(boardCards);
            combinedHighestHand.Sort((x, y) => y.cardValue.CompareTo(x.cardValue));
        }

        List<Card> otherCombinedHighestHand = GameController.instance.tiedPlayers[1].hand;
        if (GameController.instance.tiedPlayers[0].hand.Count < 7)
        {
            otherCombinedHighestHand.AddRange(boardCards);
            otherCombinedHighestHand.Sort((x, y) => y.cardValue.CompareTo(x.cardValue));
        }

        for (int i = 0; i < combinedHighestHand.Count; i++)
        {
            if (combinedHighestHand[i].cardValue > otherCombinedHighestHand[i].cardValue)
            {
                return 1;
            }
            else if (combinedHighestHand[i].cardValue < otherCombinedHighestHand[i].cardValue)
            {
                return -1;
            }
        }

        return 0;

    }

    private Card GetPairCard(List<Card> hand)
    {

        for (int i = 0; i < hand.Count - 1; i++)
        {
            if (hand[i].cardValue == hand[i + 1].cardValue)
            {
                return hand[i];
            }
        }
        return null;
    }
    List<Card> pairCards = new List<Card>();
    private Card GetTwoPairCard(List<Card> hand)
    {
        for (int i = 0; i < hand.Count - 1; i++)
        {
            if (hand[i].cardValue == hand[i + 1].cardValue)
            {
                pairCards.Add(hand[i]);
                i++;
            }

            if (pairCards.Count == 2)
            {

                return pairCards[i];

            }
        }
        return null;
    }

    public void BotHandControl(List<Card> boardCards, Player player)
    {
        _gameState = GameController.instance.gameState;

        List<Card> botHandList = new List<Card>();

        if (player.hand.Count > 1 && _gameState == GameState.PreFlop && player.isFolded == false)
        {
            for (int i = 0; i < hand.Count; i++)
            {
                botHandList.Add(hand[i]);
            }

            PokerHand ptk = new PokerHand();
            ptk.EarlyTourBotHandControl(botHandList, player);
            earlyTourBotHandValue = ptk.earlyStrenght;

            Debug.Log("İlk tur kart kontrolü yapıldı. " + "Player; " + player.name + " , El gücü " + earlyTourBotHandValue);

        }
        else if (_gameState != GameState.PreFlop && _gameState != GameState.Showdown && player.isFolded == false)
        {
            for (int i = 0; i < hand.Count; i++)
            {
                botHandList.Add(hand[i]);
            }

            for (int i = 0; i < boardCards.Count; i++)
            {
                botHandList.Add(boardCards[i]);
            }

            PokerHand ptk = new PokerHand();

            ptk.BotAISetPokerHand(botHandList.ToArray());
            earlyTourBotHandValue = ptk.earlyStrenght;
            earlyTourBotHandValueString = ptk.BotAIprintResult();

            Debug.Log(_gameState + " turunda kart kontrolü yapıldı. " + "Player; " + player.name + " / Hand.Count; " + botHandList.Count);
            Debug.Log("Player; " + player.name + " / El gücü: " + earlyTourBotHandValue);
        }
    }


}