using Database;
using Game;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Serialization;

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
      _cardDealerAnim.AnimateFoldCardDeal(_mainPlayer.hand[i].gameObject, GameObject.Find("PlaceholdersContainer").gameObject.transform.position);
    }

    _mainPlayer.ClearHand();
    _mainPlayer.ClearBets();

    // Sýradaki oyuncuya geçin
    IsBettingButtonActive();
  }

  public void Call()
  {
    // Mevcut bahis kontrolü
    int currentBet = GameController.instance.currentBet;

    // Bahsi kabul edin ve paradan düþürün
    _mainPlayer.betAmount = currentBet;
    _mainPlayer.money -= currentBet;
    PlayerManager.Instance.playerMoney -= currentBet;
    FirebaseAuthManager.Instance.UpdateMoney(PlayerManager.Instance.playerMoney);
    GameController.instance.AddToCurrentBet(currentBet);

    UpdatePlayerUI(_mainPlayer);

    _mainPlayer.ShowPlayerAction("Call");

    HideBettingButtons();

    // Sýradaki oyuncuya geçin
    IsBettingButtonActive();
  }

  public void Raise()
  {
    // InputField'ý aktif hale getirin
    raiseAmountInput.gameObject.SetActive(true);

    // Enter tuþuna basýlmasýný dinleyin
    raiseAmountInput.onEndEdit.AddListener(OnRaiseAmountInputEndEdit);

    // Raise butonuna basýlmasýný dinleyin
    raiseButton.onClick.AddListener(OnRaiseButtonClick);
  }

  private void OnRaiseAmountInputEndEdit(string text)
  {
    // Enter tuþuna basýldýysa
    if (Input.GetKeyDown(KeyCode.Return) && !string.IsNullOrEmpty(raiseAmountInput.text))
    {
      // Ýþlemi tamamlayýn
      ProcessRaise();
    }
  }

  private void OnRaiseButtonClick()
  {
    if (!string.IsNullOrEmpty(raiseAmountInput.text))
    {
      // Ýþlemi tamamlayýn
      ProcessRaise();
    }
  }

  private void ProcessRaise()
  {
    // Girilen deðeri int'e dönüþtürün
    int raiseAmount = int.Parse(raiseAmountInput.text);

    // Bahsi yükseltin ve paradan düþürün
    _mainPlayer.betAmount = raiseAmount;
    _mainPlayer.money -= raiseAmount;
    PlayerManager.Instance.playerMoney -= raiseAmount;
    FirebaseAuthManager.Instance.UpdateMoney(PlayerManager.Instance.playerMoney);

    // Mevcut bahsi güncelleyin
    GameController.instance.AddToCurrentBet(raiseAmount);
    UpdatePot(raiseAmount);

    // UI'yi güncelleyin
    UpdatePlayerUI(_mainPlayer);

    HideBettingButtons();

    // Sýradaki oyuncuya geçin
    IsBettingButtonActive();
  }

  public void AllIn()
  {
    // Tüm parayý bahis olarak yatýrýn
    int raiseAmount = _mainPlayer.money;

    // Bahsi yükseltin ve paradan düþürün
    _mainPlayer.betAmount = raiseAmount;
    _mainPlayer.money -= raiseAmount;
    PlayerManager.Instance.playerMoney -= raiseAmount;
    FirebaseAuthManager.Instance.UpdateMoney(PlayerManager.Instance.playerMoney);

    // Mevcut bahsi güncelleyin
    GameController.instance.AddToCurrentBet(raiseAmount);
    UpdatePot(raiseAmount);

    // UI'yi güncelleyin
    UpdatePlayerUI(_mainPlayer);

    // InputField'ý temizleyin
    raiseAmountInput.text = "";

    HideBettingButtons();

    // Sýradaki oyuncuya geçin
    IsBettingButtonActive();
  }


  public void Check()
  {
    // Mevcut bahsi kontrol edin
    int currentBet = GameController.instance.currentBet;

    // Bahis 0 ise check yapamazsýnýz
    if (currentBet == 0)
    {
      Debug.Log("You should make bet.");
      return;
    }

    // Bahsi kabul edin
    _mainPlayer.betAmount = currentBet;

    // UI'yi güncelleyin
    UpdatePlayerUI(_mainPlayer);

    HideBettingButtons();

    // Sýradaki oyuncuya geçin
    IsBettingButtonActive();
  }

  public bool IsBettingButtonActive()
  {
    // Butonlarýn aktiflik durumunu kontrol edin
    if (callButton.interactable || foldButton.interactable)
    {
      return true;
    }
    else
    {
      return false;
    }
  }


  //betAmount: Oyuncunun bahis turunda yatýrdýðý toplam bahis miktarýný temsil eder.
  //Bir oyuncu check yaparsa, betAmount deðiþmez.
  //Bir oyuncu call veya raise yaparsa, betAmount yeni bahis miktarýna güncellenir.
}