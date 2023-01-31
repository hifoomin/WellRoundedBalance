/*
using Mono.Cecil.Cil;
using MonoMod.Cil;
using WellRoundedBalance.Buffs;
using WellRoundedBalance.Eclipse;
using WellRoundedBalance.Projectiles;

namespace WellRoundedBalance.Elites
{
    internal class VoidtouchedNew : EliteBase
    {
        public static BuffDef voidCurse;
        public override string Name => ":: Elites :::::: Voidtouched";
        public static GameObject DeathBomb;
        public static DamageAPI.ModdedDamageType NullifierDeath = DamageAPI.ReserveDamageType();

        public override void Init()
        {
            var curse = Utils.Paths.Texture2D.texBuffPermanentCurse.Load<Texture2D>();
            voidCurse = ScriptableObject.CreateInstance<BuffDef>();
            voidCurse.isHidden = false;
            voidCurse.canStack = true;
            voidCurse.iconSprite = Sprite.Create(curse, new Rect(0, 0, (float)curse.width, (float)curse.height), new Vector2(0f, 0f));
            voidCurse.buffColor = new Color32(255, 75, 74, 255);
            voidCurse.isDebuff = true;

            voidCurse.name = "Void Curse";

            ContentAddition.AddBuffDef(voidCurse);

            DeathBomb = Utils.Paths.GameObject.NullifierDeathBombProjectile.Load<GameObject>().InstantiateClone("VoidtouchedExplosion", true);
            DeathBomb.GetComponent<ProjectileDamage>().damageType = DamageType.Silent;
            DeathBomb.GetComponent<ProjectileImpactExplosion>().blastDamageCoefficient = 0f;
            DamageAPI.ModdedDamageTypeHolderComponent com = DeathBomb.AddComponent<DamageAPI.ModdedDamageTypeHolderComponent>();
            com.Add(NullifierDeath);
            ContentAddition.AddProjectile(DeathBomb);
            base.Init();
        }

        public override void Hooks()
        {
            On.RoR2.CharacterMaster.OnBodyStart += CharacterMaster_OnBodyStart;
            On.RoR2.GlobalEventManager.OnHitEnemy += NullifierMoment;
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
            IL.RoR2.GlobalEventManager.OnHitEnemy += GlobalEventManager_OnHitEnemy;
            IL.RoR2.AffixVoidBehavior.FixedUpdate += AffixVoidBehavior_FixedUpdate;
            On.RoR2.GlobalEventManager.OnCharacterDeath += EnactNullifierMoment;
        }

        private void EnactNullifierMoment(On.RoR2.GlobalEventManager.orig_OnCharacterDeath orig, GlobalEventManager self, DamageReport report)
        {
            orig(self, report);
            if (NetworkServer.active && report.victimBody && report.victimBody.HasBuff(DLC1Content.Buffs.EliteVoid))
            {
                Debug.Log("firing projectile");
                FireProjectileInfo info = new();
                info.projectilePrefab = DeathBomb;
                info.position = report.damageInfo.position;
                info.damage = 0;
                info.rotation = Quaternion.identity;
                info.owner = report.victimBody.gameObject;
                ProjectileManager.instance.FireProjectile(info);
            }
        }

        private void NullifierMoment(On.RoR2.GlobalEventManager.orig_OnHitEnemy orig, GlobalEventManager self, DamageInfo info, GameObject victim)
        {
            orig(self, info, victim);
            if (NetworkServer.active && info.HasModdedDamageType(NullifierDeath))
            {
                CharacterBody victimBody = victim.GetComponent<CharacterBody>();
                if (victimBody && !victimBody.isPlayerControlled && !victimBody.bodyFlags.HasFlag(CharacterBody.BodyFlags.Mechanical))
                {
                    CharacterMaster victimMaster = victimBody.master;
                    victimMaster.teamIndex = TeamIndex.Void;
                    victimBody.teamComponent.teamIndex = TeamIndex.Void;
                    victimBody.inventory.SetEquipmentIndex(DLC1Content.Equipment.EliteVoidEquipment.equipmentIndex);
                    BaseAI ai = victimMaster.GetComponent<BaseAI>();
                    if (ai)
                    {
                        ai.enemyAttention = 0;
                        ai.ForceAcquireNearestEnemyIfNoCurrentEnemy();
                    }
                    EffectManager.SpawnEffect(Utils.Paths.GameObject.ElementalRingVoidImplodeEffect.Load<GameObject>(), new EffectData
                    {
                        origin = info.position
                    }, true);
                }
            }

            if (NetworkServer.active && info.attacker)
            {
                CharacterBody attackerBody = info.attacker.GetComponent<CharacterBody>();
                CharacterBody victimBody = info.attacker.GetComponent<CharacterBody>();

                if (attackerBody.HasBuff(DLC1Content.Buffs.EliteVoid))
                {
                    float takenDamagePercent = info.damage / victimBody.healthComponent.fullCombinedHealth * 100f;
                    int permanentDamage = Mathf.FloorToInt(takenDamagePercent * 40 / 100f);
                    for (int l = 0; l < permanentDamage; l++)
                    {
                        victimBody.AddBuff(RoR2Content.Buffs.PermanentCurse);
                    }
                }
            }
        }

        private void AffixVoidBehavior_FixedUpdate(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdsfld("RoR2.DLC1Content/Buffs", "BearVoidReady"),
                x => x.MatchCallOrCallvirt<CharacterBody>("AddBuff")))
            {
                c.Remove();
                c.Emit<Useless>(OpCodes.Ldsfld, nameof(Useless.voidtouchedSaferSpaces));
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Voidtouched Elite Safer Spaces hook");
            }
        }

        private void GlobalEventManager_OnHitEnemy(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdsfld("RoR2.DLC1Content/Buffs", "EliteVoid"),
                x => x.MatchCallOrCallvirt<CharacterBody>("HasBuff")))
            {
                c.Remove();
                c.Emit<Useless>(OpCodes.Ldsfld, nameof(Useless.uselessBuff));
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Voidtouched Elite Needletick hook");
            }

            // HOPOO GAMES ! !
            // why does it get the buff after I replace it lol
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender)
            {
                var curses = sender.GetBuffCount(voidCurse);
                if (curses > 0)
                {
                    args.baseCurseAdd += curses * 0.01f;
                }
            }
        }

        private void CharacterMaster_OnBodyStart(On.RoR2.CharacterMaster.orig_OnBodyStart orig, CharacterMaster self, CharacterBody body)
        {
            orig(self, body);
            if (body.HasBuff(DLC1Content.Buffs.EliteVoid))
            {
                if (body.GetComponent<VoidtouchedSpinnyManager>() == null)
                {
                    body.gameObject.AddComponent<VoidtouchedSpinnyManager>();
                }
            }
        }
    }

    public class VoidtouchedSpinnyManager : MonoBehaviour
    {
        public CharacterBody body;
        public int spinniesCount;
        public float spinnyHitFrequency = 1f / 10f;
        public float spinnyRadius = 3f;
        public float spinnyLength = 50f;
        public float rotationsPerSecond = 0.6f;
        public float timer;
        public GameObject spinnyLaser;
        public GameObject spinnyInstance;
        public bool shouldFireVFX;
        public Transform[] muzzles;

        private void Start()
        {
            body = GetComponent<CharacterBody>();
            spinnyLaser = VoidLaserProjectileVFX.laserPrefab;
            spinnyInstance = spinnyLaser;
            if (Run.instance.selectedDifficulty >= DifficultyIndex.Eclipse3 && Eclipse3.instance.isEnabled)
            {
                spinniesCount = 3;
            }
            else
            {
                spinniesCount = 2;
            }

            var muzzleString = "VoidtouchedSpinnyMuzzle";
            float angle = 360f / spinniesCount;
            for (int i = 0; i < spinniesCount; i++)
            {
                GameObject muzzle = new(muzzleString + i);
                muzzle.transform.parent = body.coreTransform;
                var evenRotation = Quaternion.AngleAxis(angle, Vector3.forward);
                muzzle.transform.eulerAngles = evenRotation.eulerAngles;
                muzzles[i] = muzzle.transform;
                muzzles[i].transform.eulerAngles = muzzle.transform.eulerAngles;
            }
        }

        private Ray GetSpinnyRay(int muzzleIndex)
        {
            var muzzle = muzzles[muzzleIndex];
            var rayDirection = muzzle.transform.rotation;
            var rayOrigin = muzzle.transform.position;
            return new Ray(rayOrigin, rayDirection.eulerAngles);
        }

        private void FixedUpdate()
        {
            timer += Time.fixedDeltaTime;
            if (timer >= spinnyHitFrequency)
            {
                for (int i = 0; i < spinniesCount; i++)
                {
                    BulletAttack ba = new()
                    {
                        origin = GetSpinnyRay(i).origin,
                        aimVector = GetSpinnyRay(i).direction,
                        // make this thing work with muzzles somehow?
                        minSpread = 0f,
                        maxSpread = 0f,
                        maxDistance = spinnyLength,
                        hitMask = LayerIndex.CommonMasks.bullet,
                        stopperMask = LayerIndex.world.intVal,
                        bulletCount = 1U,
                        radius = spinnyRadius,
                        smartCollision = false,
                        procCoefficient = 0f,
                        owner = gameObject,
                        weapon = gameObject,
                        damage = Run.instance ? Mathf.Sqrt(Run.instance.ambientLevel) : 0f,
                        damageColorIndex = DamageColorIndex.Default,
                        falloffModel = BulletAttack.FalloffModel.None,
                        force = 0f,
                        hitEffectPrefab = VoidLaserProjectileVFX.impactPrefab,
                        tracerEffectPrefab = VoidLaserProjectileVFX.laserPrefab,
                        isCrit = false,
                        HitEffectNormal = false,
                    };

                    if (Util.HasEffectiveAuthority(gameObject)) ba.Fire();

                    Fire();
                }

                timer = 0f;
            }
            if (timer <= 0)
            {
                shouldFireVFX = false;
                Unfire();
            }
        }

        private void Unfire()
        {
            GameObject.Destroy(spinnyInstance);
            spinnyInstance = null;
        }

        private void Fire()
        {
            if (shouldFireVFX) return;
            spinnyInstance = GameObject.Instantiate(spinnyLaser, body.transform);
            shouldFireVFX = true;
        }
    }

    public class VoidtouchedPermanentDamageController : MonoBehaviour, IOnTakeDamageServerReceiver
    {
        public HealthComponent hc;
        public CharacterBody body;

        public void Start()
        {
            hc = GetComponent<HealthComponent>();
            if (!hc)
            {
                Object.Destroy(this);
                return;
            }
            body = hc.body;
        }

        public void OnTakeDamageServer(DamageReport damageReport)
        {
            if (body && damageReport.attacker && damageReport.attackerBody && damageReport.attackerBody.HasBuff(DLC1Content.Buffs.EliteVoid))
            {
                switch (body.teamComponent.teamIndex)
                {
                    case TeamIndex.Player:
                        {
                            float takenDamagePercent = damageReport.damageDealt / hc.fullCombinedHealth * 100f;
                            int permanentDamage = Mathf.FloorToInt(takenDamagePercent * 40 / 100f);
                            for (int l = 0; l < permanentDamage; l++)
                            {
                                body.AddBuff(RoR2Content.Buffs.PermanentCurse);
                            }
                        }
                        break;
                }
            }
        }
    }
}
*/