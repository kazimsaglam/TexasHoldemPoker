using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndOfTourPanel : MonoBehaviour
{
    public static EndOfTourPanel instance;

    [SerializeField]
    GameObject endOfTourPanel;

    GameController _gameController;
    DeckManager _deckManager;
    CardDealerAnimation _cardDealerAnimation;
    public GameObject[] players;
    public List<GameObject> playerObjects;
    public List<Player> tourWinner;

    public List<GameObject> textPosition;
    public List<GameObject> cardPosition;
    public GameObject placeholderBoard;
    public TextMeshProUGUI potText;
    private int _betTextCount = 4;

    List<Player> _handValuePlayers;
    
    private void Awake()
    {
        instance = this;
        _gameController = GetComponent<GameController>();
        _deckManager = GetComponent<DeckManager>();
        _cardDealerAnimation = GetComponent<CardDealerAnimation>();
    }

    private void Start()
    {
        players = _deckManager.placeholderPlayerHands;
        tourWinner = _gameController.winners;
        _gameController.EndOfTour += HandleEndOfTour;
        _handValuePlayers = _gameController.playersAndBots;

    }

    public void CardInfo()
    {
        for (int i = 0; i < players.Length; i++)
        {
            GameObject playerParent = players[i];

            GameObject playerObject = playerParent.transform.GetChild(0).gameObject;
            playerObjects.Add(playerObject);
            playerObjects[i].transform.position = cardPosition[i].transform.position;
            playerObjects[i].SetActive(true);
            playerObjects[i].transform.GetChild(0).position = textPosition[i].transform.position;
            playerObjects[i].transform.GetChild(1).position = textPosition[i + _betTextCount].transform.position;



        }

    }
    public void TourWinnerHandleText()
    {//Daha düzgün yazýlabilir

        if (tourWinner[0] == _handValuePlayers[0])
        {
            cardPosition[0].transform.GetChild(1).gameObject.SetActive(true);

            var child = cardPosition[0].transform.GetChild(3).gameObject;
            var textComponent = child.GetComponent<TMPro.TextMeshProUGUI>();
            if (textComponent != null)
            {
                textComponent.text = _handValuePlayers[0].handValueString;
            }
            child.SetActive(true);

        }
        else if (tourWinner[0] == _handValuePlayers[1])
        {
            cardPosition[1].transform.GetChild(1).gameObject.SetActive(true);

            var child = cardPosition[1].transform.GetChild(3).gameObject;
            var textComponent = child.GetComponent<TMPro.TextMeshProUGUI>();
            if (textComponent != null)
            {
                textComponent.text = _handValuePlayers[1].handValueString;
            }
            child.SetActive(true);
        }
        else if (tourWinner[0] == _handValuePlayers[2])
        {
            cardPosition[2].transform.GetChild(1).gameObject.SetActive(true);

            var child = cardPosition[2].transform.GetChild(3).gameObject;
            var textComponent = child.GetComponent<TMPro.TextMeshProUGUI>();
            if (textComponent != null)
            {
                textComponent.text = _handValuePlayers[2].handValueString;
            }
            child.SetActive(true);
           
        }
        else if (tourWinner[0] == _handValuePlayers[3])
        {
            cardPosition[3].transform.GetChild(1).gameObject.SetActive(true);

            var child = cardPosition[3].transform.GetChild(3).gameObject;
            var textComponent = child.GetComponent<TMPro.TextMeshProUGUI>();
            if (textComponent != null)
            {
                textComponent.text = _handValuePlayers[3].handValueString;
            }
            child.SetActive(true);
           
        }

    }

    public IEnumerator Winners()
    {
        yield return new WaitForSeconds(5f);
        foreach (Player player in tourWinner)
        {
            AnimateWinnerCardDeal(player, cardPosition[4].transform.position);
            yield return new WaitForSeconds(2f);
            TourWinnerHandleText();
        }
    }

    private void AnimateWinnerCardDeal(Player winnerCard, Vector3 targetPosition)
    {
        _cardDealerAnimation.AnimateWinnerCardDeal(winnerCard, targetPosition);
    }   
    private void HandleEndOfTour()
    {
        endOfTourPanel.SetActive(true);
        StartCoroutine(Winners());
    }
    public void ReturnButton()
    {
        SceneManager.LoadScene("Scenes/InGame");
    }
    public void MainScene()
    {
        SceneManager.LoadScene("Scenes/MainMenuScene");
    }

}