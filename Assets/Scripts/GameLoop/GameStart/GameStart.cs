using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class GameStart : MonoBehaviour
{
    
    public Deck deck = new Deck();
    public Card[] cardSpriteList;
    public Player[] players = new Player[4];

    public GameObject[] cards = new GameObject[24];

    public Text turnText;

    public Card[] pool;

    Vector3 deckLocation = new Vector3(0, 0, -5);

    Queue<Player> turnOrder = new Queue<Player>();

    int cardsDealt = 0;

    bool trumpFlipped = false;

    public GameObject trumpCard { get; private set; }

    public bool turnOrderSetup { get; private set; }

    private int playerPosition = 0;

    bool newDeal = false;
    public bool handBeingDealt;

    public const float animationSpeed = 1.5f;

    TakeTurn takeTurn;
    //sounds
    public string shuffleSoundName;
    public string dealSoundName;
    public string cardMusic;

    // Start is called before the first frame update
    void Start()
    {
        InitCards();
        GameManager.Instance.PlayAudioMusic(cardMusic);
        takeTurn = gameObject.GetComponent<TakeTurn>();
        //Generate the Deck of cards
        deck.Generate();
        StartHand();


        players[0].SetComputerPlayer();

        //set partners
        players[0].SetPartner(players[2]);
        players[1].SetPartner(players[3]);

        //Get the Dealer
        int deal = GameManager.Instance.Random(0, 4);
        switch (deal)
        {
            case 1:
                players[1].SetDealer(true);
                break;
            case 2:
                players[2].SetDealer(true);
                break;
            case 3:
                players[3].SetDealer(true);
                break;
            case 0:
            default:
                players[0].SetDealer(true);
                break;
        }
        SetTurnOrder();
    }

    // Update is called once per frame
    void Update()
    {
        if (newDeal)
        {
            FixTurnOrder();
            SetTurnOrder();
        }
        newDeal = false;
        if (turnOrder.Count == 4 && cardsDealt < 20)
        {
            StartCoroutine(DealCards());
            cardsDealt++;
        }
        if (cardsDealt == 20 && trumpFlipped == false)
        {

            StartCoroutine("FlipTrump");
            trumpFlipped = true;
            turnOrderSetup = true;
            handBeingDealt = false;
        }       
    }

    void StartHand()
    {
        Deck.Shuffle(deck);
        GameManager.Instance.PlayAudioSound(shuffleSoundName);
    }

    public void StartNewHand()
    {

        handBeingDealt = true;
        Deck.Shuffle(deck);
        GameManager.Instance.PlayAudioSound(shuffleSoundName);
        players[0].SetDealer(true);
        players[3].SetDealer(false);
        RecenterDeck();
        trumpFlipped = false;
        trumpCard = null;
        newDeal = true;
        cardsDealt = 0;
        turnOrderSetup = false;        
    }

    void FixTurnOrder()
    {
        while(turnOrder.Count != 0)
        {
            turnOrder.Dequeue();
        }
    }

    void SetTurnOrder()
    {
        while (!players[3].dealer)
        {
            var temp = players[0];
            players[0] = players[1];
            players[1] = players[2];
            players[2] = players[3];
            players[3] = temp;
            playerPosition--;
            if (playerPosition == -1)
                playerPosition = 3;
        }
        foreach(var player in players)
        {
            turnOrder.Enqueue(player);
        }
    }

    void RecenterDeck()
    {
        foreach (GameObject obj in cards)
        {
            HideCard(obj);
            obj.SetActive(true);
        }
    }

    //Intialize Cards
    public void InitCards()
    {
        Suit suit;
        for (int i = 0; i < cards.Length; i++)
        {

            GameObject g = Instantiate(cards[i], Vector3.zero, Quaternion.identity);
            cards[i] = g;
        }
        int count = 0;
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                suit = (Suit)i;
                cards[count].GetComponent<Card>().suit = suit;
                cards[count].AddComponent<BoxCollider2D>();
                switch (j)
                {
                    case 0:
                        cards[count].GetComponent<Card>().value = j;
                        break;
                    case 1:
                        cards[count].GetComponent<Card>().value = j;
                        break;
                    case 2:
                        cards[count].GetComponent<Card>().value = j;
                        break;
                    case 3:
                        cards[count].GetComponent<Card>().value = j;
                        break;
                    case 4:
                        cards[count].GetComponent<Card>().value = j;
                        break;
                    case 5:
                        cards[count].GetComponent<Card>().value = j;
                        break;
                    default:
                        cards[count].GetComponent<Card>().value = -1;
                        break;
                }
                count++;
            }
        }
    }


    IEnumerator DealCards()
    {
        yield return new WaitForSeconds(0.1f);

        //deals card to players hand
        Player tempPlayer = turnOrder.Dequeue();
        Card card = deck.DealCard(tempPlayer);



        AnimateCardMove(card.value, card.suit, tempPlayer.cardPositions[tempPlayer.hand.cards.Count - 1]);
        deck.RemoveFromDeck(card);
        if (tempPlayer == players[playerPosition])
        {
            RevealCard(card.value, card.suit);
        }
        turnOrder.Enqueue(tempPlayer);
    }

    IEnumerator FlipTrump()
    {
        yield return new WaitForSeconds(1.5f);
        List<GameObject> remainingDeck = new List<GameObject>();
        Card topCard = deck.cards[0];
        Player dealer = GetDealer();

        foreach (Card c in deck.cards)
        {
            remainingDeck.Add(GetCardPrefab(c.value, c.suit));
        }
        trumpCard = GetCardPrefab(topCard.value, topCard.suit);

        this.transform.position = dealer.transform.GetChild(0).position;
        foreach (GameObject g in remainingDeck)
        {
            g.transform.position = dealer.transform.GetChild(0).position;
        }

        trumpCard.transform.position = new Vector3(trumpCard.transform.position.x, trumpCard.transform.position.y, -7);
        RevealCard(GameManager.GetValue(trumpCard), GameManager.GetSuit(trumpCard));
    }



    public Card GetCard(int value, Suit suit, Player player = null)
    {
        Card card = null;
        if(player == null)
        {
            foreach(Card c in deck.cards)
            {
                if (c.value == value && c.suit == suit)
                {
                    card = c;
                }
            }
        }
        else
        {
            foreach(Card c in player.hand.cards)
            {
                if(c.value == value && c.suit == suit)
                {
                    card = c;
                }
            }
        }
        return card;
    }

    public void RevealCard(int value, Suit suit)
    {
        GameObject cardPrefab = GetCardPrefab(value, suit);
        GameObject[] sprites = GameObject.FindGameObjectsWithTag("Card");
        foreach (Card c in pool)
        {
            if(value == c.value && suit == c.suit)
            {
                cardPrefab.GetComponent<SpriteRenderer>().sprite = c.sprite;
            }
        }
    }

    public Queue<Player> GetTurnOrder()
    {
        return turnOrder;
    }

    public void AddCardToDeck(Card card)
    {
        deck.AddCard(card);
    }

    public void AddCardsToDeck(List<Card> cards)
    {
        foreach(Card c in cards)
        {
            deck.AddCard(c);
        }
    }

    public Player GetDealer()
    {        
        foreach(Player player in players)
        {
            if (player.dealer)
            {
                return player;
            }
        }
        //no dealer then return last player in array
        return players[players.Count() - 1];
    }

    public void RemoveCardFromDeck(Card card)
    {
        deck.RemoveFromDeck(card);
    }

    public GameObject GetCardPrefab(int value, Suit suit)
    {
        GameObject g = GameObject.FindGameObjectWithTag("Card");

        foreach (GameObject c in cards)
        {
            if (c.GetComponent<Card>().value == value && c.GetComponent<Card>().suit == suit)
            {
                g = c.gameObject;
            }
        }
        return g;
    }

    public void AnimateCardMove(int value, Suit suit, Vector3 locationToMoveTo)
    {
        GameObject cardToAnimate = GetCardPrefab(value, suit);
        iTween.MoveTo(cardToAnimate, locationToMoveTo, animationSpeed);
    }

    public void AnimateCardPlay(int value, Suit suit, Player player)
    {
        GameObject cardToAnimate = GetCardPrefab(value, suit);
        iTween.MoveTo(cardToAnimate, player.playCardLocation, animationSpeed);
    }

    public void AnimateTrick(Trick currentTrick)
    {
        int i = 0;
        Suit cardSuit = Suit.None;
        int cardValue = -1;
        takeTurn.trickFinishedAnimating = false;
        foreach (Card card in currentTrick.hand.cards)
        {
            iTween.MoveTo(GetCardPrefab(card.value, card.suit), new Vector3(0, 0, -5), animationSpeed);
            HideCard(GetCardPrefab(card.value, card.suit));
        }
        Debug.Log(currentTrick.winner.transform.position);
        foreach (Card card in currentTrick.hand.cards)
        {
            Debug.Log(i);
            if (i < currentTrick.hand.cards.Count - 1)
            {
                GetCardPrefab(card.value, card.suit).SetActive(false);
                i++;
            }
            else
            {
                iTween.MoveTo(GetCardPrefab(card.value, card.suit), currentTrick.winner.transform.position, animationSpeed);
                cardValue = card.value;
                cardSuit = card.suit;
            }
        }
        StartCoroutine(HideTrick(cardValue, cardSuit));
    }

    public IEnumerator HideTrick(int value, Suit suit)
    {
        yield return new WaitForSeconds(1);
        GetCardPrefab(value, suit).SetActive(false);
        takeTurn.trickFinishedAnimating = true;
    }

    public void HideCard(GameObject card)
    {
        card.GetComponent<SpriteRenderer>().sprite = GameManager.Instance.cardBack;
    }

}
