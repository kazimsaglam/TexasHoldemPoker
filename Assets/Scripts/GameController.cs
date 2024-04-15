using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Database;
using Game;
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
  public int playerStartingMoney = 1000;
  public int botStartingMoney = 500;

  public int numPlayers;
  public int currentPlayerIndex;

  public int minimumBet = 20;
  public int currentBet;
  public int pot;

  private int _smallBlind;
  private int _bigBlind;


  private GameState _gameState;

  private DeckManager _deckManager;

  public List<Player> winners;
  public event Action EndOfTour;

  private void Awake()
  {
    instance = this;
  }

  private void Start()
  {
    _deckManager = GetComponent<DeckManager>();

    CreatePlayers();

    StartGame();
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
        player.SetPlayer($"Player{i + 1}", playerStartingMoney, PlayerType.Player);
        playersAndBots.Add(player);
      }
      else // create AI bot
      {
        playerOrBot = Instantiate(botPrefab, _deckManager.placeholderPlayerHands[i]
          .transform.position, Quaternion.identity, _deckManager.placeholderPlayerHands[i].transform);
        Player bot = playerOrBot.GetComponent<BotPlayer>();
        bot.SetPlayer($"Bot {i}", botStartingMoney, PlayerType.Bot);
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

    _gameState = GameState.PreFlop;
    UpdateGameState();
  }

  public void UpdateGameState()
  {
    switch (_gameState)
    {
      case GameState.PreFlop:
        _deckManager.DealPlayerHands();
        StartCoroutine(StartBettingRound());
        break;

      case GameState.Flop:
        _deckManager.DealBoardCards();
        StartCoroutine(ProcessBettingRound());
        break;

      case GameState.Turn:
        _deckManager.DealBoardCards();
        StartCoroutine(ProcessBettingRound());
        break;

      case GameState.River:
        _deckManager.DealBoardCards();
        StartCoroutine(ProcessBettingRound());
        break;

      case GameState.Showdown:
        EvaluateHands();
        EndOfTourPanel.instance.CardInfo();
        EndOfTour?.Invoke();
        // Determine the winner
        //Player winner = playersAndBots.OrderByDescending(player => player.handValue).First();

        //// Give the pot to the winner
        //winner.money += pot;

        break;
    }
  }


  public IEnumerator StartBettingRound()
  {
    yield return new WaitForSeconds(3f);

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

      if (!currentPlayer.isFolded)
      {
        // Handle player turn
        yield return HandlePlayerTurn(currentPlayer);
      }

      // Move to next player
      NextPlayer();
    } while (currentPlayer.betRoundIndex != 2);

    _gameState++;
    UpdateGameState();
  }


  public IEnumerator HandlePlayerTurn(Player currentPlayer)
  {
    // Handle Human Player Interaction
    if (currentPlayerIndex == 0)
    {
      // Show betting buttons for human player interaction
      UIManager.instance.ShowBettingButtons();

      while (UIManager.instance.IsBettingButtonActive())
      {
        // Wait for player bet
        yield return null;
      }
    }
    else if (currentPlayer is BotPlayer botPlayer)
    {
      // Simulate bot thinking time
      yield return new WaitForSeconds(2f);

      // Implement bot strategy based on hand strength, pot size, etc.
      botPlayer.MakeDecision(currentBet, pot);
    }

    // Increase the bet round index of the current player
    currentPlayer.betRoundIndex++;

    // Update UI after bot's decision
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


  private void EvaluateHands()
  {
    foreach (Player player in playersAndBots)
    {
      player.CompareHand(_deckManager.boardCards);
    }

    int maxHandValue = playersAndBots.Max(player => player.handValue);

    List<Player> tiedPlayers = playersAndBots.Where(player => player.handValue == maxHandValue).ToList();
    Debug.Log("Equal strength player count:" + tiedPlayers.Count);
    if (tiedPlayers.Count > 1)
    {
      Player winningPlayer = tiedPlayers[0];
      foreach (Player player in tiedPlayers)
      {
        int compareResult = player.CompareHighestCard(winningPlayer);
        if (compareResult > 0)
        {
          winningPlayer = player;
        }
      }

      Debug.Log("Winner's hand strength: " + winningPlayer.playerName);
      EndGame(winningPlayer);
    }
    else
    {
      Player winner = playersAndBots.OrderByDescending(player => player.handValue).First();
      EndGame(winner);
    }
  }


  public void EndGame(Player winner)
  {
    winners.Add(winner);
    if (winner.playerType == PlayerType.Player)
    {
      PlayerManager.Instance.playerMoney += pot;
      Debug.Log(PlayerManager.Instance.playerMoney);
      FirebaseAuthManager.Instance.UpdateMoney(PlayerManager.Instance.playerMoney);
      int winCount = int.Parse(PlayerManager.Instance.winCount);
      FirebaseAuthManager.Instance.UpdateWinCount(winCount);
      int gameCount = int.Parse(PlayerManager.Instance.totalGameCount);
      FirebaseAuthManager.Instance.UpdateGameCount(gameCount);
    }
    else
    {
      int gameCount = int.Parse(PlayerManager.Instance.totalGameCount);
      FirebaseAuthManager.Instance.UpdateGameCount(gameCount);
    }


    // Kazanan oyuncuyu belirle
    //Player winnerPlayer = playersAndBots[0];


    // Oyun sonucunu göster
    //UIManager.instance.ShowEndGameUI(winnerPlayer);

    // Reset the game state
    //ResetGame();

    // Start the next game
    //StartGame();
  }

  void ResetGame()
  {
    // Reset the game state

    // Reset the pot

    // Reset player bets

    // Shuffle the deck

    // Deal initial cards

    // Set up the blinds

    // Update UI
  }
}