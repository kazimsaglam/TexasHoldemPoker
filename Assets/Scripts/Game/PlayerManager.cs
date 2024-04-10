using System;
using Database;
using UnityEngine;

namespace Game
{
  public class PlayerManager : MonoBehaviour
  {
    public string playerName { get; set; }
    public int playerMoney { get; set; }
    public static PlayerManager Instance { get; set; }

    private async void Start()
    {
      playerName = await FirebaseAuthManager.Instance.GetFullName();
      playerMoney = int.Parse(await FirebaseAuthManager.Instance.GetMoney());
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