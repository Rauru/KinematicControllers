using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidBodyController : MonoBehaviour
{
    public Rigidbody rb;
    public float acceleration = 50;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
       

        
    }
    void FixedUpdate(){
         float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 moveInput = new Vector3(horizontalInput,0, verticalInput);
        rb.MovePosition(transform.position + moveInput * acceleration* Time.deltaTime);

        if(Input.GetKey(KeyCode.Space)){
            rb.MovePosition(transform.position + moveInput * 100* Time.deltaTime);

        }
    }

}

