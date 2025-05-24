using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Character))]
[RequireComponent(typeof(CameraController))]
public class PlayerInput : MonoBehaviour
{
    [SerializeField, RequireAndAssignComponent] Character character;
    [SerializeField, RequireAndAssignComponent] CameraController cameraController;

    private void FixedUpdate() 
    {
        var yInput = Input.GetAxisRaw("Vertical");
        var xInput = Input.GetAxisRaw("Horizontal");

        var input = Camera.main.transform.forward.CollapseAxis(Axis.Y).normalized * yInput + Camera.main.transform.right * xInput;
        character.Move(input.x, input.z);
    }

    private void LateUpdate() 
    {
        cameraController.RotateCamera(Input.GetAxisRaw("CamHorizontal"), Input.GetAxisRaw("CamVertical"));
    }
}
