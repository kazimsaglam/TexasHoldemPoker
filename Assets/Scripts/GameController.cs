using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum GameState { PreFlop, Flop, Turn, River, Showdown }
public class GameController : MonoBehaviour
{
    public static GameController instance;

    public List<Player> playersAndBots;
    public GameObject playerPrefab;
    public GameObject botPrefab;

    public int numPlayers;
    public int currentPlayerIndex;

    public int minimumBet = 20;
    public int currentBet;
    public int pot;

    private int smallBlind;
    private int bigBlind;

    private GameState gameState;
    private DeckManager deckManager;


    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        deckManager = GetComponent<DeckManager>();

        CreatePlayers();

        StartGame();
    }


    void CreatePlayers()
    {
        playersAndBots = new List<Player>();
        GameObject playerOrBot;

        for (int i = 0; i < numPlayers; i++)
        {
            if (i < 1) // create human player
            {
                playerOrBot = Instantiate(playerPrefab, deckManager.placeholderPlayerHands[i].transform.position, Quaternion.identity, deckManager.placeholderPlayerHands[i].transform);
                Player player = playerOrBot.GetComponent<Player>();
                player.SetPlayer($"Player {i + 1}", 1000);
                playersAndBots.Add(player);
            }
            else // create AI bot
            {
                playerOrBot = Instantiate(botPrefab, deckManager.placeholderPlayerHands[i].transform.position, Quaternion.identity, deckManager.placeholderPlayerHands[i].transform);
                Player bot = playerOrBot.GetComponent<BotPlayer>();
                bot.SetPlayer($"Bot {(i)}", 500);
                playersAndBots.Add(bot);
            }
        }
    }


    public void AddToCurrentBet(int amount)
    {
        if(amount > currentBet)
        {
            currentBet = amount;
        }
    }


    public void StartGame()
    {
        // Initialize small blind and big blind
        smallBlind = minimumBet;
        bigBlind = smallBlind * 2;

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

        // Start game
        gameState = GameState.PreFlop;
        UpdateGameState();
    }

    public void UpdateGameState()
    {
        switch (gameState)
        {
            case GameState.PreFlop:
                // Oyunculara kart daðýt
                deckManager.DealPlayerHands();

                // Bahis turunu baþlat
                StartCoroutine(StartBettingRound());
                break;

            case GameState.Flop:
                // Masaya 3 kart daðýt
                deckManager.DealBoardCards();

                // Oyuncu bahislerini sýfýrla
                ResetPlayerBets();

                // Tüm oyunculara sýrayla bahis yaptýr
                StartCoroutine(ProcessBettingRound());
                break;

            case GameState.Turn:
                // Masaya 4. kartý daðýt
                deckManager.DealBoardCards();

                // Oyuncu bahislerini sýfýrla
                ResetPlayerBets();

                // Tüm oyunculara sýrayla bahis yaptýr
                StartCoroutine(ProcessBettingRound());
                break;

            case GameState.River:
                // Masaya 5. kartý daðýt
                deckManager.DealBoardCards();

                // Oyuncu bahislerini sýfýrla
                ResetPlayerBets();

                // Tüm oyunculara sýrayla bahis yaptýr
                StartCoroutine(ProcessBettingRound());
                break;

            case GameState.Showdown:
                // Oyuncu bahislerini sýfýrla
                ResetPlayerBets();

                // Her oyuncunun el deðerini hesapla
                EvaluateHands();

                // Kazananý belirle
                Player winner = playersAndBots.OrderByDescending(player => player.handValue).First();

                // Kazanan oyuncuya pot'u ver
                winner.money += pot;

                // Oyunun sonucunu göster
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

        // Bahis turu tamamlandýðýnda devam eden iþlemleri gerçekleþtir
        gameState++;
        UpdateGameState();
    }


    public IEnumerator HandlePlayerTurn(Player currentPlayer)
    {
        // Handle Human Player Interaction
        if (currentPlayerIndex == 0)
        {
            SoundManager.instance.PlayDingSound();

            // Show betting buttons for human player interaction
            UIManager.instance.ShowBettingButtons();

            while (UIManager.instance.IsBettingButtonActive())
            {
                // Oyuncunun bahis yapmasýný bekleyin
                yield return null;
            }
        }
        else if (currentPlayer is BotPlayer)
        {
            BotPlayer botPlayer = currentPlayer as BotPlayer;

            // Simulate bot thinking time
            yield return new WaitForSeconds(2f);

            botPlayer.MakeDecision(currentBet, pot);
        }

        // Increase the bet round index of the current player
        currentPlayer.betRoundIndex++;

        // Update UI after bot's decision
        UIManager.instance.UpdatePlayerUI(currentPlayer);

        yield return new WaitForSeconds(1f);
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
        smallBlindPlayer.MakeBet(smallBlind);
        UIManager.instance.UpdatePlayerUI(smallBlindPlayer);

        // Big blind
        currentPlayerIndex = (currentPlayerIndex + 1) % playersAndBots.Count;
        Player bigBlindPlayer = playersAndBots[currentPlayerIndex];
        bigBlindPlayer.MakeBet(bigBlind);
        UIManager.instance.UpdatePlayerUI(bigBlindPlayer);

        NextPlayer();
    }

    public void ResetPlayerBets()
    {
        foreach (var player in playersAndBots)
        {
            player.ClearBets();
            UIManager.instance.UpdatePlayerUI(player);
        }
    }


    private void EvaluateHands()
    {
        // Tüm oyuncularýn ellerini deðerlendir
        foreach (Player player in playersAndBots)
        {
            player.CompareHand(deckManager.boardCards);
        }
    }


    public void EndGame()
    {
        // Kazanan oyuncuyu belirle
        Player winnerPlayer = playersAndBots[0];

        // Kazananýn potu almasýný saðla
        winnerPlayer.money += pot;

        // Oyun sonucunu göster
        //UIManager.instance.ShowEndGameUI(winnerPlayer);

        // Reset the game state
        ResetGame();

        // Start the next game
        StartGame();
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
