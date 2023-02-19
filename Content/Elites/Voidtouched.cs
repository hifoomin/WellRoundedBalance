using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2.Audio;
using System.Collections;
using WellRoundedBalance.Buffs;
using WellRoundedBalance.Eclipse;

namespace WellRoundedBalance.Elites
{
    internal class Voidtouched : EliteBase
    {
        public static BuffDef useless;
        public static BuffDef voidCurse;
        public static GameObject DeathBomb;
        public static DamageAPI.ModdedDamageType NullifierDeath = DamageAPI.ReserveDamageType();
        public override string Name => ":: Elites :::::: Voidtouched";

        public override void Init()
        {
            useless = ScriptableObject.CreateInstance<BuffDef>();
            useless.name = "Voidtouched Deletion";
            useless.isHidden = true;

            var curse = Utils.Paths.Texture2D.texBuffPermanentCurse.Load<Texture2D>();
            voidCurse = ScriptableObject.CreateInstance<BuffDef>();
            voidCurse.isHidden = false;
            voidCurse.canStack = true;
            voidCurse.iconSprite = Sprite.Create(curse, new Rect(0, 0, (float)curse.width, (float)curse.height), new Vector2(0f, 0f));
            voidCurse.buffColor = new Color32(255, 75, 74, 255);
            voidCurse.isDebuff = true;

            voidCurse.name = "Void Curse";

            ContentAddition.AddBuffDef(useless);
            ContentAddition.AddBuffDef(voidCurse);

            DeathBomb = Utils.Paths.GameObject.NullifierDeathBombProjectile.Load<GameObject>().InstantiateClone("VoidtouchedExplosion", true);
            DeathBomb.GetComponent<ProjectileDamage>().damageType = DamageType.Silent;
            var projectileImpactExplosion = DeathBomb.GetComponent<ProjectileImpactExplosion>();

            projectileImpactExplosion.blastDamageCoefficient = 0f;

            var impactEffect = PrefabAPI.InstantiateClone(Utils.Paths.GameObject.NullifierDeathBombExplosion.Load<GameObject>(), "VoidtouchedExplosionImpactEffect", false);

            var effectComponent = impactEffect.GetComponent<EffectComponent>();
            effectComponent.soundName = "Play_voidRaid_fog_explode";

            ContentAddition.AddEffect(impactEffect);

            projectileImpactExplosion.impactEffect = impactEffect;

            var projectileController = DeathBomb.GetComponent<ProjectileController>();
            projectileController.flightSoundLoop = Utils.Paths.LoopSoundDef.lsdVoidCampCenter.Load<LoopSoundDef>();

            DamageAPI.ModdedDamageTypeHolderComponent com = DeathBomb.AddComponent<DamageAPI.ModdedDamageTypeHolderComponent>();
            com.Add(NullifierDeath);
            ContentAddition.AddProjectile(DeathBomb);

            base.Init();
        }

        public override void Hooks()
        {
            On.RoR2.HealthComponent.Awake += HealthComponent_Awake;
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
            IL.RoR2.GlobalEventManager.OnHitEnemy += GlobalEventManager_OnHitEnemy;
            IL.RoR2.AffixVoidBehavior.FixedUpdate += AffixVoidBehavior_FixedUpdate;
            On.RoR2.GlobalEventManager.OnHitEnemy += GlobalEventManager_OnHitEnemy1;
            On.RoR2.GlobalEventManager.OnCharacterDeath += GlobalEventManager_OnCharacterDeath;
            RoR2.CharacterBody.onBodyInventoryChangedGlobal += CharacterBody_onBodyInventoryChangedGlobal;
        }

        private void CharacterBody_onBodyInventoryChangedGlobal(CharacterBody characterBody)
        {
            var vbm = characterBody.GetComponent<VoidtouchedBallsManager>();
            if (characterBody.HasBuff(DLC1Content.Buffs.EliteVoid))
            {
                if (vbm == null)
                {
                    characterBody.gameObject.AddComponent<VoidtouchedBallsManager>();
                }
            }
            else if (vbm != null)
            {
                characterBody.gameObject.RemoveComponent<VoidtouchedBallsManager>();
            }
        }

        private void GlobalEventManager_OnCharacterDeath(On.RoR2.GlobalEventManager.orig_OnCharacterDeath orig, GlobalEventManager self, DamageReport report)
        {
            orig(self, report);
            if (NetworkServer.active && report.victimBody && report.victimBody.HasBuff(DLC1Content.Buffs.EliteVoid))
            {
                FireProjectileInfo info = new()
                {
                    projectilePrefab = DeathBomb,
                    position = report.damageInfo.position,
                    damage = 0,
                    rotation = Quaternion.identity,
                    owner = report.victimBody.gameObject
                };
                ProjectileManager.instance.FireProjectile(info);
            }
        }

        private void GlobalEventManager_OnHitEnemy1(On.RoR2.GlobalEventManager.orig_OnHitEnemy orig, GlobalEventManager self, DamageInfo info, GameObject victim)
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
                x => x.MatchLdsfld(typeof(DLC1Content.Buffs), "EliteVoid")))
            {
                c.Remove();
                c.Emit<Useless>(OpCodes.Ldsfld, nameof(Useless.uselessBuff));
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Voidtouched Elite Needletick hook");
            }
        }

        private void HealthComponent_Awake(On.RoR2.HealthComponent.orig_Awake orig, HealthComponent self)
        {
            self.gameObject.AddComponent<VoidtouchedPermanentDamageController>();
            orig(self);
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
    }

    public class VoidtouchedBallsManager : MonoBehaviour
    {
        public CharacterBody body;
        public GameObject prefab = Projectiles.VoidBall.prefab;
        private float timer;
        public float interval = 5f;
        public int ballCount;

        private void Start()
        {
            body = GetComponent<CharacterBody>();
            if (Run.instance.selectedDifficulty >= DifficultyIndex.Eclipse3 && Eclipse3.instance.isEnabled)
            {
                ballCount = 5;
            }
            else
            {
                ballCount = 3;
            }
        }

        private void FixedUpdate()
        {
            timer += Time.fixedDeltaTime;
            if (timer >= interval)
            {
                var randomDelay = Run.instance.nextStageRng.RangeFloat(0.12f, 0.4f);
                StartCoroutine(SummonBalls(ballCount, randomDelay));
                timer = 0;
            }
        }

        private IEnumerator SummonBalls(int count, float delay)
        {
            var playerList = CharacterBody.readOnlyInstancesList.Where(x => x.isPlayerControlled).ToArray();
            for (int i = 0; i < playerList.Length; i++)
            {
                for (int j = 0; j < count; j++)
                {
                    var fpi = new FireProjectileInfo
                    {
                        projectilePrefab = prefab,
                        damage = Run.instance ? 5f + Mathf.Sqrt(Run.instance.ambientLevel * 200f) : 0f,
                        rotation = Quaternion.identity,
                        owner = gameObject,
                        crit = body.RollCrit(),
                        //position = Util.ApplySpread(playerList[i].footPosition, 7f * j, 10f * j, 1f, 0.08f)
                        position = new Vector3(playerList[i].footPosition.x + Run.instance.treasureRng.RangeFloat(-15f * j, 15f * j), playerList[i].footPosition.y, playerList[i].footPosition.z + Run.instance.runRNG.RangeFloat(-15f * j, 15f * j))
                    };
                    ProjectileManager.instance.FireProjectile(fpi);
                    yield return new WaitForSeconds(delay);
                }
            }
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