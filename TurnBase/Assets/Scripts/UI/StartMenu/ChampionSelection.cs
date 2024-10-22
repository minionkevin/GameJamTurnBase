using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChampionSelection : MonoBehaviour
{
    public bool isPlayerASelected;
    public bool isPlayerBSelected;

    public void PlayerASelected()
    {
        isPlayerASelected = true;
        isPlayerBSelected = false;

    }

    public void PlayerBSelected()
    {
        isPlayerASelected = false;
        isPlayerBSelected = true;
    }
}
