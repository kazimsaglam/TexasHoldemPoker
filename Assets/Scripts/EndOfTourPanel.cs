using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    _gameController.EndOfTour += HandleEndOfTour;
  }

  public void CardInfo()
  {
    for (int i = 0; i < players.Length; i++)
    {
      GameObject playerParent = players[i];

      GameObject playerObject = playerParent.transform.GetChild(0).gameObject;
      playerObjects.Add(playerObject);
      playerObjects[i].transform.position = cardPosition[i].transform.position;
      playerObjects[i].transform.GetChild(0).position = textPosition[i].transform.position;
      playerObjects[i].transform.GetChild(1).position = textPosition[i + 4].transform.position;
    }
  }

  public IEnumerator Winners( /*int i*/)
  {
    yield return new WaitForSeconds(5f);
    foreach (Player player in tourWinner)
    {
      AnimateWinnerCardDeal(player, cardPosition[4].transform.position);
    }
  }

  private void AnimateWinnerCardDeal(Player winnerCard, Vector3 targetPosition)
  {
    _cardDealerAnimation.AnimateWinnerCardDeal(winnerCard, targetPosition);
  }

  private void HandleEndOfTour()
  {
    endOfTourPanel.SetActive(true);
    StartCoroutine(Winners( /*0*/));
  }
}