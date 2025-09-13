using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyButton : MonoBehaviour
{
    
    private Button button;
    public Sprite Uzbek;
    public Sprite Arab;
    public Sprite Eng;
    public Sprite Rus;
    
    public Sprite Active_Uzbek;
    public Sprite Active_Arab;
    public Sprite Active_Eng;
    public Sprite Active_Rus;
    
    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        Debug.Log("OnClick");
        GameManager.instance.InitMyButton(this);
        button.image.color = Color.white;
        ChangeLang();
    }

    public void SetDefault()
    {
        if(button==null)
            button = GetComponent<Button>();
        button.image.color = Color.clear;
    }

    public void ChangeLang()
    {
        switch (GameManager.instance.CurrentLeng)
        {
            case 1:
            {
                button.image.sprite = Active_Uzbek;
                break;
            }
            case 2:
            {
                button.image.sprite = Active_Arab;
                break;
            }
            case 3:
            {
                button.image.sprite = Active_Eng;
                break;
            }
            case 4:
            {
                button.image.sprite = Active_Rus;
                break;
            }
        }
    }

}
