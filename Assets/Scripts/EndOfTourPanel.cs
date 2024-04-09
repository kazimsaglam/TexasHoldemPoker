using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndOfTourPanel : MonoBehaviour
{
    public static EndOfTourPanel instance;
    [SerializeField] GameObject endOfTourPanel;
    GameController gameController;
    DeckManager deckManager;
    CardDealerAnimation CardDealerAnimation;
    public GameObject[] Players;
    public List<GameObject> playerObjects;
    public List<Player> TourWinner;

    public List<GameObject> textPosition;
    public List<GameObject> cardPosition;



    private void Awake()
    {
        instance = this;
        gameController = GetComponent<GameController>();
        deckManager = GetComponent<DeckManager>();
        CardDealerAnimation = GetComponent<CardDealerAnimation>();

    }
    private void Start()
    {
        Players = deckManager.placeholderPlayerHands;
        TourWinner = gameController.Winners;
        gameController.EndOfTour += HandleEndOfTour;
        gameController.EndOfTour += HandleEndOfTour;
    }

    public void CardInfo()
    {

        for (int i = 0; i < Players.Length; i++)
        {

            GameObject playerParent = Players[i];

            GameObject playerObject = playerParent.transform.GetChild(0).gameObject;
            playerObjects.Add(playerObject);
            playerObjects[i].transform.position = cardPosition[i].transform.position;
            playerObjects[i].transform.GetChild(0).position = textPosition[i].transform.position;
            playerObjects[i].transform.GetChild(1).position = textPosition[i + 4].transform.position;
        }







    }
    public IEnumerator Winners(/*int i*/)
    {
        yield return new WaitForSeconds(5f);
        for (int i = 0; i < TourWinner.Count; i++)
        {
            AnimateWinnerCardDeal(TourWinner[i], cardPosition[4].transform.position);
        }

    }
    private void AnimateWinnerCardDeal(Player winnerCard, Vector3 targetPosition)
    {
        CardDealerAnimation.AnimateWinnerCardDeal(winnerCard, targetPosition);
    }
    private void HandleEndOfTour()
    {
        endOfTourPanel.SetActive(true);
        StartCoroutine(Winners(/*0*/));
    }
}
