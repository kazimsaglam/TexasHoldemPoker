using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDealerAnimation : MonoBehaviour
{
    public float dealDuration = 1f;
    public float winnerDealDuration = 10f;
    public void AnimateCardDeal(GameObject card, Vector3 targetPos)
    {
        StartCoroutine(AnimateCardMovement(card, targetPos));
    }

    public void AnimateFoldCardDeal(GameObject card, Vector3 targetPos)
    {
        StartCoroutine(AnimateFoldCardMovement(card, targetPos));
    }

    public void AnimateWinnerCardDeal(Player player, Vector3 targetPos)
    {
        StartCoroutine(AnimateWinnerCardMovement(player, targetPos));
    }

    IEnumerator AnimateCardMovement(GameObject card, Vector3 targetPos)
    {
        Vector3 startPos = card.transform.position;

        float startTime = Time.time;
        while(Time.time - startTime < dealDuration)
        {
            float t = (Time.time - startTime) / dealDuration;

            card.transform.position = Vector3.Lerp(startPos, targetPos, t);

            yield return null;
        }

        card.transform.position = targetPos;
    }

    IEnumerator AnimateFoldCardMovement(GameObject card, Vector3 targetPos)
    {
        Vector3 startPos = card.transform.position;

        float startTime = Time.time;
        while (Time.time - startTime < dealDuration)
        {
            float t = (Time.time - startTime) / dealDuration;

            card.transform.position = Vector3.Lerp(startPos, targetPos, t);

            yield return null;
        }

        card.transform.position = targetPos;
        card.SetActive(false);
        //Destroy(card);
    }
    IEnumerator AnimateWinnerCardMovement(Player winnerCard, Vector3 targetPos)
    {
        Vector3 startPos = winnerCard.transform.position;


        float endofTourPanel = Time.time;
        while ((Time.time - endofTourPanel) < winnerDealDuration)
        {
            float t = (Time.time - endofTourPanel) / dealDuration;
            winnerCard.transform.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }
        winnerCard.transform.position = targetPos;

    }

}
