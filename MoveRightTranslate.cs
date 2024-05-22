using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveRightTranslate : MonoBehaviour
{
    public float x=10;
    public float y=0;
    public float z =0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(new Vector3(x,y,z)*Time.deltaTime);   
    }
}
