using RoR2;
using RoR2.CharacterAI;
using System.Linq;
using UnityEngine;

namespace UltimateCustomRun.Enemies
{
    public class StoneGolem : EnemyBase
    {
        public static float LaserDamage;
        public static float LaserDuration;
        public static bool Tweaks;
        public override string Name => ":::: Enemies :: Stone Golem";

        public override void Init()
        {
            LaserDamage = ConfigOption(2.5f, "Laser Damage", "Vanilla is 3.125 with Damage Tweaks and 2.5 without.\nRecommended Value: 2.5");
            LaserDuration = ConfigOption(3f, "Laser Charge Up Duration", "Vanilla is 3.\nRecommended Value: 1.8");
            Tweaks = ConfigOption(false, "Enable Damage Tweaks and AI Tweaks?", "Vanilla is false.\nRecommended Value: true");
            base.Init();
        }

        public override void Hooks()
        {
            Berf();
        }

        public static void Berf()
        {
            var master = Resources.Load<CharacterMaster>("prefabs/charactermasters/GolemMaster").GetComponent<CharacterMaster>();
            var body = Resources.Load<CharacterBody>("prefabs/characterbodies/GolemBody").GetComponent<CharacterBody>();
            var bodybase = Resources.Load<CharacterBody>("prefabs/characterbodies/GolemBody");
            if (Tweaks)
            {
                body.baseDamage = 16f;

                AISkillDriver ai = (from x in master.GetComponents<AISkillDriver>()
                                    where x.skillSlot == SkillSlot.Secondary
                                    select x).First();
                ai.selectionRequiresAimTarget = true;
                ai.activationRequiresAimTargetLoS = true;
                ai.activationRequiresAimConfirmation = true;

                On.EntityStates.GolemMonster.ClapState.OnEnter += (orig, self) =>
                {
                    EntityStates.GolemMonster.ClapState.damageCoefficient = 3.5f;
                    EntityStates.GolemMonster.ClapState.radius = 6f;
                    EntityStates.GolemMonster.ClapState.duration = 1.7f;
                    orig(self);
                };
            }

            On.EntityStates.GolemMonster.ChargeLaser.OnEnter += (orig, self) =>
            {
                EntityStates.GolemMonster.ChargeLaser.baseDuration = LaserDuration;
                orig(self);
            };

            On.EntityStates.GolemMonster.FireLaser.OnEnter += (orig, self) =>
            {
                EntityStates.GolemMonster.FireLaser.damageCoefficient = LaserDamage;
                // 3.125 to match vanilla
                // 2.5 for 20% nerf
                orig(self);
            };
        }
    }
}