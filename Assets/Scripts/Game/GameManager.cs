using Database;
using UnityEngine;

namespace Game
{
  public class GameManager : MonoBehaviour
  {
    private void Start()
    {
      FirebaseAuthManager.OnMoneyUpdate += UpdateMoney;
    }

    private void UpdateMoney(int newMoneyValue)
    {
      FirebaseAuthManager.Instance.UpdateMoney(newMoneyValue);
    }
      
  }
}