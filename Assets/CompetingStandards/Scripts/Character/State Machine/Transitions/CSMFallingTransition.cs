using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CompetingStandards.CSM.Transitions
{
    public class CSMFallingTransition : Transition
    {
        [SerializeField] bool shouldBeFalling;

        // ---

        public override bool CanTransition()
        {
            return shouldBeFalling == SourceCharacter.RB.velocity.y < 0f;
        }
    }
}
