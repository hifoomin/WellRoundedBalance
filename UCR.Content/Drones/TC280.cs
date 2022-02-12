using RoR2;
using UnityEngine;

namespace UltimateCustomRun.Drones
{
    public static class TC280
    {
        public static void Buff()
        {
            var tc = Resources.Load<CharacterBody>("prefabs/characterbodies/MegaDroneBody");
            tc.baseRegen = 50f;
        }
    }
}
