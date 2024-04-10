using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[Serializable]
public class Card : MonoBehaviour, IComparable
{
    public CardValue cardValue;
    public CardColor cardColor;

    public GameObject cardFront;
    public GameObject cardBack;

    private void Start()
    {
        GameController.instance.EndOfTour += FinishTour;
    }

    public void UpdateCardVisual(bool isFaceUp)
    {
        cardFront.SetActive(false);
        cardBack.SetActive(false);

        if (isFaceUp)
        {
            cardFront.SetActive(true);
        }
        else
        {
            cardBack.SetActive(true);
        }
    }
    public void FinishTour()
    {
        cardBack?.SetActive(false);
        cardFront.SetActive(true);
    }

    /// <summary>
    /// Icomparable interface
    /// </summary> 
    /// <param name="obj">object to compare</param>
    /// <returns>-1 < obj, 0 = obj, 1 > obj </returns>
    public int CompareTo(object obj)
    {
        
        if (obj is Card)
        {
            return this.cardValue.CompareTo((obj as Card).cardValue);
        }
        else
        {
            throw new ArgumentException("Object to compare is not a Card");
        }
    }

    
}
