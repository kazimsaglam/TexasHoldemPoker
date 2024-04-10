using System.Collections.Generic;
using Database;
using Game;
using TMPro;
using UnityEngine;

public enum PlayerType
{
  Player,
  Bot,
  None
}

public class Player : MonoBehaviour
{
  public string playerName;
  public int money;
  public PlayerType playerType;

  public List<Card> hand;
  public int handValue;
  public string handValueString;
  public bool isFolded = false;
  public int betAmount; // Player's current bet
  public int betRoundIndex;

  public TextMeshProUGUI moneyText;
  public TextMeshProUGUI betText;
  public GameObject playerActionTextPrefab;
  public Transform playerActionTextContainer;


  public void SetPlayer(string fullName, int startingMoney, PlayerType type)
  {
    gameObject.name = fullName;
    playerName = fullName;
    money = startingMoney;
    playerType = type;
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

    ShowPlayerAction("Call");
  }

  public void Call() // amount: Current bet
  {
    int callAmount = Mathf.Min(GameController.instance.currentBet, money);
    MakeBet(callAmount);
    if (playerType == PlayerType.Player)
    {
      PlayerManager.Instance.playerMoney -= callAmount;
      Debug.Log(PlayerManager.Instance.playerMoney);
      FirebaseAuthManager.Instance.UpdateMoney(PlayerManager.Instance.playerMoney);
    }
  }

  public void Fold()
  {
    isFolded = true;
  }

  protected void Check()
  {
    // Bahis yapmadan sırayı bir sonraki oyuncuya geçmek için kullanılır
    if (GameController.instance.minimumBet >= money)
    {
      // Masadaki en yüksek bahis minimum bahse eşit veya daha düşükse check yapılır
      // Oyuncu elindeki tüm fişleri pot'a koyar
      money -= GameController.instance.minimumBet;
      GameController.instance.pot += GameController.instance.minimumBet;
    }
  }

  public void Raise(int amount)
  {
    // Raise the bet
    int raiseAmount = Mathf.Min(amount, money);
    MakeBet(raiseAmount);
  }

  public void AllIn()
  {
    // Tüm fişlerini masaya koymak için kullanılır
    money += GameController.instance.pot; // Pot'a eşit miktarda fiş eklenir
    GameController.instance.pot = 0; // Pot sıfırlanır
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
    textObj.GetComponent<TextMeshProUGUI>().text = action;
  }


  // Checks what kind of match you have in your hand and writes it to the UI
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
}