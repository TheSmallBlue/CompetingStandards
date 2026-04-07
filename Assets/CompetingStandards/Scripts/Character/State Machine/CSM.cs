using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CompetingStandards.CSM
{
    [CreateAssetMenu(fileName = "State Machine", menuName = "CompetingStandards/Character State Machine", order = 1)]
    public class StateMachine : ScriptableObject
    {
        [SerializeField] StateWithTransitions[] states;

        // ---

        bool initialized = false;

        int currentStateIndex;

        // ---

        public CSM.State CurrentState => states[currentStateIndex].state;
        public CSM.TransitionGroup[] CurrentTransitionGroups => states[currentStateIndex].transitionGroups;

        public Character SourceCharacter { get; private set; }

        // ---

        public void Initialize(Character source)
        {
            SourceCharacter = source;

            foreach (var stateWithTransitions in states)
            {
                stateWithTransitions.state.Initialize(this);

                foreach (var transition in stateWithTransitions.transitionGroups)
                {
                    transition.InitializeTransitions(this);
                }
            }

            initialized = true;
        }

        public void ChangeStateTo(int stateIndex)
        {
            if (!initialized) throw new System.Exception("State Machine not initialized!");

            Debug.Log(stateIndex);

            CurrentState.ExitState();

            currentStateIndex = stateIndex;

            CurrentState.EnterState();
        }

        public void UpdateStateMachine(UpdateType updateType)
        {
            if (!initialized) throw new System.Exception("State Machine not initialized!");

            CurrentState.UpdateState(updateType);

            var activeTransitonGroup = CurrentTransitionGroups.FirstOrDefault(x => x.CanTransition(updateType));
            //var activeTransitonGroup = CurrentTransitionGroups.FirstOrDefault(x => x.IncludedTransitions.All(x => x.transition.CanTransition(updateType)));
            if (activeTransitonGroup != default(TransitionGroup))
            {
                ChangeStateTo(activeTransitonGroup.ToIndex);
                
                foreach (var transition in CurrentTransitionGroups.SelectMany(x => x.IncludedTransitions).Select(x => x.transition))
                {
                    transition.OnTransitionedInto();
                }
            }
        }

        // ---
        [System.Serializable]
        public struct StateWithTransitions
        {
            [SerializeReference, SubclassPicker] 
            public CSM.State state;
            public CSM.TransitionGroup[] transitionGroups;
        }

        // ---

        public enum UpdateType
        {
            Update,
            FixedUpdate,
            LateUpdate
        }
    }
}
