using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SmallBlue.InspectorUtilities;
using CompetingStandards;

[RequireComponent(typeof(Character))]
[RequireComponent(typeof(CameraController))]
public class TempInput : MonoBehaviour
{
    // TODO: Replace with unity's new input system

    [SerializeField, RequireAndAssignComponent] Character character;
    [SerializeField, RequireAndAssignComponent] CameraController cameraController;

    // ---

    private void FixedUpdate()
    {
        var xInput = Input.GetAxisRaw("Horizontal");
        var yInput = Input.GetAxisRaw("Vertical");

        var input = Camera.main.transform.forward.CollapseAxis(Axis.Y).normalized * yInput + Camera.main.transform.right * xInput;
        character.Move(input);
    }

    private void LateUpdate() 
    {
        cameraController.RotateCamera(Input.GetAxisRaw("CamHorizontal"), Input.GetAxisRaw("CamVertical"));
    }
}
