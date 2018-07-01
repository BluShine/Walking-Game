using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkingController : MonoBehaviour {

    Transform cameraTransform;
    Rigidbody body;
    MoveBodyToTarget bodyPID;
    public FootController foot;
    Rigidbody footBody;
    MoveBodyToTarget footPID;

    public float gravity = 10f;

    public float standingHeight = .5f;
    public float leanAmount = .3f;
    public Vector3 footRestPosition = Vector3.down;
    public float footStepTime = .2f;
    float stepTimer = 1000;
    public float kickForce = 5;//velocity change applied to the body when lifting the foot.
    public float liftForce = 5;//velocity change applied to the foot when lifting the foot.

    public float stepDistance = 1.5f;

    public Transform shoePreview;
    public Transform invalidShoe;
    MeshRenderer previewMesh;
    MeshRenderer invalidMesh;

    bool onGround = true;
    static float CASTRADIUS = .5f;

    static string XAXIS = "Horizontal";
    static string YAXIS = "Vertical";
    static string FOOTBUTTON = "Fire1";
    bool footDown = false;
    bool footUp = false;

    static Vector3 ZEROY = Vector3.right + Vector3.forward;

    public LayerMask raycastMask;

	// Use this for initialization
	void Start () {
        cameraTransform = gameObject.GetComponentInChildren<Camera>().transform;
        body = GetComponent<Rigidbody>();
        bodyPID = GetComponent<MoveBodyToTarget>();
        footPID = foot.transform.GetComponent<MoveBodyToTarget>();
        footBody = foot.GetComponent<Rigidbody>();
        previewMesh = shoePreview.GetComponent<MeshRenderer>();
        previewMesh.enabled = false;
        invalidMesh = invalidShoe.GetComponent<MeshRenderer>();
        invalidMesh.enabled = false;
        previewMesh.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
        footUp = footUp || Input.GetButtonUp(FOOTBUTTON);
        footDown = footDown || Input.GetButtonDown(FOOTBUTTON);
        //Debug.Log(footUp);
	}

    private void FixedUpdate()
    {
        //input handling
        Vector3 inputVector = new Vector3(Input.GetAxis(XAXIS), 0, Input.GetAxis(YAXIS)) * leanAmount;
        inputVector = Quaternion.Euler(0, cameraTransform.rotation.eulerAngles.y, 0) * inputVector;

        //foot movement
        if(footDown && foot.isOnGround())
        {
            body.AddForce((body.position - footBody.position).normalized * Mathf.Max(0, kickForce - footBody.velocity.magnitude), ForceMode.VelocityChange);
            footBody.AddForce((body.position - footBody.position).normalized * liftForce, ForceMode.VelocityChange);
        }
        if (footUp)
        {
            invalidMesh.enabled = false;
            previewMesh.enabled = false;
            Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
            Ray downRay = new Ray(transform.position, Vector3.down);
            RaycastHit hit;
            footPID.PIDReset();
            if (Physics.SphereCast(ray, .1f, out hit, 10000, raycastMask) && Vector3.Scale(ZEROY, hit.point - footBody.position).magnitude <= stepDistance)
            {
                footPID.targetPos = hit.point;
            }
            else if (Physics.SphereCast(downRay, .1f, out hit, stepDistance * 2, raycastMask))
            {
                footPID.targetPos = hit.point;
                //Debug.Log("Failed to step!");
            }
            else
            {
                stepTimer = footStepTime + 1;
                footPID.pidEnabled = false;
            }
            body.AddForce(Vector3.down * gravity, ForceMode.Acceleration);
        }
        else if (Input.GetButton(FOOTBUTTON))
        {
            shoePreview.transform.position = transform.position + cameraTransform.forward * stepDistance;
            Ray preRay = new Ray(cameraTransform.position, cameraTransform.forward);
            RaycastHit preHit;
            if (Physics.SphereCast(preRay, .1f, out preHit, 10000, raycastMask))
            {
                float distance = Vector3.Scale(ZEROY, preHit.point - footBody.position).magnitude;
                if(distance <= stepDistance)
                {
                    previewMesh.enabled = true;
                    invalidMesh.enabled = false;
                    shoePreview.position = preHit.point;
                } else
                {
                    previewMesh.enabled = false;
                    invalidMesh.enabled = true;
                    invalidShoe.position = preHit.point;
                }
                shoePreview.position = preHit.point;
            } else
            {
                invalidMesh.enabled = false;
                previewMesh.enabled = false;
            }
            bodyPID.pidEnabled = false;
            bodyPID.PIDReset();
            //footPID.pidEnabled = true;
            //footPID.targetPos = transform.position + footRestPosition;
            stepTimer = 0;
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
                bodyPID.targetPos = foot.transform.position + Vector3.up * standingHeight + inputVector;
            }
            else
            {
                bodyPID.pidEnabled = false;
                bodyPID.PIDReset();
            }
        }

        //gravity
        //body.AddForce(Vector3.down * gravity, ForceMode.Acceleration);

        footDown = false;
        footUp = false;
    }
}
