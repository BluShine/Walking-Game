using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootController : MonoBehaviour
{
    Rigidbody body;

    public float walkFriction = 10;
    public float landingGripForce = 5;
    public float gravity = 10f;
    public float groundMassMultiplier = 10; //increases mass while on the ground, to improve grip.

    bool onGround = true;
    bool isColliding = false;
    bool wasOnGround = true;
    static float CASTRADIUS = .15f;
    public LayerMask castMask;

    MoveBodyToTarget pidController;

    float startMass = 1;

    // Use this for initialization
    void Start()
    {
        body = GetComponent<Rigidbody>();
        pidController = GetComponent<MoveBodyToTarget>();
        startMass = body.mass;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        RaycastHit hit;
        if (Physics.BoxCast(transform.position + Vector3.up * .1f, new Vector3(CASTRADIUS, .01f, CASTRADIUS), Vector3.down, out hit, Quaternion.identity, .2f, castMask))
        {
            if (hit.normal.y > 0 && body.velocity.y <= .01f)
            {
                onGround = true;
            }
        }

        if (onGround && !pidController.pidEnabled)
        {
            body.mass = startMass * groundMassMultiplier;
            //friction
            body.AddForce(accelToStop(new Vector3(0, body.velocity.y, 0), landingGripForce, Time.fixedDeltaTime) + 
                accelToStop(new Vector3(body.velocity.x, 0, body.velocity.z), walkFriction, Time.fixedDeltaTime), ForceMode.VelocityChange);
        }
        else if(!pidController.pidEnabled)
        {
            body.mass = startMass;
        }
        //gravity
        body.AddForce(Vector3.down * gravity, ForceMode.Acceleration);
        wasOnGround = onGround;
        onGround = false;
    }

    public bool isOnGround()
    {
        return wasOnGround;
    }

    private void OnCollisionEnter(Collision collision)
    {
        isColliding = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        isColliding = false;
    }

    private void OnCollisionStay(Collision collision)
    {
        //this shit is unreliable and mostly useless.
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
