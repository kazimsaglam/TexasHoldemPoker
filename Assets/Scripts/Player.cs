using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class Player : MonoBehaviour
{
    public string playerName;
    public int money;

    public List<Card> hand;
    public int handValue;
    public string handValueString;
    public bool isFolded = false;
    public int betAmount; // Oyuncunun mevcut bahisi
    public int betRoundIndex;

    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI betText;
    public GameObject playerActionTextPrefab;
    public Transform playerActionTextContainer;


    public void SetPlayer(string name, int startingMoney)
    {
        this.gameObject.name = name;
        playerName = name;
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

        ShowPlayerAction("Call");
    }

    public void Call() // amount: Masadaki mevcut bahis
    {
        int callAmount = Mathf.Min(GameController.instance.currentBet, money);
        MakeBet(callAmount);
    }

    public void Fold()
    {
        isFolded = true;
    }

    protected void Check()
    {
        // Bahis yapmadan s�ray� bir sonraki oyuncuya ge�mek i�in kullan�l�r
        if (GameController.instance.minimumBet >= money)
        {
            // Masadaki en y�ksek bahis minimum bahse e�it veya daha d���kse check yap�l�r
            // Oyuncu elindeki t�m fi�leri pot'a koyar
            money -= GameController.instance.minimumBet;
            GameController.instance.pot += GameController.instance.minimumBet;
        }
    }

    public void Raise(int amount)
    {
        // Bahsi y�kseltmek i�in kullan�l�r
        int raiseAmount = Mathf.Min(amount, money);
        MakeBet(raiseAmount);
    }

    public void AllIn()
    {
        // T�m fi�lerini masaya koymak i�in kullan�l�r
        money += GameController.instance.pot; // Pot'a e�it miktarda fi� eklenir
        GameController.instance.pot = 0; // Pot s�f�rlan�r
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


    // Checks what kind of match you have onyour hand and writes it to the UI
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

}
