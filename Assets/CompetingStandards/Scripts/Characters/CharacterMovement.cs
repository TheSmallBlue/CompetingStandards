using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public partial class Character : MonoBehaviour
{
    [RequireAndAssignComponent] public Rigidbody RB;

    // ---

    [Header("Basic Movement")]
    [SerializeField] Vector3 _gravity = new Vector3(0, -9.8f, 0);
    public Vector3 Gravity => _gravity;

    [Space]
    [SerializeField, Tooltip("The speed limit our character will be able to reach when we move it in flat ground. If LimitSpeed is turned on, it's also the Maximum Speedl Limit.")] 
    float _maxBasicSpeed;

    [SerializeField, Tooltip("The acceleration we'll use to reach our max speed, and the decceleration we'll use to reach our stopping speed. The bigger the number, the faster we'll reach our desired speed")] 
    float _basicAcceleration, _basicDecceleration;
    
    [SerializeField, Tooltip("If enabled, will cap our speed to the value set in MaxBasicSpeed, regardless of slope influence or any other factor.")] 
    bool _limitSpeed = false;

    [Header("Slope Movement")]
    [SerializeField] float _slopeForce = 5f;

    [Header("Wall Check")]
    [SerializeField] float _wallCheckDistance;
    [SerializeField] float _wallCheckHeight;
    [SerializeField] float _wallCheckRadius;
    [SerializeField] float _wallCheckAngle;
    [SerializeField] LayerMask _wallCheckMask;
    
    [Header("Ground Check")]
    [SerializeField] float _groundCheckDistance;
    [SerializeField] float _groundedCheckRadius;
    [SerializeField] LayerMask _groundCheckMask;

    [Header("Ground Snapping")]
    [SerializeField] float _groundSnappingFloorHeight;
    [SerializeField] float _groundSnappingRequiredDistance;
    [SerializeField] float _groundSnappingForce;

    private void OnValidate() 
    {
        var rb = GetComponent<Rigidbody>();

        // Turn off gravity, make sure we cant rotate.
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    /// <summary>
    /// Applies gravity and moves the character horizontally in the given direction. Any value on the Y axis will be ignored.
    /// </summary>
    /// <param name="xInput"> The X axis of the desired movement direction of the character. (</param>
    /// <param name="zInput"> The Z axis of the desired movement direction of the character. </param>
    public void Move(float xInput, float zInput)
    {
        Vector3 force = Vector3.zero;

        var gravity = GetGravityForce();
        DebugVector("Gravity", new(gravity, Color.white));
        force += gravity;

        var horMovement = GetHorizontalMovementForce(xInput, zInput);
        DebugVector("Horizontal Movement", new(horMovement, Color.red));
        force += horMovement;

        var slopeForce = GetSlopeForce(RB.velocity);
        DebugVector("Slope Force", new(slopeForce, Color.blue));
        force += slopeForce;

        var snapToFloor = GetSnapToFloorForce(force);
        force += snapToFloor;

        DebugVector("Slope Dif", new(horMovement + slopeForce, Color.green));

        RB.AddForce(force);
    }

    protected virtual Vector3 GetGravityForce() => _gravity;

    protected virtual Vector3 GetHorizontalMovementForce(float xInput, float zInput)
    {
        Vector3 direction = new Vector3(xInput, 0, zInput);
        Vector3 horizontalVelocity = RB.velocity.CollapseAxis(Axis.Y);
        Vector3 velocity = RB.velocity;
        bool grounded = GroundedCheck(out RaycastHit groundHit);

        // Project our desired direction on the floor.
        // That way we can climb slopes correctly!
        direction = Vector3.ProjectOnPlane(direction, GroundedCheck(out RaycastHit groundInfo, velocity.normalized) ? groundInfo.normal : Vector3.up);
        //direction = direction.normalized * direction.magnitude;

        // If we have a wall in front of us, we should stop in the direction the wall is in.
        if (WallCheck(direction, out RaycastHit wallInfo))
        {
            direction = Vector3.ProjectOnPlane(direction, wallInfo.normal);
        }

        // If our speed isn't limited, and if our speed is bigger than our usual max speed;
        // Set our new reachable speed to our current speed.
        float currentHorSpeed = grounded ? velocity.magnitude : horizontalVelocity.magnitude;
        float maxSpeed = currentHorSpeed > _maxBasicSpeed && !_limitSpeed ? currentHorSpeed : _maxBasicSpeed;

        // Get our desired velocity, the velocity we want to reach
        // Then, get the difference between it and our current velocity
        Vector3 desiredVelocity = direction.normalized * maxSpeed;
        Vector3 velocityDif = desiredVelocity - (grounded ? velocity : horizontalVelocity);
        float accel = direction.magnitude > 0.01f ? _basicAcceleration : _basicDecceleration;

        // If there's no input and we're standing on an angled floor, let the slope force do all the movement
        if (direction.magnitude < 0.1f && Vector3.Angle(groundHit.normal, Vector3.up) > 10f)
            accel = 0f;

        DebugVector("Desired Velocity", new(desiredVelocity, Color.red * 0.5f));

        // Multiply the velocity needed to reach our desired velocity via our acceleration.
        // The bigger the number, the faster we reach our desired velocity.
        return velocityDif * accel;
    }

    protected virtual Vector3 GetSlopeForce(Vector3 horizontalMovement)
    {
        // If we're not grounded, we can't be on a slope!
        if(!GroundedCheck(out RaycastHit groundInfo, horizontalMovement.normalized)) return Vector3.zero;

        // Get the forward vector of the slope
        Vector3 slopeForward = Vector3.Cross(Vector3.Cross(groundInfo.normal, Vector3.up), groundInfo.normal);

        // Multiply the forward vector by our 
        return -slopeForward * _slopeForce;
    }

    public Vector3 GetSnapToFloorForce(Vector3 intendedForce)
    {
        // We can only snap to a floor if we're standing on one already...
        if (!GroundedCheck(out RaycastHit currentFloorInfo)) return Vector3.zero;
        // ...And if it's not a steep ramp
        if (Vector3.Angle(currentFloorInfo.normal, Vector3.up) > 10f) return Vector3.zero;
        // Only snap to the floor below us if there's more floor ahead of us.
        if (!GroundedCheck(out RaycastHit futureFloorInfo, intendedForce.normalized)) return Vector3.zero;
        // If we're still on the floor, there's no need to snap us to it.
        Vector3 intendedFloorHeight = transform.position + Vector3.up * _groundSnappingFloorHeight;
        Vector3 currentFloorHeight = currentFloorInfo.point;
        if (Vector3.Distance(intendedFloorHeight, currentFloorHeight) < _groundSnappingRequiredDistance) return Vector3.zero;

        return (currentFloorHeight - intendedFloorHeight).normalized * _groundSnappingForce;
    }

    // -- Checks --

    public bool GroundedCheck(out RaycastHit hitInfo, Vector3 offset = default)
    {
        if(Physics.Raycast(transform.position + offset, Vector3.down, out hitInfo, _groundCheckDistance, _groundCheckMask)) return true;

        if(Physics.SphereCast(transform.position + offset, _groundedCheckRadius, Vector3.down, out hitInfo, _groundCheckDistance, _groundCheckMask)) return true;

        return false;
    }

    public bool WallCheck(Vector3 direction, out RaycastHit hitInfo)
    {
        bool wallHit = Physics.CapsuleCast(transform.position + Vector3.up * _wallCheckHeight * 0.5f, transform.position - Vector3.up * _wallCheckHeight * 0.5f, _wallCheckRadius, direction, out hitInfo, _wallCheckDistance, _wallCheckMask);

        if(!wallHit) return false;

        GroundedCheck(out RaycastHit groundInfo);
        if(Vector3.Angle(groundInfo.normal, hitInfo.normal) < _wallCheckAngle) return false;

        return true;
    }
}
