using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [Header("BUTTONS")]
    public Button foldButton;
    public Button checkButton;
    public Button callButton;
    public Button raiseButton;
    public Button AllInButton;
    public TMP_InputField raiseAmountInput;
    public TextMeshProUGUI potText;

    private Player mainPlayer;
    private CardDealerAnimation cardDealerAnim;


    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        cardDealerAnim = GetComponent<CardDealerAnimation>();
        mainPlayer = GameController.instance.playersAndBots[0];

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
        mainPlayer.isFolded = true;

        HideBettingButtons();

        for (int i = 1; i >= 0; i--)
        {
            cardDealerAnim.AnimateFoldCardDeal(mainPlayer.hand[i].gameObject , GameObject.Find("PlaceholdersContainer").gameObject.transform.position);
        }

        mainPlayer.ClearHand();

        mainPlayer.ShowPlayerAction("Fold");

        SoundManager.instance.PlayFoldSound();

        // Sýradaki oyuncuya geç
        IsBettingButtonActive();
    }

    public void Call()
    {
        // Call yapmak için gereken para miktarýný hesapla
        int callAmount = GameController.instance.currentBet - mainPlayer.betAmount;

        if(mainPlayer.money <= callAmount)
        {
            AllIn();
            return;
        }

        // Bahsi kabul et ve paradan düþ
        mainPlayer.betAmount += callAmount;
        mainPlayer.money -= callAmount;

        UpdatePot(callAmount);

        // UI'yi güncelle
        UpdatePlayerUI(mainPlayer);

        mainPlayer.ShowPlayerAction("Call");

        SoundManager.instance.PlayCallAndRaiseSound();

        HideBettingButtons();

        // Sýradaki oyuncuya geç
        IsBettingButtonActive();
    }

    public void Raise()
    {
        // InputField'ý aktif hale getir
        raiseAmountInput.gameObject.SetActive(true);

        // Enter tuþuna basýlmasýný dinle
        raiseAmountInput.onEndEdit.AddListener(OnRaiseAmountInputEndEdit);

        // Raise butonuna basýlmasýný dinle
        raiseButton.onClick.AddListener(OnRaiseButtonClick);

        // Önerilen bahis tutarý hesaplama
        int suggestedRaise = GameController.instance.currentBet * 2;
        raiseAmountInput.placeholder.GetComponent<TMP_Text>().text = suggestedRaise.ToString();
    }

    private void OnRaiseAmountInputEndEdit(string text)
    {
        // Enter tuþuna basýldýysa
        if (Input.GetKeyDown(KeyCode.Return) && !string.IsNullOrEmpty(raiseAmountInput.text) && int.Parse(raiseAmountInput.text) > GameController.instance.currentBet)
        {
            // Ýþlemi tamamla
            ProcessRaise();
        }
    }

    private void OnRaiseButtonClick()
    {
        if (!string.IsNullOrEmpty(raiseAmountInput.text) && int.Parse(raiseAmountInput.text) > GameController.instance.currentBet)
        {
            // Ýþlemi tamamla
            ProcessRaise();
        }
    }

    private void ProcessRaise()
    {
        int raiseAmount = int.Parse(raiseAmountInput.text);

        if(raiseAmount >= mainPlayer.money)
        {
            AllIn();
            return;
        }

        // Bahsi yükselt ve paradan düþ
        mainPlayer.betAmount += raiseAmount;
        mainPlayer.money -= raiseAmount;

        // Mevcut bahsi güncelle
        GameController.instance.AddToCurrentBet(raiseAmount);
        UpdatePot(raiseAmount);

        // UI'yi güncelle
        UpdatePlayerUI(mainPlayer);

        mainPlayer.ShowPlayerAction($"Raýse: {raiseAmount}");

        SoundManager.instance.PlayCallAndRaiseSound();

        HideBettingButtons();

        raiseAmountInput.text = null;

        // Sýradaki oyuncuya geç
        IsBettingButtonActive();
    }

    public void AllIn()
    {
        // Tüm parayý bahis olarak yatýr
        int raiseAmount = mainPlayer.money;

        // Bahsi yükselt ve paradan düþ
        mainPlayer.betAmount += raiseAmount;
        mainPlayer.money -= raiseAmount;

        // Mevcut bahsi güncelle
        GameController.instance.AddToCurrentBet(raiseAmount);
        UpdatePot(raiseAmount);

        // UI'yi güncelle
        UpdatePlayerUI(mainPlayer);

        mainPlayer.ShowPlayerAction("All In");

        SoundManager.instance.PlayCallAndRaiseSound();

        HideBettingButtons();

        // Sýradaki oyuncuya geç
        IsBettingButtonActive();
    }


    public void Check()
    {
        // Mevcut bahsi kontrol et
        int currentBet = GameController.instance.currentBet;

        // Bahis 0 ise check yapamazsýn
        if (currentBet == 0)
        {
            Debug.Log("Bahis yapmanýz gerekiyor.");
            return;
        }

        // Bahsi kabul et
        mainPlayer.betAmount = currentBet;

        // UI'yi güncelle
        UpdatePlayerUI(mainPlayer);

        mainPlayer.ShowPlayerAction("Check");

        SoundManager.instance.PlayCheckSound();

        HideBettingButtons();

        // Sýradaki oyuncuya geç
        IsBettingButtonActive();
    }

    public bool IsBettingButtonActive()
    {
        // Butonlarýn aktiflik durumunu kontrol et
        if (callButton.interactable || foldButton.interactable)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    ///////////////////////******** Paramýz currentBet'den az kaldýysa oyun direkt olarak AllIn Yapsýn. ********///////////////////////


    //betAmount: Oyuncunun bahis turunda yatýrdýðý toplam bahis miktarýný temsil eder.
    //Bir oyuncu check yaparsa, betAmount deðiþmez.
    //Bir oyuncu call veya raise yaparsa, betAmount yeni bahis miktarýna güncellenir.
}
