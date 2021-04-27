using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animate : MonoBehaviour
{

    GameStart gameStart;

    private void Start()
    {
        gameStart = GameObject.FindGameObjectWithTag("CardManager").GetComponent<GameStart>();
    }







}
