﻿using UnityEngine;
using System.Collections;

public class CarController : MonoBehaviour {
    public float Speed = 20;
    public float SidewaysCompensation = 0;
    public float SuspensionStrength = 5;
    public float TurningSpeed = 2;
    public bool DebugOn = true;
    public float SuspensionHeight = 3;
    public float Drag = 5;

    [HideInInspector] public bool Blocked = false;
    public float CurrentSpeed{ get { return Body.velocity.magnitude*2.23693629f; }}

    private Rigidbody Body;

    // Use this for initialization
    void Start () {
        Body = GetComponent<Rigidbody>();
        Body.centerOfMass = new Vector3(0, -1, 0);
    }

    public void Move (float steering, float accel) {
        RaycastHit raycastInfo = new RaycastHit();
        float raycastDistance = SuspensionHeight + 1;
        bool grounded = Physics.Raycast(Body.position, -transform.up, out raycastInfo, raycastDistance);

        if (DebugOn) Debug.DrawRay(Body.position, -transform.up, Color.red, -1, false);

        int direction;
        if (accel != 0) direction = (int)(accel / Mathf.Abs(accel));
        else direction = 1;

        Body.AddRelativeTorque(0, steering * TurningSpeed * CurrentSpeed/Speed * direction, 0);

        if (grounded) {
            Body.drag = 5;

            Vector3 velocity = Body.velocity;
            float sidewaysVelocity = transform.InverseTransformDirection(Body.velocity).z;

            Vector3 force = transform.rotation * new Vector3(accel * Speed, 0, -sidewaysVelocity * SidewaysCompensation);
            Vector3 groundNormal = raycastInfo.normal;
            if (DebugOn) Debug.DrawRay(raycastInfo.point, groundNormal, Color.green, -1, false);

            Vector3 projectedForce = Vector3.ProjectOnPlane(force, groundNormal);
            if (DebugOn) Debug.DrawRay(Body.position, projectedForce, Color.blue, -1, false);

            Vector3 forcePosition = Body.position + transform.rotation * new Vector3(5 * direction, -2f, 0);
            if (DebugOn) Debug.DrawLine(forcePosition, Body.position, Color.black, -1, false);
            Body.AddForceAtPosition(projectedForce, forcePosition);

            Blocked = (velocity.magnitude < 1 && force.magnitude >= Speed);

            if (DebugOn && Blocked) Debug.Log("Blocked");
        } else {
            Body.drag = 0;
        }

        //distance of the wheels to the car
        float xDist = 1f;
        float yDist = 0.8f;
        Suspension(3, Body.position + transform.rotation * new Vector3(xDist, 0, yDist));
        Suspension(4, Body.position + transform.rotation * new Vector3(-xDist, 0, yDist));
        Suspension(1, Body.position + transform.rotation * new Vector3(xDist, 0, -yDist));
        Suspension(2, Body.position + transform.rotation * new Vector3(-xDist, 0, -yDist));
    }


    void Suspension (int index, Vector3 origin) {
        Vector3 direction = -transform.up;
        RaycastHit info = new RaycastHit();
        bool grounded = Physics.Raycast(origin, direction, out info, SuspensionHeight);
        Transform wheel = transform.GetChild(0).GetChild(index);

        if (grounded) {
            float wheelHeight = 0.25f;
            wheel.position = new Vector3(info.point.x, info.point.y + wheelHeight, info.point.z);
            float strength = SuspensionStrength / info.distance - SuspensionStrength;
            Vector3 push = transform.rotation * new Vector3(0, strength, 0);
            Body.AddForceAtPosition(push, origin);
        } else {
            wheel.position = origin + direction * SuspensionHeight;
        }
       
        wheel.Rotate(new Vector3(0,0, CurrentSpeed * 0.5f));

        if (DebugOn) {
            if (grounded) {
                Debug.DrawLine(origin, info.point, Color.white, -1, false);
            } else {
                Debug.DrawLine(origin, origin + direction * SuspensionHeight, Color.white, -1, false);
            }
        }
    }
}
