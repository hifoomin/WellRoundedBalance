using RoR2;
using UnityEngine;
using System.Linq;
using RoR2.CharacterAI;

namespace UltimateCustomRun.Enemies
{
    public static class GreaterWisp
    {
        public static CharacterMaster master;
        public static void Buff()
        {
            master = Resources.Load<CharacterMaster>("prefabs/charactermasters/GreaterWispMaster").GetComponent<CharacterMaster>();

            AISkillDriver ai = (from x in master.GetComponents<AISkillDriver>()
                                where x.minDistance == 15f
                                select x).First();
            ai.movementType = AISkillDriver.MovementType.StrafeMovetarget;
            ai.maxDistance = 100f;

            On.EntityStates.GreaterWispMonster.ChargeCannons.OnEnter += (orig, self) =>
            {
                self.baseDuration = 1.25f;
                orig(self);
            };
        }
    }
}
