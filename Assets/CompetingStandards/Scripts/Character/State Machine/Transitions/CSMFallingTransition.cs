using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CompetingStandards.CSM.Transitions
{
    public class CSMFallingTransition : Transition
    {
        [SerializeField] bool shouldBeFalling;

        // ---

        protected override bool CanTransitionFixedUpdate()
        {
            return shouldBeFalling == SourceCharacter.RB.velocity.y < 0f;
        }
    }
}
