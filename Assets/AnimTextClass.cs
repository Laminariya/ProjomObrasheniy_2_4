using System.Collections;
using System.Collections.Generic;
using BrunoMikoski.TextJuicer;
using UnityEngine;

public class AnimTextClass : MonoBehaviour
{
    
    [HideInInspector] public TMP_TextJuicer textJuicer;
    
    public List<string> textList = new List<string>();
    
    public void Init()
    {
        textJuicer = GetComponent<TMP_TextJuicer>();
    }
    
}
