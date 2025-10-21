using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace CompetingStandards
{
    // -- Visuals --
    public partial class Character : MonoBehaviour
    {
        [Header("Visuals")]
        [SerializeField] Transform playerMesh;

        [Header("Ground Snapping")]
        [SerializeField] Vector3 groundSnappingPlayerMeshOffset;
        [SerializeField] float groundSnappingTime = 0.1f;
        [SerializeField] float groundRotationTime = 0.1f;

        // ---

        Vector3? initialMeshOffset;

        Vector3 groundSnappingVelocity;
        Quaternion groundRotationVelocity;

        // ---

        private void LateUpdate()
        {
            playerMesh.localPosition = GetMeshOffset(playerMesh.localPosition);
            playerMesh.rotation = GetMeshRotation(playerMesh.rotation);
        }

        // ---

        Vector3 GetMeshOffset(Vector3 currentOffset)
        {
            return GetFloorDistanceOffset();
        }

        Vector3 GetFloorDistanceOffset()
        {
            if (initialMeshOffset == null)
                initialMeshOffset = playerMesh.localPosition;

            bool grounded = GroundedCheck(out RaycastHit groundInfo);

            Vector3 floorPoint = grounded ? groundInfo.point + groundSnappingPlayerMeshOffset : Vector3.zero;
            Vector3 dirToFloor = grounded ? floorPoint - transform.position : initialMeshOffset.Value;

            return Vector3.SmoothDamp(playerMesh.localPosition, dirToFloor, ref groundSnappingVelocity, groundSnappingTime);
        }

        // ---

        Quaternion GetMeshRotation(Quaternion currentRotation)
        {
            return GetForwardRotation();
        }

        Quaternion GetForwardRotation()
        {
            if (RB.velocity.magnitude < 0.5f) return playerMesh.rotation;

            return QuaternionUtil.SmoothDamp(playerMesh.rotation, Quaternion.LookRotation(RB.velocity.normalized, Vector3.up), ref groundRotationVelocity, groundRotationTime);
        }
    }
}
