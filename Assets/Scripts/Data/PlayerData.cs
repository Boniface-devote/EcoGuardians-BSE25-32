using System.Collections.Generic;
using System;
using UnityEngine;

namespace EcoGuardians.Data
{
    [Serializable]
    public class PlayerData
    {
        public string PlayerID { get; set; } = Guid.NewGuid().ToString(); // Unique ID for tracking progress
        public string Username { get; set; } = "EcoWarrior"; // Default username for the player
        public int CurrentScore { get; set; } = 0; // Player’s score
        public int LevelProgress { get; set; } = 0; // Current game level
        public List<string> Achievements { get; set; } = new List<string>(); // List of achievements earned

    }
}