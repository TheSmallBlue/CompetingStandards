using System.Collections;
using System.Collections.Generic;
using CompetingStandards;
using UnityEngine;

public class DashPad : MonoBehaviour
{
    [SerializeField] float speed = 500;

    // ---

    void OnTriggerEnter(Collider other)
    {
        var character = other.GetComponentInParent<Character>();
        if (character == null) return;

        character.RB.velocity = transform.forward * speed;
    }
}
