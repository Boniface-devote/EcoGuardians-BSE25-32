using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Playables;

namespace EcoGuardians.Data
{
    [Serializable]
    public class GameData : IGameData
    {
        public PlayerData Player { get; private set; } = new PlayerData(); // Player-specific progress
        public List<NPCData> NPCs { get; private set; } = new List<NPCData>(); // NPC progression tracking
        public GameStateData GameState { get; private set; } = new GameStateData(); // Stores overall game state

    }
}