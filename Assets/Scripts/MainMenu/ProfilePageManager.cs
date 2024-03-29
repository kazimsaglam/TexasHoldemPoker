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

    private async void Start()
    {
      string wins = await FirebaseAuthManager.Instance.GetWinOrTotalGameCount("win");
      winText.text = wins;
      string totalGameCount = await FirebaseAuthManager.Instance.GetWinOrTotalGameCount("totalGameCount");
      totalText.text = totalGameCount;
      int winCount = int.Parse(wins);
      int totalCount = int.Parse(totalGameCount);
      float winStatistics = (float)winCount / totalCount * 100f;
      string formattedWinStatistics = winStatistics.ToString("F2");
      statisticsText.text = formattedWinStatistics + "%";
      Debug.Log($"Win Statistics: {formattedWinStatistics}%");
    }
  }
}