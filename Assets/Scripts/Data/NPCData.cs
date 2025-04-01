using System;

namespace EcoGuardians.Data
{

    [Serializable]
    public class NPCData
    {
        public string NPCID { get; set; } // Unique identifier for the NPC
        public string NPCType { get; set; }   // "vendor" or "shopper"
        public int ResistanceLevel { get; set; } // How many times they must be educated before adopting waste-sorting habits
        public string EducationStatus { get; set; } = "uneducated"; // Educated or uneducated // e.g., "uneducated", "educated"

    }
}