using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CompetingStandards.CSM;

namespace CompetingStandards.CSM.Transitions
{
    [System.Serializable]
    public class CSMInputTransition : Transition
    {
        public override bool CanTransition()
        {
            return true;
        }
    }
}