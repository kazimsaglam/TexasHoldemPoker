using Database;
using Game;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UI;
using UnityEngine;
using Random = UnityEngine.Random;


public enum GameState
{
    PreFlop,
    Flop,
    Turn,
    River,
    Showdown
}

public class GameController : MonoBehaviour
{
    public static GameController instance;

    public List<Player> playersAndBots;
    public GameObject playerPrefab;
    public GameObject botPrefab;

    public int botStartingMoney = 1500;

    public int numPlayers;
    public static int currentPlayerIndex;

    public int minimumBet = 1;
    public int currentBet;
    public int pot;

    private int _maxExperience;
    private const int WinExp = 200;
    private const int LoseExp = 100;

    private int _currentLevel;

    private int _smallBlind;
    private int _bigBlind;

    public GameState gameState;
    private DeckManager _deckManager;

    public List<Player> winners;
    public event Action EndOfTour;
    public List<int> botPower;
    public int playersCount = 4;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        _deckManager = GetComponent<DeckManager>();
        BotPower();
        CreatePlayers();

        StartGame();
    }
    public void BotPower()
    {

        botPower = new List<int>();


        for (int i = 0; i < playersCount; i++)
        {
            botPower.Add(Random.Range(0, 10));
        }

    }
    private void CreatePlayers()
    {
        playersAndBots = new List<Player>();

        for (int i = 0; i < numPlayers; i++)
        {

            GameObject playerOrBot;
            if (i < 1) // create human player
            {
                playerOrBot = Instantiate(playerPrefab, _deckManager.placeholderPlayerHands[i]
                  .transform.position, Quaternion.identity, _deckManager.placeholderPlayerHands[i].transform);
                Player player = playerOrBot.GetComponent<Player>();
                player.SetPlayer($"Player {i + 1}", int.Parse(MainMenuUIManager.instance.moneyAmount), PlayerType.Player, 0);
                playersAndBots.Add(player);
            }
            else // create AI bot
            {
                playerOrBot = Instantiate(botPrefab, _deckManager.placeholderPlayerHands[i]
                  .transform.position, Quaternion.identity, _deckManager.placeholderPlayerHands[i].transform);
                Player bot = playerOrBot.GetComponent<BotPlayer>();
                bot.SetPlayer($"Bot {i}", botStartingMoney, PlayerType.Bot, botPower[i]);
                Debug.Log("Bot power: " + botPower[i]);
                playersAndBots.Add(bot);
            }
        }
    }


    public void AddToCurrentBet(int amount)
    {
        currentBet = amount;
    }


    public void StartGame()
    {
        // Initialize small blind and big blind
        _smallBlind = minimumBet;
        _bigBlind = _smallBlind * 2;

        // Reset the game state
        pot = 0;
        UIManager.instance.UpdatePot(pot);
        currentBet = 0;
        currentPlayerIndex = 0;

        // Reset player bets and hands and update UI data at the beginning
        foreach (Player player in playersAndBots)
        {
            player.ClearBets();
            player.hand.Clear();
            UIManager.instance.UpdatePlayerUI(player);
        }

        gameState = GameState.PreFlop;
        UpdateGameState();
    }

    public void UpdateGameState()
    {
        switch (gameState)
        {
            case GameState.PreFlop:


                StartCoroutine(StartBettingRound());
                break;

            case GameState.Flop:
                _deckManager.DealBoardCards();
                StartCoroutine(BotHand());
                StartCoroutine(ProcessBettingRound());
                break;

            case GameState.Turn:
                _deckManager.DealBoardCards();
                StartCoroutine(BotHand());
                StartCoroutine(ProcessBettingRound());
                break;

            case GameState.River:
                _deckManager.DealBoardCards();
                StartCoroutine(BotHand());
                StartCoroutine(ProcessBettingRound());
                break;

            case GameState.Showdown:
                StartCoroutine(BotHand()); //Gerekli olmayabilir.
                EvaluateHands();
                EndOfTourPanel.instance.CardInfo();
                EndOfTour?.Invoke();
                break;
        }
    }


    public IEnumerator StartBettingRound()
    {
        yield return new WaitForSeconds(1f);

        _deckManager.DealPlayerHands();
        StartCoroutine(BotHand());

        yield return new WaitForSeconds(1f);

        // Start with small and big blinds
        MakeBlindBets();

        // Start the betting process
        yield return StartCoroutine(ProcessBettingRound());
    }


    public IEnumerator ProcessBettingRound()
    {
        yield return new WaitForSeconds(2f);

        // Initialize bet round index for all players
        foreach (Player player in playersAndBots)
        {
            player.betRoundIndex = 0;
        }

        Player currentPlayer;
        do
        {
            currentPlayer = playersAndBots[currentPlayerIndex];

            if (!currentPlayer.isFolded && currentPlayer.money != 0)
            {
                // Handle player turn
                yield return HandlePlayerTurn(currentPlayer);
            }

            // Move to next player
            NextPlayer();
        } while (playersAndBots[currentPlayerIndex].betRoundIndex < 1);

        gameState++;
        UpdateGameState();
    }


    public IEnumerator HandlePlayerTurn(Player currentPlayer)
    {
        // Handle Human Player Interaction
        if (currentPlayerIndex == 0)
        {
            // Show betting buttons for human player interaction
            UIManager.instance.ShowBettingButtons();

            SoundManager.instance.PlayDingSound();

            while (UIManager.instance.IsBettingButtonActive())
            {
                // Wait for player bet
                yield return null;
            }
        }
        else if (currentPlayer is BotPlayer botPlayer)
        {
            // Simulate bot thinking time
            yield return new WaitForSeconds(7f);

            // Implement bot strategy based on hand strength, pot size, etc.
            botPlayer.MakeDecision(currentBet, pot);
        }

        // Increase the bet round index of the current player
        currentPlayer.betRoundIndex++;

        // Update UI after bot decision
        UIManager.instance.UpdatePlayerUI(currentPlayer);
    }


    public void NextPlayer()
    {
        currentPlayerIndex = (currentPlayerIndex + 1) % numPlayers;
    }

    private void MakeBlindBets()
    {
        // Small blind
        currentPlayerIndex = Random.Range(0, playersAndBots.Count);
        Player smallBlindPlayer = playersAndBots[currentPlayerIndex];
        smallBlindPlayer.MakeBet(_smallBlind);
        UIManager.instance.UpdatePlayerUI(smallBlindPlayer);

        // Big blind
        currentPlayerIndex = (currentPlayerIndex + 1) % playersAndBots.Count;
        Player bigBlindPlayer = playersAndBots[currentPlayerIndex];
        bigBlindPlayer.MakeBet(_bigBlind);
        UIManager.instance.UpdatePlayerUI(bigBlindPlayer);

        NextPlayer();
    }

    public List<Player> tiedPlayers = new List<Player>();
    private void EvaluateHands()
    {

        foreach (Player player in playersAndBots)
        {
            if (!player.isFolded)
            {
                player.CompareHand(_deckManager.boardCards);
            }
        }

        int maxHandValue = playersAndBots.Max(player => player.handValue);
        tiedPlayers = playersAndBots.Where(player => player.handValue == maxHandValue).ToList();
        Debug.Log("Equal strength player count:" + tiedPlayers.Count);

        // Eþit güce sahip birden fazla oyuncu varsa
        if (tiedPlayers.Count > 1 && tiedPlayers.Count < 3)
        {
            
            Player winningPlayer = tiedPlayers[0];

            if (tiedPlayers[0].handValue == 1)
            {
                int compareResult = Player.instance.ComparePairCard(winningPlayer, _deckManager.boardCards);
                if (compareResult > 0)
                {
                    winningPlayer = tiedPlayers[0];
                }
                else if (compareResult < 0)
                {
                    winningPlayer = tiedPlayers[1];
                }
                else if (compareResult == 0)
                {
                    // Eðer pair kartlarý eþitse
                    int compareHighestResult = Player.instance.CompareHighestCard(winningPlayer, _deckManager.boardCards);
                    if (compareHighestResult > 0)
                    {
                        winningPlayer = tiedPlayers[0];
                    }
                    else if (compareHighestResult < 0)
                    {
                        winningPlayer = tiedPlayers[1];
                    }
                }

            }
            else if (tiedPlayers[0].handValue == 2)
            {
                int compareTwoPairResult = Player.instance.CompareTwoPairCard(winningPlayer, _deckManager.boardCards);
                if (compareTwoPairResult > 0)
                {
                    winningPlayer = tiedPlayers[0];
                }
                else if (compareTwoPairResult < 0)
                {

                    winningPlayer = tiedPlayers[1];

                }
                else if (compareTwoPairResult == 0)
                {
                  
                    int compareHighestResult = Player.instance.CompareHighestCard(winningPlayer, _deckManager.boardCards);
                    if (compareHighestResult > 0)
                    {

                        winningPlayer = tiedPlayers[0];
                    }
                    else if (compareHighestResult < 0)
                    {
                        winningPlayer = tiedPlayers[1];

                    }
                }
            }
            else if (tiedPlayers[0].handValue > 3)
            {
                int compareHighestResult = Player.instance.CompareHighestCard(winningPlayer, _deckManager.boardCards);
                if (compareHighestResult > 0)
                {

                    winningPlayer = tiedPlayers[0];
                    EndGame(winningPlayer);
                }
                else if (compareHighestResult < 0)
                {
                    winningPlayer = tiedPlayers[1];
                    EndGame(winningPlayer);
                }
            }
            Debug.Log("Winner's hand strength: " + winningPlayer.playerName);
            EndGame(winningPlayer);
        }
        else if (tiedPlayers.Count > 2)
        {
            Player winningPlayer = tiedPlayers[0];

            int compareHighestResult = Player.instance.CompareHighestCard(winningPlayer, _deckManager.boardCards);
            if (compareHighestResult > 0)
            {

                winningPlayer = tiedPlayers[0];
                EndGame(winningPlayer);
            }
            else if (compareHighestResult < 0)
            {
                winningPlayer = tiedPlayers[1];
                EndGame(winningPlayer);
            }

        }
        else
        {
            Player winner = playersAndBots.OrderByDescending(player => player.handValue).First();
            EndGame(winner);
        }

    }

    public IEnumerator BotHand()
    {
        yield return new WaitForSeconds(3f);

        foreach (Player player in playersAndBots)
        {
            if (gameState == GameState.PreFlop)
            {
                player.BotHandControl(player.hand, player);

            }
            else
            {
                player.BotHandControl(_deckManager.boardCards, player);

            }
        }
    }

    public void EndGame(Player winner)
    {
        winners.Add(winner);
        //Update experience, check max exp. If user reaches to the max experience, update the level.
        int currentExperience = int.Parse(PlayerManager.Instance.currentExp);
        //Get the level
        _currentLevel = int.Parse(PlayerManager.Instance.currentLevel);
        _maxExperience = LevelData.RequiredExperiencePerLevel[_currentLevel];
        int gameCount = int.Parse(PlayerManager.Instance.totalGameCount);
        Debug.Log("Max. exp. for level " + _currentLevel + " is " + _maxExperience);

        if (winner.playerType == PlayerType.Player)
        {
            PlayerManager.Instance.playerMoney += pot;
            Debug.Log(PlayerManager.Instance.playerMoney);
            FirebaseAuthManager.Instance.UpdateMoney(PlayerManager.Instance.playerMoney);
            int winCount = int.Parse(PlayerManager.Instance.winCount);
            winCount++;
            FirebaseAuthManager.Instance.UpdateWinCount(winCount);

            //Update game count

            gameCount++;
            FirebaseAuthManager.Instance.UpdateGameCount(gameCount);

            //Update Experience
            currentExperience += WinExp;
            CheckUpdateLevelConditions(currentExperience);
        }
        else
        {
            gameCount++;
            currentExperience += LoseExp;

            FirebaseAuthManager.Instance.UpdateGameCount(gameCount);

            CheckUpdateLevelConditions(currentExperience);
        }
    }

    public void CheckUpdateLevelConditions(int currentExperience)
    {
        if (currentExperience >= _maxExperience)
        {
            currentExperience -= _maxExperience;
            _currentLevel++;

            FirebaseAuthManager.Instance.UpdateExperience(currentExperience);
            FirebaseAuthManager.Instance.UpdateLevel(_currentLevel);
        }
        else
        {
            FirebaseAuthManager.Instance.UpdateExperience(currentExperience);
        }
    }


}