using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlay : MonoBehaviour
{

    GameStart gameStart;
    public Player[] players = new Player[4];


    private string playCardSoundName = "PlayCard";
    // Start is called before the first frame update
    void Start()
    {
        gameStart = GameObject.FindGameObjectWithTag("CardManager").GetComponent<GameStart>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Trick PlayCard(Player player, Card card, Trick trick)
    {

        GameObject cardPrefab;
        if (trick.lead == null)
        {
            trick.lead = card;
            trick.winner = player;
            trick.winningCard = card;
        }
        else
        {
            if (!trick.winningCard.MatchesTrump() && card.MatchesTrump())
            {
                trick.winningCard = card;
                trick.winner = player;
            }
            else if (trick.winningCard.MatchesTrump() && !card.MatchesTrump())
            {

            }
            else if (trick.winningCard.MatchesTrump() && card.MatchesTrump())
            {
                if (trick.winningCard.RealValue() < card.RealValue())
                {
                    trick.winningCard = card;
                    trick.winner = player;
                }
            }
            else if (trick.lead.suit != card.suit && !card.MatchesTrump())
            {

            }
            else if (trick.lead.suit == card.suit && !card.MatchesTrump() && !trick.winningCard.MatchesTrump())
            {
                if (card.RealValue() > trick.winningCard.RealValue())
                {
                    trick.winningCard = card;
                    trick.winner = player;
                }
            }
        }
        trick.hand.AddCard(card);
        if (card != null)
        {
            cardPrefab = gameStart.GetCardPrefab(card.value, card.suit);
            gameStart.RevealCard(GameManager.GetValue(cardPrefab), GameManager.GetSuit(cardPrefab));
            gameStart.AnimateCardPlay(GameManager.GetValue(cardPrefab), GameManager.GetSuit(cardPrefab), player);
            GameManager.Instance.PlayAudioSound(playCardSoundName);
        }
        return trick;
    }


    //Card Play Functions
    #region 
    public bool IsCardValid(Deck hand, Card cardToPlay, Suit trumpSuit, Card leadCard = null)
    {
        if (leadCard == null)
        {
            return true;
        }
        else
        {
            if (leadCard.suit == cardToPlay.suit && leadCard.MatchesTrump() == cardToPlay.MatchesTrump())
            {
                return true;
            }

            if (leadCard.MatchesTrump() && cardToPlay.MatchesTrump())
            {
                return true;
            }

            foreach (Card card in hand.cards)
            {
                if (leadCard.suit != trumpSuit && leadCard.MatchesTrump())
                {
                    if (trumpSuit == card.suit || (leadCard.MatchesTrump() && card.MatchesTrump()))
                    {
                        return false;
                    }
                }
                else
                {
                    if ((leadCard.suit == card.suit && !leadCard.MatchesTrump() && !card.MatchesTrump()) || (leadCard.MatchesTrump() && card.MatchesTrump()))
                    {
                        return false;
                    }
                }
            }
        }
        return true;
    }

    public void PickupTrump(Player thePlayer, GameObject discardedCardPrefab, GameObject trumpCardPrefab)
    {
        Card discardedCard;
        Card trumpCard;


        discardedCard = gameStart.GetCard(GameManager.GetValue(discardedCardPrefab), GameManager.GetSuit(discardedCardPrefab), thePlayer);

        trumpCard = gameStart.GetCard(GameManager.GetValue(trumpCardPrefab), GameManager.GetSuit(trumpCardPrefab));

        Vector3 tempLocation = discardedCardPrefab.transform.position;

        //handle animations
        gameStart.AnimateCardMove(GameManager.GetValue(discardedCardPrefab), GameManager.GetSuit(discardedCardPrefab), trumpCardPrefab.transform.position);
        gameStart.AnimateCardMove(GameManager.GetValue(trumpCardPrefab), GameManager.GetSuit(trumpCardPrefab), tempLocation);
        gameStart.HideCard(discardedCardPrefab);
        gameStart.RevealCard(trumpCard.value, trumpCard.suit);


        if (thePlayer == players[1] || thePlayer == players[2] || thePlayer == players[3])
        {
            gameStart.HideCard(trumpCardPrefab);
        }
        thePlayer.hand.AddCard(trumpCard);
        thePlayer.hand.RemoveFromDeck(discardedCard);
        gameStart.deck.AddCard(discardedCard);
        gameStart.RemoveCardFromDeck(trumpCard);
    }
    #endregion
}
