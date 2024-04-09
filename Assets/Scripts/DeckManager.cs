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

    private GameObject cardObj;
    private int numCardsBoard = 0;
    private CardDealerAnimation cardDealerAnim;


    private void Start()
    {
        cardDealerAnim = GetComponent<CardDealerAnimation>();

        deck = listOfCards;
        deck.Shuffle();
    }


    // Button to draw the 3 first cards. Any consecutive call will draw one card till there are 5
    public void DealBoardCards()
    {
        StartCoroutine(DealBoardCardsCoroutine());
    }

    private IEnumerator DealBoardCardsCoroutine()
    {
        if (numCardsBoard < 3) //if there's less than 3 we haven't started yet, so we draw "The Flop", 3 cards on the board
        {
            for (int i = 0; i < 3; i++)
            {
                var cardObj = CreateCardObject(deck[i], transform.position, placeholderBoard.transform, i);
                boardCards.Add(cardObj.GetComponent<Card>());
                deck.Remove(deck[i]);
                numCardsBoard++;

                AnimateCardDeal(cardObj, placeholderBoard.transform.position + Vector3.right * cardSpacing * i);
                SoundManager.instance.PlayCardDealSound();

                yield return new WaitForSeconds(dealDuration);
            }
        }
        else if (numCardsBoard < 5) //We drew the flop already and we need to draw two other cards on the board
        {
            var cardObj = CreateCardObject(deck[0], transform.position, placeholderBoard.transform, numCardsBoard++);

            boardCards.Add(cardObj.GetComponent<Card>());
            deck.Remove(deck[0]);

            AnimateCardDeal(cardObj, placeholderBoard.transform.position + Vector3.right * cardSpacing * (numCardsBoard - 1));
            SoundManager.instance.PlayCardDealSound();

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
                GameObject cardObj;
                if(i == 0)
                {
                    cardObj = CreateCardObject(deck[j], transform.position, GameController.instance.playersAndBots[i].transform, j);
                    cardObj.GetComponent<Card>().UpdateCardVisual(true);
                }
                else
                {
                    cardObj = CreateCardObject(deck[j], transform.position, GameController.instance.playersAndBots[i].transform, j);
                    cardObj.GetComponent<Card>().UpdateCardVisual(false);
                }

                GameController.instance.playersAndBots[i].GetComponent<Player>().hand.Add(cardObj.GetComponent<Card>());
                deck.Remove(deck[j]);

                AnimateCardDeal(cardObj, placeholderPlayerHands[i].transform.position + Vector3.right * cardSpacing * j);
                SoundManager.instance.PlayCardDealSound();

                yield return new WaitForSeconds(dealDuration);
            }
        }
    }


    // Instantiates a card 
    private GameObject CreateCardObject(CardData data, Vector3 pos, Transform parent, int numCard)
    {
        cardObj = Instantiate(cardPrefab, new Vector3(pos.x + (cardSpacing * numCard), pos.y, 0), Quaternion.identity, parent);

        cardObj.GetComponent<Card>().cardValue = data.cardValue;
        cardObj.GetComponent<Card>().cardColor = data.cardColor;
        cardObj.name = $"{data.cardValue} {data.cardColor}";
        cardObj.transform.GetChild(1).GetComponent<Image>().sprite = data.cardSprite;

        return cardObj;
    }

    private void AnimateCardDeal(GameObject cardObj, Vector3 targetPosition)
    {
        cardDealerAnim.AnimateCardDeal(cardObj, targetPosition);
    }

}
