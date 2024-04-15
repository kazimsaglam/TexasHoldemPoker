using Database;
using Game;
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

  private Player _mainPlayer;
  private CardDealerAnimation _cardDealerAnim;

  private void Awake()
  {
    instance = this;
  }

  private void Start()
  {
    _cardDealerAnim = GetComponent<CardDealerAnimation>();
    _mainPlayer = GameController.instance.playersAndBots[0];

    HideBettingButtons();
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
    GameController.instance.pot = newPot;
    potText.text = $"Pot: ${GameController.instance.pot}";
  }


  public void UpdatePlayerUI(Player player)
  {
    player.moneyText.text = $"Money: ${player.money}";
    player.betText.text = $"Bet: ${player.betAmount}";
  }

  public void Fold()
  {
    _mainPlayer.isFolded = true;

    HideBettingButtons();

    for (int i = 1; i >= 0; i--)
    {
      _cardDealerAnim.AnimateFoldCardDeal(_mainPlayer.hand[i].gameObject,
        GameObject.Find("PlaceholdersContainer").gameObject.transform.position);
    }

    _mainPlayer.ClearHand();
    _mainPlayer.ClearBets();

    IsBettingButtonActive();
  }

  public void Call()
  {
    // Current bet control
    int currentBet = GameController.instance.currentBet;

    // Accept the bet and decrease the player's money.
    _mainPlayer.betAmount = currentBet;
    _mainPlayer.money -= currentBet;
    PlayerManager.Instance.playerMoney -= currentBet;
    FirebaseAuthManager.Instance.UpdateMoney(PlayerManager.Instance.playerMoney);
    Debug.Log(PlayerManager.Instance.playerMoney);
    GameController.instance.AddToCurrentBet(currentBet);

    UpdatePlayerUI(_mainPlayer);

    _mainPlayer.ShowPlayerAction("Call");

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
  }

  private void OnRaiseAmountInputEndEdit(string text)
  {
    if (Input.GetKeyDown(KeyCode.Return) && !string.IsNullOrEmpty(raiseAmountInput.text))
    {
      ProcessRaise();
    }
  }

  private void OnRaiseButtonClick()
  {
    if (!string.IsNullOrEmpty(raiseAmountInput.text))
    {
      ProcessRaise();
    }
  }

  private void ProcessRaise()
  {
    int raiseAmount = int.Parse(raiseAmountInput.text);

    // raise the bet
    _mainPlayer.betAmount = raiseAmount;
    _mainPlayer.money -= raiseAmount;
    PlayerManager.Instance.playerMoney -= raiseAmount;
    FirebaseAuthManager.Instance.UpdateMoney(PlayerManager.Instance.playerMoney);
    Debug.Log(PlayerManager.Instance.playerMoney);

    // Update the current bet
    GameController.instance.AddToCurrentBet(raiseAmount);
    UpdatePot(raiseAmount);

    UpdatePlayerUI(_mainPlayer);
    HideBettingButtons();

    // Go to the next player
    IsBettingButtonActive();
  }

  public void AllIn()
  {
    // Put all your money in.
    int raiseAmount = _mainPlayer.money;

    _mainPlayer.betAmount = raiseAmount;
    _mainPlayer.money -= raiseAmount;
    PlayerManager.Instance.playerMoney -= raiseAmount;
    FirebaseAuthManager.Instance.UpdateMoney(PlayerManager.Instance.playerMoney);
    Debug.Log(PlayerManager.Instance.playerMoney);

    // MUpdate the current bet
    GameController.instance.AddToCurrentBet(raiseAmount);
    UpdatePot(raiseAmount);

    UpdatePlayerUI(_mainPlayer);
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

    HideBettingButtons();

    // Go to the next player
    IsBettingButtonActive();
  }

  public bool IsBettingButtonActive()
  {
    return callButton.interactable || foldButton.interactable;
  }
}