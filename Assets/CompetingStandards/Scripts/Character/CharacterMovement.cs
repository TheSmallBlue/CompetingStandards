using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SmallBlue.InspectorUtilities;

namespace CompetingStandards
{
    // -- Movement --

    [RequireComponent(typeof(Rigidbody))]
    public partial class Character : MonoBehaviour
    {
        [RequireAndAssignComponent] public Rigidbody RB;

        // ---

        [Header("Basic Movement")]
        [SerializeField] Vector3 _gravity = new Vector3(0, -9.8f, 0);

        [Space]
        [SerializeField] float _maxBasicSpeed;

        [SerializeField] float _basicAcceleration, _basicDecceleration;

        [SerializeField] float _speedAdaptabilityDelta = 1f;

        [Header("Slope Movement")]
        [SerializeField] float _slopeForce = 5f;
        [SerializeField] float _maxSlopeAngle = 90f;
        [SerializeField] float _requiredAngleSpeed = 60f;

        [Header("Wall Check")]
        [SerializeField] float _wallCheckDistance;
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

        // ---

        float maxSpeed;
        float maxSpeedVelocity;

        // ---

        private void OnValidate()
        {
            var rb = GetComponent<Rigidbody>();

            rb.useGravity = false;
            rb.constraints = RigidbodyConstraints.FreezeRotation;
        }

        /// <summary>
        /// Moves the character horizontally in the given direction. The Y axis is ignored.
        /// </summary>
        /// <param name="xInput"> The X axis of the desired movement direction of the character. </param>
        /// <param name="zInput"> The Z axis of the desired movement direction of the character. </param>
        public void Move(Vector3 direction)
        {
            Vector3 force = Vector3.zero;

            var gravity = GetGravityForce();
            force += gravity;

            var horMovement = GetHorizontalMovementForce(direction);
            DebugVector("Horizontal Movement", new DebugVectorInfo() { position = transform.position, direction = horMovement, color = Color.red });
            force += horMovement;

            var slopeForce = GetSlopeForce(RB.velocity);
            DebugVector("Slope Force", new DebugVectorInfo() { position = transform.position, direction = slopeForce, color = new Color(0.000f, 1.000f, 0.533f, 1.000f) });
            force += slopeForce;

            var snapToFloor = GetSnapToFloorForce(force);
            force += snapToFloor;

            DebugVector("Slope Dif", new DebugVectorInfo() { position = transform.position, direction = horMovement + slopeForce, color = Color.green });

            RB.AddForce(force);
        }

        // --

        protected virtual Vector3 GetGravityForce() => _gravity;

        protected virtual Vector3 GetHorizontalMovementForce(Vector3 direction)
        {
            Vector3 currentVelocity = RB.velocity;
            Vector3 currentHorizontalVelocity = currentVelocity.CollapseAxis(Axis.Y);

            bool isGrounded = GroundedCheck(out RaycastHit groundInfo);
            Vector3 currentFloorNormal = isGrounded ? groundInfo.normal : Vector3.up;

            // Project desired movement onto ground below.
            // If there is no ground, move like we're on a flat plane.
            direction = Vector3.ProjectOnPlane(direction.CollapseAxis(Axis.Y), currentFloorNormal);

            // Normally we want to speed up to our maxBasicSpeed.
            // However, if our speed somehow gets above that, we want to maintain that new speed value.
            // So, we adjust our maxSpeed accordingly.
            float currentSpeed = isGrounded ? currentVelocity.magnitude : currentHorizontalVelocity.magnitude;
            maxSpeed = currentSpeed > maxSpeed ? currentSpeed : Mathf.SmoothDamp(maxSpeed, currentSpeed, ref maxSpeedVelocity, _speedAdaptabilityDelta);
            maxSpeed = Mathf.Clamp(maxSpeed, _maxBasicSpeed, Mathf.Infinity);

            // Get the amount of velocity we need to add to our current velocity to reach our desired speed.
            Vector3 desiredVelocity = direction.normalized * maxSpeed;
            Vector3 activeVelocity = isGrounded ? currentVelocity : currentHorizontalVelocity;
            Vector3 velocityDif = desiredVelocity - activeVelocity;

            float accel = direction.magnitude > 0.01f ? _basicAcceleration : _basicDecceleration;

            // If there's no input direction and we're standing on an angled floor, 
            // let the slope force do all the movement
            if (direction.magnitude < 0.1f && Vector3.Angle(groundInfo.normal, Vector3.up) > 10f)
                accel = 0f;

            Vector3 finalVelocity = velocityDif * accel;

            // If we have a wall in front of us, we should cancel our velocity in the direction the wall is in.
            if (WallCheck(direction, out RaycastHit wallInfo))
                finalVelocity = finalVelocity.CollapseDirection(wallInfo.normal);

            return finalVelocity;
        }

        protected virtual Vector3 GetSlopeForce(Vector3 horizontalMovement)
        {
            if (!GroundedCheck(out RaycastHit groundInfo)) return Vector3.zero;

            float floorAngle = Vector3.Angle(Vector3.up, groundInfo.normal);

            float neededClimbSpeed = Mathf.Asin(floorAngle / _maxSlopeAngle) * (_requiredAngleSpeed * 2 / Mathf.PI);

            float speedDifference = neededClimbSpeed - RB.velocity.CollapseDirection(groundInfo.normal).magnitude;

            Vector3 slopeForward = -Vector3.Cross(Vector3.Cross(groundInfo.normal, Vector3.up), groundInfo.normal);

            DebugVector("Slope Normal", new DebugVectorInfo() { position = groundInfo.point, direction = groundInfo.normal * 3f, color = Color.blue });
            DebugVector("Slope Forward", new DebugVectorInfo() { position = groundInfo.point + Vector3.up, direction = slopeForward.normalized * 3f, color = new Color(0.000f, 0.765f, 1.000f, 1.000f) });

            float slopeForce = Vector3.Dot(horizontalMovement, slopeForward) < 0 ? _slopeForce : _slopeForce * 10;

            return slopeForward * Mathf.Clamp(speedDifference, 1f, Mathf.Infinity) * slopeForce;
        }

        public Vector3 GetSnapToFloorForce(Vector3 intendedForce)
        {
            // We must be grounded
            if (!GroundedCheck(out RaycastHit currentFloorInfo)) return Vector3.zero;
            // We must be above the floor
            if (Vector3.Distance(transform.position, currentFloorInfo.point) < _groundSnappingRequiredDistance) return Vector3.zero;
            // There has to be more floor in front of us
            if (!GroundedCheck(out RaycastHit futureFloorInfo, intendedForce.normalized)) return Vector3.zero;

            return (currentFloorInfo.point - transform.position).normalized * _groundSnappingForce;
        }

        // -- Checks --

        public bool GroundedCheck(out RaycastHit hitInfo, Vector3 offset = default)
        {
            if (Physics.Raycast(transform.position + Vector3.up * 0.5f + offset, Vector3.down, out hitInfo, _groundCheckDistance, _groundCheckMask))
            {
                if (Physics.Raycast(transform.position + Vector3.up * 0.5f + offset, -transform.up, out hitInfo, _groundCheckDistance, _groundCheckMask))
                    return true;
            }

            if (Physics.SphereCast(transform.position + Vector3.up * 0.5f + offset, _groundedCheckRadius, Vector3.down, out hitInfo, _groundCheckDistance, _groundCheckMask))
            {
                if (Physics.SphereCast(transform.position + Vector3.up * 0.5f + offset, _groundedCheckRadius, -transform.up, out hitInfo, _groundCheckDistance, _groundCheckMask))
                    return true;
            }

            return false;
        }

        public bool WallCheck(Vector3 direction, out RaycastHit hitInfo)
        {
            bool wallHit = Physics.SphereCast(transform.position + Vector3.up * 0.5f, _wallCheckRadius, direction, out hitInfo, _wallCheckDistance, _wallCheckMask);

            if (!wallHit) return false;

            GroundedCheck(out RaycastHit groundInfo);
            if (Vector3.Angle(groundInfo.normal, hitInfo.normal) < _wallCheckAngle) return false;

            return true;
        }
    }
}
