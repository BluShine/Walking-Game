using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootController : MonoBehaviour
{
    Rigidbody body;

    public float walkFriction = 10;
    public float landingGripForce = 5;
    public float gravity = 10f;

    bool onGround = true;
    public bool wasOnGround = true;
    static float CASTRADIUS = .09f;

    MoveBodyToTarget pidController;

    // Use this for initialization
    void Start()
    {
        body = GetComponent<Rigidbody>();
        pidController = GetComponent<MoveBodyToTarget>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        if (onGround && !pidController.pidEnabled)
        {
            //friction
            body.AddForce(accelToStop(new Vector3(0, body.velocity.y, 0), landingGripForce, Time.fixedDeltaTime) + 
                accelToStop(new Vector3(body.velocity.x, 0, body.velocity.z), walkFriction, Time.fixedDeltaTime), ForceMode.VelocityChange);
        }
        else if(!pidController.pidEnabled)
        {
            //gravity
            body.AddForce(Vector3.down * gravity, ForceMode.Acceleration);
        }
        wasOnGround = onGround;
        onGround = false;
    }

    public bool isOnGround()
    {
        return wasOnGround;
    }

    private void OnCollisionStay(Collision collision)
    {
        RaycastHit rayHit;
        if (Physics.SphereCast(transform.position, CASTRADIUS, -transform.up, out rayHit))
        {
            if (rayHit.normal.y > 0 && body.velocity.y <= 0)
            {
                onGround = true;
            }
        }

    }

    //applies a velocity change until the velocity reaches zero;
    static Vector3 accelToStop(Vector3 velocity, float force, float deltaTime)
    {
        if (velocity.magnitude > force * deltaTime)
        {
            return velocity.normalized * -force * deltaTime;
        }
        else
        {
            return -velocity;
        }
    }
}
