using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BrunoMikoski.TextJuicer;
using UnityEngine;

public class StandbyClass : MonoBehaviour
{

    public float DealayChange;
    public List<LangTextJuicer> langTextJuicers = new List<LangTextJuicer>();
    
    private List<TMP_TextJuicer> allTextJuicers = new List<TMP_TextJuicer>();
    private GameManager _manager;
    private int _currentTextJuicer = 0;
    private float _time;
    [HideInInspector] public List<TMP_TextJuicer> TextJuicers = new List<TMP_TextJuicer>();
    
    void Start()
    {
        _manager = GameManager.instance;
        allTextJuicers = GetComponentsInChildren<TMP_TextJuicer>(true).ToList();
        TextJuicers.Add(allTextJuicers[2]);
        TextJuicers.Add(allTextJuicers[3]);
        TextJuicers.Add(allTextJuicers[4]);
    }

    private void Update()
    {
        if (gameObject.activeSelf && Time.time - _time > DealayChange)
        {
            _time = Time.time;
            StartCoroutine(ChangeZitati());
        }
    }

    public void Show()
    {
        
    }

    public void Hide()
    {
        
    }

    public void ChangeLanguage()
    {
        if (_manager.CurrentLeng == 0)
        {
            for (int i = 0; i < TextJuicers.Count; i++)
            {
                TextJuicers[i].Text = langTextJuicers[_currentTextJuicer].uzb[i];
            }
        }
        if (_manager.CurrentLeng == 1)
        {
            for (int i = 0; i < TextJuicers.Count; i++)
            {
                TextJuicers[i].Text = langTextJuicers[_currentTextJuicer].rus[i];
            }
        }
    }


    IEnumerator ChangeZitati()
    {
        _currentTextJuicer++;
        if (_currentTextJuicer == langTextJuicers.Count)
            _currentTextJuicer = 0;

        float progress = 1f;
        while (progress > 0f)
        {
            progress -= Time.deltaTime * _manager.SpeedAnimText;
            foreach (var juicer in TextJuicers)
            {
                juicer.SetProgress(progress);
                juicer.Update();
            }

            yield return null;
        }

        ChangeLanguage();

        progress = 0f;
        while (progress < 1f)
        {
            progress += Time.deltaTime * _manager.SpeedAnimText;
            foreach (var juicer in TextJuicers)
            {
                juicer.SetProgress(progress);
                juicer.Update();
            }

            yield return null;
        }
    }

}

[Serializable]
public class LangTextJuicer
{
    public List<string> uzb = new List<string>();
    public List<string> rus = new List<string>();
}
