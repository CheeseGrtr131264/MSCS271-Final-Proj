using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShowCombo : MonoBehaviour
{
    [SerializeField] HeroKnightComboStuff combo;
    [SerializeField] TMP_Text currComboText;
    [SerializeField] TMP_Text prevInputText;

    // Update is called once per frame
    void Update()
    {
        if(combo != null)
        {
            if (currComboText != null)
            {
                string comboString = combo.GetComboString();
                if (comboString != "")
                {
                    currComboText.text = comboString;
                }
                else
                {
                    currComboText.text = "No combo";
                }
            }
            if(prevInputText != null)
            {
                string prevInputString = combo.GetPrevInputString();
                if (prevInputString != "")
                {
                    prevInputText.text = prevInputString;
                }
                else
                {
                    prevInputText.text = "No inputs";
                }
            }
            
        }
        else
        {
            Debug.LogError("Show combo not fully connected");
        }
    }
}
