using System;
using System.Collections;
using System.Collections.Generic;
using _GAME.Scripts.Character;
using _GAME.Scripts.Level;
using UnityEngine;

public class LevelFinishZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Bot"))
        {
            var character = other.GetComponent<Character>();
            GameManager.Instance.CharacterReachedFinish(character);
        }
    }

}