namespace _GAME.Scripts.FSM.Bridge
{
    using System;
    using System.Collections.Generic;
    using _GAME.Scripts.Character;
    using _GAME.Scripts.Floor;
    using UnityEngine;

    public class GateTargetManager : MonoBehaviour
    {
        public static GateTargetManager Instance { get; private set; }

        private readonly Dictionary<int, BotController> _reservedGates = new Dictionary<int, BotController>();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(this);
            }
        }

        public bool IsGateAvailable(int gateIndex)
        {
            return !this._reservedGates.ContainsKey(gateIndex);
        }

        public bool ReserveGate(int gateIndex, BotController bot)
        {
            if (IsGateAvailable(gateIndex))
            {
                this._reservedGates[gateIndex] = bot;
                return true;
            }
            return false;
        }

        public void ReleaseGate(int gateIndex, BotController bot)
        {
            if (this._reservedGates.TryGetValue(gateIndex, out var reservedBot) && reservedBot == bot)
            {
                this._reservedGates.Remove(gateIndex);
            }
        }

        public int NearestAvailableGate(BotController bot, List<FloorGate> gates)
        {
            var bestGateIndex   = -1;
            var closestDistance = float.MaxValue;

            for (var i = 0; i < gates.Count; i++)
            {
                if (!IsGateAvailable(i))
                {
                    continue;
                }

                var distance = Vector3.Distance(bot.transform.position, gates[i].transform.position);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    bestGateIndex   = i;
                }
            }
            return bestGateIndex;
        }

        public bool IsGateReservedForBot(int gateIndex, BotController bot)
        {
            return _reservedGates.TryGetValue(gateIndex, out var reservedBot) && reservedBot == bot;
        }

        private void Update()
        {
            this.DebugReservedGates();
        }

        public void DebugReservedGates()
        {
            if(_reservedGates.Count <= 0) return;
            foreach (var kvp in _reservedGates)
            {
                Debug.Log($"Gate {kvp.Key} is reserved for bot {kvp.Value.name}");
            }
        }
    }
}