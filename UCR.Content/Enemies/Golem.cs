using RoR2;
using UnityEngine;
using System.Linq;
using RoR2.CharacterAI;

namespace UltimateCustomRun.Enemies
{
    public static class Golem
    {
        public static CharacterMaster master;
        public static CharacterBody body;
        public static CharacterBody bodybase;
        public static void Berf()
        {
            master = Resources.Load<CharacterMaster>("prefabs/charactermasters/GolemMaster").GetComponent<CharacterMaster>();
            body = Resources.Load<CharacterBody>("prefabs/characterbodies/GolemBody").GetComponent<CharacterBody>();
            body.baseDamage = 16f;
            bodybase = Resources.Load<CharacterBody>("prefabs/characterbodies/GolemBody");

            AISkillDriver ai = (from x in master.GetComponents<AISkillDriver>()
                                where x.skillSlot == SkillSlot.Secondary
                                select x).First();
            ai.selectionRequiresAimTarget = true;
            ai.activationRequiresAimTargetLoS = true;
            ai.activationRequiresAimConfirmation = true;

            On.EntityStates.GolemMonster.ChargeLaser.OnEnter += (orig, self) =>
            {
                EntityStates.GolemMonster.ChargeLaser.baseDuration = 1.8f;
                orig(self);
            };

            On.EntityStates.GolemMonster.FireLaser.OnEnter += (orig, self) =>
            {
                EntityStates.GolemMonster.FireLaser.blastRadius = 3.5f;
                EntityStates.GolemMonster.FireLaser.damageCoefficient = 2.5f;
                // 3.125 to match vanilla
                // 2.5 for 20% nerf
                orig(self);
            };
            On.EntityStates.GolemMonster.ClapState.OnEnter += (orig, self) =>
            {
                EntityStates.GolemMonster.ClapState.damageCoefficient = 3.5f;
                EntityStates.GolemMonster.ClapState.radius = 6f;
                EntityStates.GolemMonster.ClapState.duration = 1.7f;
                orig(self);
            };
        }
    }
}
