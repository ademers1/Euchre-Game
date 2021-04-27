using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayMainMenuMusic : MonoBehaviour
{
    public string mainMenuMusicName;

    private void Start()
    {
        GameManager.Instance.PlayAudioMusic(mainMenuMusicName);
    }


}
