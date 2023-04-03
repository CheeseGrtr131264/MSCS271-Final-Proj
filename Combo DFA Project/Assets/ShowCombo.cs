using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShowCombo : MonoBehaviour
{
    [SerializeField] HeroKnightComboStuff combo;
    [SerializeField] TMP_Text text;

    // Update is called once per frame
    void Update()
    {
        if(combo != null && text != null)
        {
            string comboString = combo.GetComboString();
            if(comboString != "")
            {
                text.text = combo.GetComboString();
            }
            else
            {
                text.text = "Start combo pls";
            }
            
        }
        else
        {
            Debug.LogError("Show combo not fully connected");
        }
    }
}
