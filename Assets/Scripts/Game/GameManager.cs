using System;
using Database;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace Game
{
  public class GameManager : MonoBehaviour
  {
    [SerializeField]
    private TextMeshProUGUI welcomeText;

    private void Start()
    {
      ShowWelcomeMessage();
      FirebaseAuthManager.OnMoneyUpdate += UpdateMoney;
    }

    private void ShowWelcomeMessage()
    {
      welcomeText.text = $"Welcome to the Game Scene, {References.userName}";
    }

    private void UpdateMoney(int newMoneyValue)
    {
      FirebaseAuthManager.Instance.UpdateMoney(newMoneyValue);
    }

    public void OnSignOut()
    {
      FirebaseAuthManager.OnLogOut();
    }
  }
}