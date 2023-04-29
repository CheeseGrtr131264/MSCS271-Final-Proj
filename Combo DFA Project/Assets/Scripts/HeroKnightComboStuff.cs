using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HeroKnightComboStuff : MonoBehaviour
{
    [SerializeField] float falloutTime;
    private Animator animator;

    
    //Example
    int[,] delta;
    // stored in the structure of [ [P] , [K] , [R] , [L] , [U] , [D] ]
    string[] combos = { "RPK", "LRLP", "UPKL", "PDLR", "DPU", "KLPDUR" };
    private string comboString; 
    private float falloutTimer;
    public string[,] sortedCombos;
    List<List<int>> pre_delta;
    Queue<(int, string[])> nodesToImplement;
    
    int nodeNum = 0;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        nodesToImplement = new Queue<(int, string[])>();
        pre_delta = new List<List<int>>();
        InitializeDelta();
        //Acount for first Node
        DFAGen();
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

    void InitializeDelta()
    {
        int maxSize = 0;
        for (int i = 0; i < combos.Length; i++)
        {
            for(int j = 0; j < combos[i].Length; j++)
            {
                maxSize++;
            }
        }
        delta = new int[maxSize, 6];
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

    void DFAGen()
    {
        int iterations = 0;
        //First iteration

        StateGen(0, combos);
        while (nodesToImplement.Count != 0 && iterations < 1000)
        {
            iterations++;
            (int, string[]) nextInfo = nodesToImplement.Dequeue();
            StateGen(nextInfo.Item1, nextInfo.Item2);
        }
    }

    void StateGen(int nodeIndex, string[] states)
    {
        int thisNodeIndex = nodeIndex;

        //Next is where we store the node's future connections
        //Will be passed into future iterations
        //                  p    k    r    l    u    d
        string[,] next = { { }, { }, { }, { }, { }, { } };
        //Maximum number of potential states, probably less than this but needed for array generation
        int maxLength = states.Length;
        //Initialized so each index could potentially hold all future connections
        next = new string[6,maxLength];
        //Holds the number of connections for each action
        int[] lengths = new int[6];

        //Sort into relevant group, Cut the first letter
        for (int i = 0; i < states.Length; i++)
        {
            int relevantArrayLength = -1;
            //Make lower for less errors
            char firstChar = char.ToLower(states[i][0]);

            //Sorting here
            //For each, set next to be string - first character
            //increase lengths by 1
            switch (firstChar)
            {
                case 'p':
                    next[0, lengths[0]] = states[i].Substring(1);
                    lengths[0]++;
                    break;
                case 'k':
                    next[1, lengths[1]] = states[i].Substring(1);
                    lengths[1]++;
                    break;
                case 'r':
                    next[2, lengths[2]] = states[i].Substring(1);
                    lengths[2]++;
                    break;
                case 'l':
                    next[3, lengths[3]] = states[i].Substring(1);
                    lengths[3]++;
                    break;
                case 'u':
                    next[4, lengths[4]] = states[i].Substring(1);
                    lengths[4]++;
                    break;
                case 'd':
                    next[5, lengths[5]] = states[i].Substring(1);
                    lengths[5]++;
                    break;
                default:
                    Debug.LogError($"Character {firstChar} is not recognized");
                    break;
            }
        }

        //Print for debug purposes
        for (int i = 0; i < states.Length; i++)
        {
            for(int j = 0; j <= lengths[i]; j++)
            {
                
                print(next[i, j]);
            }
        }


        //Initialize future connections for this node
        int[] specific_delta = {0, 0, 0, 0, 0, 0};
        for (int i = 0; i < states.Length; i++)
        {
            //If there will be a future node, 

            //ERROR HERE. PROBS NEEDS TO BE 6 OR SOMETHIN
            if (next.GetLength(i) != 0)
            {
                nodeNum++;
                specific_delta[i] = nodeNum;
                int length = next.GetLength(i);
                string[] nextArray = new string[length];
                for (int j = 0; j < length; j++)
                {
                    nextArray[j] = next[i, j];
                }

                (int, string[]) node = (nodeNum, nextArray);
                nodesToImplement.Enqueue(node);
            }
            //End of the combo
            else
            {
                specific_delta[i] = -1;
            }

        }

        //Set delta to proper values
        for(int i = 0; i < 6; i++)
        {
            delta[thisNodeIndex, i] = specific_delta[i];
        }
       
    }
}
