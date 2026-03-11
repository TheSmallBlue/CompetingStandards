using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CompetingStandards.CSM.Transitions
{
    public class CSMLogicTransition : CSM.Transition
    {
        [SerializeField] CSM.StateMachine.Transitions[] includedTransitions;
        [SerializeField] LogicTypes logicType;

        // ---

        CSM.Transition[] transitions => includedTransitions.Select(x => x.transition).ToArray();

        // ---

        public override bool CanTransition()
        {
            switch (logicType)
            {
                case LogicTypes.ALL:
                    return transitions.All(x => x.CanTransition());
                case LogicTypes.ANY:
                    return transitions.Any(x => x.CanTransition());
                case LogicTypes.NONE:
                    return !transitions.Any(x => x.CanTransition());
                default:
                    return false;
            }
        }

        // ---

        enum LogicTypes
        {
            ALL,
            ANY,
            NONE
        }
    }
}
