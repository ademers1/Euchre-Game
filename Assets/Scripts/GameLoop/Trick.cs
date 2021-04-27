using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trick 
{
    public Deck hand = new Deck();
    public Card lead = null;
    public Player winner = null;
    public Card winningCard = null;
    public int score;

    public Trick()
    {
    
    }

    public void SetWinner(Player player, int score)
    {
        winner = player;
        winner.AddScore(score);
    }


}
