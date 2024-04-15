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

    public static ProfilePageManager Instance { get; set; }

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

      // Fetch the full name

      int wins = int.Parse(PlayerManager.Instance.winCount);
      int totalCount = int.Parse(PlayerManager.Instance.totalGameCount);

      float winStatistics = (float)wins / totalCount * 100f;
      string formattedWinStatistics = winStatistics.ToString("F2");
      statisticsText.text = formattedWinStatistics + "%";
      Debug.Log($"Win Statistics: {formattedWinStatistics}%");
    }
  }
}