using System;
using WellRoundedBalance.Gamemodes.Eclipse;

namespace WellRoundedBalance.Elites.Tier1
{
    internal class Mending : EliteBase<Mending>
    {
        public override string Name => ":: Elites : Mending";

        [ConfigField("Healing Beam Radius", "", 25f)]
        public static float radius;

        [ConfigField("Heal Coefficient Per Second", "Decimal.", 3f)]
        public static float healFraction;

        [ConfigField("Heal Nova Radius", "", 30f)]
        public static float healNovaRadius;

        [ConfigField("Self Heal Health Threshold", "Decimal.", 0.35f)]
        public static float selfHealThreshold;

        [ConfigField("Self Heal Regen Boost", "", 24f)]
        public static float selfHealRegen;

        [ConfigField("Self Heal Healing", "Decimal.", 0.1f)]
        public static float selfHealHealing;

        [ConfigField("On Hit Healing Target Regen Boost", "", 8f)]
        public static float onHitHealingTargetRegenBoost;

        [ConfigField("On Hit Healing Target Regen Boost Eclipse 3+", "Only applies if you have Eclipse Changes enabled.", 12f)]
        public static float onHitHealingTargetRegenBoostE3;

        public static BuffDef regenBoost;
        public static BuffDef selfRegen;

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
            regenBoost.name = "Mending Elite Regeneration Boost";

            selfRegen = ScriptableObject.CreateInstance<BuffDef>();
            selfRegen.isHidden = false;
            selfRegen.canStack = false;
            selfRegen.isDebuff = false;
            selfRegen.iconSprite = Main.wellroundedbalance.LoadAsset<Sprite>("Assets/WellRoundedBalance/texMendingRegen.png");
            selfRegen.buffColor = new Color32(109, 231, 97, 255);
            selfRegen.name = "Mending Elite Self Regeneration Boost";

            ContentAddition.AddBuffDef(regenBoost);
            ContentAddition.AddBuffDef(selfRegen);

            healVFX = Utils.Paths.GameObject.IgniteExplosionVFX.Load<GameObject>().InstantiateClone("MendingHealingAura", false);

            var igniteParticleSystem = healVFX.GetComponent<ParticleSystem>().main.startColor;
            igniteParticleSystem.color = new Color32(52, 224, 75, 255);

            var trans = healVFX.transform;

            var omniParticleSystemRenderer = trans.GetChild(0).GetComponent<ParticleSystemRenderer>();

            var newMat = UnityEngine.Object.Instantiate(Utils.Paths.Material.matOmniHitspark3Gasoline.Load<Material>());
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
            On.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;
            IL.RoR2.GlobalEventManager.OnCharacterDeath += GlobalEventManager_OnCharacterDeath;
            IL.RoR2.HealthComponent.ServerFixedUpdate += HealthComponent_ServerFixedUpdate;
        }

        private void HealthComponent_ServerFixedUpdate(ILContext il)
        {
            ILCursor c = new(il);
            if (c.TryGotoNext(x => x.MatchStfld(typeof(HealthComponent), nameof(HealthComponent.regenAccumulator))))
            {
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<float, HealthComponent, float>>((regenAccumulator, self) =>
                {
                    if (self.body.HasBuff(selfRegen)) regenAccumulator += Time.fixedDeltaTime * (selfHealHealing / 3f) * self.fullHealth;
                    return regenAccumulator;
                });
            }
            else Logger.LogError("Failed to apply Mending Elite Regen hook");
        }

        private void GlobalEventManager_OnCharacterDeath(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdsfld("RoR2.DLC1Content/Buffs", "EliteEarth")))
            {
                c.Remove();
                c.Emit<Buffs.Useless>(OpCodes.Ldsfld, nameof(Buffs.Useless.uselessBuff));
            }
            else
            {
                Logger.LogError("Failed to apply Mending Elite Healing Core Deletion hook");
            }
        }

        private void HealthComponent_TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent victim, DamageInfo damageInfo)
        {
            var victimBody = victim.body;
            if (victimBody && victimBody.HasBuff(DLC1Content.Buffs.EliteEarth))
            {
                var mendingController = victim.GetComponent<MendingController>();
                if (mendingController && !mendingController.healed && victim.combinedHealthFraction <= selfHealThreshold)
                {
                    victimBody.AddTimedBuff(selfRegen, 3f);
                    mendingController.healed = true;
                }
            }
            orig(victim, damageInfo);
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

            var mendingController = info.attacker.GetComponent<MendingController>();
            if (!mendingController)
            {
                return;
            }
            if (Util.CheckRoll(100f * info.procCoefficient))
            {
                if (Util.HasEffectiveAuthority(info.attacker))
                {
                    mendingController.Proc();
                }
            }
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.HasBuff(regenBoost))
            {
                bool e3 = Run.instance && Run.instance.selectedDifficulty >= DifficultyIndex.Eclipse3 && Eclipse3.instance.isEnabled;
                var regenStack = e3 ? onHitHealingTargetRegenBoost : onHitHealingTargetRegenBoostE3;
                args.baseRegenAdd += regenStack + 0.2f * regenStack * (sender.level - 1);
            }
            if (sender.HasBuff(selfRegen))
            {
                var regenStack = selfHealRegen;
                args.baseRegenAdd += regenStack + 0.2f * regenStack * (sender.level - 1);
            }
        }

        private static void Overwrite(On.RoR2.AffixEarthBehavior.orig_Start orig, AffixEarthBehavior self)
        {
            if (self.gameObject.GetComponent<MendingController>() == null)
                self.body.gameObject.AddComponent<MendingController>();
            return;
        }

        public class MendingController : MonoBehaviour
        {
            public float radius = 0f;
            public CharacterBody healerBody;
            public HealthComponent healerHc;
            public static readonly SphereSearch healSphereSearch = new();
            public static readonly List<HurtBox> healHurtBoxBuffer = new();
            public bool healed = false;

            private TetherVfxOrigin vfxOrigin;
            public HealthComponent target;
            public float stopwatch = 0f;
            public float delay = 1f;
            public TeamIndex team;
            public static List<MendingController> mendingControllers = new();

            public void Start()
            {
                healerBody = GetComponent<CharacterBody>();
                healerHc = healerBody?.healthComponent;
                if (healerBody)
                    radius = healNovaRadius + healerBody.radius;

                vfxOrigin = gameObject.AddComponent<TetherVfxOrigin>();
                vfxOrigin.tetherPrefab = Utils.Paths.GameObject.AffixEarthTetherVFX.Load<GameObject>();
                team = GetComponent<TeamComponent>().teamIndex;
                mendingControllers.Add(this);
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
                        target.Heal(healerBody.damage * healFraction, new(), true);
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

                    if (!healerBody.HasBuff(DLC1Content.Buffs.EliteEarth))
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
                    origin = transform.position,
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
                if (com.body == healerBody)
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
                foreach (MendingController controller in mendingControllers)
                {
                    if (controller != this && controller.target == com)
                    {
                        return false;
                    }
                }
                return true;
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
                        if (body && !body.HasBuff(DLC1Content.Buffs.EliteEarth) && body.teamComponent.teamIndex == healerBody.teamComponent.teamIndex)
                        {
                            body.AddTimedBuff(regenBoost, 3f);
                        }
                    }
                }

                healHurtBoxBuffer.Clear();

                if (!healerBody.isPlayerControlled)
                {
                    EffectManager.SpawnEffect(healVFX, new EffectData
                    {
                        origin = corePosition,
                        scale = radius
                    }, true);
                }
            }

            public void OnDestroy()
            {
                mendingControllers.Remove(this);
            }
        }
    }
}