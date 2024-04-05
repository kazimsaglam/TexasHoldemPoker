using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDealerAnimation : MonoBehaviour
{
    public float dealDuration = 1f;

    public void AnimateCardDeal(GameObject card, Vector3 targetPos)
    {
        StartCoroutine(AnimateCardMovement(card, targetPos));
    }

    public void AnimateFoldCardDeal(GameObject card, Vector3 targetPos)
    {
        StartCoroutine(AnimateFoldCardMovement(card, targetPos));
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

        Destroy(card);
    }

}
