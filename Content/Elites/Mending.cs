using WellRoundedBalance.Gamemodes.Eclipse;
using static WellRoundedBalance.Elites.Mending;

namespace WellRoundedBalance.Elites
{
    internal class Mending : EliteBase<Mending>
    {
        public override string Name => "Elites :::: Mending";

        [ConfigField("Healing Beam Radius", "", 25f)]
        public static float radius;

        [ConfigField("Heal Coefficient Per Second", "", 3f)]
        public static float healFraction;

        [ConfigField("Heal Nova Radius", "", 30f)]
        public static float healNovaRadius;

        [ConfigField("On Hit Healing Target Regen Boost", "", 7f)]
        public static float onHitHealingTargetRegenBoost;

        [ConfigField("On Hit Healing Target Regen Boost Eclipse 3+", "Only applies if you have Eclipse Changes enabled.", 9f)]
        public static float onHitHealingTargetRegenBoostE3;

        public static BuffDef regenBoost;

        private static GameObject healVFX;

        public override void Init()
        {
            base.Init();
            regenBoost = ScriptableObject.CreateInstance<BuffDef>();
            regenBoost.isHidden = false;
            regenBoost.canStack = false;
            regenBoost.isDebuff = false;
            regenBoost.iconSprite = Main.wellroundedbalance.LoadAsset<Sprite>("Assets/WellRoundedBalance/texMendingRegen.png");
            regenBoost.buffColor = new Color32(161, 231, 79, 255);

            ContentAddition.AddBuffDef(regenBoost);

            healVFX = PrefabAPI.InstantiateClone(Utils.Paths.GameObject.IgniteExplosionVFX.Load<GameObject>(), "MendingHealingAura", false);

            var igniteParticleSystem = healVFX.GetComponent<ParticleSystem>().main.startColor;
            igniteParticleSystem.color = new Color32(52, 224, 75, 255);

            var trans = healVFX.transform;

            var omniParticleSystemRenderer = trans.GetChild(0).GetComponent<ParticleSystemRenderer>();

            var newMat = GameObject.Instantiate(Utils.Paths.Material.matOmniHitspark3Gasoline.Load<Material>());
            newMat.SetTexture("_RemapTex", Utils.Paths.Texture2D.texRampAntler.Load<Texture2D>());

            omniParticleSystemRenderer.material = newMat;

            var pointLight = trans.GetChild(1).GetComponent<Light>();
            pointLight.color = new Color32(18, 206, 15, 255);

            var flamesParticleSystem = trans.GetChild(2).GetComponent<ParticleSystem>().colorOverLifetime;

            Gradient greenGradient = new();
            greenGradient.SetKeys(
                new GradientColorKey[] { new GradientColorKey(new Color32(170, 255, 158, 255), 0f), new GradientColorKey(new Color32(36, 233, 0, 255), 0.424f), new GradientColorKey(Color.black, 1f) },
                new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0f, 1f) });

            flamesParticleSystem.color = greenGradient;

            ContentAddition.AddEffect(healVFX);
        }

        public override void Hooks()
        {
            IL.RoR2.AffixEarthBehavior.FixedUpdate += AffixEarthBehavior_FixedUpdate;
            On.RoR2.AffixEarthBehavior.Start += Overwrite;
            GlobalEventManager.onServerDamageDealt += GlobalEventManager_onServerDamageDealt;
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
        }

        private void AffixEarthBehavior_FixedUpdate(ILContext il)
        {
            ILCursor c = new(il);
            c.Emit(OpCodes.Ret);
        }

        private void GlobalEventManager_onServerDamageDealt(DamageReport report)
        {
            var info = report.damageInfo;
            if (!NetworkServer.active)
            {
                return;
            }

            if (!info.attacker)
            {
                return;
            }

            var healOnHitController = info.attacker.GetComponent<HealOnHitController>();
            if (!healOnHitController)
            {
                return;
            }
            if (Util.CheckRoll(100f * info.procCoefficient))
            {
                if (Util.HasEffectiveAuthority(info.attacker))
                {
                    healOnHitController.Proc();
                }
            }
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.HasBuff(regenBoost))
            {
                bool e3 = Run.instance.selectedDifficulty >= DifficultyIndex.Eclipse3 && Eclipse3.instance.isEnabled;
                var regenStack = e3 ? onHitHealingTargetRegenBoost : onHitHealingTargetRegenBoostE3;
                args.baseRegenAdd += regenStack + 0.2f * regenStack * (sender.level - 1);
            }
        }

        private static void Overwrite(On.RoR2.AffixEarthBehavior.orig_Start orig, AffixEarthBehavior self)
        {
            if (self.gameObject.GetComponent<HealTetherController>() == null)
                self.body.gameObject.AddComponent<HealTetherController>();
            if (self.gameObject.GetComponent<HealOnHitController>() == null)
                self.body.gameObject.AddComponent<HealOnHitController>();
            return;
        }

        public class HealTetherController : MonoBehaviour
        {
            private TetherVfxOrigin vfxOrigin;
            public HealthComponent target;
            private float stopwatch = 0f;
            private float delay = 1f;
            private TeamIndex team;
            private CharacterBody body;
            public static List<HealTetherController> healTetherControllers = new();

            public void Start()
            {
                vfxOrigin = base.gameObject.AddComponent<TetherVfxOrigin>();
                vfxOrigin.tetherPrefab = Utils.Paths.GameObject.AffixEarthTetherVFX.Load<GameObject>();
                team = GetComponent<TeamComponent>().teamIndex;
                body = GetComponent<CharacterBody>();
                healTetherControllers.Add(this);
            }

            public void FixedUpdate()
            {
                stopwatch += Time.fixedDeltaTime;
                if (stopwatch >= delay)
                {
                    stopwatch = 0f;
                    target = FetchTarget();
                    if (target && NetworkServer.active)
                    {
                        target.Heal(body.damage * healFraction, new(), true);
                    }

                    if (target)
                    {
                        vfxOrigin.SetTetheredTransforms(new() { target.transform });
                    }
                    else
                    {
                        if (vfxOrigin.tetheredTransforms != null && vfxOrigin.tetheredTransforms.Count > 0)
                        {
                            vfxOrigin.RemoveTetherAt(0);
                        }
                    }

                    if (!body.HasBuff(DLC1Content.Buffs.EliteEarth))
                    {
                        Destroy(vfxOrigin);
                        Destroy(this);
                    }
                }
            }

            public HealthComponent FetchTarget()
            {
                SphereSearch search = new()
                {
                    origin = base.transform.position,
                    radius = radius,
                    mask = LayerIndex.entityPrecise.mask
                };
                search.RefreshCandidates();
                search.OrderCandidatesByDistance();
                search.FilterCandidatesByDistinctHurtBoxEntities();
                return search.GetHurtBoxes().FirstOrDefault(x => CheckIsValid(x.healthComponent))?.healthComponent ?? null;
            }

            public bool CheckIsValid(HealthComponent com)
            {
                if (com.body == body)
                {
                    return false;
                }
                if (com.body.teamComponent.teamIndex != team)
                {
                    return false;
                }
                if (com.body.HasBuff(DLC1Content.Buffs.EliteEarth))
                {
                    return false;
                }
                foreach (HealTetherController controller in healTetherControllers)
                {
                    if (controller != this && controller.target == com)
                    {
                        return false;
                    }
                }
                return true;
            }

            public void OnDestroy()
            {
                healTetherControllers.Remove(this);
            }
        }

        public class HealOnHitController : MonoBehaviour
        {
            public float radius = 0f;
            public CharacterBody healerBody;
            public HealthComponent healerHc;
            public static List<HealOnHitController> healOnHitControllers = new();
            public static readonly SphereSearch healSphereSearch = new();
            public static readonly List<HurtBox> healHurtBoxBuffer = new();

            public void Start()
            {
                healOnHitControllers.Add(this);
                healerBody = GetComponent<CharacterBody>();
                healerHc = healerBody?.healthComponent;
                if (healerBody)
                    radius = healNovaRadius + healerBody.radius;
            }

            public void Proc()
            {
                if (!healerBody)
                {
                    return;
                }

                if (!healerHc)
                {
                    return;
                }

                if (!healerHc.alive)
                {
                    return;
                }

                Vector3 corePosition = healerBody.corePosition;
                healSphereSearch.origin = corePosition;
                healSphereSearch.mask = LayerIndex.entityPrecise.mask;
                healSphereSearch.radius = radius;
                healSphereSearch.RefreshCandidates();
                healSphereSearch.FilterCandidatesByDistinctHurtBoxEntities();
                healSphereSearch.OrderCandidatesByDistance();
                healSphereSearch.GetHurtBoxes(healHurtBoxBuffer);
                healSphereSearch.ClearCandidates();

                for (int i = 0; i < healHurtBoxBuffer.Count; i++)
                {
                    var hurtBox = healHurtBoxBuffer[i];
                    if (hurtBox.healthComponent)
                    {
                        var body = hurtBox.healthComponent.body;
                        if (body && body.teamComponent.teamIndex == healerBody.teamComponent.teamIndex)
                        {
                            body.AddTimedBuff(regenBoost, 3f);
                        }
                    }
                }

                healHurtBoxBuffer.Clear();

                EffectManager.SpawnEffect(healVFX, new EffectData
                {
                    origin = corePosition,
                    scale = radius
                }, true);
            }

            public void OnDestroy()
            {
                healOnHitControllers.Remove(this);
            }
        }
    }
}