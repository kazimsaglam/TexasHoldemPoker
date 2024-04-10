using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class DeckManager : MonoBehaviour
{
  public List<CardData> deck;
  public GameObject cardPrefab;
  public List<CardData> listOfCards;

  public float dealDuration = 1f;
  public int cardSpacing = 80;

  [Header("Placeholders Variables")]
  public GameObject placeholderBoard;

  public GameObject[] placeholderPlayerHands;


  public List<Card> boardCards;


  private GameObject _cardObject;
  private int _cardsOnBoard = 0;
  private CardDealerAnimation _cardDealerAnim;


  private void Start()
  {
    _cardDealerAnim = GetComponent<CardDealerAnimation>();

    deck = listOfCards;
    deck.Shuffle();
  }


  // Draw the 3 first cards. Any consecutive call will draw one card till there are 5
  public void DealBoardCards()
  {
    StartCoroutine(DealBoardCardsCoroutine());
  }

  private IEnumerator DealBoardCardsCoroutine()
  {
    if (_cardsOnBoard < 3) //if there's less than 3 we haven't started yet, so we draw "The Flop", 3 cards on the board
    {
      for (int i = 0; i < 3; i++)
      {
        GameObject cardObject = CreateCardObject(deck[i], transform.position,
          placeholderBoard.transform, i);
        boardCards.Add(cardObject.GetComponent<Card>());
        deck.Remove(deck[i]);
        _cardsOnBoard++;

        AnimateCardDeal(cardObject, placeholderBoard.transform.position
                                    + Vector3.right * cardSpacing * i);

        yield return new WaitForSeconds(dealDuration);
      }
    }
    else if (_cardsOnBoard < 5) //We drew the flop already and we need to draw two other cards on the board
    {
      GameObject cardObject = CreateCardObject(deck[0], transform.position,
        placeholderBoard.transform, _cardsOnBoard++);

      boardCards.Add(cardObject.GetComponent<Card>());
      deck.Remove(deck[0]);

      AnimateCardDeal(cardObject, placeholderBoard.transform.position
                               + Vector3.right * cardSpacing * (_cardsOnBoard - 1));

      yield return new WaitForSeconds(dealDuration);
    }
  }


  /// Draws 2 card per player for 4 playersAndBots.
  public void DealPlayerHands()
  {
    StartCoroutine(DealPlayerCardsCoroutine());
  }

  IEnumerator DealPlayerCardsCoroutine()
  {
    for (int j = 0; j < 2; j++) //Number of cards to be drawn
    {
      for (int i = 0; i < 4; i++) //Number of people
      {
        GameObject cardObject;
        if (i == 0)
        {
          cardObject = CreateCardObject(deck[j], transform.position, 
            GameController.instance.playersAndBots[i].transform, j);
          cardObject.GetComponent<Card>().UpdateCardVisual(true);
        }
        else
        {
          cardObject = CreateCardObject(deck[j], transform.position, 
            GameController.instance.playersAndBots[i].transform, j);
          cardObject.GetComponent<Card>().UpdateCardVisual(false);
        }

        GameController.instance.playersAndBots[i].GetComponent<Player>()
          .hand.Add(cardObject.GetComponent<Card>());
        deck.Remove(deck[j]);

        AnimateCardDeal(cardObject, placeholderPlayerHands[i]
          .transform.position + Vector3.right * cardSpacing * j);

        yield return new WaitForSeconds(dealDuration);
      }
    }
  }


  // Instantiates a card 
  private GameObject CreateCardObject(CardData data, Vector3 pos, Transform parent, int numCard)
  {
    _cardObject = Instantiate(cardPrefab, 
      new Vector3(pos.x + (cardSpacing * numCard), pos.y, 0), Quaternion.identity, parent);

    _cardObject.GetComponent<Card>().cardValue = data.cardValue;
    _cardObject.GetComponent<Card>().cardColor = data.cardColor;
    _cardObject.name = $"{data.cardValue} {data.cardColor}";
    _cardObject.transform.GetChild(1).GetComponent<Image>().sprite = data.cardSprite;

    return _cardObject;
  }

  private void AnimateCardDeal(GameObject cardObject, Vector3 targetPosition)
  {
    _cardDealerAnim.AnimateCardDeal(cardObject, targetPosition);
  }
}