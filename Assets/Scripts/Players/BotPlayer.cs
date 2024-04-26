using UnityEngine;

public class BotPlayer : Player
{
    private int botStraight;
    private int decision;
    GameState _state;


    private void Start()
    {

        gameObject.tag = "Bot";

    }

    private void EasyBot()
    {//Bl�f Man

        botStraight = earlyTourBotHandValue;
        if (botStraight == 0)
        {
            decision = Random.Range(0, 4); 
            Debug.Log("Player; " + playerName + " / Easy bot karar: " + decision);

        }
        else if (botStraight == 1)
        {
            decision = Random.Range(4, 7);
            Debug.Log("Player; " + playerName + " / Easy bot karar: " + decision);

        }
        else if (botStraight > 3 )
        {
            decision = Random.Range(7, 12);
            Debug.Log("Player; " + playerName + " / Easy bot karar: " + decision);

        }
        

    }
    private void MediumBot()
    {//Halktan Biri
        botStraight = earlyTourBotHandValue;
        if (botStraight == 0)
        {
            decision = Random.Range(0, 3);
            Debug.Log("Player; " + playerName + " / Medium bot karar�: " + decision);

        }
        else if (botStraight == 1)
        {
            decision = Random.Range(4, 9);
            Debug.Log("Player; " + playerName + " / Medium bot karar�: " + decision);

        }
        else if (botStraight > 2)
        {
            decision = Random.Range(10, 12);
            Debug.Log("Player; " + playerName + " / Medium bot karar�: " + decision);

        }
        
    }
    private void HardBot()
    {

        botStraight = earlyTourBotHandValue;
        if (botStraight == 0)
        {

            decision = Random.Range(0, 2);
            Debug.Log("Player; " + playerName + " / Hard bot karar: " + decision);

        }
        else if (botStraight == 1)
        {

            decision = Random.Range(5, 8);
            Debug.Log("Player; " + playerName + " / Hard bot karar: " + decision);
        }
        else if (botStraight >= 2)
        {
            decision = Random.Range(9, 12);
            Debug.Log("Player; " + playerName + " / Hard bot karar: " + decision);
        }
      
    }

    public void MakeDecision(int currentBet = 0, int pot = 0)
    {
        if (botsPower < 3)
        {
            EasyBot();
            Debug.Log("Easy bot �al��t�.");
        }
        else if (botsPower > 3 && botStraight < 7)
        {
            MediumBot();
            Debug.Log("Medium bot �al��t�.");
        }
        else
        {
            HardBot();
            Debug.Log("Hard bot �al��t�.");

        }

        _state = GameController.instance._gameState;

        // Karar verin:
        if (ShouldFold(currentBet, pot, decision))
        {
            BotFold();
            UIManager.instance.Fold();
            Debug.Log(playerName + " / Fold �al��t�. Karar: " + decision);
        }
        else if (ShouldCall(currentBet, pot, decision))
        {
            BotCall();
            Debug.Log("Player; " + playerName + " / Call �al��t�. Karar: " + decision);

        }
        else if (ShouldRaise(currentBet, pot, decision))
        {
            // Raise
            int raiseAmount = Mathf.Clamp(currentBet + 20, 20, 200);
            betAmount += raiseAmount;
            BotRaise(raiseAmount);
            Debug.Log("Player; " + playerName + " / Raise �al��t�. Karar; " + decision);

        }

    }

    bool ShouldFold(int currentBet, int pot, int karar)
    {
        // El zay�fsa ve bahis y�ksekse fold et
        return currentBet > pot / 4 && karar <= 2;

    }

    bool ShouldCall(int currentBet, int pot, int karar)
    {
        // El g��l� ise ve bahis makul ise call et
        return currentBet <= pot && karar <= 8 && karar >= 3;
    }

    bool ShouldRaise(int currentBet, int pot, float karar)
    {
        // El �ok g��l�yse ve bahis d���kse raise et
        return currentBet < pot / 4 && karar <= 12 && karar > 9;

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