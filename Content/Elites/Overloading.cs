using RoR2.Navigation;
using WellRoundedBalance.Buffs;
using WellRoundedBalance.Gamemodes.Eclipse;

namespace WellRoundedBalance.Elites
{
    internal class Overloading : EliteBase<Overloading>
    {
        public static BuffDef overloadingSpeedBuff;
        public override string Name => ":: Elites ::: Overloading";

        [ConfigField("Passive Movement Speed Gain", "Decimal.", 0.5f)]
        public static float passiveMovementSpeedGain;

        [ConfigField("Ally Buff Movement Speed Gain", "Decimal.", 0.5f)]
        public static float allyBuffMovementSpeedGain;

        [ConfigField("Ally Buff Movement Speed Gain Eclipse 3+", "Decimal. Only applies if you have Eclipse Changes enabled.", 0.75f)]
        public static float allyBuffMovementSpeedGainE3;

        [ConfigField("Aggressive Teleport Cooldown", "Do not set it to the same value as Defensive Teleport Cooldown", 5f)]
        public static float aggressiveTeleportCooldown;

        [ConfigField("Defensive Teleport Cooldown", "Do not set it to the same value as Aggressive Teleport Cooldown", 7f)]
        public static float defensiveTeleportCooldown;

        [ConfigField("Maximum Speed Aura Radius", "", 40f)]
        public static float maxSpeedAuraRadius;

        [ConfigField("Minimum Speed Aura Radius", "", 13f)]
        public static float minSpeedAuraRadius;

        private static GameObject SpeedAura;

        public static GameObject tpEffect;

        public override void Init()
        {
            var speedBuff = Utils.Paths.Texture2D.texBuffKillMoveSpeed.Load<Texture2D>();

            overloadingSpeedBuff = ScriptableObject.CreateInstance<BuffDef>();
            overloadingSpeedBuff.isHidden = false;
            overloadingSpeedBuff.isDebuff = false;
            overloadingSpeedBuff.canStack = false;
            overloadingSpeedBuff.buffColor = new Color32(66, 98, 219, 255);
            overloadingSpeedBuff.iconSprite = Sprite.Create(speedBuff, new Rect(0f, 0f, (float)speedBuff.width, (float)speedBuff.height), new Vector2(0f, 0f));
            overloadingSpeedBuff.name = "Overloading Speed Buff";

            ContentAddition.AddBuffDef(overloadingSpeedBuff);

            SpeedAura = PrefabAPI.InstantiateClone(Utils.Paths.GameObject.RailgunnerMineAltDetonated.Load<GameObject>(), "AntihealZone");
            Transform areaIndicator = SpeedAura.transform.Find("AreaIndicator");
            Transform softGlow = areaIndicator.Find("SoftGlow");
            Transform sphere = areaIndicator.Find("Sphere");
            Transform light = areaIndicator.Find("Point Light");
            Transform core = areaIndicator.Find("Core");

            softGlow.gameObject.SetActive(false);
            light.gameObject.SetActive(false);
            core.gameObject.SetActive(false);

            MeshRenderer renderer = sphere.GetComponent<MeshRenderer>();
            Material[] mats = renderer.sharedMaterials;
            mats[0] = Utils.Paths.Material.matMoonbatteryCrippleRadius.Load<Material>();
            mats[1] = Utils.Paths.Material.matCrippleSphereIndicator.Load<Material>();
            renderer.SetSharedMaterials(mats, 2);

            var buffWard = SpeedAura.GetComponent<BuffWard>();
            buffWard.buffDef = overloadingSpeedBuff;
            buffWard.expires = false;
            buffWard.expireDuration = 10000;
            buffWard.invertTeamFilter = false;

            SpeedAura.RemoveComponent<SlowDownProjectiles>();
            var teamFilter = SpeedAura.AddComponent<TeamFilter>();
            teamFilter.teamIndex = TeamIndex.None;
            teamFilter.defaultTeam = TeamIndex.Monster;

            PrefabAPI.RegisterNetworkPrefab(SpeedAura);

            tpEffect = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>("RoR2/Junk/Parent/ParentTeleportEffect.prefab").WaitForCompletion(), "LunarConstructTeleport", false);
            var particles = tpEffect.transform.GetChild(0);
            var ringParticle = particles.GetChild(0).GetComponent<ParticleSystemRenderer>();

            var moonRamp = Addressables.LoadAssetAsync<Texture2D>("RoR2/Base/Common/ColorRamps/texRampLunarWispFire.png").WaitForCompletion();

            var newRing = Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Parent/matParentTeleportPortal.mat").WaitForCompletion());
            newRing.SetTexture("_RemapTex", moonRamp);

            ringParticle.sharedMaterial = newRing;

            particles.GetChild(1).gameObject.SetActive(false);

            var energyInitialParticle = particles.GetChild(3).GetComponent<ParticleSystemRenderer>();
            energyInitialParticle.sharedMaterial = newRing;
            energyInitialParticle.gameObject.transform.localScale = Vector3.one * 0.25f;

            var eps = particles.GetChild(3).GetComponent<ParticleSystem>().main;
            eps.duration = 0.17f;

            particles.GetChild(4).gameObject.SetActive(false);

            ContentAddition.AddEffect(tpEffect);

            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.GlobalEventManager.OnHitAll += GlobalEventManager_OnHitAll;
            On.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats1;

            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
            IL.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
        }

        private void GlobalEventManager_OnHitAll(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdsfld(typeof(RoR2Content.Buffs), "AffixBlue")))
            {
                c.Remove();
                c.Emit<Useless>(OpCodes.Ldsfld, nameof(Useless.uselessBuff));
            }
            else
            {
                Logger.LogError("Failed to apply Overloading Deletion 2 hook");
            }
        }

        private void CharacterBody_RecalculateStats1(On.RoR2.CharacterBody.orig_RecalculateStats orig, CharacterBody self)
        {
            orig(self);
            if (NetworkServer.active && self.HasBuff(RoR2Content.Buffs.AffixBlue))
            {
                self.moveSpeed *= 1f + passiveMovementSpeedGain;
                if (!self.GetComponent<OverloadingController>())
                {
                    self.gameObject.AddComponent<OverloadingController>();
                }
            }
            if (!self.HasBuff(RoR2Content.Buffs.AffixBlue))
            {
                if (self.GetComponent<OverloadingController>())
                {
                    self.gameObject.RemoveComponent<OverloadingController>();
                }
            }
        }

        private void CharacterBody_RecalculateStats(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdsfld(typeof(RoR2Content.Buffs), "AffixBlue")))
            {
                c.Remove();
                c.Emit<Useless>(OpCodes.Ldsfld, nameof(Useless.uselessBuff));
            }
            else
            {
                Logger.LogError("Failed to apply Overloading Deletion 1 hook");
            }
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            bool e3 = Run.instance.selectedDifficulty >= DifficultyIndex.Eclipse3 && Eclipse3.instance.isEnabled;
            if (sender && sender.HasBuff(overloadingSpeedBuff) && !sender.HasBuff(RoR2Content.Buffs.AffixBlue))
            {
                args.moveSpeedMultAdd += e3 ? allyBuffMovementSpeedGainE3 : allyBuffMovementSpeedGain;
            }
        }

        private class OverloadingController : MonoBehaviour
        {
            public HealthComponent hc;
            public CharacterBody cb;
            public GameObject wardInstance;
            public BuffWard ward;
            public TeamFilter teamFilter;
            public float stopwatch = 0f;

            public void Start()
            {
                hc = GetComponent<HealthComponent>();
                cb = hc.body;

                if (NetworkServer.active)
                {
                    wardInstance = GameObject.Instantiate(SpeedAura, transform);
                    teamFilter = wardInstance.GetComponent<TeamFilter>();

                    ward = wardInstance.GetComponent<BuffWard>();
                    ward.radius = Util.Remap(cb.baseMaxHealth, 0f, 2100f, minSpeedAuraRadius, maxSpeedAuraRadius);

                    NetworkServer.Spawn(wardInstance);
                }
            }

            public void FixedUpdate()
            {
                stopwatch += Time.fixedDeltaTime;
                if (ward.transform.localPosition != Vector3.zero) ward.transform.localPosition = Vector3.zero;
                if (ward.transform.position != gameObject.transform.position) ward.transform.position = gameObject.transform.position;
                if (ward.teamFilter.teamIndex != cb.teamComponent.teamIndex) teamFilter.teamIndex = cb.teamComponent.teamIndex;
                if (ward.teamFilter != teamFilter) ward.teamFilter = teamFilter;
                if (!hc.alive && NetworkServer.active) Destroy(this);
                if (stopwatch >= GetDelay())
                {
                    stopwatch = 0f;
                    HandleTeleport(PickTeleportPosition());
                }
            }

            public void OnDestroy()
            {
                if (NetworkServer.active)
                {
                    Destroy(wardInstance);
                }
            }

            public float GetDelay()
            {
                return (hc.health / hc.fullCombinedHealth) > 0.5 ? aggressiveTeleportCooldown : defensiveTeleportCooldown;
            }

            public void HandleTeleport(Vector3 pos)
            {
                if (cb.isPlayerControlled)
                {
                    return;
                }
                Vector3 current = transform.position;
                EffectManager.SpawnEffect(tpEffect, new EffectData
                {
                    scale = 0.66f,
                    origin = current
                }, true);
                EffectManager.SpawnEffect(tpEffect, new EffectData
                {
                    scale = 0.66f,
                    origin = pos
                }, true);
                TeleportHelper.TeleportBody(cb, pos + new Vector3(0, 1, 0));
            }

            public Vector3[] PickValidPositions(float min, float max, NodeGraph.Node[] nodes)
            {
                NodeGraph.Node[] validNodes = nodes.Where(x => Vector3.Distance(x.position, transform.position) > min && Vector3.Distance(x.position, transform.position) < max).ToArray();
                if (validNodes.Length <= 1)
                {
                    return new Vector3[] { transform.position };
                }
                return validNodes.Select(node => node.position).ToArray();
            }

            public Vector3 PickTeleportPosition()
            {
                if (!SceneInfo.instance || !SceneInfo.instance.groundNodes)
                {
                    return transform.position;
                }

                NodeGraph.Node[] nodes = SceneInfo.instance.groundNodes.nodes;
                Vector3[] validPositions;
                if (GetDelay() == aggressiveTeleportCooldown)
                {
                    validPositions = PickValidPositions(0, 25, nodes);
                }
                else
                {
                    validPositions = PickValidPositions(25, 45, nodes);
                }
                return validPositions.GetRandom();
            }
        }
    }
}