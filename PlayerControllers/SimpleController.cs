using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleController : MonoBehaviour
{
    Rigidbody  rb;
    // Start is called before the first frame update
    void Start()
    {
        rb=GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 movementInput = new Vector3(horizontalInput,0,verticalInput);
        rb.AddForce(movementInput*50*Time.deltaTime);
        rb.velocity = movementInput*10;
        //transform.position += movementInput * Time.deltaTime;
    }
}
