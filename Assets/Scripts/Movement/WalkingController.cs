using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkingController : MonoBehaviour {

    Transform cameraTransform;
    Rigidbody body;
    MoveBodyToTarget bodyPID;
    public FootController foot;
    MoveBodyToTarget footPID;

    public float gravity = 10f;

    public float standingHeight = .5f;
    public float leanAmount = .3f;
    public Vector3 footRestPosition = Vector3.down;
    public float footStepTime = .2f;
    float stepTimer = 1000;

    bool onGround = true;
    static float CASTRADIUS = .5f;

    static string XAXIS = "Horizontal";
    static string YAXIS = "Vertical";
    static string FOOTBUTTON = "Fire1";
    float holdTimer = 0;

    public LayerMask raycastMask;

	// Use this for initialization
	void Start () {
        cameraTransform = gameObject.GetComponentInChildren<Camera>().transform;
        body = GetComponent<Rigidbody>();
        bodyPID = GetComponent<MoveBodyToTarget>();
        footPID = foot.transform.GetComponent<MoveBodyToTarget>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void FixedUpdate()
    {
        //input handling
        Vector3 inputVector = new Vector3(Input.GetAxis(XAXIS), 0, Input.GetAxis(YAXIS)) * leanAmount;
        inputVector = Quaternion.Euler(0, cameraTransform.rotation.eulerAngles.y, 0) * inputVector;

        //foot movement
        if (Input.GetButton(FOOTBUTTON))
        {
            bodyPID.pidEnabled = false;
            bodyPID.PIDReset();
            footPID.pidEnabled = true;
            footPID.targetPos = transform.position + footRestPosition;
            stepTimer = 0;
            body.AddForce(Vector3.down * gravity, ForceMode.Acceleration);
        }
        else if (Input.GetButtonUp(FOOTBUTTON))
        {
            Ray ray = new Ray(transform.position, cameraTransform.forward);
            Ray downRay = new Ray(transform.position, Vector3.down);
            RaycastHit hit;
            footPID.PIDReset();
            if(Physics.SphereCast(ray, .1f, out hit, standingHeight + CASTRADIUS + .1f, raycastMask))
            {
                footPID.targetPos = hit.point;
            } else if(Physics.SphereCast(downRay, .1f, out hit, standingHeight + CASTRADIUS + .1f, raycastMask))
            {
                footPID.targetPos = hit.point;
            } else
            {
                stepTimer = footStepTime + 1;
                footPID.pidEnabled = false;
            }
            body.AddForce(Vector3.down * gravity, ForceMode.Acceleration);
        }
        else
        {
            stepTimer += Time.fixedDeltaTime;
            if(stepTimer <= footStepTime)
            {
                footPID.pidEnabled = true;
            } else
            {
                footPID.pidEnabled = false;
                footPID.PIDReset();
            }
            //standing
            if (foot.isOnGround())
            {
                bodyPID.pidEnabled = true;
                bodyPID.targetPos = foot.transform.position + Vector3.up * (standingHeight + CASTRADIUS) + inputVector;
            }
            else
            {
                bodyPID.pidEnabled = false;
                bodyPID.PIDReset();
                //gravity
                body.AddForce(Vector3.down * gravity, ForceMode.Acceleration);
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        RaycastHit rayHit;
        if(Physics.SphereCast(transform.position, CASTRADIUS, -transform.up, out rayHit))
        {
            if(rayHit.normal.y > 0 && body.velocity.y <= 0)
            {
                onGround = true;
            }
        }

    }

    //applies a velocity change until the velocity reaches zero;
    static Vector3 accelToStop(Vector3 velocity, float force, float deltaTime)
    {
        if(velocity.magnitude > force * deltaTime)
        {
            return velocity.normalized * -force * deltaTime;
        } else
        {
            return -velocity;
        }
    }
}
