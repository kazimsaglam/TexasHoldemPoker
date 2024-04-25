using Game;
using TMPro;
using UnityEngine;

namespace MainMenu
{
  public class ProfilePageManager : MonoBehaviour
  {
    [SerializeField]
    private TextMeshProUGUI winText;

    [SerializeField]
    private TextMeshProUGUI totalText;

    [SerializeField]
    private TextMeshProUGUI statisticsText;

    [SerializeField]
    private TextMeshProUGUI levelText;

    [SerializeField]
    private TextMeshProUGUI nameText;

    public static ProfilePageManager Instance { get; set; }

    private float _winStatistics;

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

    private void Start()
    {
      totalText.text = PlayerManager.Instance.totalGameCount;

      winText.text = PlayerManager.Instance.winCount;

      levelText.text = PlayerManager.Instance.currentLevel;

      nameText.text = "Welcome, " + PlayerManager.Instance.playerName;

      int wins = int.Parse(PlayerManager.Instance.winCount);
      int totalCount = int.Parse(PlayerManager.Instance.totalGameCount);

      if (wins == 0 && totalCount == 0)
      {
        _winStatistics = 0;
      }
      else
      {
        _winStatistics = (float)wins / totalCount * 100f;
      }

      string formattedWinStatistics = _winStatistics.ToString("F2");
      statisticsText.text = formattedWinStatistics + "%";
      Debug.Log($"Win Statistics: {formattedWinStatistics}%");
    }
  }
}