using System;
using UnityEngine;

namespace EcoGuardians.Data
{
    [Serializable]
    public class WasteItem
    {
        public string WasteID { get; set; } // Unique identifier for the waste item
        public string Category { get; set; }  // Type of waste (e.g., Recyclable, Landfill, Organic, Hazardous)
        public bool Collected { get; set; } = false; // Flag to check if the waste is collected
    }
}
