using Mono.Cecil.Cil;
using MonoMod.Cil;
using WellRoundedBalance.Eclipse;

namespace WellRoundedBalance.Elites
{
    internal class Voidtouched : EliteBase
    {
        public static BuffDef useless;
        public static BuffDef voidCurse;
        public override string Name => ":: Elites :::::: Voidtouched";
        public static GameObject DeathBomb;
        public static DamageAPI.ModdedDamageType NullifierDeath = DamageAPI.ReserveDamageType();

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

        private void EnactNullifierMoment(On.RoR2.GlobalEventManager.orig_OnCharacterDeath orig, GlobalEventManager self, DamageReport report) {
            orig(self, report);
            if (NetworkServer.active && report.victimBody && report.victimBody.HasBuff(DLC1Content.Buffs.EliteVoid)) {
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

        private void NullifierMoment(On.RoR2.GlobalEventManager.orig_OnHitEnemy orig, GlobalEventManager self, DamageInfo info, GameObject victim) {
            orig(self, info, victim);
            if (NetworkServer.active && info.HasModdedDamageType(NullifierDeath)) {
                CharacterBody victimBody = victim.GetComponent<CharacterBody>();
                if (victimBody && !victimBody.isPlayerControlled && !victimBody.bodyFlags.HasFlag(CharacterBody.BodyFlags.Mechanical)) {
                    CharacterMaster victimMaster = victimBody.master;
                    victimMaster.teamIndex = TeamIndex.Void;
                    victimBody.teamComponent.teamIndex = TeamIndex.Void;
                    victimBody.inventory.SetEquipmentIndex(DLC1Content.Equipment.EliteVoidEquipment.equipmentIndex);
                    BaseAI ai = victimMaster.GetComponent<BaseAI>();
                    if (ai) {
                        ai.enemyAttention = 0;
                        ai.ForceAcquireNearestEnemyIfNoCurrentEnemy();
                    }
                    EffectManager.SpawnEffect(Utils.Paths.GameObject.ElementalRingVoidImplodeEffect.Load<GameObject>(), new EffectData {
                        origin = info.position
                    }, true);
                }
            }

            if (NetworkServer.active && info.attacker) {
                CharacterBody attackerBody = info.attacker.GetComponent<CharacterBody>();
                CharacterBody victimBody = info.attacker.GetComponent<CharacterBody>();

                if (attackerBody.HasBuff(DLC1Content.Buffs.EliteVoid)) {
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
                c.Emit<Blazing>(OpCodes.Ldsfld, nameof(useless));
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
                c.Emit<Voidtouched>(OpCodes.Ldsfld, nameof(useless));
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Voidtouched Elite Needletick hook");
            }
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
                if (body.GetComponent<VoidtouchedBallsManager>() == null)
                {
                    body.gameObject.AddComponent<VoidtouchedBallsManager>();
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
                var playerList = CharacterBody.readOnlyInstancesList.Where(x => x.isPlayerControlled).ToArray();
                for (int i = 0; i < playerList.Length; i++)
                {
                    for (int j = 0; j < ballCount; j++)
                    {
                        var fpi = new FireProjectileInfo
                        {
                            projectilePrefab = prefab,
                            damage = Run.instance ? 5f + Mathf.Sqrt(Run.instance.ambientLevel * 200f) : 0f,
                            rotation = Quaternion.identity,
                            owner = gameObject,
                            crit = body.RollCrit(),
                            position = Util.ApplySpread(playerList[i].footPosition, 5f * j, 7f * j, 1f, 0.08f)
                        };
                        ProjectileManager.instance.FireProjectile(fpi);
                        timer = 0f;
                    }
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