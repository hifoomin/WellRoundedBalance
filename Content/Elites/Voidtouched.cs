using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace WellRoundedBalance.Elites
{
    internal class Voidtouched : EliteBase
    {
        public static BuffDef useless;
        public static BuffDef voidCurse;
        public override string Name => ":: Elites ::::: Voidtouched";

        public override void Init()
        {
            useless = ScriptableObject.CreateInstance<BuffDef>();
            useless.name = "Useless Buff";
            useless.isHidden = true;

            var curse = Utils.Paths.Texture2D.texBuffPermanentCurse.Load<Texture2D>();
            voidCurse = ScriptableObject.CreateInstance<BuffDef>();
            voidCurse.isHidden = false;
            voidCurse.canStack = true;
            voidCurse.iconSprite = Sprite.Create(curse, new Rect(0, 0, (float)curse.width, (float)curse.height), new Vector2(0f, 0f));
            voidCurse.buffColor = new Color32(255, 75, 74, 255);
            voidCurse.isDebuff = true;

            ContentAddition.AddBuffDef(useless);
            ContentAddition.AddBuffDef(voidCurse);

            base.Init();
        }

        public override void Hooks()
        {
            On.RoR2.CharacterMaster.OnBodyStart += CharacterMaster_OnBodyStart;
            On.RoR2.HealthComponent.Awake += HealthComponent_Awake;
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
            IL.RoR2.GlobalEventManager.OnHitEnemy += GlobalEventManager_OnHitEnemy;
            IL.RoR2.AffixVoidBehavior.FixedUpdate += AffixVoidBehavior_FixedUpdate;
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
                x => x.MatchLdsfld("RoR2.DLC1Content/Buffs", "EliteVoid")))
            {
                c.Remove();
                c.Emit<Blazing>(OpCodes.Ldsfld, nameof(useless));
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
            if (Run.instance.selectedDifficulty >= DifficultyIndex.Eclipse3)
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
                            damage = body.damage * 1.5f,
                            rotation = Quaternion.identity,
                            owner = gameObject,
                            crit = body.RollCrit(),
                            position = Util.ApplySpread(playerList[i].footPosition, 4f * j, 6f * j, 1f, 0f)
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