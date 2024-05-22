using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
public class CollideAndSlideController : MonoBehaviour
{   
    private float horizontalInput;
    private float verticalInput;
    private Vector3 spawnPosition;
    private Vector3 currentPosition;
    private float timeTaken;
    [SerializeField] private float jumpButtonPressedTime=0;
    private float buttonPressWindow= 1;
    [SerializeField] private float jumpSpeed = 35;
    [SerializeField] private float jumpHeight = 5;
    [SerializeField] private float gravityStrength = 9.8f;
    [SerializeField] private float gravityMultiplier = 1;
    int maxBounces =5;
    float skinWidth = 0.01155f;// 0.015f;
    public GameObject currentHitObject;
    [SerializeField] float maxSlopeAngle = 55 ;
    //public float gravity = 25;
    [SerializeField] private float acceleration = 5;
    [SerializeField] private int maxSpeed = 35;
    [SerializeField] private float verticalVelocity = 0;
    public bool isGrounded = true;
    Bounds bounds;
    public Collider collider;
    [SerializeField] LayerMask layerMask;
    Vector3 velocity = Vector3.zero;
    Vector3 vdistance;
    float hitdist;
    public TextMeshProUGUI angleText;

    Quaternion groundSurfaceRotation = Quaternion.identity;

    
    /// <BetterJump variables>
    [SerializeField] private float jump_Height = 5;
    [SerializeField] private float time_To_Peak = 1;
    [SerializeField] private float time_to_Descent = 1;

    [SerializeField] private float jump_Velocity;
    [SerializeField] private float jump_gravity;
    [SerializeField] private float fall_gravity;
    /// </summary>
    
    // Start is called before the first frame update
    void Start()
    {
        // bounds = collider.bounds;
        // bounds.Expand(-2* skinWidth);
        spawnPosition = transform.position; 
        currentPosition= Vector3.zero;
    }

    void OnDrawGizmosSelected(){
        Gizmos.color = Color.red;
        Debug.DrawLine(transform.position, transform.position+ vdistance*hitdist);
        Gizmos.DrawWireSphere(transform.position+vdistance*hitdist*1.5f,bounds.extents.x+0.2f);
        Gizmos.color = Color.yellow;
        Vector3 start = transform.position;
        Vector3 end = start + transform.up;
        Debug.DrawLine(transform.position, end);
        //Gizmos.DrawWireSphere(transform.position+vdistance*hitdist,bounds.extents.x+0.2f);
    }

    private Vector3 ProjectAndScale(Vector3 vec, Vector3 normal){
        float mag = vec.magnitude;
        vec = Vector3.ProjectOnPlane(vec, normal).normalized;
        vec *=mag;
        return vec;
    }

        private Vector3 CollideAndSlide(Vector3 vel, Vector3 pos, int depth, bool gravityPass, Vector3 velInit){
        if(depth>= maxBounces){
            return Vector3.zero;
        }
        float dist = vel.magnitude + skinWidth;
        RaycastHit hit;
        if(!gravityPass){
            vdistance = vel.normalized*5;
        }
        if(Physics.SphereCast(pos, bounds.extents.x, vel.normalized, out hit, dist, layerMask, QueryTriggerInteraction.Collide))
        //if(Physics.SphereCast(pos, bounds.extents.x, vel.normalized, out hit, dist, layerMask, QueryTriggerInteraction.Collide))
        {

           
            hitdist = hit.distance;
            Debug.Log("" + hit.collider.gameObject);
            Vector3 snapToSurface = vel.normalized *(hit.distance - skinWidth);
            Vector3 leftOver= vel - snapToSurface;
            
            float angle = Vector3.Angle(Vector3.up, hit.normal);
            float anglewall = Vector3.Angle(Vector3.right, hit.normal);
            Quaternion targetRotation = Quaternion.FromToRotation(transform.up, hit.normal);            
            angleText.text = "Angle" + angle;
           
            angleText.text += "\vsurf normal" + hit.normal;
            if(snapToSurface.magnitude <=skinWidth){
                snapToSurface = Vector3.zero;
            }
            if(hit.collider.isTrigger){
                return vel;
            }
            
            float testwall =  Vector3.Dot(vel.normalized, hit.normal);
            angleText.text += "\v testwall" + testwall;
             angleText.text += "\vpre snapToSurface" + snapToSurface;
            
            //normal ground/slope
            // if(gravityPass){
            //      groundSurfaceRotation=  Quaternion.FromToRotation(Vector3.up, hit.normal);
            //      Quaternion rot = Quaternion.FromToRotation(Vector3.up, hit.normal);

            //     //          Calculate the angle from Quaternion.identity
            //     float objectangle = Quaternion.Angle(Quaternion.identity, groundSurfaceRotation);
            //     angleText.text += "\vlocal rotation" + objectangle;
            //      if(verticalVelocity<=0){
            //             isGrounded = true;
            //          }
            //          transform.up= hit.normal;
            //        return snapToSurface;
            // }

            if(angle <= maxSlopeAngle){
                if(gravityPass){
                    float floornormalangle = Vector3.Angle(transform.up, hit.normal);
                     angleText.text += "\vnormal angle" + floornormalangle;
                     angleText.text += "\vgrav angle" + angle;
                     ///changing velocity before moving possible conflict
                   // verticalVelocity = 1* fall_gravity;// *Time.deltaTime;
                     //gravityMultiplier = 1;
                     //////////
                     ///
                     if(verticalVelocity<=0){
                        isGrounded = true;
                     }
                    
                   
                    
                    Vector3 Cross = Vector3.Cross(transform.up, hit.normal).normalized;
                    if (Vector3.Dot(vel.normalized, hit.normal) < 0)
                    {
                        //Debug.Log("The vector is pointing downward.");
                        if(Cross.magnitude> 0.0011){
                        
                        transform.up= hit.normal;
                        }
                    }
                      
                    return snapToSurface;
                    
                }
                //if angle >90 collision fails in grav pass
                leftOver = ProjectAndScale(leftOver, hit.normal);
            }else if(false){
                   // use normal.y to make it hard to go up certain angle slopes
                //    float scale = 1 - Vector3.Dot(
                //     new Vector3(hit.normal.x,hit.normal.y, hit.normal.z).normalized,
                //     - new Vector3(velInit.x,hit.normal.y,velInit.z).normalized
                // );
                // leftOver = ProjectAndScale(leftOver,hit.normal)*scale;
                //
            }
            //wall or steep slope
            else{
                float scale = 1 - Vector3.Dot(
                    new Vector3(hit.normal.x,0, hit.normal.z).normalized,
                    - new Vector3(velInit.x,0,velInit.z).normalized
                );
                leftOver = ProjectAndScale(leftOver,hit.normal)*scale;
            }
            return snapToSurface+ CollideAndSlide(leftOver, pos+snapToSurface, depth+1, gravityPass, velInit);
        }
        isGrounded = false;
        return vel;
    }


    
    private void move(){
       verticalVelocity -= 1* gravityStrength *Time.deltaTime;
        float time = Time.deltaTime;
        if(Input.GetKey(KeyCode.Space) &&isGrounded){
            //isGrounded = false;
            verticalVelocity += jumpSpeed;
             Debug.Log("not grounded" );   
        }
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 inputMovement = new Vector3(horizontalInput , verticalVelocity, verticalInput) *acceleration* Time.deltaTime;
        
        inputMovement = CollideAndSlide(inputMovement, transform.position,0,false, inputMovement);
        Vector3 gravity =  new Vector3(inputMovement.x,verticalVelocity* time,inputMovement.z);
        inputMovement += CollideAndSlide(gravity, transform.position,0,true, gravity);
        transform.Translate( inputMovement);
        //transform.position+= moveAmount;
    }
    // Update is called once per frame
    void Update()
    {   
        bounds = collider.bounds;
        bounds.Expand(-2* skinWidth);
        
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        // if((horizontalInput != 0 ||verticalInput !=0) &&acceleration <= maxSpeed){
        //     acceleration += 0.5f * Time.deltaTime;
        // }else if(acceleration >=3){
        //      acceleration -= 1f;
        // }

        
        //transform.position += moveInputs *Time.deltaTime;
        
        
         // Calculate acceleration based on input
        Vector3 inputAcceleration = new Vector3(horizontalInput, 0, verticalInput) * acceleration;

        // Update velocity
        velocity += inputAcceleration * Time.deltaTime;

        // Clamp velocity to max speed
        velocity = Vector3.ClampMagnitude(velocity, 15);

        // Update position based on velocity
        //transform.position += velocity * Time.deltaTime;


        Vector3 moveInputs = new Vector3(horizontalInput,0,verticalInput);
        //move(moveInputs);
        //move(velocity);
        //SimpleMove();
        Move2();
        //move();
        
    }
    void FixedUpdate(){
        //  bounds = collider.bounds;
        // bounds.Expand(-2* skinWidth);
        
        // horizontalInput = Input.GetAxis("Horizontal");
        // verticalInput = Input.GetAxis("Vertical");

       
        
        //  // Calculate acceleration based on input
        // Vector3 inputAcceleration = new Vector3(horizontalInput, 0, verticalInput) * acceleration;

        // // Update velocity
        // velocity += inputAcceleration * Time.deltaTime;

        // // Clamp velocity to max speed
        // velocity = Vector3.ClampMagnitude(velocity, 15);

        // // Update position based on velocity
        // //transform.position += velocity * Time.deltaTime;


        // Vector3 moveInputs = new Vector3(horizontalInput,0,verticalInput);
        // //move(moveInputs);
        // //move(velocity);
        // //SimpleMove();
        // Move2();
        // //move();
    }

    private void BasicJump(){
         float deltaTime = Time.deltaTime;
        
        float jumpforce = Mathf.Sqrt(jumpHeight * (gravityStrength *gravityMultiplier) * 2);
        Debug.Log("JUMPFORCE" + jumpforce);
       
        if(Input.GetKey(KeyCode.Space) &&isGrounded){
            verticalVelocity = jumpforce;
             Debug.Log("not grounded" );
             isGrounded = false; 
             jumpButtonPressedTime =0;
              gravityMultiplier = 1;
        }

        
        if(!isGrounded){
            verticalVelocity += -gravityMultiplier* gravityStrength *Time.deltaTime;
            jumpButtonPressedTime += deltaTime;
            if(jumpButtonPressedTime < buttonPressWindow && Input.GetKeyUp(KeyCode.Space)){
                gravityMultiplier = 2;
                 Debug.Log(" release space bar");
            }
            if(verticalVelocity <=0){
                gravityMultiplier = 10;
                //asd
            }
            
        }else{
            gravityMultiplier = 1;
        }

        
    }

    private void BetterJump(){
    jump_Velocity = (2.0f * jump_Height)/time_To_Peak;
    jump_gravity = (-2.0f * jump_Height)/(time_To_Peak * time_To_Peak);
    fall_gravity = (-2.0f * jump_Height)/(time_to_Descent*time_to_Descent);
        // if( verticalVelocity < 0 ){
        //     return jump_gravity;
        // }
        // else{
        //     return fall_gravity;
        // }
        // if  jumpButtonPressedTime  >= timetopeak verticalvelocity += fallgravity 
        
        if(!isGrounded){
            if(verticalVelocity > 0){
             verticalVelocity += jump_gravity *Time.deltaTime;
            
        }else{
             verticalVelocity += fall_gravity *Time.deltaTime;
        }

         jumpButtonPressedTime += Time.deltaTime;
         if(jumpButtonPressedTime >=1){
             //groundSurfaceRotation = Quaternion.identity;
             transform.up = Vector3.up;
         }


        }
       
        if(Input.GetKey(KeyCode.Space) &&isGrounded){
            verticalVelocity = jump_Velocity;
             Debug.Log("not grounded" );
             isGrounded = false; 
             jumpButtonPressedTime =0;
              gravityMultiplier = 1;
        }
    }
    private void Move2(){
        float deltaTime = Time.deltaTime;
        //BasicJump();
        BetterJump();
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 inputMovement = new Vector3(horizontalInput , 0, verticalInput) *acceleration *deltaTime;
        //angleText.text += "inputMovement" + inputMovement;


        Vector3 gravity = transform.up *verticalVelocity*deltaTime; //groundSurfaceRotation* new Vector3(0,verticalVelocity,0)* deltaTime;
        
       // angleText.text +="\vfirst collpass" +inputMovement;
        inputMovement = CollideAndSlide(transform.rotation*inputMovement, transform.position,0,false, inputMovement);
        inputMovement += CollideAndSlide(gravity, transform.position+inputMovement,0,true, gravity);
        //angleText.text +="\vsecond collpass" +inputMovement;

        transform.Translate( inputMovement, Space.World);
        //transform.position += inputMovement*1;
       //transform.rotation = groundSurfaceRotation;
        
    }

    private Vector3 SimpleCollideAndSlide(Vector3 vel, Vector3 pos, int depth){
        if(depth>=maxBounces){
            return Vector3.zero;
        }
        float dist = vel.magnitude + skinWidth;
        
        RaycastHit hit;
        if(
            Physics.SphereCast(pos, bounds.extents.x, vel.normalized, out hit, dist, layerMask)
        ){
            vdistance= vel.normalized;
            hitdist =  hit.distance;
           
            currentHitObject = hit.transform.gameObject;
            //Debug.Log(hit.transform.gameObject.name + "dist" + hit.distance);
            Vector3 snapToSurface = vel.normalized * (hit.distance - skinWidth);
            Vector3 leftOver = vel-snapToSurface;

            if(snapToSurface.magnitude <= skinWidth){
                snapToSurface =Vector3.zero;
            }   

            float mag = leftOver.magnitude;
            leftOver = Vector3.ProjectOnPlane(leftOver, hit.normal).normalized;
            leftOver *= mag;
            return snapToSurface + SimpleCollideAndSlide(leftOver, pos+snapToSurface, depth+1);
        }
        return vel;
    }
    public void SimpleMove(){
        verticalVelocity -= 1* gravityStrength *Time.deltaTime;
        float time;
        if(Input.GetKey(KeyCode.Space) &&isGrounded){
            //isGrounded = false;
            verticalVelocity += jumpSpeed;
             Debug.Log("not grounded" );
            // time = 1;
             time =  Time.deltaTime;
            //Vector3 jump = new Vector3(0, 10,0);
            //transform.position += jump *Time.deltaTime;
            //transform.Translate( new Vector3(0, 11,0) *Time.deltaTime); 
        }else{
             //verticalVelocity = -1* gravityStrength * ;
             time =   Time.deltaTime;
        }
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 inputMovement = new Vector3(horizontalInput *acceleration, verticalVelocity, verticalInput*acceleration) * Time.deltaTime;
       // Vector3 gravity =  new Vector3(horizontalInput*acceleration,verticalVelocity  ,verticalInput*acceleration)* time;
        Vector3 moveAmount = inputMovement;// +gravity;
        moveAmount = SimpleCollideAndSlide(inputMovement, transform.position,0);
        //moveAmount = new Vector3(moveAmount.x, moveAmount.y, moveAmount.z);
        transform.Translate(inputMovement );
    }

    void OnTriggerEnter(Collider other) {
        Destroy(other.gameObject);
    }
}
