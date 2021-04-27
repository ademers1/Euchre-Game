using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Deck hand;
    Player partner;
    public bool dealer { get; private set; }
    public int score { get; set; } = 0;
    public int tricksWon { get; private set; }
    public Vector3[] cardPositions;
    public int playerNumber;
    public Vector3 playCardLocation;
    public bool isComputer { get; private set; } = true;

    public void SetPartner(Player player)
    {
        partner = player;
        player.partner = this;
    }

    public Player GetPartner()
    {
        return partner;
    }

    public void AddScore(int score)
    {
        this.score += score;
        partner.score = this.score;
    }

    public void AddTrick()
    {
        tricksWon++;
        partner.tricksWon = tricksWon;
    }

    public void ResetTrick()
    {
        tricksWon = 0;
        partner.tricksWon = tricksWon;
    }

    public void SetDealer(bool isDealer)
    {
        this.dealer = isDealer;
    }

    public void SetComputerPlayer()
    {
        this.isComputer = false;
    }
}
