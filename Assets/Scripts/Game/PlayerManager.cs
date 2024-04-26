using Database;
using UnityEngine;

namespace Game
{
    public class PlayerManager : MonoBehaviour
    {
        public string playerName { get; set; }
        public int playerMoney { get; set; }

        public string currentExp { get; set; }

        public string maxExp { get; set; }
        public string currentLevel { get; set; }
        public string totalGameCount { get; set; }
        public string winCount { get; set; }
        public static PlayerManager Instance { get; set; }

        private async void Start()
        {
            playerName = await FirebaseAuthManager.Instance.GetFullName();
            playerMoney = int.Parse(await FirebaseAuthManager.Instance.GetMoney());
            totalGameCount = await FirebaseAuthManager.Instance
              .GetWinOrTotalGameCount("totalGameCount");

            winCount = await FirebaseAuthManager.Instance
              .GetWinOrTotalGameCount("win");

            //Get user's current experience point
            currentExp = await FirebaseAuthManager.Instance.GetExperience();

            //Get Player's Level
            currentLevel = await FirebaseAuthManager.Instance.GetLevel();
        }

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
    }
}