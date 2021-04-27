using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ComputerAI : MonoBehaviour
{
    public Card trumpCard;
    Deck hand = new Deck();

    public GameObject[] cards = new GameObject[24];
    //check if hand is size 6 
    //returns shud make trump, shud go alone, shud drop card, card to drop
    public (bool Pickup, bool Alone, Card cardToDiscard) EvaluateMakingTrump(Suit trumpSuit, GameObject trumpPrefab = null)
    {
        hand = gameObject.GetComponent<Player>().hand.Clone();
        //CLONE Card
        Card leastLikelyToWin = null;

        int[] threeHighestTrumpValues = new int[3];
        int[] threeHighestNonTrumpValues = new int[3];
        int trumpAmount = 0;
        //hand = CorrectTrumpValues(hand, trumpSuit);


        //if they are dealer they get an extra card to use for deciding on making trump
        if(trumpPrefab != null)
        {
            trumpCard = trumpPrefab.GetComponent<Card>();
            hand.AddCard(trumpCard);
        }

        foreach(var c in hand.cards)
        {
            if (c.suit == trumpSuit || c.MatchesTrump(trumpSuit))
            {
                trumpAmount++;
                setHighest3(c.RealValue(trumpSuit), threeHighestTrumpValues);
                /*var high2 = c.RealValue(trumpSuit);
                for (int i = 0; i < threeHighestTrumpValues.Length; i++)
                {
                    if (threeHighestTrumpValues[i] < high2)
                    {
                        System.Array.Sort(threeHighestTrumpValues)
                        var temp = threeHighestTrumpValues[i];
                        threeHighestTrumpValues[i] = high2;
                        high2 = temp;
                    }
                }*/
            }
            else
            {
                setHighest3(c.RealValue(trumpSuit), threeHighestNonTrumpValues);
            }
        }
        if(trumpAmount <= 1)
        {
            return (false, false, leastLikelyToWin);
        }
        else if(trumpAmount == 2)
        {
            //if highest value trump is greter than an ace and your second highest value non trump is greater than a queen
            if(threeHighestTrumpValues[0] >= 5 && threeHighestTrumpValues[1] >= 3)
            {
                //then we check if their highest value trump is an ace and they have 2 other aces 
                //or they have a comination of 3 kings or an ace and a queen or better
                if((threeHighestNonTrumpValues[0] == 5 && threeHighestNonTrumpValues[1] == 5) || threeHighestNonTrumpValues.Sum() >= 12)
                {
                    if(trumpPrefab != null)
                    {
                        leastLikelyToWin = GetLowestValueCard(trumpSuit, trumpAmount);
                    }
                    return (true, false, leastLikelyToWin);
                }
            }
        }
        //if they have 3 trump
        else if(trumpAmount == 3)
        {
            //here we check if their highest non trump is a queen or better or 
            //they have a queen or better as their third highest trump or they're highest trump value is ane ace or better or their second highest trump value is 4
            // if(threeHighestNonTrumpValues[0] >= 3 || threeHighestTrumpValues[2] >= 3)
            if (threeHighestNonTrumpValues[0] >= 4 || threeHighestNonTrumpValues[0] == 3 || threeHighestTrumpValues[2] >= 2 || (threeHighestTrumpValues[0] >= 6 && threeHighestTrumpValues[1] >= 4))
            {
                if (trumpPrefab != null)
                {
                    leastLikelyToWin = GetLowestValueCard(trumpSuit, trumpAmount);
                }
                return (true, false, leastLikelyToWin);
                
            }
            else
            {
                return (false, false, leastLikelyToWin);
            }
        }
        else if(trumpAmount == 4)
        {   
            //go alone
            if(threeHighestTrumpValues[0] >= 5 && threeHighestTrumpValues[1] >= 4 && threeHighestNonTrumpValues[0] >= 4)
            {
                if (trumpPrefab != null)
                {
                    leastLikelyToWin = GetLowestValueCard(trumpSuit, trumpAmount);
                }
                return (true, true, leastLikelyToWin);                
            }
            else
            {
                if (trumpPrefab != null)
                {
                    leastLikelyToWin = GetLowestValueCard(trumpSuit, trumpAmount);
                }
                return (true, false, leastLikelyToWin);       
            }
        }
        else
        {
            leastLikelyToWin = GetLowestValueCard(trumpSuit, trumpAmount);
            return (true, false, leastLikelyToWin);          
        }
        return (false, false, leastLikelyToWin);
    }

    public Card GetDiscardCard(Suit trumpSuit, Player computerPlayer)
    {
        Deck hand = computerPlayer.hand;
        Card leastLikelyToWin = null;
        int lowestValue = 5;
        foreach (var c in hand.cards)
        {
            if (!c.MatchesTrump(trumpSuit) && c.suit != trumpSuit)
            {
                if (c.RealValue(trumpSuit) <= lowestValue)
                {
                    lowestValue = c.RealValue(trumpSuit);
                    leastLikelyToWin = c;
                }
            }
        }
        if (leastLikelyToWin == null)
        {
            foreach (var c in hand.cards)
            {
                if (c.MatchesTrump(trumpSuit) || c.suit == trumpSuit)
                {
                    if (c.value <= lowestValue)
                    {
                        lowestValue = c.RealValue(trumpSuit);
                        leastLikelyToWin = c;
                    }
                }
            }
        }
        Debug.Log(leastLikelyToWin.suit + " " + leastLikelyToWin.value);
        return leastLikelyToWin;
    }

    public Card GetLowestValueCard(Suit trumpSuit, int trumpAmount)
    {
        Card lowestValueCard = null;
        int lowestValue = 5;
        foreach (var c in hand.cards)
        {
            if (!c.MatchesTrump(trumpSuit) && c.suit != trumpSuit && c.RealValue(trumpSuit) <= lowestValue)
            {
                lowestValue = c.RealValue(trumpSuit);
                lowestValueCard = c;
            }
            
            if (trumpAmount == hand.cards.Count && c.MatchesTrump(trumpSuit) && 
                c.suit == trumpSuit && c.RealValue(trumpSuit) <= lowestValue)
            {
                lowestValue = c.RealValue(trumpSuit);
                lowestValueCard = c;
            }
        }
        return lowestValueCard;
    }

    public Card GetLowestValueCard(Deck hand)
    {
        Card lowestValueCard = null;
        foreach(Card c in hand.cards)
        {
            if(!c.MatchesTrump() && lowestValueCard == null)
            {
                lowestValueCard = c;
            }
            if(!c.MatchesTrump() && lowestValueCard == null)
            {
                if(c.RealValue() < lowestValueCard.RealValue())
                {
                    lowestValueCard = c;
                }
            }
        }
        if(lowestValueCard == null)
        {
            lowestValueCard = GetLowestTrump(hand);
        }
        return lowestValueCard;
    }

    public Card GetBestValidCard(Player player, Player trumpMaker, Suit trumpSuit, Trick trick)
    { 
        Card cardToPlay;

        bool leading = trick.lead == null ? true : false;
        bool ourTeamMadeTrump = trumpMaker == player || player.GetPartner() ? true : false;
        Suit suitToFollow = Suit.None;


        if (leading)
        {
            if (ourTeamMadeTrump)
            {
                cardToPlay = GetHighestTrump(player.hand);
                if(cardToPlay != null)
                {
                    return cardToPlay;
                }                
            }
            cardToPlay = GetHighestNonTrump(player.hand);
            return cardToPlay;
        }
        else
        {
            //GetSuitToFollow
            if (trick.lead.MatchesTrump(trumpSuit))
            {
                suitToFollow = trumpSuit;
            }
            else
            {
                suitToFollow = trick.lead.suit;
            }
            bool canFollowSuit = false;
            foreach(Card card in player.hand.cards)
            {
                //if suit to follow = trump then we want to make any trump will in players hand will change canFollowSuit to true;
                //if suit to follow != trump than we want to make canFollowSuit true only if the suitToFollow has a non trump suit of suit to follow;
                /*
                 * Suit to Folllow hearts and hearts is trump
                 * if they have only jack of diamonds 
                 * suit to follow = true
                 * 
                 * Suit to follow is Clubs and Trump is spades
                 * if they have jack of clubs but no other club
                 * suit to follow = false
                 * 
                 * Suit to follow is Diamonds and hearts is trump if the only diamond they have is jack of diamonds
                 * suit to follow = false
                 */ 
                if((card.suit == suitToFollow && card.RealValue() < 6) || (card.MatchesTrump() &&  suitToFollow == trumpSuit))
                {
                    canFollowSuit = true;
                    break;
                }
            }
            if (canFollowSuit)
            {
                /*
                do we win trick 
                yes -> throw least valuable 
                no  -> throw most valuable
                */
                if(trick.winner == player.GetPartner())
                {
                    if(suitToFollow == trumpSuit)
                    {
                        cardToPlay = GetLowestTrump(player.hand);
                        return cardToPlay;
                    }
                    cardToPlay = GetLeastValuableCardBasedOnSuit(player.hand, suitToFollow);
                    return cardToPlay;
                }
                else
                {
                    if(suitToFollow == trumpSuit)
                    {
                        cardToPlay = GetHighestTrump(player.hand);
                        if (cardToPlay.RealValue(trumpSuit) > trick.lead.RealValue(trumpSuit))
                        {
                            return cardToPlay;
                        }
                        else
                        {
                            return GetLowestTrump(player.hand);
                        }
                    }
                    cardToPlay = GetMostValuableCardBasedOnSuit(player.hand, suitToFollow);
                    return cardToPlay;
                }
            }
            else
            {
                if(suitToFollow == trumpSuit)
                {
                    cardToPlay = GetLowestValueCard(player.hand);
                }
                else
                {
                    cardToPlay = GetLowestTrump(player.hand);
                    if(cardToPlay == null)
                    {
                        cardToPlay = GetLowestValueCard(player.hand);
                    }
                }
                return cardToPlay;
            }
        }
    }

    public void setHighest3(int value, int[] arr)
    {
        for (int i = 0; i < arr.Length; i++)
        {
            if (arr[i] < value)
            {
                var temp = arr[i];
                arr[i] = value;
                value = temp;
            }
        }
    }

    public Card GetHighestTrump(Deck deck)
    {
        Card highestValuetrump = null;
        foreach(Card card in deck.cards)
        {
            if(highestValuetrump == null && card.MatchesTrump())
            {
                highestValuetrump = card;
            }
            if(highestValuetrump != null && card.MatchesTrump())
            {
                if(card.RealValue() > highestValuetrump.RealValue())
                {
                    highestValuetrump = card;
                }
            }
        }
        return highestValuetrump;
    }

    public Card GetLowestTrump(Deck deck)
    {
        Card lowestValuetrump = null;
        foreach (Card card in deck.cards)
        {
            if (lowestValuetrump == null && card.MatchesTrump())
            {
                lowestValuetrump = card;
            }
            if (lowestValuetrump != null && card.MatchesTrump())
            {
                if (card.RealValue() < lowestValuetrump.RealValue())
                {
                    lowestValuetrump = card;
                }
            }
        }
        return lowestValuetrump;
    }

    public Card GetHighestNonTrump(Deck deck)
    {
        Card highestNonTrump = null;
        foreach(Card card in deck.cards)
        {
            if(highestNonTrump == null && !card.MatchesTrump())
            {
                highestNonTrump = card;
            }
            if(highestNonTrump != null && !card.MatchesTrump())
            {
                if(card.RealValue() > highestNonTrump.RealValue())
                {
                    highestNonTrump = card;
                }
            }
        }
        return highestNonTrump;
    }

    public Card GetLeastValuableCardBasedOnSuit(Deck hand, Suit suit)
    {
        Card leastValuableCard = null;
        foreach(Card card in hand.cards)
        {
            if(card.suit == suit && leastValuableCard == null && !card.MatchesTrump())
            {
                leastValuableCard = card;
            }
            
            if(card.suit == suit && leastValuableCard != null && !card.MatchesTrump())
            {
                if(card.RealValue() < leastValuableCard.RealValue())
                {
                    leastValuableCard = card;
                }
            }
        }
        return leastValuableCard;
    }

    public Card GetMostValuableCardBasedOnSuit(Deck hand, Suit suit)
    {
        Card mostValuableCard = null;
        foreach (Card card in hand.cards)
        {
            if (card.suit == suit && mostValuableCard == null && !card.MatchesTrump())
            {
                mostValuableCard = card;
            }

            if (card.suit == suit && mostValuableCard != null && !card.MatchesTrump())
            {
                if (card.RealValue() > mostValuableCard.RealValue())
                {
                    mostValuableCard = card;
                }
            }
        }
        return mostValuableCard;
    }
}
