using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public System.Random rand = new System.Random();

    private AudioManager audioManager;

    public GameStart gameStart;

    public Player[] players = new Player[4];

    public Sprite cardBack;
    public int Random(int i, int j)
    {
        int random = rand.Next(i, j);
        return random;
    }

    public static int GetValue(GameObject cardObject)
    {
        int val = cardObject.GetComponent<Card>().value;
        return val;
    }

    public static Suit GetSuit(GameObject cardObject)
    {
        Suit suit = cardObject.GetComponent<Card>().suit;
        return suit;
    }

    public void GetCards()
    {
        gameStart = GameObject.FindGameObjectWithTag("CardManager").GetComponent<GameStart>();
    }

    private void Awake()
    {
        cardBack = GameObject.FindGameObjectWithTag("HiddenCard").GetComponent<SpriteRenderer>().sprite;

        audioManager = AudioManager.instance;
        if(audioManager == null)
        {
            Debug.Log("No AudioManager found in the Scene.");
        }
    }





    //Play Audio
    #region
    public void PlayAudioSound(string _name)
    {
        audioManager.PlaySound(_name);
    }

    public void PlayAudioMusic(string _name)
    {
        audioManager.PlayMusic(_name);
    }
    #endregion
}
