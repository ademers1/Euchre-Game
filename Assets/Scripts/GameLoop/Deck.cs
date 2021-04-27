using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class Deck 
{
    public List<Card> cards;

    public Deck() { cards = new List<Card>();  }

    public Deck Clone()
    {
        Deck deck = new Deck();
        deck.cards = cards.Select(item => item).ToList();
        return deck;
    }

    public void Generate()
    {
        GameObject[] cardObjects = GameObject.FindGameObjectsWithTag("Card");
        
        foreach (GameObject o in cardObjects)
        {
            o.GetComponent<Card>().sprite = o.GetComponent<SpriteRenderer>().sprite;
            cards.Add(o.GetComponent<Card>());
        }
    }

    public static void Shuffle(Deck deck)
    {
        System.Random rand = new System.Random();
        for (int i = 0; i < deck.cards.Count; i++)
        {
            int j = rand.Next(i, deck.cards.Count);
            Card temporary = deck.cards[i];
            deck.cards[i] = deck.cards[j];
            deck.cards[j] = temporary;
        }
    }

    public Card DealCard(Player player)
    {
        player.hand.AddCard(cards[0]);
        return cards[0];
    }

    public void AddCard(Card card)
    {
        cards.Add(card);
    }

    public void RemoveFromDeck(Card card)
    {
        cards.Remove(card);
    }
}
