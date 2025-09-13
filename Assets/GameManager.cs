using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public float SpeedAnimText;
    public static GameManager instance;
    public GameObject ActivaScreen;
    public GameObject DefoultScreen;
    public GameObject DefaultSCreen2;

    public Image BG1;
    public Image BG2;
    public GameObject Send;

    public GameObject CountSendParent;

    public Button b_langUzbec;
    public Button b_langRus;
    
    public TMP_Text CountSend;

    public int CurrentLeng;
    
    public List<LangContent> LangContent = new List<LangContent>();
    public float SpeedAnim;

    private int _countSend = 0;
    private float _timeout;
    public float Timeout;
    private Color _langColor;
    private StandbyClass _standbyClass;
    
    private List<AnimTextClass> _animText = new List<AnimTextClass>();
    
    private void Awake()
    {
        if(instance == null)
            instance = this;
        if (Display.displays.Length > 1)
        {
            Display.displays[1].Activate(); //Включаем второй дисплей. Он будет отображать камеры и канвасы которые помечены как Display2
        }
    }

    void Start()
    {
        if (PlayerPrefs.HasKey("CountSend"))
            _countSend = PlayerPrefs.GetInt("CountSend");
        else
        {
            PlayerPrefs.SetInt("CountSend", 0);
        }

        _animText = FindObjectsOfType<AnimTextClass>(true).ToList();
        foreach (var textClass in _animText)
        {
            textClass.Init();
        }
        Debug.Log(_animText.Count);
        _standbyClass = FindObjectOfType<StandbyClass>(true);
        
        _langColor = b_langRus.image.color;
        b_langRus.onClick.AddListener(OnRus);
        b_langUzbec.onClick.AddListener(OnUzbec);
        OnRus();
        ActivaScreen.SetActive(false);
        DefoultScreen.SetActive(true);
        DefaultSCreen2.SetActive(true);
        OnCountSend();
        Send.gameObject.SetActive(false);
        //OnRus();
    }

    private void Update()
    {
        if (Input.anyKeyDown)
        {
            _timeout = Time.time;
        }

        if (!DefoultScreen.activeSelf && Time.time - _timeout > Timeout)
        {
            SetDefoultScreen();
        }
    }

    private void SetDefoultScreen()
    {
        ActivaScreen.SetActive(false);
        DefoultScreen.SetActive(true);
        DefaultSCreen2.SetActive(true);
        PaintClass.Instance.ClearTexture(Color.clear);
    }

    private void OnUzbec()
    {
        b_langRus.image.DOFade(0, 0.2f);
        b_langUzbec.image.DOFade(1, 0.2f);
        CurrentLeng = 0;
        DefaultSCreen2.SetActive(false);
        DefoultScreen.SetActive(false);
        ActivaScreen.SetActive(true);
        StartCoroutine(AnimText());
    }

    private void OnRus()
    {
        b_langRus.image.DOFade(1, 0.2f);
        b_langUzbec.image.DOFade(0, 0.2f);
        CurrentLeng = 1;
        DefaultSCreen2.SetActive(false);
        DefoultScreen.SetActive(false);
        ActivaScreen.SetActive(true);
        StartCoroutine(AnimText());
    }

    IEnumerator AnimText()
    {
        float progress = 1f;

        while (progress>0f)
        {
            progress -= Time.deltaTime * SpeedAnim;
            foreach (var textClass in _animText)
            {
                textClass.textJuicer.SetProgress(progress);
                textClass.textJuicer.Update();
            }

            foreach (var textJuicer in _standbyClass.TextJuicers) 
            {
                textJuicer.SetProgress(progress);
                textJuicer.Update();
            }
            yield return null;
        }
        
        foreach (var textClass in _animText)
        {
            textClass.textJuicer.Text = textClass.textList[CurrentLeng];
        }
        _standbyClass.ChangeLanguage();
        
        progress = 0f;

        while (progress<1f)
        {
            progress += Time.deltaTime * SpeedAnim;
            foreach (var textClass in _animText)
            {
                textClass.textJuicer.SetProgress(progress);
                textClass.textJuicer.Update();
            }

            foreach (var textJuicer in _standbyClass.TextJuicers) 
            {
                textJuicer.SetProgress(progress);
                textJuicer.Update();
            }
            yield return null;
        }
    }

    private void ChangeLang()
    {
        foreach (var textClass in _animText)
        {
            textClass.textJuicer.Text = textClass.textList[CurrentLeng];
        }
    }

    public void InitMyButton(MyButton button)
    {
        OnCountSend();
        Send.gameObject.SetActive(false);
        PaintClass.Instance.ClearTexture(Color.clear);
    }

    private void OnCountSend()
    {
        Send.gameObject.SetActive(false);
        CountSendParent.gameObject.SetActive(true);
        CountSend.text = _countSend.ToString();
    }

    public void AddCountSend()
    {
        _countSend++;
        PlayerPrefs.SetInt("CountSend", _countSend);
        CountSendParent.gameObject.SetActive(false);
        Send.gameObject.SetActive(true);
    }

    private void OnDefoultScreen()
    {
        DefoultScreen.SetActive(false);
        DefaultSCreen2.SetActive(false);
    }

}

[Serializable]
public class LangContent
{
    public Sprite BG_1;
    public Sprite BG_2;
    public Sprite Send;
    public Sprite CountSendParent;
}

