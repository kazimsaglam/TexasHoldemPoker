using Database;
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

    public string totalGameCount { get; set; }
    public string winCount { get; set; }

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

    private async void Start()
    {
      totalGameCount = await FirebaseAuthManager.Instance
        .GetWinOrTotalGameCount("totalGameCount");
      totalText.text = totalGameCount;

      winCount = await FirebaseAuthManager.Instance
        .GetWinOrTotalGameCount("win");
      winText.text = winCount;

      int wins = int.Parse(winCount);
      int totalCount = int.Parse(totalGameCount);

      float winStatistics = (float)wins / totalCount * 100f;
      string formattedWinStatistics = winStatistics.ToString("F2");
      statisticsText.text = formattedWinStatistics + "%";
      Debug.Log($"Win Statistics: {formattedWinStatistics}%");
    }
  }
}