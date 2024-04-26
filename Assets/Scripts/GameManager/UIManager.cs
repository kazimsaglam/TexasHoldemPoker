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
    public Button callButton;
    public Button raiseButton;
    public Button allInButton;
    public TMP_InputField raiseAmountInput;

    public TextMeshProUGUI potText;

    private Player _mainPlayer;
    public List<Player> playerList;
    private CardDealerAnimation _cardDealerAnim;
    int _numberOfFoldPlayers;


    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        _cardDealerAnim = GetComponent<CardDealerAnimation>();
        playerList = GameController.instance.playersAndBots;
        _mainPlayer = GameController.instance.playersAndBots[0];
        GameController.instance.EndOfTour += ButtonActiveControl;
        GameController.instance.EndOfTour += PotTransformChange;
        GameController.instance.EndOfTour += IsFoldedControl;


        HideBettingButtons();
    }

    public void ButtonActiveControl()
    {
        callButton.gameObject.SetActive(false);
        foldButton.gameObject.SetActive(false);
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
        if (playerList[GameController.currentPlayerIndex].isFolded && _numberOfFoldPlayers < 2)
        {
            _cardDealerAnim.AnimateFoldCardDeal(playerList[GameController.currentPlayerIndex].gameObject, GameObject.Find("PlaceholdersContainer").gameObject.transform.position);
            Player.instance.ShowPlayerAction("Fold");
            playerList[GameController.currentPlayerIndex].ClearBets();

            _numberOfFoldPlayers += 1;
            
            Debug.Log("Number of fold players: " + _numberOfFoldPlayers);
            IsBettingButtonActive();
        }
        else if (_numberOfFoldPlayers == 2)
        {// Oyunda iki kiþinin kalmasý için
            playerList[GameController.currentPlayerIndex].isFolded = false;
            Call();
            Debug.Log("Fold çalýþmalý...Call çalýþtý");
        }
    }
    public void PlayerFoldButton()
    {
        _numberOfFoldPlayers += 1;

        playerList[0].isFolded = true;
        _cardDealerAnim.AnimateFoldCardDeal(playerList[0].gameObject, GameObject.Find("PlaceholdersContainer").gameObject.transform.position);
        playerList[0].ClearBets();

        _mainPlayer.ShowPlayerAction("Fold");
        SoundManager.instance.PlayFoldSound();

        HideBettingButtons();
    }
    public void IsFoldedControl()
    {
        for (int i = 0; i< playerList.Count; i++)
        {
            if (playerList[i].isFolded)
            {
               EndOfTourPanel.instance.cardPosition[i].transform.GetChild(0).gameObject.SetActive(true);
            }
        }
    }

    public void Call()
    {
        // Current bet control
        int currentBet = GameController.instance.currentBet;

        if (playerList[GameController.currentPlayerIndex].money <= currentBet)
        {
            AllIn();
            return;
        }

        // Accept the bet and decrease the player's money.
        playerList[GameController.currentPlayerIndex].betAmount += currentBet;
        playerList[GameController.currentPlayerIndex].money -= currentBet;

        PlayerManager.Instance.playerMoney -= currentBet;
        FirebaseAuthManager.Instance.UpdateMoney(PlayerManager.Instance.playerMoney);
        Debug.Log(PlayerManager.Instance.playerMoney);
        GameController.instance.AddToCurrentBet(currentBet);

        UpdatePot(currentBet);
        UpdatePlayerUI(playerList[GameController.currentPlayerIndex]);
        if(playerList[GameController.currentPlayerIndex].isFolded != true)
        {

        }
        playerList[GameController.currentPlayerIndex].ShowPlayerAction("Call");
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

    public bool IsBettingButtonActive()
    {
        return callButton.interactable || foldButton.interactable;
    }
    public void PotTransformChange()
    {
        potText.transform.position= EndOfTourPanel.instance.potText.transform.position; 
    }
}
