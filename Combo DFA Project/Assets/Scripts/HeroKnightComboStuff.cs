using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class HeroKnightComboStuff : MonoBehaviour
{
    [SerializeField] float falloutTime;
    private Animator animator;
    private float moveStrength = 0;
    public List<string> debugNodeGraph;
    //Example
    int[,] delta;
    private float maxSpeed;
    // stored in the structure of [ [P] , [K] , [R] , [L] , [U] , [D] ]

    //[SerializeField] string[] combos;
    Dictionary<string, string> combos = new Dictionary<string, string>() { 
        ["RPK"] = "Attack1" , 
        ["LRP"] = "Attack3",
        ["UPKL"] = "Roll",
        ["PDLR"] = "Block",
        ["DPU"] = "Jump",
        ["KLPDUP"] = "DeathNoBlood"

    
    }; //, "LRLP", "UPKL", "PDLR", "DPU", "KLPDUP" });
    //string[] combos = { "RPK", "LRLP", "UPKL", "PDLR", "DPU", "KLPDUP" };

    //string[] combos = { "RPK" };
    private string comboString;
    private string lastInputs = "";
    private float falloutTimer;
    public string[,] sortedCombos;
    int lastInputsMaxLength = 10;
    List<List<int>> pre_delta;
    Queue<(int, string[])> nodesToImplement;
    private Rigidbody2D knightRB;
    float lastInputDeleteTime = 3;
    int nodeNum = 0;



    float lastDeleteTime = -1;

    int currentDeltaNode = 0;
    float moveSpeed = 5;

    // Start is called before the first frame update
    void Start()
    {
        knightRB = GetComponent<Rigidbody2D>();
        debugNodeGraph = new List<string>();
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
    public string GetPrevInputString()
    {
        return lastInputs;
    }
    // Update is called once per frame
    void Update()
    {
        if (moveStrength != 0)
        {
            Vector2 pastVel = knightRB.velocity;
            float thing = pastVel.x + (moveStrength * moveSpeed);
            // knightRB.velocity = new Vector2(Mathf.Clamp(thing, -maxSpeed, maxSpeed), 0);

           // knightRB.velocity = new Vector2(pastVel.x + moveStrength * moveSpeed, 0);
            knightRB.velocity += new Vector2(Mathf.Clamp(thing, -maxSpeed, maxSpeed), 0);
        }
        //Reference - how to call mouse button down
        //Input.GetMouseButtonDown(0)

        //Reference - how to call get key down
        //Input.GetKeyDown("e")

        //if (Input.GetMouseButtonDown(0) || Input.GetKeyDown("j"))
        //{
        //    AddToCombo('p');
        //}
        //else if (Input.GetMouseButtonDown(1) || Input.GetKeyDown("k"))
        //{
        //    AddToCombo('k');
        //}
        //else if (Input.GetKeyDown(KeyCode.RightArrow))
        //{
        //    AddToCombo('r');
        //}
        //else if (Input.GetKeyDown(KeyCode.LeftArrow))
        //{
        //    AddToCombo('l');
        //}
        //else if (Input.GetKeyDown(KeyCode.UpArrow))
        //{
        //    AddToCombo('u');
        //}
        //else if (Input.GetKeyDown(KeyCode.DownArrow))
        //{
        //    AddToCombo('d');
        //}

        if (comboString != "")
        {
            //Update value of timer since last input
            falloutTimer += Time.deltaTime;

            //Reset combo if too much time has passed since last input
            if (falloutTimer >= falloutTime)
            {
                ResetCombo();
            }
        }
        if(lastInputs != "")
        {
            if(Time.time - lastInputDeleteTime > lastDeleteTime)
            {
                lastInputDeleteTime = Time.time;
                lastInputs = lastInputs.Substring(0, lastInputs.Length - 1);
            }
            
        }
        //else if (lastDeleteTime != -1)
        //{
        //    lastDeleteTime = -1;
        //}
    }

    public void OnUp(InputAction.CallbackContext value)
    {
        if (value.started)
        {
            AddToCombo('u');
        }
    }
    public void OnDown(InputAction.CallbackContext value)
    {
        if (value.started)
        {
            AddToCombo('d');
        }
    }
    public void OnRight(InputAction.CallbackContext value)
    {
        if (value.started)
        {
            AddToCombo('r');
        }
    }
    public void OnLeft(InputAction.CallbackContext value)
    {
        if (value.started)
        {
            AddToCombo('l');
        }
    }
    public void OnPunch(InputAction.CallbackContext value)
    {
        if (value.started)
        {
            AddToCombo('p');
        }
    }
    public void OnKick(InputAction.CallbackContext value)
    {
        if (value.started)
        {
            AddToCombo('k');
        }
        
    }
    public void OnMoveLR(InputAction.CallbackContext value)
    {
        print("On Move LR called");
        //if(value.valueType == )
        moveStrength = value.ReadValue<float>();
        //if (!value.canceled)
        //{
        //    knightRB.AddForce(new Vector2(value.ReadValue<float>() * 20, 0));
        //}
        //else
        //{
        //    moveStrength = 0;
        //}
    }



    void InitializeDelta()
    {
        int maxSize = 0;
        string[] comboStrings = combos.Keys.ToArray();
        for (int i = 0; i < comboStrings.Length; i++)
        {
            for (int j = 0; j < comboStrings[i].Length; j++)
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
        //Debug.Log("Combo string: " + comboString);

        if(lastInputs.Length == 0)
        {
            lastDeleteTime = Time.time;
        }
        lastInputs = letter.ToString() + lastInputs;
        if (lastInputs.Length > lastInputsMaxLength)
        {
            lastInputs = lastInputs.Substring(0, lastInputsMaxLength);
        }

        int actionNum = ConvertActionToNum(letter);
        if (actionNum != -1)
        {
            currentDeltaNode = delta[currentDeltaNode, actionNum];
            if (currentDeltaNode == 0)
            {
                ResetCombo();
            }
        }

        //Check if combo string has options
        CheckForCombo();

        //Reset fallout timer
        falloutTimer = 0;
    }

    int ConvertActionToNum(char action)
    {
        switch (char.ToLower(action))
        {
            case 'p':
                return 0;
            case 'k':
                return 1;
            case 'r':
                return 2;
            case 'l':
                return 3;
            case 'u':
                return 4;
            case 'd':
                return 5;
            default:
                return -1;
        }
    }

    void PerformCombo(string comboString)
    {
        if (combos.GetValueOrDefault(comboString, null) != null)
        {
            animator.Play(combos[comboString]);
        }
        //switch (comboString.ToLower())
        //{
        //    case "rpk":
        //        animator.Play("Attack1", 0, 0);
        //        break;

        //    case "lrlp":
        //        animator.Play("Attack3", 0, 0);
        //        break;

        //    case "upkl":
        //        animator.Play("Roll", 0, 0);
        //        break;

        //    case "pdkl":
        //        animator.Play("Block", 0, 0);
        //        break;

        //    case "dpu":
        //        knightRB.AddForce(new Vector2(0, 5), ForceMode2D.Impulse);
        //        animator.Play("Jump", 0, 0);
        //        break;

        //    case "klpdup":
        //        animator.Play("DeathNoBlood", 0, 0);
        //        break;
        //}
    }
    void CheckForCombo()
    {
        //Check if combo is done
        if (currentDeltaNode == -1)
        {
            PerformCombo(comboString.ToLower());
            //Play combo based on string
            
            Debug.Log($"Combo {comboString} Done!!!!");
            ResetCombo();
        }
    }

    void ResetCombo()
    {
        falloutTimer = 0;
        comboString = "";
        currentDeltaNode = 0;
    }

    void DFAGen()
    {
        int iterations = 0;
        //First iteration

        StateGen(0, combos.Keys.ToArray());
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
        next = new string[6, maxLength];
        //Holds the number of connections for each action
        int[] lengths = new int[6];

        //Sort into relevant group, Cut the first letter
        for (int i = 0; i < states.Length; i++)
        {
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
        //for (int i = 0; i < states.Length; i++)
        //{
        //    for (int j = 0; j <= lengths[i]; j++)
        //    {


        //    }
        //}


        //Initialize future connections for this node
        int[] specific_delta = { 0, 0, 0, 0, 0, 0 };
        for (int i = 0; i < 6; i++)
        {
            //If there will be a future node, 

            //ERROR HERE. PROBS NEEDS TO BE 6 OR SOMETHIN
            if (lengths[i] != 0)
            {
                if (lengths[i] == 1 && next[i, 0] == "")
                {
                    //End of the combo

                    specific_delta[i] = -1;


                }
                else
                {
                    nodeNum++;
                    specific_delta[i] = nodeNum;
                    int length = lengths[i];
                    string[] nextArray = new string[length];
                    for (int j = 0; j < length; j++)
                    {
                        nextArray[j] = next[i, j];
                    }

                    (int, string[]) node = (nodeNum, nextArray);
                    nodesToImplement.Enqueue(node);
                }
            }


        }

        //Set delta to proper values
        for (int i = 0; i < 6; i++)
        {
            delta[thisNodeIndex, i] = specific_delta[i];
        }

        //Store for debug purposes
        string fortnite = "";
        for (int j = 0; j < 6; j++)
        {
            switch (j)
            {
                case 0:
                    fortnite += "Punch: ";
                    break;
                case 1:
                    fortnite += "Kick: ";
                    break;
                case 2:
                    fortnite += "Right: ";
                    break;
                case 3:
                    fortnite += "Left: ";
                    break;
                case 4:
                    fortnite += "Up: ";
                    break;
                case 5:
                    fortnite += "Down: ";
                    break;

            }

            fortnite += specific_delta[j].ToString();
            if (j != 5)
            {
                fortnite += ", ";
            }
        }
        debugNodeGraph.Add(fortnite);
    }
}
