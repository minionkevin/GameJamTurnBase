using UnityEngine;

public class ChampionSelection : MonoBehaviour
{

    public void PlayerASelected()
    {
        PlayerData.IsPlayerA = true;
    }

    public void PlayerBSelected()
    {
        PlayerData.IsPlayerA = false;
    }
}

public static class PlayerData
{
    public static bool IsPlayerA;
}