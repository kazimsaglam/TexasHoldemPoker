using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
  public class LeaderboardRow : MonoBehaviour
  {
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI degreeText;

    public void SetNameText(string playerName)
    {
      if (nameText != null)
      {
        nameText.text = playerName;
      }
    }

    public void SetMoneyText(string money)
    {
      if (moneyText != null)
      {
        moneyText.text = money;
      }
    }

    public void SetDegreeText(string degree)
    {
      if (degreeText != null)
      {
        degreeText.text = degree;
      }
    }
  }
}