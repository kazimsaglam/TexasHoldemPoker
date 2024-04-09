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
        GameController.instance.EndOfTour += ButtonActiveControll;
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
        GameController.instance.pot= newPot;
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
        mainPlayer.ClearBets();

        // S�radaki oyuncuya ge�in
        IsBettingButtonActive();
    }

    public void Call()
    {
        // Mevcut bahis kontrol�
        int currentBet = GameController.instance.currentBet;

        // Bahsi kabul edin ve paradan d���r�n
        mainPlayer.betAmount = currentBet;
        mainPlayer.money -= currentBet;

        GameController.instance.AddToCurrentBet(currentBet);

        // UI'yi g�ncelleyin
        UpdatePlayerUI(mainPlayer);

        mainPlayer.ShowPlayerAction("Call");

        HideBettingButtons();

        // S�radaki oyuncuya ge�in
        IsBettingButtonActive();
    }

    public void Raise()
    {
        // InputField'� aktif hale getirin
        raiseAmountInput.gameObject.SetActive(true);

        // Enter tu�una bas�lmas�n� dinleyin
        raiseAmountInput.onEndEdit.AddListener(OnRaiseAmountInputEndEdit);

        // Raise butonuna bas�lmas�n� dinleyin
        raiseButton.onClick.AddListener(OnRaiseButtonClick);       
    }

    private void OnRaiseAmountInputEndEdit(string text)
    {
        // Enter tu�una bas�ld�ysa
        if (Input.GetKeyDown(KeyCode.Return) && !string.IsNullOrEmpty(raiseAmountInput.text))
        {
            // ��lemi tamamlay�n
            ProcessRaise();
        }
    }

    private void OnRaiseButtonClick()
    {
        if (!string.IsNullOrEmpty(raiseAmountInput.text))
        {
            // ��lemi tamamlay�n
            ProcessRaise();
        }
    }

    private void ProcessRaise()
    {
        // Girilen de�eri int'e d�n��t�r�n
        int raiseAmount = int.Parse(raiseAmountInput.text);

        // Bahsi y�kseltin ve paradan d���r�n
        mainPlayer.betAmount = raiseAmount;
        mainPlayer.money -= raiseAmount;

        // Mevcut bahsi g�ncelleyin
        GameController.instance.AddToCurrentBet(raiseAmount);
        UpdatePot(raiseAmount);

        // UI'yi g�ncelleyin
        UpdatePlayerUI(mainPlayer);

        HideBettingButtons();

        // S�radaki oyuncuya ge�in
        IsBettingButtonActive();
    }

    public void AllIn()
    {
        // T�m paray� bahis olarak yat�r�n
        int raiseAmount = mainPlayer.money;

        // Bahsi y�kseltin ve paradan d���r�n
        mainPlayer.betAmount = raiseAmount;
        mainPlayer.money -= raiseAmount;

        // Mevcut bahsi g�ncelleyin
        GameController.instance.AddToCurrentBet(raiseAmount);
        UpdatePot(raiseAmount);

        // UI'yi g�ncelleyin
        UpdatePlayerUI(mainPlayer);

        // InputField'� temizleyin
        raiseAmountInput.text = "";

        HideBettingButtons();

        // S�radaki oyuncuya ge�in
        IsBettingButtonActive();
    }


    public void Check()
    {
        // Mevcut bahsi kontrol edin
        int currentBet = GameController.instance.currentBet;

        // Bahis 0 ise check yapamazs�n�z
        if (currentBet == 0)
        {
            Debug.Log("Bahis yapman�z gerekiyor.");
            return;
        }

        // Bahsi kabul edin
        mainPlayer.betAmount = currentBet;

        // UI'yi g�ncelleyin
        UpdatePlayerUI(mainPlayer);

        HideBettingButtons();

        // S�radaki oyuncuya ge�in
        IsBettingButtonActive();
    }

    public bool IsBettingButtonActive()
    {
        // Butonlar�n aktiflik durumunu kontrol edin
        if (callButton.interactable || foldButton.interactable)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public void ButtonActiveControll()
    {
        callButton.gameObject.SetActive(false);
        foldButton.gameObject.SetActive(false);
        checkButton.gameObject.SetActive(false);
        raiseButton.gameObject.SetActive(false);

    }


    //betAmount: Oyuncunun bahis turunda yat�rd��� toplam bahis miktar�n� temsil eder.
    //Bir oyuncu check yaparsa, betAmount de�i�mez.
    //Bir oyuncu call veya raise yaparsa, betAmount yeni bahis miktar�na g�ncellenir.
}
