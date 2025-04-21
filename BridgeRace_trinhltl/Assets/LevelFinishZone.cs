using System;
using System.Collections;
using System.Collections.Generic;
using _GAME.Scripts.Character;
using UnityEngine;

public class LevelFinishZone : MonoBehaviour
{
    public List<Character> winners = new List<Character>();

    private int listMaxLength = 3;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Bot"))
        {
            if (other.CompareTag("Player"))
            {
                //TODO : Finish the level right away
            }

            var character = other.GetComponent<Character>();
            AddWinner(character);
        }
    }

    private void AddWinner(Character character)
    {
        if (winners.Count < listMaxLength)
        {
            winners.Add(character);
        }
        else
        {
            Debug.Log("Winner list is full.");
        }
    }
}