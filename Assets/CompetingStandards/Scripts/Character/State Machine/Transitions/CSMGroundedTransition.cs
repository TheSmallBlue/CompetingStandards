using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CompetingStandards.CSM.Transitions
{
    public class CSMGroundedTransition : Transition
    {
        [SerializeField] bool shouldBeGrounded;

        // ---

        public override bool CanTransition()
        {
            return shouldBeGrounded == SourceCharacter.GroundedCheck(out RaycastHit hitInfo);
        }
    }
}
