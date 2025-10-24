using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CompetingStandards.CSM;
using UnityEngine.InputSystem;

namespace CompetingStandards.CSM.Transitions
{
    [System.Serializable]
    public class CSMInputTransition : Transition
    {
        [SerializeField] InputActionReference input;
        public override bool CanTransition()
        {
            return true;
        }
    }
}