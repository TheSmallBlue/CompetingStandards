using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CompetingStandards
{
    // -- State Machine --
    
    public partial class Character : MonoBehaviour
    {
        [Header("State Machine")]
        [SerializeField] CSM.StateMachine stateMachine;

        // ---

        private void Awake() 
        {
            stateMachine.Initialize(this);
        }

        void Update()
        {
            stateMachine.UpdateStateMachine(CSM.StateMachine.UpdateType.Update);
        }
    }
}
