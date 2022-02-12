using RoR2;
using UnityEngine;
using System.Linq;
using RoR2.CharacterAI;

namespace UltimateCustomRun.Enemies
{
    public static class LunarWisp
    {
        public static CharacterBody body;
        public static void Nerf()
        {
            body = Resources.Load<CharacterBody>("prefabs/characterbodies/LunarWispBody").GetComponent<CharacterBody>();
            body.baseMoveSpeed = 14f;
        }
    }
}
