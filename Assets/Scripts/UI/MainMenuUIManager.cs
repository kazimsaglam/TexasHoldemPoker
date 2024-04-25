using Database;
using Firebase.Database;
using Game;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
  public class MainMenuUIManager : MonoBehaviour
  {
    public static MainMenuUIManager instance;

    [HideInInspector]
    public string moneyAmount;

    [SerializeField]
    private GameObject profilePanel;

    [SerializeField]
    private GameObject leaderboardPanel;

    [SerializeField]
    private GameObject mainMenuPanel;

    [SerializeField]
    private TextMeshProUGUI moneyText;

    private DatabaseReference _databaseReference;

    private async void Start()
    {
      _databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
      moneyAmount = await FirebaseAuthManager.Instance.GetMoney();
      moneyText.text = moneyAmount;
    }

    private void Awake()
    {
      CreateInstance();
    }

    private void CreateInstance()
    {
      if (instance == null)
      {
        instance = this;
      }
    }


    public void OpenProfilePanel()
    {
      mainMenuPanel.SetActive(false);
      profilePanel.SetActive(true);
    }

    public void OnOpenLeaderboardPanel()
    {
      leaderboardPanel.SetActive(true);
      mainMenuPanel.SetActive(false);
    }

    public void OnCloseLeaderboardPanel()
    {
      leaderboardPanel.SetActive(false);
      mainMenuPanel.SetActive(true);
    }

    public void OnCloseProfilePanel()
    {
      profilePanel.SetActive(false);
      mainMenuPanel.SetActive(true);
    }

    public void OnOpenInGameScene()
    {
      SceneManager.LoadScene("Scenes/InGame");
    }

    public void OnSignOut()
    {
      FirebaseAuthManager.Instance.OnLogOut();
    }

    public void MoneyHack()
    {
        PlayerManager.Instance.playerMoney += 2000;
        Debug.Log(PlayerManager.Instance.playerMoney);
        FirebaseAuthManager.Instance.UpdateMoney(PlayerManager.Instance.playerMoney);
    }
}
}