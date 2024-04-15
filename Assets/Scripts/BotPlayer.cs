using UnityEngine;

public class BotPlayer : Player
{
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
  public void Call()
  {
    int callAmount = Mathf.Min(GameController.instance.currentBet, money);
    MakeBet(callAmount);
  }

  public void Raise(int amount)
  {
    // Raise the bet.
    int raiseAmount = Mathf.Min(amount, money);
    MakeBet(raiseAmount);
  }

  protected void Check()
  {
    // Skip betting
    if (GameController.instance.minimumBet >= money)
    {
      // If the highest bet on the table is equal or smaller than the minimum bet, player can check.
      money -= GameController.instance.minimumBet;
      GameController.instance.pot += GameController.instance.minimumBet;
    }
  }

  public void Fold()
  {
    isFolded = true;
  }

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