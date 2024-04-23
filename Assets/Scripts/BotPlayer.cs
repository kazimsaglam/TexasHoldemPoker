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
    {//Bl�f Man

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
            Debug.Log("Player; " + playerName + " / Medium bot karar�: " + karar);

        }
        else if (botStraight == 1)
        {
            karar = Random.Range(3, 4);
            Debug.Log("Player; " + playerName + " / Medium bot karar�: " + karar);

        }
        else if (botStraight > 2)
        {
            karar = Random.Range(5, 9);
            Debug.Log("Player; " + playerName + " / Medium bot karar�: " + karar);

        }
        else if (botStraight > 3)
        {
            karar = Random.Range(9, 12);
            Debug.Log("Player; " + playerName + " / Easy bot karar: " + karar);
        }
    }
    private void HardBot()
    {//Do�rucu Davut

        botStraight = earlyTourBotHandValue;
        if (botStraight == 0)
        {

            karar = Random.Range(0, 2);
            Debug.Log("Player; " + playerName + " / Hard bot karar: " + karar);

        }
        else if (botStraight == 1)
        {

            karar = Random.Range(3, 4);
            Debug.Log("Player; " + playerName + " / Hard bot karar: " + karar);
        }
        else if (botStraight > 2)
        {
            karar = Random.Range(5, 9);
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
            Debug.Log("Easy bot �al��t�.");
        }
        else if (botsPower < 7)
        {
            MediumBot();
            Debug.Log("Medium bot �al��t�.");
        }
        else
        {
            HardBot();
            Debug.Log("Hard bot �al��t�.");

        }

        State = GameController.instance._gameState;

        // Karar verin:
        if (ShouldFold(currentBet, pot, karar))
        {
            BotFold();
            UIManager.instance.Fold();
            Debug.Log(playerName + " / Fold �al��t�. Karar: " + karar);
        }
        else if (ShouldCheck(currentBet, pot, karar))
        {
            // Check
            if (State != GameState.PreFlop)
            {
                BotCheck();
                Debug.Log("Player; " + playerName + " / Check �al��t�. Karar: " + karar);
            }
            else
            {
                BotCall(currentBet, pot); 
                UIManager.instance.Call();
                Debug.Log(" ZORUNLU " + "Player; " + playerName + " / Call �al��t�. Karar: " + karar);
            }

        }
        else if (ShouldCall(currentBet, pot, karar))
        {
            // Call
            //betAmount = GameController.instance.currentBet;??
            BotCall(currentBet, pot); //
            UIManager.instance.Call();
            Debug.Log("Player; " + playerName + " / Call �al��t�. Karar: " + karar);

        }
        else if (ShouldRaise(currentBet, pot, karar))
        {
            // Raise
            int raiseAmount = Mathf.Clamp(currentBet + 20, 20, 200);
            betAmount = raiseAmount;
            BotRaise(raiseAmount);
            Debug.Log("Player; " + playerName + " / Raise �al��t�. Karar; " + karar);

        }

    }

    bool ShouldFold(int currentBet, int pot, int karar)
    {
        // El zay�fsa ve bahis y�ksekse fold et
        return currentBet > pot / 4 && karar <= 2;
    }

    bool ShouldCheck(int currentBet, int pot, int karar)
    {
        // El orta seviyedeyse ve bahis makul ise check et
        return currentBet <= pot / 2 && karar <= 4 || karar >= 3;
    }

    bool ShouldCall(int currentBet, int pot, int karar)
    {
        // El g��l� ise ve bahis makul ise call et
        return currentBet <= pot && karar <= 9 || karar >= 5;
    }

    bool ShouldRaise(int currentBet, int pot, float karar)
    {
        // El �ok g��l�yse ve bahis d���kse raise et
        return currentBet < pot / 4 && karar <= 12 || karar > 10;
    }

    public void BotFold()
    {
        isFolded = true;
    }

    protected void BotCheck()
    {
        // Skip betting
        if (GameController.instance.minimumBet >= money)
        {
            // If the highest bet on the table is equal or smaller than the minimum bet, player can check.
            money -= GameController.instance.minimumBet;
            GameController.instance.pot += GameController.instance.minimumBet;
        }
    }

    public void BotCall(int currentBet, int karar)
    {
        int callAmount = Mathf.Min(GameController.instance.currentBet, money);
        MakeBet(callAmount);
    }

    public void BotRaise(int amount)
    {
        // Raise the bet.
        int raiseAmount = Mathf.Min(amount, money);
        MakeBet(raiseAmount);
    }
}