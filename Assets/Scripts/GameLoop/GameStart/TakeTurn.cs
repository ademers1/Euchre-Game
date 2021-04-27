using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TakeTurn : MonoBehaviour
{

    public List<GameObject> RedScore;
    public List<GameObject> BlackScore;

    public Text trumpMakerText;

    Suit trumpSuitFlipped;

    public Suit trump = Suit.None;

    int turnCount = 0;

    bool turn;
    bool trumpSet;

    public Text turnText;
    public Text instructionsText;

    public Player[] players = new Player[4];

    Player playersTurn;

    Player trumpMaker;

    bool trumpTurnedDown;

    Queue<Player> turnOrder;

    GameStart gameStart;

    GamePlay gamePlay;

    GameObject trumpCard;
    Player playerToIgnore;

    bool isLoneHand;

    public Button heartsTrumpBtn;
    public Button diamondsTrumpBtn;
    public Button spadesTrumpBtn;
    public Button clubsTrumpBtn;
    public Button aloneButton;
    public Button notAloneButton;

    public Image trumpImage;

    public Sprite spriteHearts;
    public Sprite spriteDiamonds;
    public Sprite spriteSpades;
    public Sprite spriteClubs;

    bool decidingAlone = false;

    bool playerMustDiscard;

    bool resetTurnOrder = false;

    bool shouldWait = false;

    public bool trickFinishedAnimating = true;

    Trick currentTrick;
    Trick[] tricks = new Trick[5];

    int trickCount = 0;

    bool newTurnOrder = false;
    Trick lastTrick = new Trick();
    int timesPlayed = 0;

    bool skipThisPlayer; 

    // Start is called before the first frame update
    void Start()
    {
        gameStart = gameObject.GetComponent<GameStart>();
        tricks[trickCount] = new Trick();
        currentTrick = tricks[trickCount];
        gamePlay = gameObject.GetComponent<GamePlay>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!gameStart.handBeingDealt)
        {
            if (gameStart.turnOrderSetup && turnOrder == null)
            {
                turnOrder = gameStart.GetTurnOrder();
            }
            if (gameStart.trumpCard != null)
            {
                trumpCard = gameStart.trumpCard;
            }
            if (trumpCard != null && !trumpSet)
            {
                PickTrump();
            }
            if (trumpSet && !decidingAlone)
            {
                if (!resetTurnOrder)
                {
                    ResetTurnOrder();
                }
                if (resetTurnOrder)
                {
                    if (newTurnOrder)
                    {
                        ResetTurnOrderToWinner();
                    }
                    if (!shouldWait && !newTurnOrder)
                    {
                        StartCoroutine("PlayCard");
                    }
                }
            }
        }
    }

    private void ResetTurnOrder()
    {
        if (turnOrder.Count < 4 && playersTurn != null)
        {
            turnOrder.Enqueue(playersTurn);
        }
        Player player = turnOrder.Dequeue();
        turnOrder.Enqueue(player);
        if (player.dealer)
        {
            resetTurnOrder = true;
        }
    }

    private void ResetTurnOrderToWinner()
    {
        
        
        if (turnOrder.Count > 0)
        {
            turnOrder.Dequeue();
        }
        else
        {
            if (lastTrick != null)
            {
                if (lastTrick.winner == players[0])
                {
                    turnOrder.Enqueue(players[0]);
                    turnOrder.Enqueue(players[1]);
                    turnOrder.Enqueue(players[2]);
                    turnOrder.Enqueue(players[3]);
                    newTurnOrder = false;
                    timesPlayed = 0;
                    lastTrick = null;
                }
                else if (lastTrick.winner == players[1])
                {
                    turnOrder.Enqueue(players[1]);
                    turnOrder.Enqueue(players[2]);
                    turnOrder.Enqueue(players[3]);
                    turnOrder.Enqueue(players[0]);
                    newTurnOrder = false;
                    timesPlayed = 0;
                    lastTrick = null;
                }
                else if (lastTrick.winner == players[2])
                {
                    turnOrder.Enqueue(players[2]);
                    turnOrder.Enqueue(players[3]);
                    turnOrder.Enqueue(players[0]);
                    turnOrder.Enqueue(players[1]);
                    newTurnOrder = false;
                    timesPlayed = 0;
                    lastTrick = null;
                }
                else if (lastTrick.winner == players[3])
                {
                    turnOrder.Enqueue(players[3]);
                    turnOrder.Enqueue(players[0]);
                    turnOrder.Enqueue(players[1]);
                    turnOrder.Enqueue(players[2]);
                    newTurnOrder = false;
                    timesPlayed = 0;
                    lastTrick = null;
                }
            }
        }
    }

    void PickTrump()
    {
        if (!turn)
        {
            if(trumpMaker == null)
            {
                ManageTrump();
            }
            
            turn = true;            
            playersTurn = turnOrder.Dequeue();
        }

        if (playersTurn == players[0])
        {
            turnText.text = "Turn: Yours";
            if (trumpMaker == null)
            {
                if (!trumpTurnedDown)
                {
                    //if trump isn't turned down then I can make trump here by clicking the card above the trump stack
                    if (Input.GetMouseButtonDown(0))
                    {
                        //here is the raycast which gets where I click to see if I click the card
                        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                        GameObject card = null;
                        if (hit.collider != null)
                        {
                            card = hit.collider.gameObject;
                        }

                        if (card != null && card == trumpCard)
                        {
                            //if i clicked on the trump card then we make it trump here
                            
                            decidingAlone = true;
                            trumpMaker = playersTurn;
                            trump = card.GetComponent<Card>().suit;
                            
                            trumpMakerText.text = playersTurn.name;
                            UpdateTrumpPicture();
                            if (playersTurn.dealer)
                            {
                                instructionsText.gameObject.SetActive(true);
                                playerMustDiscard = true;
                                GetPlayersDiscardedCard();
                            }
                            else
                            {
                                trumpSet = true;
                                aloneButton.gameObject.SetActive(true);
                                notAloneButton.gameObject.SetActive(true);
                                foreach (Player p in players)
                                {
                                    if (p.dealer)
                                    {
                                        GetComputersDiscardedCard(p);
                                    }
                                }
                            }
                        }
                        else
                        {
                            //here i end my turn as I passed on making the top card trump
                            turn = false;
                        }
                    }
                }
                else
                {
                    DisplayTrumpButtons();
                }
            }
            else if (trumpMaker != null && playerMustDiscard)
            {
                GetPlayersDiscardedCard();
            }
        }
        else
        {
            //Sets Computers Trump if Applicable
            turnText.text = "Turn: CP" + playersTurn.playerNumber;
            if (trumpMaker == null)
            {
                if (!trumpTurnedDown)
                {
                    (bool, bool, Card) tuple = (false, false, null);
                    if (playersTurn.dealer)
                    {
                        tuple = playersTurn.GetComponent<ComputerAI>().EvaluateMakingTrump(GameManager.GetSuit(trumpCard), trumpCard);
                    }
                    else
                    {
                        tuple = playersTurn.GetComponent<ComputerAI>().EvaluateMakingTrump(GameManager.GetSuit(trumpCard));
                    }
                    if (playersTurn.dealer)
                    {                                              
                        if (tuple.Item1)
                        {
                            GameObject discardCard = gameStart.GetCardPrefab(tuple.Item3.value, tuple.Item3.suit);
                            Debug.Log(GameManager.GetSuit(discardCard) + " " + GameManager.GetValue(discardCard));
                            trumpMaker = playersTurn;
                            trump = trumpCard.GetComponent<Card>().suit;
                            trumpSet = true;
                            trumpMakerText.text = playersTurn.name;
                            UpdateTrumpPicture();
                            gamePlay.PickupTrump(playersTurn, discardCard, trumpCard);
                        }
                        turn = false;
                    }
                    else
                    {
                        if (tuple.Item1 || playerMustDiscard)
                        {
                            foreach (var player in players)
                            {
                                if (!player.Equals(playersTurn) && player.dealer)
                                {
                                    if (player.playerNumber == 0)
                                    {
                                        playerMustDiscard = true;
                                        instructionsText.gameObject.SetActive(true);
                                        GetPlayersDiscardedCard();
                                    }
                                    else
                                    {
                                        GetComputersDiscardedCard(player);
                                    }
                                }
                            }
                        }
                        else
                        {
                            turn = false;
                        }
                    }
                }
                else
                {
                    ShouldMakeOtherTrump(Suit.Hearts);
                    ShouldMakeOtherTrump(Suit.Diamonds);
                    ShouldMakeOtherTrump(Suit.Spades);
                    ShouldMakeOtherTrump(Suit.Clubs);
                    turn = false;
                }
            }
            else
            {
                turn = false;
            }
        }
        if (!turn)
        {
            if (turnOrder.Count < 4)
                turnOrder.Enqueue(playersTurn);
        }
    }

    IEnumerator PlayCard()
    {
        if (!newTurnOrder)
        {
            shouldWait = true;
            if (isLoneHand && !turn)
            {
                turn = true;
                playersTurn = turnOrder.Dequeue();
                if(playersTurn == playerToIgnore)
                {
                    turn = false;
                    skipThisPlayer = true;
                }
                else
                {
                    skipThisPlayer = false;
                }
            }
            else if (!turn)
            {
                turn = true;

                playersTurn = turnOrder.Dequeue();
            }

            if (playersTurn == players[0] && !skipThisPlayer)
            {
                shouldWait = false;
                turnText.text = "Turn: Yours";
                if (Input.GetMouseButtonDown(0))
                {
                    RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                    GameObject card = null;
                    if (hit.collider != null)
                    {
                        card = hit.collider.gameObject;
                    }
                    if (card != null)
                    {
                        List<GameObject> playersHand = GetPlayersHandPrefab(players[0]);
                        Card cardToPlay = gameStart.GetCard(card.GetComponent<Card>().value, card.GetComponent<Card>().suit, playersTurn);
                        if (playersHand.Contains(card) && gamePlay.IsCardValid(playersTurn.hand, cardToPlay, trump, currentTrick.lead))
                        {
                            currentTrick = gamePlay.PlayCard(playersTurn, cardToPlay, currentTrick);
                            playersTurn.hand.RemoveFromDeck(cardToPlay);
                            turn = false;
                            //remove trick from play 
                            if (currentTrick.hand.cards.Count == 4 || currentTrick.hand.cards.Count == 3 && isLoneHand)
                            {
                                newTurnOrder = true;
                                StartCoroutine("RemoveTrickFromPlay");
                            }
                        }
                    }
                }
            }
            else if(!skipThisPlayer)
            {
                //Gets Best Valid Card To Play
                turnText.text = "Turn: CP" + playersTurn.playerNumber;
                Card cardToPlay = playersTurn.GetComponent<ComputerAI>().GetBestValidCard(playersTurn, trumpMaker, trump, currentTrick);
                yield return new WaitForSeconds(0.5f);
                currentTrick = gamePlay.PlayCard(playersTurn, cardToPlay, currentTrick);
                playersTurn.hand.RemoveFromDeck(cardToPlay);
                turn = false;
                if (currentTrick.hand.cards.Count == 4 || currentTrick.hand.cards.Count == 3 && isLoneHand)
                {
                    //remove trick from play
                    newTurnOrder = true;
                    StartCoroutine("RemoveTrickFromPlay");

                }
                shouldWait = false;
            }
            else
            {
                shouldWait = false;
            }
            if (!turn)
            {
                if (turnOrder.Count < 4)
                    turnOrder.Enqueue(playersTurn);
            }
        }
    }

    IEnumerator RemoveTrickFromPlay()
    {
        gameStart.handBeingDealt = true;
        yield return new WaitForSeconds(0.5f);
        gameStart.AnimateTrick(currentTrick);
        yield return new WaitForSeconds(1f);
        gameStart.handBeingDealt = false;


        currentTrick.winner.AddTrick();       
        lastTrick = tricks[trickCount];
        
        if (trickCount < 4)
        {
            trickCount++;
        }
        else
        {
            trickCount = 0;
        }
        gameStart.AddCardsToDeck(lastTrick.hand.cards);
        
        tricks[trickCount] = new Trick();
        currentTrick = tricks[trickCount];

        if (playersTurn.hand.cards.Count == 0)
        {
            if (isLoneHand)
            {
                gameStart.AddCardsToDeck(playerToIgnore.hand.cards);
                for(int i = 0; i < 5; i++)
                {
                    playerToIgnore.hand.RemoveFromDeck(playerToIgnore.hand.cards[0]);
                }
            }
            bool didYouWin = players[0].tricksWon > players[1].tricksWon ? true : false;
            if (didYouWin)
            {
                if(players[0].tricksWon < 5)
                {
                    if (players[1] == trumpMaker || players[3] == trumpMaker)
                        players[0].AddScore(2);
                    else
                        players[0].AddScore(1);
                }
                else
                {
                    players[0].AddScore(2);
                }
                foreach (Player player in players)
                {
                    player.ResetTrick();
                }
                if (players[0].score >= 10)
                {
                    SceneManager.LoadScene("EndGame");
                }
                else
                {
                    ShowBlackScore(players[0].score);
                    ResetTrump();
                    gameStart.StartNewHand();
                }
            }
            else
            {
                if (players[1].tricksWon < 5)
                {
                    if (players[0] == trumpMaker || players[2] == trumpMaker)
                        players[1].AddScore(2);
                    else
                        players[1].AddScore(1);
                }
                else
                {
                    players[1].AddScore(2);
                }
                foreach (Player player in players)
                {
                    player.ResetTrick();
                }
                if (players[1].score >= 10)
                {
                    SceneManager.LoadScene("Loss");
                }
                else
                {
                    ShowRedScore(players[1].score);
                    ResetTrump();
                    gameStart.StartNewHand();
                }
            }

        }

    }

    void ShowRedScore(int redScore)
    {
        foreach(GameObject gameObject in RedScore)
        {
            gameObject.SetActive(false);
        }
        RedScore[redScore].SetActive(true);
    }

    void ShowBlackScore(int blackScore)
    {
        foreach(GameObject gameObject in BlackScore)
        {
            gameObject.SetActive(false);
        }
        BlackScore[blackScore].SetActive(true);
    }

    void GetPlayersDiscardedCard()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            GameObject card = null;
            List<GameObject> playersHand = GetPlayersHandPrefab(players[0]);

            if (hit.collider != null)
            {
                card = hit.collider.gameObject;
            }
            if (card != null)
            {
                foreach (var gameObject in playersHand)
                {
                    if (card == gameObject)
                    {
                        gamePlay.PickupTrump(players[0], card, trumpCard);
                        instructionsText.gameObject.SetActive(false);
                        playerMustDiscard = false;
                        if (players[0].dealer && playersTurn == players[0])
                        {
                            aloneButton.gameObject.SetActive(true);
                            notAloneButton.gameObject.SetActive(true);
                        }
                        trumpMaker = playersTurn;
                        trumpSet = true;
                        turn = false;
                        trumpMakerText.text = playersTurn.name;
                        trump = trumpCard.GetComponent<Card>().suit;
                        UpdateTrumpPicture();
                    }
                }
            }
        }
    }

    void GetComputersDiscardedCard(Player theComputer)
    {
        Card cardToDiscard = theComputer.GetComponent<ComputerAI>().GetDiscardCard(trumpCard.GetComponent<Card>().suit, theComputer);
        Debug.Log(playersTurn);
        Debug.Log(theComputer);
        GameObject discardCardPrefab = null;
        try
        {
           discardCardPrefab = gameStart.GetCardPrefab(cardToDiscard.value, cardToDiscard.suit);
        }
        catch(Exception e)
        {
            Debug.Log(e);
        }
        gamePlay.PickupTrump(theComputer, discardCardPrefab, trumpCard);
        trumpMaker = playersTurn;
        trump = trumpCard.GetComponent<Card>().suit;
        trumpSet = true;
        trumpMakerText.text = playersTurn.name;
        UpdateTrumpPicture();
        if(playersTurn != players[0])
        turn = false;
    }

    public void OnTrumpButtonClick()
    {
        string name = EventSystem.current.currentSelectedGameObject.name;
        string text = EventSystem.current.currentSelectedGameObject.GetComponentInChildren<Text>().text;
        if (name == "HeartsTrump" && text != "pass")
        {
            PlayerMakeOtherTrump(Suit.Hearts);
        }
        if (name == "DiamondsTrump" && text != "pass")
        {
            PlayerMakeOtherTrump(Suit.Diamonds);
        }
        if (name == "SpadesTrump" && text != "pass")
        {
            PlayerMakeOtherTrump(Suit.Spades);
        }
        if (name == "ClubsTrump" && text != "pass")
        {
            PlayerMakeOtherTrump(Suit.Clubs);
        }
        heartsTrumpBtn.gameObject.SetActive(false);
        diamondsTrumpBtn.gameObject.SetActive(false);
        spadesTrumpBtn.gameObject.SetActive(false);
        clubsTrumpBtn.gameObject.SetActive(false);
        heartsTrumpBtn.GetComponentInChildren<Text>().text = "Hearts";
        diamondsTrumpBtn.GetComponentInChildren<Text>().text = "Diamonds";
        spadesTrumpBtn.GetComponentInChildren<Text>().text = "Spades";
        clubsTrumpBtn.GetComponentInChildren<Text>().text = "Clubs";
    }

    public void PlayerMakeOtherTrump(Suit suit)
    {
        trumpMaker = playersTurn;
        trump = suit;
        turn = false;
        trumpSet = true;
        trumpMakerText.text = playersTurn.name;
        UpdateTrumpPicture();
    }

    public void GoAlone()
    {
        isLoneHand = true;
        aloneButton.gameObject.SetActive(false);
        notAloneButton.gameObject.SetActive(false);
        playerToIgnore = players[2];
        foreach(Card card in playerToIgnore.hand.cards)
        {
            gameStart.GetCardPrefab(card.value, card.suit).SetActive(false);
        }
        decidingAlone = false;
        turn = false;
    }

    public void DontGoAlone()
    {
        aloneButton.gameObject.SetActive(false);
        notAloneButton.gameObject.SetActive(false);
        decidingAlone = false;
        turn = false;
    }

    void UpdateTrumpPicture()
    {
        switch (trump)
        {
            case Suit.Hearts:
                trumpImage.sprite = spriteHearts;
                break;
            case Suit.Diamonds:
                trumpImage.sprite = spriteDiamonds;
                break;
            case Suit.Spades:
                trumpImage.sprite = spriteSpades;
                break;
            case Suit.Clubs:
                trumpImage.sprite = spriteClubs;
                break;
            case Suit.None:
                trumpImage.sprite = null;
                break;
            default:
                trumpImage.sprite = null;
                break;
        }
    }

    void DisplayTrumpButtons()
    {
        //here we set trump buttons for the player to choose a different trump to active
        heartsTrumpBtn.gameObject.SetActive(true);
        diamondsTrumpBtn.gameObject.SetActive(true);
        spadesTrumpBtn.gameObject.SetActive(true);
        clubsTrumpBtn.gameObject.SetActive(true);
        switch (trumpCard.GetComponent<Card>().suit)
        {
            case Suit.Hearts:
                heartsTrumpBtn.GetComponentInChildren<Text>().text = "Pass";
                break;
            case Suit.Diamonds:
                diamondsTrumpBtn.GetComponentInChildren<Text>().text = "Pass";
                break;
            case Suit.Spades:
                spadesTrumpBtn.GetComponentInChildren<Text>().text = "Pass";
                break;
            case Suit.Clubs:
                clubsTrumpBtn.GetComponentInChildren<Text>().text = "Pass";
                break;
        }
    }

    void ManageTrump()
    {
        
        if (trumpMaker == null && turnCount == 4 && !trumpTurnedDown)
        {
            //if trump isn't made and everyone has had a chance to make trump then
            //turn down the card that was on the stack and let them choose a different trump
            trumpTurnedDown = true;
            gameStart.HideCard(trumpCard);            
        }   
        else if(trumpMaker == null && turnCount == 4 && trumpTurnedDown)
        {
            //Redo Hand
        }
        if (turnCount == 4)
        {
            turnCount = 0;
        }
        else
        {
            turnCount++;
        }
    }
    
    List<GameObject> GetPlayersHandPrefab(Player thePlayer)
    {
        int handSize = thePlayer.hand.cards.Count;
        List<GameObject> playersCards = new List<GameObject>();
        for(int i = 0; i < handSize; i++)
        {
            GameObject temp = gameStart.GetCardPrefab(thePlayer.hand.cards[i].value, thePlayer.hand.cards[i].suit);
            playersCards.Add(temp);
        }
        return playersCards;
    }

    void ShouldMakeOtherTrump(Suit suit)
    {
        (bool, bool, Card) tuple;
        tuple = playersTurn.GetComponent<ComputerAI>().EvaluateMakingTrump(suit);
        if (tuple.Item1)
        {
            if (tuple.Item2)
            {
                isLoneHand = true;
                playerToIgnore = playersTurn.GetPartner();
            }
            turn = false;
            trumpMaker = playersTurn;
            trump = suit;
            trumpSet = true;
            trumpMakerText.text = playersTurn.name;
            UpdateTrumpPicture();
        }
    }

    void ResetTrump()
    {
        trumpCard = null;
        turn = false;
        isLoneHand = false;
        decidingAlone = false;
        playerToIgnore = null;
        turnCount = 0;
        trumpTurnedDown = false;
        trumpMaker = null;
        trump = Suit.None;
        trumpSet = false;
        trumpMakerText.text = "";
        UpdateTrumpPicture();
        newTurnOrder = false;
        shouldWait = false;
        resetTurnOrder = false;
        skipThisPlayer = false;
    }
}
