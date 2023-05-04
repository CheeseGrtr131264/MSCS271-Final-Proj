using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnvironmentManager : MonoBehaviour
{
    public int startingThueMorse = -1;
    public List<GameObject> environments;
    public Transform player;

    private GameObject rightEnvironment;
    private int currentThueMorse = -1;
    private int currentEnviroNum = -1;
    int[,] delta;
   
    // Start is called before the first frame update
    void Start()
    {
        GenerateDelta();
        if(startingThueMorse == -1)
        {
            startingThueMorse = Random.Range(10, 100);
        }
        int startPos = Random.Range(0, environments.Count);
        currentThueMorse = startingThueMorse;
        currentEnviroNum = startPos;
        var newEnviroInstance = Instantiate(environments[startPos], new Vector3(0, 0, 0), Quaternion.identity, transform);
        rightEnvironment = newEnviroInstance;
        MakeNewRight();
        
        
    }

    void GenerateDelta()
    {
        if (environments != null && environments.Count != 0)
        {
            delta = new int[environments.Count, 2];
            for (int i = 0; i < environments.Count; i++)
            {
                int zeroConnection = -1;
                int iterations = 0;
                while ((zeroConnection == -1 || zeroConnection == i) && iterations < 100)
                {
                    zeroConnection = -1;
                    iterations++;
                    zeroConnection = Random.Range(0, environments.Count);
                }
                if (zeroConnection == -1)
                {
                    zeroConnection = i;
                }
                delta[i, 0] = zeroConnection;
                int oneConnection = -1;
                while ((oneConnection == -1 || oneConnection == i || oneConnection == zeroConnection) && iterations < 100)
                {
                    oneConnection = -1;
                    iterations++;
                    oneConnection = Random.Range(0, environments.Count);
                }
                if (oneConnection == -1)
                {
                    oneConnection = i;
                }
                delta[i, 1] = oneConnection;

            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        GameObject rightEnvironmentObj = rightEnvironment;
        if(player.position.x > rightEnvironmentObj.transform.position.x - 6)
        {
             MakeNewRight(); 
        }
    }

    private void MakeNewRight()
    {
        GameObject rightEnvironmentObj = rightEnvironment;
        GameObject newEnviro = GenerateNewEnvironment(currentEnviroNum, currentThueMorse);
        var oldEnvPos = rightEnvironmentObj.transform.position;
        //reparent new enviroment
        newEnviro.transform.parent = transform;

        //Set position
        newEnviro.transform.position = new Vector3(oldEnvPos.x + 12, oldEnvPos.y, oldEnvPos.z);
        rightEnvironment = newEnviro;
    }

    GameObject GenerateNewEnvironment(int nodePos, int thueMorseNum)
    {
        thueMorseNum += 1; 
        int newNodePos = -1;
        string binaryNodePos = Convert.ToString(nodePos, 2);
        int numOnes = binaryNodePos.Split('1').Length - 1;
        newNodePos = delta[nodePos,numOnes % 2];
        GameObject newEnviro = Instantiate(environments[newNodePos], new Vector3(0, 0, 0), Quaternion.identity);
        currentEnviroNum = newNodePos;
        currentThueMorse = thueMorseNum;
        return newEnviro;
    }

}
