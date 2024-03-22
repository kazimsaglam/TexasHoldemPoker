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

    #region placeholders variables

    public GameObject placeholderBoard;
    public GameObject placeholderHand;

    public GameObject[] placeholderPlayerHands;

    #endregion

    public List<Card> boardCards;
    public List<Card>[] playerCards = new List<Card>[4];

    public GameObject drawPlayerCardsButton;

    private GameObject cardObj;
    private int numHand = 1;
    private int numCardsBoard = 0;

    private CardDealerAnimation cardDealerAnim;


    private void Start()
    {
        cardDealerAnim = GetComponent<CardDealerAnimation>();

        for (int i = 0; i < playerCards.Length; i++)
        {
            playerCards[i] = new List<Card>();
        }

        deck = listOfCards;
        deck.Shuffle();
    }


    // Button to draw the 3 first cards. Any consecutive call will draw one card till there are 5
    public void DrawFlopCards()
    {
        StartCoroutine(DrawFlopCardsCoroutine());
    }


    private IEnumerator DrawFlopCardsCoroutine()
    {
        if (numCardsBoard < 3) //if there's less than 3 we haven't started yet, so we draw "The Flop", 3 cards on the board
        {
            for (int i = 0; i < 3; i++)
            {
                var cardObj = InstantiateCard(deck[i], transform.position, placeholderBoard.transform, i);
                //num_hand++;
                boardCards.Add(cardObj.GetComponent<Card>());
                deck.Remove(deck[i]);
                numCardsBoard++;

                cardDealerAnim.AnimateCardDeal(cardObj, placeholderBoard.transform.position + Vector3.right * cardSpacing * i);

                yield return new WaitForSeconds(dealDuration);
            }
        }
        else if (numCardsBoard < 5) //We drew the flop already and we need to draw two other cards on the board
        {
            var cardObj = InstantiateCard(deck[0], transform.position, placeholderBoard.transform, numCardsBoard++);

            boardCards.Add(cardObj.GetComponent<Card>());
            deck.Remove(deck[0]);

            cardDealerAnim.AnimateCardDeal(cardObj, placeholderBoard.transform.position + Vector3.right * cardSpacing * (numCardsBoard - 1));

            yield return new WaitForSeconds(dealDuration);
        }
    }


    /// Draws 2 card per player for 4 players.
    public void DrawPlayerHands()
    {
        if (playerCards[0].Count == 0)
        {
            StartCoroutine(DrawPlayerCardsCoroutine());
        } 
    }


    // Instantiates a card 
    private GameObject InstantiateCard(CardData data, Vector3 pos, Transform parent, int numCard)
    {
        cardObj = Instantiate(cardPrefab, new Vector3(pos.x + (cardSpacing * numCard), pos.y, 0), Quaternion.identity, parent);

        cardObj.GetComponent<Card>().cardValue = data.cardValue;
        cardObj.GetComponent<Card>().cardColor = data.cardColor;
        cardObj.name = $"{data.cardValue} {data.cardColor}";
        cardObj.transform.GetChild(1).GetComponent<Image>().sprite = data.cardSprite;

        return cardObj;
    }

    IEnumerator DrawPlayerCardsCoroutine()
    {
        drawPlayerCardsButton.SetActive(false);
        for (int j = 0; j < 2; j++) //Number of cards to be drawn
        {
            for (int i = 0; i < 4; i++) //Number of people
            {
                var cardObj = InstantiateCard(deck[j], transform.position, placeholderPlayerHands[i].transform, j);
                playerCards[i].Add(cardObj.GetComponent<Card>());
                deck.Remove(deck[j]);

                cardDealerAnim.AnimateCardDeal(cardObj, placeholderPlayerHands[i].transform.position + Vector3.right * cardSpacing * j);

                yield return new WaitForSeconds(dealDuration);
            }
        }
    }

}
