using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace CompetingStandards.CSM
{
    [System.Serializable]
    public class TransitionGroup
    {
        public TransitionHolder[] IncludedTransitions;

        public int ToIndex;

        // ---

        public void InitializeTransitions(CSM.StateMachine source)
        {
            foreach (var heldTransition in IncludedTransitions)
            {
                heldTransition.transition.Initialize(source);
            }
        }

        public bool CanTransition() => IncludedTransitions.Select(x => x.transition).All(x => x.CanTransition());

        // ---

        [System.Serializable]
        public struct TransitionHolder
        {
            [SerializeReference, SubclassPicker] public CSM.Transition transition;
        }
    }
}
