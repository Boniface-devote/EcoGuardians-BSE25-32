using System.Collections.Generic;
using UnityEngine;

// Interface for all storable game data
namespace EcoGuardians.Data
{
    public interface IGameData
    {
        PlayerData Player { get; }
        List<NPCData> NPCs { get; }
        GameStateData GameState { get; }
    }
}