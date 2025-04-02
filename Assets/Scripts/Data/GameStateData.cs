using System.Collections.Generic;
using System;
using UnityEngine;

namespace EcoGuardians.Data
{
    [Serializable]
    public class GameStateData
    {
        public Dictionary<string, string> MarketStatus { get; set; } = new Dictionary<string, string>(); // e.g., "area1": "cleaned" or "polluted"
        public List<WasteItem> WasteItems { get; set; } = new List<WasteItem>(); // Tracks all waste in the game
        public float LevelTimer { get; set; } = 120f; // Default timer, adjustable

    }
}