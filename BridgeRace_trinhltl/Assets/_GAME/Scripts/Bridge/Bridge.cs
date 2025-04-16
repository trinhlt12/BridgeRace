using System;
using System.Collections;
using System.Collections.Generic;
using _GAME.Scripts.Character;
using _GAME.Scripts.Floor;
using UnityEngine;

public class Bridge : MonoBehaviour
{
    [SerializeField] private FloorGate floorGate;
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            var bots = FloorManager.Instance.allBots;
            foreach (var bot in bots)
            {
                if (bot.GetCurrentTargetGateIndex() == FloorManager.Instance.GetFloorGateIndex(this.floorGate))
                {
                    bot.TargetGateOccupied();
                }
            }
        }
    }
}