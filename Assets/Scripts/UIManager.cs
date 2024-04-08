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

        // S�radaki oyuncuya ge�
        IsBettingButtonActive();
    }

    public void Call()
    {
        // Call yapmak i�in gereken para miktar�n� hesapla
        int callAmount = GameController.instance.currentBet - mainPlayer.betAmount;

        if(mainPlayer.money <= callAmount)
        {
            AllIn();
            return;
        }

        // Bahsi kabul et ve paradan d��
        mainPlayer.betAmount += callAmount;
        mainPlayer.money -= callAmount;

        UpdatePot(callAmount);

        // UI'yi g�ncelle
        UpdatePlayerUI(mainPlayer);

        mainPlayer.ShowPlayerAction("Call");

        SoundManager.instance.PlayCallAndRaiseSound();

        HideBettingButtons();

        // S�radaki oyuncuya ge�
        IsBettingButtonActive();
    }

    public void Raise()
    {
        // InputField'� aktif hale getir
        raiseAmountInput.gameObject.SetActive(true);

        // Enter tu�una bas�lmas�n� dinle
        raiseAmountInput.onEndEdit.AddListener(OnRaiseAmountInputEndEdit);

        // Raise butonuna bas�lmas�n� dinle
        raiseButton.onClick.AddListener(OnRaiseButtonClick);

        // �nerilen bahis tutar� hesaplama
        int suggestedRaise = GameController.instance.currentBet * 2;
        raiseAmountInput.placeholder.GetComponent<TMP_Text>().text = suggestedRaise.ToString();
    }

    private void OnRaiseAmountInputEndEdit(string text)
    {
        // Enter tu�una bas�ld�ysa
        if (Input.GetKeyDown(KeyCode.Return) && !string.IsNullOrEmpty(raiseAmountInput.text) && int.Parse(raiseAmountInput.text) > GameController.instance.currentBet)
        {
            // ��lemi tamamla
            ProcessRaise();
        }
    }

    private void OnRaiseButtonClick()
    {
        if (!string.IsNullOrEmpty(raiseAmountInput.text) && int.Parse(raiseAmountInput.text) > GameController.instance.currentBet)
        {
            // ��lemi tamamla
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

        // Bahsi y�kselt ve paradan d��
        mainPlayer.betAmount += raiseAmount;
        mainPlayer.money -= raiseAmount;

        // Mevcut bahsi g�ncelle
        GameController.instance.AddToCurrentBet(raiseAmount);
        UpdatePot(raiseAmount);

        // UI'yi g�ncelle
        UpdatePlayerUI(mainPlayer);

        mainPlayer.ShowPlayerAction($"Ra�se: {raiseAmount}");

        SoundManager.instance.PlayCallAndRaiseSound();

        HideBettingButtons();

        raiseAmountInput.text = null;

        // S�radaki oyuncuya ge�
        IsBettingButtonActive();
    }

    public void AllIn()
    {
        // T�m paray� bahis olarak yat�r
        int raiseAmount = mainPlayer.money;

        // Bahsi y�kselt ve paradan d��
        mainPlayer.betAmount += raiseAmount;
        mainPlayer.money -= raiseAmount;

        // Mevcut bahsi g�ncelle
        GameController.instance.AddToCurrentBet(raiseAmount);
        UpdatePot(raiseAmount);

        // UI'yi g�ncelle
        UpdatePlayerUI(mainPlayer);

        mainPlayer.ShowPlayerAction("All In");

        SoundManager.instance.PlayCallAndRaiseSound();

        HideBettingButtons();

        // S�radaki oyuncuya ge�
        IsBettingButtonActive();
    }


    public void Check()
    {
        // Mevcut bahsi kontrol et
        int currentBet = GameController.instance.currentBet;

        // Bahis 0 ise check yapamazs�n
        if (currentBet == 0)
        {
            Debug.Log("Bahis yapman�z gerekiyor.");
            return;
        }

        // Bahsi kabul et
        mainPlayer.betAmount = currentBet;

        // UI'yi g�ncelle
        UpdatePlayerUI(mainPlayer);

        mainPlayer.ShowPlayerAction("Check");

        SoundManager.instance.PlayCheckSound();

        HideBettingButtons();

        // S�radaki oyuncuya ge�
        IsBettingButtonActive();
    }

    public bool IsBettingButtonActive()
    {
        // Butonlar�n aktiflik durumunu kontrol et
        if (callButton.interactable || foldButton.interactable)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    ///////////////////////******** Param�z currentBet'den az kald�ysa oyun direkt olarak AllIn Yaps�n. ********///////////////////////


    //betAmount: Oyuncunun bahis turunda yat�rd��� toplam bahis miktar�n� temsil eder.
    //Bir oyuncu check yaparsa, betAmount de�i�mez.
    //Bir oyuncu call veya raise yaparsa, betAmount yeni bahis miktar�na g�ncellenir.
}
