using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TestSlideController : MonoBehaviour
{
    // Start is called before the first frame update
    public TextMeshProUGUI angleText;
    public Collider collider;
    Bounds bounds;
    int maxBounces =10;
    float skinWwidth = 0.0515f;
    public float maxSlopeAngle = 55;
    public float gravityForce = -9.8f;
    public float acceleration =5;
    public LayerMask layerMask;
    public Vector3 projectAndScaleVector;
    Quaternion groundSurfaceRotation;

    private Vector3 CollideAndSlide(Vector3 vel, Vector3 pos, int depth, bool gravityPass, Vector3 velInit){
        if(depth>= maxBounces){
            return Vector3.zero;
        }
        float dist = vel.magnitude +skinWwidth;
        RaycastHit hit;
        if(Physics.SphereCast(pos, bounds.extents.x, vel.normalized, out hit, dist, layerMask))
        {
            Debug.Log("" + hit.collider.gameObject);
            Vector3 snapToSurface = vel.normalized *(hit.distance - skinWwidth);
            Vector3 leftOver= vel - snapToSurface;
            /////////////////ANGLES
            angleText.text += "e ";
            float angle = Vector3.Angle(Vector3.up, hit.normal);
           if(!gravityPass){
             angleText.text += "\vSides angle" + angle;
           }else{
             angleText.text += "\vgrav angle" + angle;
           }
           float ballfloorangle = Vector3.Angle(transform.up, hit.normal);
            if(gravityPass){
             angleText.text += "\vtransform angle" + ballfloorangle;
           }


            ///////////Angles
            if(snapToSurface.magnitude <=skinWwidth){
                snapToSurface = Vector3.zero;
            }
            //normal ground/slope
            if(angle <= maxSlopeAngle){
                if(gravityPass){
                     Quaternion targetRotation = Quaternion.FromToRotation(transform.up, hit.normal);
                      float angleNormal = Vector3.Angle(transform.up,  hit.normal);
                     Vector3 Cross = Vector3.Cross(transform.up, hit.normal).normalized;
                     Vector3 rotationAxis = Cross.normalized;
                     Quaternion rotation = Quaternion.AngleAxis(angleNormal, rotationAxis);
                     // Apply the rotation to the object
                   // transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation * transform.rotation, angle);
                    if(Cross.magnitude> 0.0011f){
                        //groundSurfaceRotation = Quaternion.identity;
                        //groundSurfaceRotation=  Quaternion.FromToRotation(transform.up, hit.normal);
                        groundSurfaceRotation=  Quaternion.FromToRotation(Vector3.up, hit.normal);
                         transform.up = hit.normal;
                         
                        // groundSurfaceRotation  = Quaternion.AngleAxis(angleNormal, rotationAxis);
                     }
                    
                      //groundSurfaceRotation=  Quaternion.FromToRotation(Cross.normalized, Vector3.up.normalized);
                    //groundSurfaceRotation=  Quaternion.FromToRotation(transform.up, hit.normal);
                    return snapToSurface;//+= snapToSurface.normalized*skinWwidth;
                }
                leftOver = ProjectAndScale(leftOver, hit.normal);
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
        }else{
            if(gravityPass){
                //transform.rotation= Quaternion.identity;
                //groundSurfaceRotation= Quaternion.identity;
            }
            
        }

        
        return vel;
    }

    private Vector3 ProjectAndScale(Vector3 vec, Vector3 normal){
        float mag = vec.magnitude;
        vec = Vector3.ProjectOnPlane(vec, normal).normalized;
        projectAndScaleVector = vec;
        vec *=mag;
        
        return vec;
       
    }

     void OnDrawGizmosSelected(){
         Gizmos.DrawWireSphere(transform.position+projectAndScaleVector,bounds.extents.x+0.2f);
     }

    // Start is called before the first frame update
    void Start()
    {
        //collider = GetComponent<Collider>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {   
        bounds = collider.bounds;
        bounds.Expand(-2* skinWwidth);

        Move();
       
        
    }


    private void Move(){
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        float frontInput; 
        if(Input.GetKey(KeyCode.UpArrow)){
            frontInput = 1;
        }else if(Input.GetKey(KeyCode.DownArrow)){
            frontInput = -1;
        }else{
            frontInput = 0;
        }
        if(Input.GetKey(KeyCode.Q)){
            transform.rotation= Quaternion.identity;
        }
        Vector3 inputMovement = new Vector3(horizontalInput , 0, frontInput)*acceleration *Time.deltaTime;
        Vector3 gravity = transform.up * gravityForce * Time.deltaTime;//transform.rotation* new Vector3(0,-9.8f,0)* Time.deltaTime;
        angleText.text = "gravity" + gravity;
        angleText.text += "gravity rotated" + transform.rotation*gravity;
        inputMovement = CollideAndSlide( transform.rotation*inputMovement , transform.position,0,false, inputMovement);
        inputMovement += CollideAndSlide( gravity, transform.position+inputMovement,0,true, gravity);
        
        transform.Translate( inputMovement, Space.World);//, Space.Self);
        //transform.rotation = groundSurfaceRotation;//transform.ri
       
    }
}
