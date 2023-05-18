using System.Collections;
using WellRoundedBalance.Buffs;
using WellRoundedBalance.Gamemodes.Eclipse;

namespace WellRoundedBalance.Elites
{
    internal class Blazing : EliteBase<Blazing>
    {
        public override string Name => ":: Elites : Blazing";

        [ConfigField("Death Pool Projectile Count", "", 4)]
        public static int deathPoolProjectileCount;

        [ConfigField("Death Pool Projectile Count E3+", "Only applies if you have Eclipse Changes enabled.", 6)]
        public static int deathPoolProjectileCountE3;

        [ConfigField("Fire Pool Damage Per Second", "Decimal.", 1.5f)]
        public static float firePoolDamagePerSecond;

        public static BuffDef lessDamage;

        public override void Init()
        {
            base.Init();
            lessDamage = ScriptableObject.CreateInstance<BuffDef>();
            lessDamage.isHidden = true;
            lessDamage.isDebuff = false;
            lessDamage.canStack = false;
            lessDamage.isCooldown = false;

            ContentAddition.AddBuffDef(lessDamage);
        }

        public override void Hooks()
        {
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
            IL.RoR2.CharacterBody.UpdateFireTrail += CharacterBody_UpdateFireTrail1;
            CharacterBody.onBodyInventoryChangedGlobal += CharacterBody_onBodyInventoryChangedGlobal;
            RoR2.GlobalEventManager.onCharacterDeathGlobal += GlobalEventManager_onCharacterDeathGlobal;
        }

        private void GlobalEventManager_onCharacterDeathGlobal(DamageReport report)
        {
            var victim = report.victim;
            if (!victim)
            {
                return;
            }
            var blazingController = victim.GetComponent<BlazingController>();
            if (!blazingController)
            {
                return;
            }
            blazingController.canFire = true;
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender && sender.HasBuff(lessDamage))
            {
                args.damageMultAdd -= 0.25f;
            }
        }

        private void CharacterBody_onBodyInventoryChangedGlobal(CharacterBody characterBody)
        {
            var sfp = characterBody.GetComponent<BlazingController>();
            if (characterBody.HasBuff(RoR2Content.Buffs.AffixRed))
            {
                if (sfp == null)
                {
                    characterBody.gameObject.AddComponent<BlazingController>();
                    characterBody.AddBuff(lessDamage);
                }
            }
            else if (sfp != null)
            {
                characterBody.gameObject.RemoveComponent<BlazingController>();
                characterBody.RemoveBuff(lessDamage);
            }
        }

        private void CharacterBody_UpdateFireTrail1(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdsfld("RoR2.RoR2Content/Buffs", "AffixRed")))
            {
                c.Remove();
                c.Emit<Useless>(OpCodes.Ldsfld, nameof(Useless.uselessBuff));
            }
            else
            {
                Logger.LogError("Failed to apply Blazing Elite Fire Trail Deletion hook");
            }
        }
    }

    public class BlazingController : MonoBehaviour
    {
        public CharacterBody body;
        public HealthComponent hc;
        public GameObject deathProjectile = Projectiles.Molotov.singlePrefab;
        public GameObject passiveProjectile = Projectiles.MolotovBig.singlePrefab;

        public float timer;
        public int passiveProjectileCount = 2;
        public float passiveProjectileInterval = 12f;
        public float passiveProjectileAngle;
        public float passiveDelayBetweenProjectiles = 0.5f;

        public float deathTimer;
        public float delay = 1f;
        public int deathProjectileCount = 4;
        public float deathProjectileAngle;
        public float deathDelayBetweenProjectiles = 0.2f;
        public bool canFire = false;

        public void Start()
        {
            body = GetComponent<CharacterBody>();
            hc = body.healthComponent;
            deathProjectileCount = Eclipse3.CheckEclipse() ? Blazing.deathPoolProjectileCountE3 : Blazing.deathPoolProjectileCount;
            deathProjectileAngle = 360f / deathProjectileCount;
            passiveProjectileAngle = 360f / passiveProjectileCount;
        }

        public void FixedUpdate()
        {
            if (canFire)
            {
                deathTimer += Time.fixedDeltaTime;
                if (deathTimer >= delay)
                {
                    deathTimer = 0f;
                    StartCoroutine(FireDeathProjectiles());
                }
            }
            timer += Time.fixedDeltaTime;
            if (timer >= passiveProjectileInterval)
            {
                timer = 0f;
                StartCoroutine(FireProjectiles());
            }
        }

        public IEnumerator FireProjectiles()
        {
            var position = body.corePosition + Vector3.up * 10f;
            var rotation = Quaternion.identity;

            for (int i = 0; i < passiveProjectileCount; i++)
            {
                if (Util.HasEffectiveAuthority(gameObject))
                {
                    FireProjectileInfo info = new()
                    {
                        owner = body.gameObject,
                        damage = body.damage * Blazing.firePoolDamagePerSecond * 0.2f,
                        crit = false,
                        position = position,
                        rotation = rotation,
                        projectilePrefab = passiveProjectile,
                        damageTypeOverride = DamageType.IgniteOnHit
                    };
                    ProjectileManager.instance.FireProjectile(info);
                }

                rotation *= Quaternion.Euler(0f, passiveProjectileAngle, 0f);
                yield return new WaitForSeconds(passiveDelayBetweenProjectiles);
            }
        }

        public IEnumerator FireDeathProjectiles()
        {
            var position = body.corePosition + Vector3.up * 6f;
            var rotation = Quaternion.identity;

            for (int i = 0; i < deathProjectileCount; i++)
            {
                if (Util.HasEffectiveAuthority(gameObject))
                {
                    FireProjectileInfo info = new()
                    {
                        owner = body.gameObject,
                        damage = body.damage * Blazing.firePoolDamagePerSecond * 0.2f,
                        crit = false,
                        position = position,
                        rotation = rotation,
                        projectilePrefab = deathProjectile,
                        damageTypeOverride = DamageType.IgniteOnHit
                    };
                    ProjectileManager.instance.FireProjectile(info);
                }

                rotation *= Quaternion.Euler(0f, deathProjectileAngle, 0f);
                yield return new WaitForSeconds(deathDelayBetweenProjectiles);
            }

            if (NetworkServer.active)
            {
                Destroy(this);
            }
        }
    }
}