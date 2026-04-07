using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CompetingStandards.CSM;
using UnityEngine.InputSystem;

namespace CompetingStandards.CSM.Transitions
{
    [System.Serializable]
    public class CSMInputPressedTransition : Transition
    {
        [SerializeField] InputActionReference input;
        [SerializeField] bool waitUntilFixedUpdate;

        // ---

        bool inputPressed;

        // ---

        public override void OnTransitionedInto()
        {
            inputPressed = false;
        }

        // ---

        protected override bool CanTransitionUpdate()
        {
            if(!waitUntilFixedUpdate)
                return input.action.IsPressed();
            
            if(input.action.IsPressed())
                inputPressed = true;

            return false;
        }

        protected override bool CanTransitionFixedUpdate()
        {
            return inputPressed;
        }
    }
}