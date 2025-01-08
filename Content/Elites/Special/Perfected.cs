using System;
using WellRoundedBalance.Buffs;
using WellRoundedBalance.Gamemodes.Eclipse;
using WellRoundedBalance.Mechanics.Health;

namespace WellRoundedBalance.Elites.Special
{
    internal class Perfected : EliteBase<Perfected>
    {
        [ConfigField("Projectile Damage", "Decimal.", 1f)]
        public static float projectileDamage;

        [ConfigField("Projectile Fire Interval", "", 9f)]
        public static float projectileFireInterval;

        [ConfigField("Projectile Fire Interval E3+", "Only applies if you have Eclipse Changes enabled.", 7.5f)]
        public static float projectileFireIntervalE3;

        [ConfigField("Delay Between Projectiles", "", 0.5f)]
        public static float delayBetweenProjectiles;

        [ConfigField("Death Lunar Coin Drop Chance", "", 20f)]
        public static float lunarCoinDropChance;

        [ConfigField("Spawn On Loop", "", true)]
        public static bool spawnOnLoop;

        public override string Name => ":: Elites :: Perfected";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.CharacterBody.UpdateAffixLunar += CharacterBody_UpdateAffixLunar;
            IL.RoR2.HealthComponent.TakeDamageProcess += HealthComponent_TakeDamageProcess;

            // CharacterBody.onBodyInventoryChangedGlobal += CharacterBody_onBodyInventoryChangedGlobal;
            // in favor of the bottom
            On.RoR2.CharacterBody.AddBuff_BuffIndex += CharacterBody_AddBuff_BuffIndex;
            On.RoR2.CharacterBody.RemoveBuff_BuffIndex += CharacterBody_RemoveBuff_BuffIndex;
            Changes();
        }

        private void CharacterBody_RemoveBuff_BuffIndex(On.RoR2.CharacterBody.orig_RemoveBuff_BuffIndex orig, CharacterBody self, BuffIndex buffType)
        {
            orig(self, buffType);
            if (buffType == RoR2Content.Buffs.AffixLunar.buffIndex)
            {
                self.gameObject.RemoveComponent<PerfectedController>();
            }
        }

        private void CharacterBody_AddBuff_BuffIndex(On.RoR2.CharacterBody.orig_AddBuff_BuffIndex orig, CharacterBody self, BuffIndex buffType)
        {
            orig(self, buffType);
            if (buffType == RoR2Content.Buffs.AffixLunar.buffIndex)
            {
                if (self.GetComponent<PerfectedController>() == null)
                {
                    self.gameObject.AddComponent<PerfectedController>();
                }
            }
        }

        [SystemInitializer(typeof(EliteCatalog))]
        public static void ChangeTier()
        {
            if (spawnOnLoop)
            {
                var perfectedEliteTierDef = EliteAPI.VanillaEliteTiers.Where(x => x.eliteTypes.Contains(RoR2Content.Elites.Lunar)).First();
                perfectedEliteTierDef.isAvailable = (rules) => Run.instance.loopClearCount > 0 && (rules == SpawnCard.EliteRules.Default || rules == SpawnCard.EliteRules.Lunar);
            }
        }

        private void CharacterBody_onBodyInventoryChangedGlobal(CharacterBody characterBody)
        {
            var sfp = characterBody.GetComponent<PerfectedController>();
            if (characterBody.HasBuff(RoR2Content.Buffs.AffixLunar))
            {
                if (sfp == null)
                {
                    characterBody.gameObject.AddComponent<PerfectedController>();
                }
            }
            else if (sfp != null)
            {
                characterBody.gameObject.RemoveComponent<PerfectedController>();
            }
        }

        private void HealthComponent_TakeDamageProcess(ILContext il)
        {
            ILCursor c = new(il);
            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdsfld("RoR2.RoR2Content/Buffs", "AffixLunar")))
            {
                c.Remove();
                c.Emit<Useless>(OpCodes.Ldsfld, nameof(Useless.uselessBuff));
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Perfected Cripple hook");
            }
        }

        private void CharacterBody_UpdateAffixLunar(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcR4(0.3f)))
            {
                c.Next.Operand = projectileDamage;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Perfected Projectile Damage hook");
            }

            c.Index = 0;

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcR4(0.1f)))
            {
                c.Next.Operand = delayBetweenProjectiles;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Perfected Projectile Interval hook");
            }

            c.Index = 0;

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcR4(10f)))
            {
                c.Index++;
                c.EmitDelegate<Func<float, float>>((orig) =>
                {
                    orig = Eclipse3.CheckEclipse() ? projectileFireIntervalE3 : projectileFireInterval;
                    return orig;
                });
            }
        }

        private void Changes()
        {
            var projectile = Utils.Paths.GameObject.LunarMissileProjectile.Load<GameObject>();

            var projectileSimple = projectile.GetComponent<ProjectileSimple>();
            projectileSimple.enableVelocityOverLifetime = true;
            projectileSimple.velocityOverLifetime = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.1f, 0f), new Keyframe(0.1f + Mathf.Epsilon, 1f), new Keyframe(1f, 1f));
            projectileSimple.desiredForwardSpeed = 120f;
        }
    }

    public class PerfectedController : MonoBehaviour
    {
        public CharacterBody body;
        public HealthComponent hc;
        public float timer;
        public float interval = 0.5f;

        public void Start()
        {
            body = GetComponent<CharacterBody>();
            hc = body.healthComponent;
        }

        public void FixedUpdate()
        {
            timer += Time.fixedDeltaTime;
            if (timer >= interval)
            {
                if (!hc.alive)
                {
                    Destroy(this);
                }
                timer = 0f;
            }
        }

        public void OnDestroy()
        {
            if (Util.CheckRoll(Perfected.lunarCoinDropChance))
                PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(RoR2Content.MiscPickups.LunarCoin.miscPickupIndex), body.corePosition, Vector3.up * 2f * body.radius);
        }
    }
}