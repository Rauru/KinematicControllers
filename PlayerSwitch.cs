using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerSwitch : MonoBehaviour
{
    public GameObject[] playercontrollers;
    public int playerIndex;
    public GameObject currentPlayer;
    // Start is called before the first frame update
    void Start()
    {

        playerSwitching(0);
    }

    void playerSwitching(int i){
        playerIndex = i;
        Vector3 spawnPos = new Vector3(0f,0.5f,0f);
        Destroy(currentPlayer);
        currentPlayer = Instantiate(playercontrollers[playerIndex], spawnPos, playercontrollers[playerIndex].transform.rotation);
    }

    // Update is called once per frame
    void Update()
    {
         if(Input.GetKey(KeyCode.Keypad0)&& playerIndex !=0){
           playerSwitching(0);

         }
         if(Input.GetKey(KeyCode.Keypad1)&& playerIndex !=1){
           playerSwitching(1);

         }
        
    }
}
