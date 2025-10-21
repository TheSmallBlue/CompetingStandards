using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CompetingStandards.CSM
{
    [System.Serializable]
    public abstract class State
    {
        public virtual void OnStateEnter() { }
        public virtual void OnStateExit() { }

        public virtual void OnStateUpdate() { }
        public virtual void OnStateFixedUpdate() { }

    }
}
