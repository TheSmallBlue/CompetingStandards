using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CompetingStandards.CSM;
using UnityEngine.InputSystem;

namespace CompetingStandards.CSM.States
{
    [System.Serializable]
    public class CharacterStateJump : CompetingStandards.CSM.State
    {
        [SerializeField] float jumpForce;
        [SerializeField] float jumpVariableMultiplier = 0.5f;
        [SerializeField] InputActionReference jumpInput;

       // ---

        Rigidbody characterRB => SoureCharacter.RB;

        // ---

        protected override void OnStateEnter()
        {
            characterRB.AddForce(Vector3.up * jumpForce);
        }

        protected override void OnStateFixedUpdate()
        {
            if(!jumpInput.action.IsPressed())
            {
                characterRB.velocity = characterRB.velocity.SetAxis(Axis.Y, characterRB.velocity.y * jumpVariableMultiplier);
            }
        }
    }
}