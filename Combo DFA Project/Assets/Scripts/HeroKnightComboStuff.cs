using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroKnightComboStuff : MonoBehaviour
{
    [SerializeField] float falloutTime;
    private Animator animator;

    private string comboString;
    private float falloutTimer;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
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
}
