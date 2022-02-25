using RoR2;
using UnityEngine;

namespace UltimateCustomRun.Drones
{
    public static class Emergency
    {
        public static void Buff()
        {
            var e = Resources.Load<CharacterBody>("prefabs/characterbodies/EmergencyDroneBody");
            e.baseRegen = 16.666f;
        }
    }
}