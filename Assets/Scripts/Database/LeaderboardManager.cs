using System.Collections.Generic;
using Firebase.Database;
using Firebase.Extensions;
using UI;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Database
{
  public class LeaderboardManager : MonoBehaviour
  {
    public DatabaseReference databaseReference;
    public Transform container;

    [SerializeField]
    public GameObject rowSlot;

    private void Start()
    {
      databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
      FetchLeaderboard();
    }


    public void FetchLeaderboard()
    {
      databaseReference.Child("leaderboard").OrderByChild("money")
        .GetValueAsync().ContinueWithOnMainThread(task =>
        {
          if (task.IsFaulted || task.IsCanceled)
          {
            Debug.LogError("Failed to fetch leaderboard data");
          }
          else if (task.IsCompleted)
          {
            DataSnapshot snapshot = task.Result;
            List<PlayerData> leaderboardData = new List<PlayerData>();

            foreach (DataSnapshot childSnapshot in snapshot.Children)
            {
              string userId = childSnapshot.Key;
              string fullName = childSnapshot.Child("playerName").Value.ToString();
              int money = int.Parse(childSnapshot.Child("money").Value.ToString());

              leaderboardData.Add(new PlayerData
              {
                playerName = fullName,
                money = money, id = userId
              });
            }

            leaderboardData.Reverse();
            int degree = 1;
            foreach (PlayerData playerData in leaderboardData)
            {
              GameObject instantiatedObject = Instantiate(rowSlot, container);
              LeaderboardRow leaderboardRow = instantiatedObject.GetComponent<LeaderboardRow>();

              leaderboardRow.SetNameText(playerData.playerName);
              leaderboardRow.SetMoneyText(playerData.money.ToString());
              leaderboardRow.SetDegreeText(degree.ToString());
              degree++;

              Debug.Log($"User: {playerData.playerName}, Money: {playerData.money}");
            }
          }
        });
    }
  }
}