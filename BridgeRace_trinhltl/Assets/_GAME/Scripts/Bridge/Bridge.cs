using System;
using System.Collections;
using System.Collections.Generic;
using _GAME.Scripts.Character;
using _GAME.Scripts.Floor;
using _GAME.Scripts.FSM.Bridge;
using UnityEngine;

public class Bridge : MonoBehaviour
{
    [SerializeField] private FloorGate floorGate;
    /*private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Bot"))
        {
            var character         = other.gameObject.GetComponent<Character>();
            var currentBridgeStep = this.gameObject.GetComponentInChildren<BridgeStep>();
            character.SetOnBridge(true);
            character.GetCurrentBridgeStep(currentBridgeStep);
            character.GetBridgeForward(this.gameObject.transform.forward);

            if (other.gameObject.CompareTag("Player"))
            {
                Debug.Log("Player is on the bridge");

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

    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Bot"))
        {
            Debug.LogWarning("Player is off the bridge");
            var character = other.gameObject.GetComponent<Character>();
            character.SetOnBridge(false);
            character.GetCurrentBridgeStep(null);
        }
    }*/
}