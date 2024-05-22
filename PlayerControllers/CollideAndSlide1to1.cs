using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class CollideAndSlide1to1 : MonoBehaviour
{
    public Collider collider;
    Bounds bounds;
    int maxBounces =5;
    float skinWwidth = 0.155f;
    public float maxSlopeAngle = 55;
    public float acceleration =5;
    public LayerMask layerMask;

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
            float angle = Vector3.Angle(Vector3.up, hit.normal);
            if(snapToSurface.magnitude <=skinWwidth){
                snapToSurface = Vector3.zero;
            }
            //normal ground/slope
            if(angle <= maxSlopeAngle){
                if(gravityPass){
                    return snapToSurface;
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
        }


        return vel;
    }

    private Vector3 ProjectAndScale(Vector3 vec, Vector3 normal){
        float mag = vec.magnitude;
        vec = Vector3.ProjectOnPlane(vec, normal).normalized;
        vec *=mag;
        return vec;
    }

    // Start is called before the first frame update
    void Start()
    {
        //collider = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {   
        bounds = collider.bounds;
        bounds.Expand(-2* skinWwidth);

        Move();
        
    }

    private void RetardMove(){
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 inputMovement = new Vector3(horizontalInput , 0, verticalInput) *acceleration;
        Vector3 moveAmount = transform.position + inputMovement * Time.deltaTime;
        Vector3 gravity =  new Vector3(0,-9.8f,0) ;
        gravity += transform.position *Time.deltaTime;
        inputMovement = CollideAndSlide(moveAmount, transform.position,0,false, moveAmount);
        inputMovement += CollideAndSlide(gravity, transform.position,0,true, gravity);
        this.transform.position = inputMovement;
    }

    private void Move(){
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 inputMovement = new Vector3(horizontalInput , 0, verticalInput) *acceleration *Time.deltaTime;
        Vector3 gravity =  new Vector3(0,-9.8f,0)* Time.deltaTime;
        inputMovement = CollideAndSlide(inputMovement, transform.position,0,false, inputMovement);
        inputMovement += CollideAndSlide(gravity, transform.position+inputMovement,0,true, gravity);
        transform.Translate( inputMovement);
    }
}
