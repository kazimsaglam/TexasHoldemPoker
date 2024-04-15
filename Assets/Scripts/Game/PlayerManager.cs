using System;
using Database;
using UnityEngine;

namespace Game
{
  public class PlayerManager : MonoBehaviour
  {
    public string playerName { get; set; }
    public int playerMoney { get; set; }

    public string totalGameCount { get; set; }
    public string winCount { get; set; }
    public static PlayerManager Instance { get; set; }

    private async void Start()
    {
      playerName = await FirebaseAuthManager.Instance.GetFullName();
      playerMoney = int.Parse(await FirebaseAuthManager.Instance.GetMoney());
      totalGameCount = await FirebaseAuthManager.Instance
        .GetWinOrTotalGameCount("totalGameCount");

      winCount = await FirebaseAuthManager.Instance
        .GetWinOrTotalGameCount("win");
      Debug.Log(playerMoney);
    }

    private void Awake()
    {
      if (Instance == null)
      {
        Instance = this;
      }
      else
      {
        Destroy(gameObject);
      }
    }
  }
}