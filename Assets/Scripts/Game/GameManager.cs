using System;
using Database;
using TMPro;
using UnityEngine;
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
    }

    private void ShowWelcomeMessage()
    {
      welcomeText.text = $"Welcome to the Game Scene, {References.userName}";
    }
  }
}