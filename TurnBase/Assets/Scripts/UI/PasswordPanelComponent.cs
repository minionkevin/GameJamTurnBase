using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PasswordPanelComponent : MonoBehaviour
{
    // 这里只是一个简单的点击按钮后开始
    // 在这个按钮点击之前应该已经把密码对好了

    public List<GameObject> NumPrefab = new List<GameObject>();
    public List<GameObject> NumImage = new List<GameObject>();
    public List<RectTransform> NumPosList = new List<RectTransform>();
    public RectTransform HintPanel = new RectTransform();
    public Text HintText;
    public RectTransform ConfirmPanel = new RectTransform();
    public Text ConfirmText;
    public Button btnContinue;
    private List<GameObject> numButton = new List<GameObject>();
    bool isPlayerA;
    List<int> passNum = new List<int>();
    int phase = 0;
    List<int> playerinput = new List<int>();
    int seedSequenceID = -1;

    private void Setup()
    {
        List<int> indices = new List<int>();
        
        for (int i = 0; i < 12; i++)
        {
            indices.Add(i);
        }
        System.Random random = new System.Random();
        for (int i = 0; i < indices.Count; i++)
        {
            int randomIndex = random.Next(i, indices.Count);
            (indices[i], indices[randomIndex]) = (indices[randomIndex], indices[i]);
        }

        for (int i = 0; i < indices.Count; i++)
        {
            var num = Instantiate(NumPrefab[indices[i]], NumPosList[i]);
            var numComponent = num.GetComponent<NumComponent>();
            numComponent.PasswordSetup(indices[i]+1, this);
            numButton.Add(num);
        }
        btnContinue.gameObject.SetActive(false);

        isPlayerA = GameManagerSingleton.Instance.IsPlayerA;
        if (isPlayerA)
        {
            seedSequenceID = Random.Range(1, 11);
            showPasswordHint();
        }
        else
        {
            HintPanel.gameObject.SetActive(true);
            HintText.text = "根据玩家A的提示依次点亮图标";
        }
    }

    public void showPasswordHint()
    {
        HintPanel.gameObject.SetActive(true);
        HintText.text = "press→";
        ConfirmText.text = "";

        for (int i = phase * 3; i < (phase + 1) * 3; i++)
        {
            passNum.Add(Password.SeedSelectSequence[seedSequenceID][i]);
        }
        if (passNum.Count == 0)
        {
            HintText.text = "seedSequenceID=" + seedSequenceID;
        }
        foreach (int n in passNum)
        {
            var num = Instantiate(NumImage[n-1], HintPanel);
            var numComponent = num.GetComponent<NumComponent>();
            numComponent.PasswordSetup(n, this);
        }
    }

    public void updatInput(int input)
    {
        updateButtonsView(input);

        if (playerinput.Count == 0)
        {
            ConfirmText.text = "";
        }

        var num = Instantiate(NumImage[input-1], ConfirmPanel);
        var numComponent = num.GetComponent<NumComponent>();
        numComponent.PasswordSetup(input, this);
        playerinput.Add(input);

        //验证密码
        if (playerinput.Count == 3)
        {
            if (phase == 0 && !isPlayerA)//初始状态的B玩家，首次输入密码时，转为反向确认种子序列，如能确认，再进行验证
            {
                seedSequenceID = Password.SequenceExist(playerinput);
                if (seedSequenceID != -1)
                {
                    passNum.Clear();
                    for (int i = 0; i < 3; i++)
                    {
                        passNum.Add(Password.SeedSelectSequence[seedSequenceID][i]);
                    }
                    if (passNum.Count == 0)
                    {
                        HintText.text = "seedSequenceID=" + seedSequenceID;
                    }
                }
                else//反向定位种子序列失败，虚构一份密码表使其获得输入错误的判定
                {
                    for (int i = 0; i < 3; i++)
                    {
                        passNum.Add(-1);
                    }
                }
            }
            if (verifyPW(playerinput))
            {
                frezzeVerifiedButtons(playerinput);
                if (phase == 3)//对齐结束
                {
                    Password.selectPWSeed(seedSequenceID);
                    //HintPanel.gameObject.SetActive(false);
                    string msg = isPlayerA ? "等待玩家B完成后，点击继续" : "点击Continue继续";
                    HintText.text = msg;
                    ConfirmPanel.gameObject.SetActive(false);
                    if(!isPlayerA)
                    {
                        for (int i = 1; i < HintPanel.childCount; i++)//keep text, remove others
                        {
                            Destroy(HintPanel.GetChild(i).gameObject);
                        }
                    }
                    btnContinue.gameObject.SetActive(true);
                    return;
                }
                //验证通过，显示等待信息
                string player = isPlayerA ? "B" : "A";
                HintText.text = "按照玩家" + player + "的提示输入：";
                //清空
                for (int i = 1; i < ConfirmPanel.childCount; i++)//keep text, remove others
                {
                    Destroy(ConfirmPanel.GetChild(i).gameObject);
                }
                for (int i = 1; i < HintPanel.childCount; i++)//keep text, remove others
                {
                    Destroy(HintPanel.GetChild(i).gameObject);
                }
                //更新下一组待输入密码
                phase++;
                //根据阶段，自己的阶段(玩家A为phase0,2;B 为1,3)有提示，处于另一名玩家阶段则只更新密码
                if ((isPlayerA && phase % 2 == 0) || (!isPlayerA && phase % 2 == 1))
                {
                    passNum.Clear();
                    showPasswordHint();
                }
                else
                {
                    updatePassNum();
                }
            }
            else
            {
                //验证失败，清空
                for (int i = 1; i < ConfirmPanel.childCount; i++)//keep text, remove others
                {
                    Destroy(ConfirmPanel.GetChild(i).gameObject);
                }
                ConfirmText.text = "验证失败，请重新输入...";
            }
            playerinput.Clear();
        }

    }
    void frezzeVerifiedButtons(List<int> btnIDList) 
    {
        foreach (var btn in numButton)
        {
            if (btnIDList.Contains(int.Parse(btn.name.Replace("Num", "").Replace("(Clone)", ""))+1))
            {
                btn.transform.GetChild(0).GetComponent<Button>().interactable = false;
            }       
        }
    }

    void updateButtonsView(int btnID)
    {
        string spriteName = "";
        //numButton[0].transform.GetChild(0).GetComponent<Button>().image = null;
        //numButton[0].transform.GetChild(0).GetComponent<Button>().OnDeselect += btnDeselect;
        //numButton[0].transform.GetChild(0).GetComponent<Button>().spriteState.highlightedSprite = null;
        //numButton[0].transform.GetChild(0).GetComponent<Button>().interactable = false;
        //Button btn = numButton[0].transform.GetChild(0).GetComponent<Button>();
        //btn.OnDeselect += fun(new Button.ButtonClickedEvent());
        //numButton[0].transform.GetChild(0).GetComponent<Button>().OnDeselect= fun;
        //numButton[1].transform.GetChild(0).GetComponent<Button>().Select();
        //numButton[2].transform.GetChild(0).GetComponent<Button>().Select();

        Sprite btnSprite = numButton[0].transform.GetChild(0).GetComponent<Button>().GetComponent<Image>().sprite;

        string a= btnSprite.name.Contains("Dark") ? "L" : "Dark";
        string b = !btnSprite.name.Contains("Dark") ? "L" : "Dark";
        spriteName = btnSprite.name.Replace(b, a);

        //btnSprite = Resources.Load<Sprite>(@"/UI/Num/" + spriteName);


    }

    bool verifyPW(List<int> input)
    {
        for (int i = 0; i < 3; i++)
        {
            if (input[i] != passNum[i])
                return false;
        }
        return true;
    }

    void updatePassNum()
    {
        passNum.Clear();
        for (int i = phase * 3; i < (phase + 1) * 3; i++)
        {
            passNum.Add(Password.SeedSelectSequence[seedSequenceID][i]);
        }
        if (passNum.Count == 0)
        {
            HintText.text = "seedSequenceID=" + seedSequenceID;
        }
    }

    public void ShowHint1()
    {
        GameManagerSingleton.Instance.FirstHintPanel.SetActive(true);
    }

    public void ShowPasswordPanel()
    {
        GameManagerSingleton.Instance.FirstHintPanel.SetActive(false);
        gameObject.SetActive(true);
        Setup();
    }

    public void ShowHint2()
    {
        gameObject.SetActive(false);
        GameManagerSingleton.Instance.SecondHintPanel.SetActive(true);
    }
    
    // 隐藏这个按钮直到所有对密码环节完成
    public void HandleStart()
    {
        GameManagerSingleton.Instance.StartGame();
        GameManagerSingleton.Instance.SecondHintPanel.SetActive(false);
    }
}
