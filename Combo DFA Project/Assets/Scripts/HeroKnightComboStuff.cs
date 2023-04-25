using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HeroKnightComboStuff : MonoBehaviour
{
    [SerializeField] float falloutTime;
    private Animator animator;

    //Example
    int[,] delta = new int[8, 2] { { 0, 1 }, { 2, 3 }, { 4, 5 }, { 6, 0 }, { 1, 2 }, { 3, 4 }, { 5, 6 }, { 7, 7 } };
    // stored in the structure of [ [P] , [K] , [R] , [L] , [U] , [D] ]
    string[] combos = { "RPK", "LRLP", "UPKL", "PDLR", "DPU", "KLPDUR" };
    private string comboString; 
    private float falloutTimer;
    public string[,] sortedCombos;
    List<List<int>> pre_delta;
    
    int nodeNum = 0;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        pre_delta = new List<List<int>>();
        //Acount for first Node
        RecursiveDFAGen(combos, nodeNum );
    }

    //Called by ShowCombo
    public string GetComboString()
    {
        return comboString;
    }
    // Update is called once per frame
    void Update()
    {
        //Reference - how to call mouse button down
        //Input.GetMouseButtonDown(0)

        //Reference - how to call get key down
        //Input.GetKeyDown("e")

        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown("j"))
        {
            AddToCombo('p');
        }
        else if (Input.GetMouseButtonDown(1) || Input.GetKeyDown("k"))
        {
            AddToCombo('k');
        }
        else if (comboString != "")
        {
            //Update value of timer since last input
            falloutTimer += Time.deltaTime;

            //Reset combo if too much time has passed since last input
            if(falloutTimer >= falloutTime)
            {
                ResetCombo();
            }
        }
    }
    void AddToCombo(char letter)
    {
        //append letter to combo string
        comboString += letter;
        Debug.Log("Combo string: " + comboString);
        //Check if combo string has options
        CheckForCombo();

        //Reset fallout timer
        falloutTimer = 0;
    }

    void CheckForCombo()
    {
        //Check if combo is done
    }

    void ResetCombo()
    {
        falloutTimer = 0;
        comboString = "";
    }

    void RecursiveDFAGen(string[] states, int nodeIndex)
    {
        //                  p    k    r    l    u    d
        string[,] next = { { }, { }, { }, { }, { }, { } };
        int maxLength = states.Length;
        next = new string[6,maxLength];
        int[] lengths = new int[6];

        for (int i = 0; i < states.Length; i++)
        {
            int relevantArrayLength = -1;
            char firstChar = char.ToLower(states[i][0]);
            switch (firstChar)
            {
                case 'p':
                    relevantArrayLength = lengths[0];
                    next[0, relevantArrayLength] = states[i];
                    relevantArrayLength++;
                    break;
                case 'k':
                    relevantArrayLength = lengths[1];
                    next[1, relevantArrayLength] = states[i].Substring(1);
                    relevantArrayLength++;
                    break;
                case 'r':
                    relevantArrayLength = lengths[2];
                    next[2, relevantArrayLength] = states[i].Substring(1);
                    relevantArrayLength++;
                    break;
                case 'l':
                    relevantArrayLength = lengths[3];
                    next[3, relevantArrayLength] = states[i].Substring(1);
                    relevantArrayLength++;
                    break;
                case 'u':
                    relevantArrayLength = lengths[4];
                    next[4, relevantArrayLength] = states[i].Substring(1);
                    relevantArrayLength++;
                    break;
                case 'd':
                    relevantArrayLength = lengths[5];
                    next[5, relevantArrayLength] = states[i].Substring(1);
                    relevantArrayLength++;
                    break;
                default:
                    Debug.LogError($"Character {firstChar} is not recognized");
                    break;
            }
        }

        for (int i = 0; i < states.Length; i++)
        {
            for(int j = 0; j <= lengths[i]; j++)
            {
                
                print(next[i, j]);
            }
           
        }
        int[] specific_delta = {0, 0, 0, 0, 0, 0};
        for (int i = 0; i < states.Length; i++)
        {
            if (next.GetLength(i) != 0)
            {
                nodeNum++;
                specific_delta[i] = nodeNum;

            }

        }
       
        sortedCombos = next;
    }
}
