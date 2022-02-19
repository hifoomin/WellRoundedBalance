using RoR2;
using UnityEngine;
using System.Linq;
using RoR2.CharacterAI;

namespace UltimateCustomRun.Enemies
{
    public class ElderLemurian : EnemyBase
    {
        public static bool aitw;
        public static bool spdtw;
        public static bool fbtw;
        public override string Name => ":::: Enemies :: Elder Lemurian";

        public override void Init()
        {
            aitw = ConfigOption(false, "Make Elder Lemurian AI smarter?", "Vanilla is false. Recommended Value: true");
            spdtw = ConfigOption(false, "Make Elder Lemurian faster?", "Vanilla is false. Recommended Value: true");
            fbtw = ConfigOption(false, "Adjust Fireballs amount, damage etc?", "Vanilla is false. Recommended Value: true");
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

            if (aitw)
            {
                AISkillDriver ai = (from x in master.GetComponents<AISkillDriver>()
                                    where x.customName == "StopAndShoot"
                                    select x).First();
                ai.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            }

            if (spdtw)
            {
                body.baseMoveSpeed = 16f;
            }

            if (fbtw)
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
