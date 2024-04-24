using UnityEngine;

public class BotPlayer : Player
{
    private int botStraight;
    private int karar;
    GameState State;


    private void Start()
    {

        gameObject.tag = "Bot";

    }

    private void EasyBot()
    {//Blöf Man

        botStraight = earlyTourBotHandValue;
        if (botStraight == 0)
        {
            karar = Random.Range(0, 4); 
            Debug.Log("Player; " + playerName + " / Easy bot karar: " + karar);

        }
        else if (botStraight == 1)
        {
            karar = Random.Range(4, 6);
            Debug.Log("Player; " + playerName + " / Easy bot karar: " + karar);

        }
        else if (botStraight == 2 || botStraight == 3)
        {
            karar = Random.Range(6, 9);
            Debug.Log("Player; " + playerName + " / Easy bot karar: " + karar);

        }
        else if (botStraight > 3)
        {
            karar = Random.Range(9, 12);
            Debug.Log("Player; " + playerName + " / Easy bot karar: " + karar);
        }

    }
    private void MediumBot()
    {//Halktan Biri
        botStraight = earlyTourBotHandValue;
        if (botStraight == 0)
        {
            karar = Random.Range(0, 2);
            Debug.Log("Player; " + playerName + " / Medium bot kararý: " + karar);

        }
        else if (botStraight == 1)
        {
            karar = Random.Range(2, 4);
            Debug.Log("Player; " + playerName + " / Medium bot kararý: " + karar);

        }
        else if (botStraight > 2)
        {
            karar = Random.Range(4, 9);
            Debug.Log("Player; " + playerName + " / Medium bot kararý: " + karar);

        }
        else if (botStraight > 3)
        {
            karar = Random.Range(9, 12);
            Debug.Log("Player; " + playerName + " / Easy bot karar: " + karar);
        }
    }
    private void HardBot()
    {//Doðrucu Davut

        botStraight = earlyTourBotHandValue;
        if (botStraight == 0)
        {

            karar = Random.Range(0, 2);
            Debug.Log("Player; " + playerName + " / Hard bot karar: " + karar);

        }
        else if (botStraight == 1)
        {

            karar = Random.Range(2, 4);
            Debug.Log("Player; " + playerName + " / Hard bot karar: " + karar);
        }
        else if (botStraight > 2)
        {
            karar = Random.Range(4, 9);
            Debug.Log("Player; " + playerName + " / Hard bot karar: " + karar);
        }
        else if (botStraight > 3)
        {
            karar = Random.Range(9, 12);
            Debug.Log("Player; " + playerName + " / Easy bot karar: " + karar);
        }
    }

    public void MakeDecision(int currentBet = 0, int pot = 0)
    {
        if (botsPower < 3)
        {
            EasyBot();
            Debug.Log("Easy bot çalýþtý.");
        }
        else if (botsPower < 7)
        {
            MediumBot();
            Debug.Log("Medium bot çalýþtý.");
        }
        else
        {
            HardBot();
            Debug.Log("Hard bot çalýþtý.");

        }

        State = GameController.instance._gameState;

        // Karar verin:
        if (ShouldFold(currentBet, pot, karar))
        {
            BotFold();
            UIManager.instance.Fold();
            Debug.Log(playerName + " / Fold çalýþtý. Karar: " + karar);
        }
        else if (ShouldCall(currentBet, pot, karar))
        {
            BotCall();
            Debug.Log("Player; " + playerName + " / Call çalýþtý. Karar: " + karar);

        }
        else if (ShouldRaise(currentBet, pot, karar))
        {
            // Raise
            int raiseAmount = Mathf.Clamp(currentBet + 20, 20, 200);
            betAmount += raiseAmount;
            BotRaise(raiseAmount);
            Debug.Log("Player; " + playerName + " / Raise çalýþtý. Karar; " + karar);

        }

    }

    bool ShouldFold(int currentBet, int pot, int karar)
    {
        // El zayýfsa ve bahis yüksekse fold et
        return currentBet > pot / 4 && karar <= 2;
    }

    bool ShouldCall(int currentBet, int pot, int karar)
    {
        // El güçlü ise ve bahis makul ise call et
        return currentBet <= pot && karar <= 9 || karar >= 5;
    }

    bool ShouldRaise(int currentBet, int pot, float karar)
    {
        // El çok güçlüyse ve bahis düþükse raise et
        return currentBet < pot / 4 && karar <= 12 || karar > 10;
    }

    public void BotFold()
    {
        isFolded = true;

        ShowPlayerAction("Fold");
        SoundManager.instance.PlayFoldSound();
    }

    public void BotCall()
    {
        int callAmount = Mathf.Min(GameController.instance.currentBet, money);
        ShowPlayerAction("Call");
        MakeBet(callAmount);
    }

    public void BotRaise(int amount)
    {
        // Raise the bet.
        int raiseAmount = Mathf.Min(amount, money);
        ShowPlayerAction($"RaIse: {raiseAmount}");
        MakeBet(raiseAmount);
    }
}