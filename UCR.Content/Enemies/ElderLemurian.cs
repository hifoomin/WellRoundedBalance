using RoR2;
using RoR2.CharacterAI;
using System.Linq;
using UnityEngine;

namespace UltimateCustomRun.Enemies
{
    public class ElderLemurian : EnemyBase
    {
        public static bool AITweaks;
        public static bool SpeedTweaks;
        public static bool FireballTweaks;
        public override string Name => ":::: Enemies :: Elder Lemurian";

        public override void Init()
        {
            AITweaks = ConfigOption(false, "Make Elder Lemurian AI smarter?", "Vanilla is false.\nRecommended Value: true");
            SpeedTweaks = ConfigOption(false, "Make Elder Lemurian faster?", "Vanilla is false.\nRecommended Value: true");
            FireballTweaks = ConfigOption(false, "Adjust Fireballs amount, Damage etc?", "Vanilla is false.\nRecommended Value: true");
            base.Init();
        }

        public override void Hooks()
        {
            Buff();
        }

        public static void Buff()
        {
            var master = Resources.Load<CharacterMaster>("prefabs/charactermasters/LemurianBruiserMaster");
            var body = Resources.Load<CharacterBody>("prefabs/characterbodies/LemurianBruiserBody");

            if (AITweaks)
            {
                AISkillDriver ai = (from x in master.GetComponents<AISkillDriver>()
                                    where x.customName == "StopAndShoot"
                                    select x).First();
                ai.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            }

            if (SpeedTweaks)
            {
                body.baseMoveSpeed = 16f;
            }

            if (FireballTweaks)
            {
                On.EntityStates.LemurianBruiserMonster.FireMegaFireball.OnEnter += (orig, self) =>
                {
                    EntityStates.LemurianBruiserMonster.FireMegaFireball.projectileCount = 10;
                    EntityStates.LemurianBruiserMonster.FireMegaFireball.totalYawSpread = 90f;
                    EntityStates.LemurianBruiserMonster.FireMegaFireball.baseFireDuration = 0.5f;
                    EntityStates.LemurianBruiserMonster.FireMegaFireball.projectileSpeed = 50f;
                    EntityStates.LemurianBruiserMonster.FireMegaFireball.damageCoefficient = 1.75f;
                    orig(self);
                };
            }
        }
    }
}