using System;
using System.Collections;
using System.Collections.Generic;
using _GAME.Scripts.Character;
using UnityEngine;

public class LevelFinishZone : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Bot"))
        {
            if (other.CompareTag("Player"))
            {
                //TODO : Finish the level right away
            }

            var character = other.GetComponent<Character>();
        }
    }

}