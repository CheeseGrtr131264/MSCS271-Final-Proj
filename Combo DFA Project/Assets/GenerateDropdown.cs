using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GenerateDropdown : MonoBehaviour
{
    public HeroKnightComboStuff comboStuff;
    TMP_Dropdown m_Dropdown;
    List<TMP_Dropdown.OptionData> m_Messages = new List<TMP_Dropdown.OptionData>();
    // Start is called before the first frame update
    void Start()
    {
        //Fetch the Dropdown GameObject the script is attached to
        m_Dropdown = GetComponent<TMP_Dropdown>();
        //Clear the old options of the Dropdown menu
        var m_options = new Dropdown.OptionData();

        if (comboStuff != null)
        {
            Dictionary<string, string> comboDict = comboStuff.GetCombos();
           

            foreach (KeyValuePair<string, string> combo in comboDict) {
                var m_NewData = new TMP_Dropdown.OptionData();
                m_NewData.text = combo.Key + " " + combo.Value;
                m_Messages.Add(m_NewData);
            }

            m_Dropdown.AddOptions(m_Messages);
        }
        else
        {
            Debug.LogError("Uninitialized combo in GenerateDropdown");
        }
    }

    //// Update is called once per frame
    //void Update()
    //{
        
    //}
}
