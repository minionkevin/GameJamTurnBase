using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BtnClick : MonoBehaviour
{
    public AudioManager audioManager;
    public void btnClickSound()
    {
        audioManager.AudioUI.clip = audioManager.AudioDic["按键点击"];
        audioManager.AudioUI.Play();
    }
    public void setActionSound()
    {
        audioManager.AudioUI.clip = audioManager.AudioDic["放入槽位"];
        audioManager.AudioUI.Play();
    }
}
