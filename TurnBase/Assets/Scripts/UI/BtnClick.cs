using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BtnClick : MonoBehaviour
{
    public AudioManager audioManager;
    public void btnClickSound()
    {
        audioManager.AudioUI.clip = audioManager.AudioDic["�������"];
        audioManager.AudioUI.Play();
    }
    public void setActionSound()
    {
        audioManager.AudioUI.clip = audioManager.AudioDic["�����λ"];
        audioManager.AudioUI.Play();
    }
}
