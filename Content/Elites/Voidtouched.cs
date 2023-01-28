using Mono.Cecil.Cil;
using MonoMod.Cil;
using WellRoundedBalance.Eclipse;
using WellRoundedBalance.Projectiles;

namespace WellRoundedBalance.Elites
{
    internal class Voidtouched : EliteBase
    {
        public static BuffDef useless;
        public static BuffDef voidCurse;
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
            spinnyLaser = VoidLaser.laserPrefab;
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
            }
        }

        private Ray GetSpinnyRay(CharacterBody body)
        {
            // make this thing work with muzzles somehow?
            var forward = body.corePosition;
            var corePosition = body.corePosition;
            forward.y = body.corePosition.y;
            return new Ray(forward, corePosition);
        }

        private void FixedUpdate()
        {
            timer += Time.fixedDeltaTime;
            if (timer >= spinnyHitFrequency)
            {
                BulletAttack ba = new()
                {
                    origin = GetSpinnyRay(body).origin,
                    aimVector = GetSpinnyRay(body).direction,
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
                    hitEffectPrefab = VoidLaser.impactPrefab,
                    tracerEffectPrefab = VoidLaser.laserPrefab,
                    isCrit = false,
                    HitEffectNormal = false,
                };

                if (Util.HasEffectiveAuthority(gameObject)) ba.Fire();

                Fire();

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