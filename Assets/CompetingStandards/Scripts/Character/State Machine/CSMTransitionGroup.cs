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

        public bool CanTransition(StateMachine.UpdateType updateType)
        {
            int transitionableTransitions = 0;

            foreach (var transition in IncludedTransitions.Select(x => x.transition))
            {
                if(transition.CanTransition(updateType))
                    transitionableTransitions++;
            }
            
            return transitionableTransitions >= IncludedTransitions.Length;
        }

        // ---

        [System.Serializable]
        public struct TransitionHolder
        {
            [SerializeReference, SubclassPicker] public CSM.Transition transition;
        }
    }
}
