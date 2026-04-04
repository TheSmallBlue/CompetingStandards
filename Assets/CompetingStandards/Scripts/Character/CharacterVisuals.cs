using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace CompetingStandards
{
    // -- Visuals --
    public partial class Character : MonoBehaviour
    {
        [Header("Visuals")]
        [SerializeField] Transform charaMesh;
        [SerializeField] Animator charaAnim;

        [Header("Ground Snapping")]
        [SerializeField] Vector3 groundSnappingPlayerMeshOffset;
        [SerializeField] float groundSnappingTime = 0.1f;
        [SerializeField] float groundRotationTime = 0.1f;

        // ---

        Vector3? initialMeshOffset;

        Vector3 groundSnappingVelocity;
        Quaternion groundRotationVelocity;

        float tiltDifference;
        float tiltDifferenceVel;

        float meshPositionTime = 0.1f;
        Vector3 meshPositionVel;

        // ---

        private void UpdateCharacterVisuals() 
        {
            Vector3 cross = Vector3.Cross(RB.velocity.normalized, Vector3.up);
            tiltDifference = -Vector3.Dot(desiredDirection.normalized, cross);
            charaAnim.SetFloat("TiltDifference", Mathf.SmoothDamp(charaAnim.GetFloat("TiltDifference"), tiltDifference, ref tiltDifferenceVel, 0.1f));

            charaAnim.SetFloat("SpeedPercentage", RB.velocity.CollapseAxis(Axis.Y).magnitude / _startingMaxSpeed);

            charaAnim.SetBool("Grounded", GroundedCheck(out RaycastHit hitInfo));
        }

        private void LateUpdate()
        {
            charaMesh.rotation = GetMeshRotation(charaMesh.rotation);

            Vector3 desiredPos = GroundedCheck(out RaycastHit hitInfo) ? hitInfo.point : transform.position;
            charaMesh.position = Vector3.SmoothDamp(charaMesh.position, desiredPos, ref meshPositionVel, meshPositionTime);
        }

        // ---

        Quaternion GetMeshRotation(Quaternion currentRotation)
        {
            return GetForwardRotation();
        }

        Quaternion GetForwardRotation()
        {
            Quaternion targetRotation = GroundedCheck(out RaycastHit hitInfo) && RB.velocity.CollapseAxis(Axis.Y).magnitude > 0.5f ? Quaternion.LookRotation(RB.velocity.normalized, Vector3.up) : Quaternion.LookRotation(RB.velocity.CollapseAxis(Axis.Y).normalized, Vector3.up);


            return QuaternionUtil.SmoothDamp(charaMesh.rotation, targetRotation, ref groundRotationVelocity, groundRotationTime);
        }
    }
}
