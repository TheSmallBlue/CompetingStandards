using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] LayerMask collisionMask;

    [Space]
    [SerializeField] float sensitivity = 300f;
    [SerializeField] float distance = 10f;

    public Transform CameraTransform => Camera.main.transform.root;

    Vector2 rotation;
    Vector3 positionVelocity;

    public void RotateCamera(float xInput, float yInput)
    {
        var input = new Vector2(xInput, -yInput);

        if(Cursor.lockState != CursorLockMode.Locked)
            Cursor.lockState = CursorLockMode.Locked;

        if(input.magnitude == 0f) return;

        rotation += input * sensitivity * Time.deltaTime;
        rotation.y = Mathf.Clamp(rotation.y, -89f, 89f);

        CameraTransform.rotation = Quaternion.Euler(rotation.y, rotation.x, CameraTransform.eulerAngles.z);
        //CameraTransform.position = transform.position + GetPositionVector();
    }
    
    private void LateUpdate() 
    {

        CameraTransform.position = transform.position + GetPositionVector();
    }

    Vector3 GetPositionVector()
    {
        Vector3 intendedDirection = -CameraTransform.forward * 10f;

        /*

        if (Physics.SphereCast(transform.position, 0.5f, intendedDirection.normalized, out RaycastHit hitInfo, intendedDirection.magnitude, collisionMask))
        {
            intendedDirection = intendedDirection.normalized * Vector3.Distance(transform.position, hitInfo.point);
        }
        */

        return intendedDirection;
    }
}
