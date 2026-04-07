using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CompetingStandards
{
    // -- Main --
    public partial class Character : MonoBehaviour
    {
        private void Awake() 
        {
            // Initialize the various modules of our character.
            SetUpCharacterMovement();
            SetUpCharacterSM();
        }

        void Update()
        {
            UpdateCharacterVisuals();
            UpdateCharacterSM();
        }

        void FixedUpdate()
        {
            FixedUpdateCharacterSM();
        }
    }
}