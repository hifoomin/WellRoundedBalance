using WellRoundedBalance.Buffs;
using WellRoundedBalance.Mechanics.Health;

namespace WellRoundedBalance.Elites
{
    internal class Perfected : EliteBase<Perfected>
    {
        [ConfigField("Projectile Damage", "Decimal.", 1f)]
        public static float projectileDamage;

        [ConfigField("Projectile Fire Interval", "", 0.5f)]
        public static float projectileFireInterval;

        [ConfigField("Spawn Lunar Coin On Death", "", true)]
        public static bool spawnLunarCoin;

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
            IL.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;

            if (spawnLunarCoin)
                CharacterBody.onBodyInventoryChangedGlobal += CharacterBody_onBodyInventoryChangedGlobal;

            Changes();
        }

        [SystemInitializer(typeof(EliteCatalog))]
        public static void ChangeTier()
        {
            if (spawnOnLoop)
            {
                var perfectedEliteTierDef = EliteAPI.VanillaEliteTiers.Where(x => x.eliteTypes.Contains(RoR2Content.Elites.Lunar)).First();
                perfectedEliteTierDef.isAvailable = (SpawnCard.EliteRules rules) => Run.instance.loopClearCount > 0 && (rules == SpawnCard.EliteRules.Default || rules == SpawnCard.EliteRules.Lunar);
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

        private void HealthComponent_TakeDamage(ILContext il)
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
                c.Next.Operand = projectileFireInterval;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Perfected Projectile Interval hook");
            }
        }

        private void Changes()
        {
            var projectile = Utils.Paths.GameObject.LunarMissileProjectile.Load<GameObject>();

            var projectileSimple = projectile.GetComponent<ProjectileSimple>();
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
            PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(RoR2Content.MiscPickups.LunarCoin.miscPickupIndex), body.corePosition, Vector3.up * 2f * body.radius);
        }
    }
}