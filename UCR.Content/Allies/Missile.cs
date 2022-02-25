using RoR2;
using UnityEngine;

namespace UltimateCustomRun.Drones
{
    public static class Missile
    {
        public static void Buff()
        {
            var m = Resources.Load<CharacterBody>("prefabs/characterbodies/MissileDroneBody");
            m.baseRegen = 12.5f;
        }
    }
}