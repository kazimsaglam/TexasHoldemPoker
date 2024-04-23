using Database;
using Game;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [Header("BUTTONS")]
    public Button foldButton;
    public Button checkButton;
    public Button callButton;
    public Button raiseButton;
    public Button allInButton;
    public TMP_InputField raiseAmountInput;

    public TextMeshProUGUI potText;

    private Player _mainPlayer; //Kazým deðiþiklik yapýldý.
    public List<Player> playerlist;
    private CardDealerAnimation _cardDealerAnim;
    int numberOfFoldPlayers;


    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        _cardDealerAnim = GetComponent<CardDealerAnimation>();
        playerlist = GameController.instance.playersAndBots;
        //_mainPlayer = GameController.instance.playersAndBots[0];
        GameController.instance.EndOfTour += ButtonActiveControl;
        HideBettingButtons();
    }

    public void ButtonActiveControl()
    {
        callButton.gameObject.SetActive(false);
        foldButton.gameObject.SetActive(false);
        checkButton.gameObject.SetActive(false);
        raiseButton.gameObject.SetActive(false);
    }

    public void ShowBettingButtons()
    {
        foldButton.interactable = true;
        callButton.interactable = true;
        raiseButton.interactable = true;
    }

    public void HideBettingButtons()
    {
        foldButton.interactable = false;
        callButton.interactable = false;
        checkButton.interactable = false;
        raiseButton.interactable = false;
        raiseAmountInput.gameObject.SetActive(false);
    }


    public void UpdatePot(int newPot)
    {
        GameController.instance.pot += newPot;
        potText.text = $"Pot: ${GameController.instance.pot}";
    }


    public void UpdatePlayerUI(Player player)
    {
        player.moneyText.text = $"Money: ${player.money}";
        player.betText.text = $"Bet: ${player.betAmount}";
    }


    public void Fold()
    {

        if (playerlist[GameController.currentPlayerIndex].isFolded && numberOfFoldPlayers < 2)

        {
            _cardDealerAnim.AnimateFoldCardDeal(playerlist[GameController.currentPlayerIndex].gameObject, GameObject.Find("PlaceholdersContainer").gameObject.transform.position);

            playerlist[GameController.currentPlayerIndex].ClearBets();

            numberOfFoldPlayers += 1;
            Debug.Log("Number of fold players: " + numberOfFoldPlayers);
            IsBettingButtonActive();
        }
        else if (numberOfFoldPlayers == 2)
        {// Oyunda iki kiþinin kalmasý için
            playerlist[GameController.currentPlayerIndex].isFolded = false;
            Call();
            Debug.Log("Fold çalýþmaLý...Call çalýþtý");
        }
        //_mainPlayer.isFolded = true;

        //HideBettingButtons();

        //for (int i = 1; i >= 0; i--)
        //{
        //    _cardDealerAnim.AnimateFoldCardDeal(_mainPlayer.hand[i].gameObject,
        //      GameObject.Find("PlaceholdersContainer").gameObject.transform.position);
        //}

        //_mainPlayer.ClearHand();
        //_mainPlayer.ClearBets();

        //_mainPlayer.ShowPlayerAction("Fold");
        //SoundManager.instance.PlayFoldSound();

        //// Go to the next player
        //IsBettingButtonActive();
    }
    public void PlayerFoldButton()
    {
        playerlist[0].isFolded = true;
        _cardDealerAnim.AnimateFoldCardDeal(playerlist[0].gameObject, GameObject.Find("PlaceholdersContainer").gameObject.transform.position);
        playerlist[0].ClearBets();
        numberOfFoldPlayers = +1;

        HideBettingButtons();
        //daha düzgün yazýlabilir

    }

    public void Call()
    {
        // Current bet control
        int currentBet = GameController.instance.currentBet;

        if (playerlist[GameController.currentPlayerIndex].money <= currentBet)
        {
            AllIn();
            return;
        }

        // Accept the bet and decrease the player's money.
        playerlist[GameController.currentPlayerIndex].betAmount = currentBet;
        playerlist[GameController.currentPlayerIndex].money -= currentBet;
        //_mainPlayer.betAmount += currentBet;
        //_mainPlayer.money -= currentBet;
        PlayerManager.Instance.playerMoney -= currentBet;
        FirebaseAuthManager.Instance.UpdateMoney(PlayerManager.Instance.playerMoney);
        Debug.Log(PlayerManager.Instance.playerMoney);
        GameController.instance.AddToCurrentBet(currentBet);

        UpdatePot(currentBet);
        UpdatePlayerUI(playerlist[GameController.currentPlayerIndex]);

        playerlist[GameController.currentPlayerIndex].ShowPlayerAction("Call");
        SoundManager.instance.PlayCallAndRaiseSound();

        HideBettingButtons();

        // Go to the next player
        IsBettingButtonActive();
    }

    public void Raise()
    {
        // Activate the input field
        raiseAmountInput.gameObject.SetActive(true);

        raiseAmountInput.onEndEdit.AddListener(OnRaiseAmountInputEndEdit);

        raiseButton.onClick.AddListener(OnRaiseButtonClick);

        // Suggested stake calculation
        int suggestedRaise = GameController.instance.currentBet * 2;
        raiseAmountInput.placeholder.GetComponent<TMP_Text>().text = suggestedRaise.ToString();
    }

    private void OnRaiseAmountInputEndEdit(string text)
    {
        if (Input.GetKeyDown(KeyCode.Return) && !string.IsNullOrEmpty(raiseAmountInput.text) && int.Parse(raiseAmountInput.text) > GameController.instance.currentBet)
        {
            ProcessRaise();
        }
    }

    private void OnRaiseButtonClick()
    {
        if (!string.IsNullOrEmpty(raiseAmountInput.text) && int.Parse(raiseAmountInput.text) > GameController.instance.currentBet)
        {
            ProcessRaise();
        }
    }

    private void ProcessRaise()
    {
        int raiseAmount = int.Parse(raiseAmountInput.text);

        if (raiseAmount >= _mainPlayer.money)
        {
            AllIn();
            return;
        }

        // raise the bet
        _mainPlayer.betAmount += raiseAmount;
        _mainPlayer.money -= raiseAmount;
        PlayerManager.Instance.playerMoney -= raiseAmount;
        FirebaseAuthManager.Instance.UpdateMoney(PlayerManager.Instance.playerMoney);
        Debug.Log(PlayerManager.Instance.playerMoney);

        // Update the current bet
        GameController.instance.AddToCurrentBet(raiseAmount);
        UpdatePot(raiseAmount);

        UpdatePlayerUI(_mainPlayer);
        _mainPlayer.ShowPlayerAction($"RaIse: {raiseAmount}");
        SoundManager.instance.PlayCallAndRaiseSound();

        raiseAmountInput.text = null;

        HideBettingButtons();

        // Go to the next player
        IsBettingButtonActive();
    }

    public void AllIn()
    {
        // Put all your money in.
        int raiseAmount = _mainPlayer.money;

        _mainPlayer.betAmount += raiseAmount;
        _mainPlayer.money -= raiseAmount;
        PlayerManager.Instance.playerMoney -= raiseAmount;
        FirebaseAuthManager.Instance.UpdateMoney(PlayerManager.Instance.playerMoney);
        Debug.Log(PlayerManager.Instance.playerMoney);

        // MUpdate the current bet
        GameController.instance.AddToCurrentBet(raiseAmount);
        UpdatePot(raiseAmount);

        UpdatePlayerUI(_mainPlayer);
        _mainPlayer.ShowPlayerAction("All In");
        SoundManager.instance.PlayCallAndRaiseSound();
        raiseAmountInput.text = "";

        HideBettingButtons();

        // Go to the next player
        IsBettingButtonActive();
    }


    public void Check()
    {
        // Check the current bet.
        int currentBet = GameController.instance.currentBet;

        // If the current bet is 0, you can't check.
        if (currentBet == 0)
        {
            Debug.Log("You should make bet.");
            return;
        }

        _mainPlayer.betAmount = currentBet;

        UpdatePlayerUI(_mainPlayer);
        _mainPlayer.ShowPlayerAction("Check");
        SoundManager.instance.PlayCheckSound();

        HideBettingButtons();

        // Go to the next player
        IsBettingButtonActive();
    }

    public bool IsBettingButtonActive()
    {
        return callButton.interactable || foldButton.interactable;
    }
}