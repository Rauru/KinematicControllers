using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
public class PlayerController : MonoBehaviour
{
    private float horizontalInput;
    private float verticalInput;
    [SerializeField] private int speed = 10;
    [SerializeField] private int acceleration = 10;
    [SerializeField] private float maxSpeed = 50;
    [SerializeField] private float jumpSpeed = 10;
    [SerializeField] private float gravity = 20f;
    [SerializeField] private float verticalVelocity = 0f;
     [SerializeField] private bool isGrounded = true;
     [SerializeField] private int groundContacts = 0;
    Collider collider;
    [SerializeField] Transform feet;
    public LayerMask m_LayerMask;
    Collider[] results;
    private Vector3 debugmove;
     float temphit;

    // Start is called before the first frame update
    void Start()
    {
        collider = GetComponent<Collider>();
    }
    private Vector3 ProjectAndScale(Vector3 vec, Vector3 normal){
        float mag = vec.magnitude;
        vec = Vector3.ProjectOnPlane(vec, normal).normalized;
        vec *=mag;
        return vec;
    }
    // Update is called once per frame

    void moveWithFeetCollider(){
        

        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        results = Physics.OverlapBox(feet.position, feet.localScale/2, Quaternion.identity,m_LayerMask );
        
        verticalVelocity -= 1* gravity *Time.deltaTime;
       // transform.position += movement * speed *Time.deltaTime;
        
        Debug.Log("overlaps" + results.Length);
        if(results.Length > 0 && verticalVelocity < 0){
            verticalVelocity=0;
            Vector3 surface=Physics.ClosestPoint(transform.position, results[0],results[0].transform.position, results[0].transform.rotation)+Vector3.up*0.5f;
            //Vector3 surface = results[0].ClosestPoint(transform.position)+ Vector3.up*0.5f;  
            // if(surface.magnitude - gravity !=0){
            //     //results[0].
            //     this.transform.position = new Vector3(this.transform.position.x, surface.magnitude - transform.position.magnitude , this.transform.position.z);
            // }
            this.transform.position = new Vector3(this.transform.position.x, surface.y , this.transform.position.z);
            isGrounded=true;
            if(results[0].gameObject.CompareTag("Pickup")){
                Destroy(results[0].gameObject);
            }
            Debug.Log("stats" + results[0].gameObject.name);
        }else {
            isGrounded = false;
        }

         
        

        if(Input.GetKey(KeyCode.Space) && isGrounded){
            isGrounded = false;
            verticalVelocity += jumpSpeed;
             Debug.Log("not grounded" + isGrounded);
            //Vector3 jump = new Vector3(0, 10,0);
            //transform.position += jump *Time.deltaTime;
             //transform.Translate( new Vector3(0, 11,0) *Time.deltaTime); 
        }
        
        Vector3 movement = new Vector3(horizontalInput *acceleration, verticalVelocity, verticalInput*acceleration);
        transform.position += movement *Time.deltaTime;
    }

    void moveWithRayCastCollider(){
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        RaycastHit hit;
        Vector3 movement = new Vector3(horizontalInput *acceleration, verticalVelocity, 0);
        Vector3 grav = new Vector3(0, verticalVelocity, 0);
        if(
            Physics.SphereCast(this.transform.position, 0.5f, grav.normalized, out hit, grav.magnitude+0.015f, m_LayerMask)
        ){
            temphit= hit.distance;
            debugmove = grav.normalized;
            Vector3 snapToSurface = grav.normalized * (hit.distance- 0.015f);
            Vector3 leftOver = grav-snapToSurface;
            if(snapToSurface.magnitude <= 0.015f){
                snapToSurface =Vector3.zero;
            }
            if(hit.transform.gameObject.CompareTag("Pickup")){
                 Destroy(results[0].gameObject);
             }
            float angle = Vector3.Angle(Vector3.up, hit.normal);
            if(angle <= 55){
                leftOver = ProjectAndScale(leftOver, hit.normal);
                if(!isGrounded){
                    grav = snapToSurface;
                }
                
            //walls or steep slope
            }else{
                 grav = snapToSurface+leftOver;
            }
            //movement += grav;
            //movement = snapToSurface+leftOver;
           
            movement +=grav;
        }
        
        if(!isGrounded){
            verticalVelocity -= 1* gravity *Time.deltaTime;
        }
        
       transform.Translate( movement *Time.deltaTime); 
     
       Debug.Log("woah "+ verticalVelocity );
       if (transform.position.y <= 0)
        {
            transform.position = new Vector3(transform.position.x, 0.5f, transform.position.z);
            isGrounded = true;
            verticalVelocity = 0;
        }

    }
    void Update()
    {
         moveWithFeetCollider();
    }

    void OnDrawGizmosSelected(){
        Gizmos.color = Color.red;
        Debug.DrawLine(transform.position, transform.position+  debugmove.normalized*temphit);
        Gizmos.DrawWireSphere(transform.position+ debugmove.normalized*temphit,0.5f+0.2f);
    }
    private void OnCollisionEnter(Collision other){
          Vector3 direction = other.GetContact(0).normal;
        if(other.gameObject.CompareTag("Ground") ){
            verticalVelocity = 0;
            groundContacts ++;
            Debug.Log("gcol contants" + groundContacts);
            Debug.Log("groundedcol " + isGrounded);
            Vector3 surface = collider.ClosestPoint(other.transform.position)+Vector3.up * 0.5f;  
            this.transform.position = new Vector3(this.transform.position.x, surface.y , this.transform.position.z);
             isGrounded= true;
        }
        else if(other.gameObject.CompareTag("Pickup")){
            isGrounded= true;
            Debug.Log("pickup");
            Destroy(other.gameObject);
        }
        
         if( direction.x == 1 ) Debug.Log("“right”"+ direction);
        if( direction.x == -1 )  Debug.Log("“left”"+ direction);
        if( direction.y == 1 )  Debug.Log("“up”"+direction);
        if( direction.y == -1 ) Debug.Log("“down”"+ direction);
        
    }

    void OnCollisionStay(Collision other)
    {
        isGrounded= true;
    }

     void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pickup"))  // Make sure the player has the tag "Player"
        {
            Debug.Log("Pickup collected!");
            // Perform your pickup logic here
            Destroy(other.gameObject);  // Optionally destroy the pickup object
        }

         
    }
    void OnCollisionExit(Collision other )
    {  // Vector2 direction = other.GetContact(0).normal;
        if (other.gameObject.CompareTag("Ground") )
        {    groundContacts --;
           // if(groundContacts == 0){
                isGrounded = false;
                 Debug.Log("g contants" + groundContacts);
           // }
            Debug.Log("weirdout");
        }
    }
}
