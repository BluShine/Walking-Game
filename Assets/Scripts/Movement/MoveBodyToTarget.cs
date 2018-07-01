using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//PID controller that moves a rigidbody towards a position.
public class MoveBodyToTarget : MonoBehaviour {

    public Rigidbody body;
    public Vector3 maxForce = Vector3.one;
    public Vector3 targetPos = Vector3.zero;

    public float kP = .2f;
    public float kI = .05f;
    public float kD = .1f;

    public float iBound = 1;

    Vector3 lastError = Vector3.zero;
    Vector3 integral = Vector3.zero;

    public bool pidEnabled = true;

    public Rigidbody opposite = null; //all forces are also applied equally and opposite to this body (unless null)

    public bool useRelativeVelocity = false;
    public Rigidbody parentBody = null;
    public float maxVelocity = Mathf.Infinity;

    // Use this for initialization
    void Start () {
		
	}

    private void FixedUpdate()
    {
        //PID controller
        if (pidEnabled)
        {
            Vector3 errorVector = targetPos - body.position;
            errorVector = transform.InverseTransformVector(errorVector); //translate to local space for better behavior along each axis
            Vector3 deriv = (errorVector - lastError) / Time.fixedDeltaTime;
            integral += errorVector * Time.fixedDeltaTime;
            integral = new Vector3(Mathf.Clamp(integral.x, -iBound, iBound),
                Mathf.Clamp(integral.y, -iBound, iBound),
                Mathf.Clamp(integral.z, -iBound, iBound));
            lastError = errorVector;

            Vector3 force = kP * errorVector + kI * integral + kD * deriv;
            if (force.magnitude > 1)
            {
                force.Normalize();
            }
            body.AddForce(Vector3.Scale(transform.TransformVector(force), maxForce), ForceMode.VelocityChange);
            opposite.AddForce(-Vector3.Scale(transform.TransformVector(force), maxForce) * (body.mass / opposite.mass), ForceMode.VelocityChange);
        }

        //limit max velocity
        if(useRelativeVelocity && parentBody != null)
        {
            Vector3 localVelocity = body.velocity - parentBody.velocity;
            if (localVelocity.magnitude > maxVelocity)
            {
                body.velocity = body.velocity - (localVelocity - localVelocity.normalized * maxVelocity);
            }
        } else
        {
            if(body.velocity.magnitude > maxVelocity)
            {
                body.velocity = body.velocity.normalized * maxVelocity;
            }
        }
    }

    public void PIDReset()
    {
        lastError = Vector3.zero;
        integral = Vector3.zero;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(targetPos, .1f);
    }
}
