using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Vector3 cameraOffset;
    public GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        
        cameraOffset =  transform.localPosition - player.transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = player.transform.position + cameraOffset; 
    }
}
