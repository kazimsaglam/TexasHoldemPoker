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
    public string playerName;
    public int money;
    public static int botsPower;


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

    GameState gameState;


    public void SetPlayer(string fullName, int startingMoney, PlayerType player, int botPower)
    {
        gameObject.name = fullName;
        playerName = fullName;
        playerType = player;
        botsPower = botPower;
        money = startingMoney;
        hand = new List<Card>();
        betAmount = 0;
        isFolded = false;
    }


    public void MakeBet(int amount)
    {
        betAmount = amount;
        money -= amount;
        UIManager.instance.UpdatePot(amount);
        GameController.instance.AddToCurrentBet(amount);

        if (playerType == PlayerType.Player)
        {
            PlayerManager.Instance.playerMoney -= GameController.instance.minimumBet;
            Debug.Log(PlayerManager.Instance.playerMoney);
            FirebaseAuthManager.Instance.UpdateMoney(PlayerManager.Instance.playerMoney);
        }

        ShowPlayerAction("Call");

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
        pk.setPokerHand(handToCompare.ToArray());

        handValue = pk.strength;
        handValueString = pk.printResult();
    }

    public int CompareHighestCard(Player otherPlayer)
    {
        //Sorts the cards in players' hands according to their value
        hand.Sort((x, y) => y.cardValue.CompareTo(x.cardValue));
        otherPlayer.hand.Sort((x, y) => y.cardValue.CompareTo(x.cardValue));

        for (int i = 0; i < hand.Count; i++)
        {
            if (hand[i].cardValue > otherPlayer.hand[i].cardValue)
            {
                return 1; // This player's card is stronger
            }

            if (hand[i].cardValue < otherPlayer.hand[i].cardValue)
            {
                return -1; // Other player's card is stronger
            }
        }

        return 0; // equal card strength
    }
    public void BotHandControl(List<Card> boardCards, Player player)
    {

        gameState = GameController.instance._gameState;

        List<Card> botHandList = new List<Card>();

        if (player.hand != null && gameState == GameState.PreFlop && player.isFolded == false)
        {
            for (int i = 0; i < hand.Count; i++)
            {
                botHandList.Add(hand[i]);

            }
            PokerHand ptk = new PokerHand();
            ptk.EarlyTourBotHandControl(botHandList, player);
            earlyTourBotHandValue = ptk.earlyStrenght;

            Debug.Log("Ýlk tur kart kontrolü yapýldý. " + "Player; " + player.name + " , El gücü " + earlyTourBotHandValue);
            //botHandList.Sort((x, y) => y.cardValue.CompareTo(x.cardValue));
            //Debug.Log("Bot hand list: " + string.Join(", ", botHandList.Select(card => card.cardValue.ToString())));

        }
        else if (gameState != GameState.PreFlop && player.isFolded == false)
        {
            for (int i = 0; i < hand.Count; i++)
            {

                botHandList.Add(hand[i]);

            }
            for (int i = 0; i < boardCards.Count; i++)
            {

                botHandList.Add(boardCards[i]);

            }
            Debug.Log(gameState + " turunda kart kontrolü yapýldý. " + "Player; " + player.name + " / Hand.Count; " + botHandList.Count);
            PokerHand ptk = new PokerHand();

            ptk.BotAISetPokerHand(botHandList.ToArray());
            earlyTourBotHandValue = ptk.earlyStrenght;
            earlyTourBotHandValueString = ptk.BotAIprintResult();
            Debug.Log("Player; " + player.name + " / El gücü: " + earlyTourBotHandValue);
        }
    }
}