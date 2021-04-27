using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Suit { Spades = 0, Clubs = 1, Hearts = 2, Diamonds = 3, None = 5 }

public static  class SuitHelper
{
    public static bool IsTwin(this Suit suit, Suit suit2)
    {
        if (suit == suit2)
            return true;

        var comparator = (int)suit % 2 == 0 ? 1 : -1;
        return suit2 == (Suit)((int)suit + comparator);
    }
}
public class Card : MonoBehaviour
{
    public Suit suit;
    public int value;
    public Sprite sprite;   
    public int trumpValue;
    public bool isValid;
    public int originalValue;

    public bool MatchesTrump(Suit trump = Suit.None)
    {
        if (trump == Suit.None)
        {
            var turnobj = GameObject.FindGameObjectWithTag("CardManager").GetComponent<TakeTurn>();
            trump = turnobj.trump;
        }
        if (trump == suit)
        {
            return true;
        }
        if (trump.IsTwin(suit) && originalValue == 2)
            return true;
        return false;
    }

    public int RealValue(Suit trump = Suit.None)
    {
        //gets the trump value for jacks
        if (trump == Suit.None)
        {
            var turnobj = GameObject.FindGameObjectWithTag("CardManager").GetComponent<TakeTurn>();
            trump = turnobj.trump;
        }
        if (trump == Suit.None) 
            return originalValue;
        if (trump == suit && originalValue == 2)
            return 7;
        if (trump.IsTwin(suit) && originalValue == 2)
            return 6;
        return originalValue;
    }

    
}
