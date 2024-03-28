using System.Collections;
using UnityEngine;

public class BotPlayer : Player
{
    public BotPlayer(string name, int startingMoney) : base(name, startingMoney)
    {
        playerName = name;
        money = startingMoney;
    }

    public void MakeDecision(int currentBet, int pot)
    {
        // Rastgele bir faktör ekleyin
        float randomFactor = Random.Range(0.0f, 1.0f);

        if (ShouldCall(currentBet, pot, randomFactor))
        {
            // Call
            betAmount = GameController.instance.currentBet;
            Call();
        }

        //// Karar verin:
        //if (ShouldFold(currentBet, pot, randomFactor))
        //{
        //    isFolded = true;
        //}
        //else if (ShouldCheck(currentBet, pot, randomFactor))
        //{
        //    // Check
        //}
        //else if (ShouldCall(currentBet, pot, randomFactor))
        //{
        //    // Call
        //    betAmount = currentBet;
        //    Call(currentBet);
        //}
        //else if (ShouldRaise(currentBet, pot, randomFactor))
        //{
        //    // Raise
        //    int raiseAmount = Mathf.Clamp(currentBet + 20, 20, 200);
        //    betAmount = raiseAmount;
        //    Raise(raiseAmount);
        //}
    }

    //bool ShouldFold(int currentBet, int pot, float randomFactor)
    //{
    //    // El zayýfsa ve bahis yüksekse fold et
    //    return currentBet > pot / 4 && randomFactor < 0.5f;
    //}

    //bool ShouldCheck(int currentBet, int pot, float randomFactor)
    //{
    //    // El orta seviyedeyse ve bahis makul ise check et
    //    return currentBet <= pot / 2 && randomFactor < 0.75f;
    //}

    bool ShouldCall(int currentBet, int pot, float randomFactor)
    {
        // El güçlü ise ve bahis makul ise call et
        return currentBet <= pot && randomFactor < 1.0f;
    }

    //bool ShouldRaise(int currentBet, int pot, float randomFactor)
    //{
    //    // El çok güçlüyse ve bahis düþükse raise et
    //    return currentBet < pot / 4 && randomFactor < 0.5f;
    //}

}
