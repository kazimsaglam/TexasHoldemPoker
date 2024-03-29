using Database;
using Firebase.Database;
using TMPro;
using UnityEngine;

namespace UI
{
  public class MainMenuUIManager : MonoBehaviour
  {
    public static MainMenuUIManager instance;

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
      string moneyAmount = await FirebaseAuthManager.Instance.GetMoney();
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

    public void OnSignOut()
    {
      FirebaseAuthManager.Instance.OnLogOut();
    }
  }
}