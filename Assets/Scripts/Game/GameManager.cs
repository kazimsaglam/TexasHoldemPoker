using System;
using Database;
using MainMenu;
using UnityEngine;

namespace Game
{
  public class GameManager : MonoBehaviour
  {
    private string _winCount;
    private string _gameCount;

    private void Start()
    {
      // FirebaseAuthManager.OnMoneyUpdate += UpdateMoney;
      // FirebaseAuthManager.OnGameCountUpdate += UpdateWinAndGameCount;
    }


    private void UpdateMoney(int newMoneyValue)
    {
      FirebaseAuthManager.Instance.UpdateMoney(newMoneyValue);
    }

    public void UpdateGameCount()
    {
      int game = int.Parse(ProfilePageManager.Instance.totalGameCount);
      game++;
      ProfilePageManager.Instance.totalGameCount = game.ToString();
      FirebaseAuthManager.Instance.UpdateGameCount(game);
    }

    public void UpdateWinAndGameCount()
    {
      int win = int.Parse(ProfilePageManager.Instance.winCount);
      win++;
      ProfilePageManager.Instance.winCount = win.ToString();

      int game = int.Parse(ProfilePageManager.Instance.totalGameCount);
      game++;
      ProfilePageManager.Instance.totalGameCount = game.ToString();

      FirebaseAuthManager.Instance.UpdateWinCount(win);
      FirebaseAuthManager.Instance.UpdateGameCount(game);
    }
  }
}