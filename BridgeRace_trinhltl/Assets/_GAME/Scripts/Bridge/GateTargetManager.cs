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

        private readonly Dictionary<GateIdentifier, BotController> _reservedGates = new Dictionary<GateIdentifier, BotController>();

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

        public bool IsGateAvailable(Floor floor, int gateIndex)
        {
            return !this._reservedGates.ContainsKey(new GateIdentifier(floor, gateIndex));
        }

        public bool ReserveGate(Floor floor, int gateIndex, BotController bot)
        {
            var gateId = new GateIdentifier(floor, gateIndex);
            if (!this._reservedGates.ContainsKey(gateId))
            {
                this._reservedGates[gateId] = bot;
                return true;
            }
            return false;
        }

        public void ReleaseGate(Floor floor, int gateIndex, BotController bot)
        {
            var gateId = new GateIdentifier(floor, gateIndex);

            if (this._reservedGates.TryGetValue(gateId, out var reservedBot) && reservedBot == bot)
            {
                this._reservedGates.Remove(gateId);
            }
        }

        public int NearestAvailableGate(BotController bot, Floor floor)
        {
            var bestGateIndex   = -1;
            var closestDistance = float.MaxValue;

            if (floor == null || floor.floorGate == null) return -1;

            for (var i = 0; i < floor.floorGate.Count; i++)
            {
                if (!IsGateAvailable(floor, i))
                {
                    continue;
                }

                var distance = Vector3.Distance(bot.transform.position, floor.floorGate[i].transform.position);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    bestGateIndex   = i;
                }
            }
            return bestGateIndex;
        }

        public bool IsGateReservedForBot(Floor floor, int gateIndex, BotController bot)
        {
            var gateId = new GateIdentifier(floor, gateIndex);
            return _reservedGates.TryGetValue(gateId, out var reservedBot) && reservedBot == bot;
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
                Debug.Log($"Gate on floor {kvp.Key.floor.name}, index {kvp.Key.gateIndex} is reserved for bot {kvp.Value.name}");
            }
        }
    }
}