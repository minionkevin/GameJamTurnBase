using UnityEngine;

public class PanelController : MonoBehaviour
{
    public GameObject panel1;
    public GameObject panel2;

    public void ShowPanel1()
    {
        panel1.SetActive(true);
        panel2.SetActive(false);
    }

    public void ShowPanel2()
    {
        panel1.SetActive(false);
        panel2.SetActive(true);
    }
}