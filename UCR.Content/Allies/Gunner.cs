using RoR2;
using UnityEngine;

namespace UltimateCustomRun.Drones
{
    public static class Gunner
    {
        public static void Buff()
        {
            var g = Resources.Load<CharacterBody>("prefabs/characterbodies/Drone1Body");
            g.baseRegen = 8.333f;
        }
    }
}