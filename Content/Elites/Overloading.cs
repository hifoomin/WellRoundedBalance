using HG;
using RoR2.Navigation;
using System.Collections;
using UnityEngine;
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

        [ConfigField("Aggressive Teleport Cooldown", "Do not set it to the same value as Defensive Teleport Cooldown", 6f)]
        public static float aggressiveTeleportCooldown;

        [ConfigField("Maximum Speed Aura Radius", "", 40f)]
        public static float maxSpeedAuraRadius;

        [ConfigField("Minimum Speed Aura Radius", "", 16f)]
        public static float minSpeedAuraRadius;

        private static GameObject SpeedAura;

        public static GameObject tpEffect;
        public static GameObject tpTracer;

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

            SpeedAura = PrefabAPI.InstantiateClone(Utils.Paths.GameObject.RailgunnerMineAltDetonated.Load<GameObject>(), "OverloadingSpeedAura");
            SpeedAura.RemoveComponent<SlowDownProjectiles>();
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
            buffWard.buffDuration = 6f;

            var teamFilter = SpeedAura.AddComponent<TeamFilter>();
            teamFilter.defaultTeam = TeamIndex.None;

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

            tpTracer = PrefabAPI.InstantiateClone(Utils.Paths.GameObject.VoidSurvivorBeamTracer.Load<GameObject>(), "OverloadingTracer", false);
            tpTracer.transform.GetChild(0).gameObject.SetActive(false);
            tpTracer.transform.GetChild(1).gameObject.SetActive(false);

            var lineRenderer = tpTracer.GetComponent<LineRenderer>();
            lineRenderer.widthMultiplier = 0.33f;
            lineRenderer.numCapVertices = 10;

            var newMat = GameObject.Instantiate(Utils.Paths.Material.matVoidSurvivorBeamTrail.Load<Material>());
            newMat.SetTexture("_RemapTex", Utils.Paths.Texture2D.texRampLunarWardDecal.Load<Texture2D>());

            lineRenderer.material = newMat;

            var animateShaderAlpha = tpTracer.GetComponent<AnimateShaderAlpha>();
            animateShaderAlpha.timeMax = 0.4f;

            ContentAddition.AddEffect(tpTracer);
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
            public float stopwatch = 0f;

            public void Start()
            {
                hc = GetComponent<HealthComponent>();
                cb = hc.body;
            }

            public void FixedUpdate()
            {
                stopwatch += Time.fixedDeltaTime;
                if (!hc.alive && NetworkServer.active)
                {
                    Destroy(this);
                }
                if (stopwatch >= GetDelay())
                {
                    StartCoroutine(Teleport());
                    stopwatch = 0f;
                }
            }

            public IEnumerator Teleport()
            {
                var currentPosition = transform.position;

                EffectManager.SpawnEffect(tpEffect, new EffectData
                {
                    scale = 0.66f,
                    origin = currentPosition
                }, true);

                yield return new WaitForSeconds(0.33f);

                HandleTeleport(PickTeleportPosition());

                yield return new WaitForSeconds(0.33f);

                EffectManager.SpawnEffect(tpEffect, new EffectData
                {
                    scale = 0.66f,
                    origin = currentPosition
                }, true);
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
                return aggressiveTeleportCooldown;
            }

            public void HandleTeleport(Vector3 nextPosition)
            {
                nextPosition += new Vector3(0, 1, 0);

                if (cb.isPlayerControlled)
                {
                    return;
                }

                if (wardInstance != null)
                {
                    NetworkServer.Destroy(wardInstance);
                }

                var currentPosition = transform.position;

                wardInstance = Object.Instantiate(SpeedAura, nextPosition, Quaternion.identity);
                wardInstance.GetComponent<BuffWard>().Networkradius = Util.Remap(cb.baseMaxHealth, 0f, 2100f, minSpeedAuraRadius, maxSpeedAuraRadius);
                wardInstance.GetComponent<TeamFilter>().teamIndex = cb.teamComponent.teamIndex;
                NetworkServer.Spawn(wardInstance);

                EffectManager.SpawnEffect(tpTracer, new EffectData
                {
                    start = currentPosition,
                    origin = nextPosition
                }, true);

                TeleportHelper.TeleportBody(cb, nextPosition);
            }

            /*
            public Vector3[] PickValidPositions(float min, float max, NodeGraph.Node[] nodes)
            {
                NodeGraph.Node[] validNodes = nodes.Where(x => Vector3.Distance(x.position, transform.position) > min && Vector3.Distance(x.position, transform.position) < max).ToArray();
                if (validNodes.Length <= 1)
                {
                    return new Vector3[] { transform.position };
                }
                return validNodes.Select(node => node.position).ToArray();
            }
            */

            public Vector3[] PickValidPositions(float min, float max, NodeGraph.Node[] nodes)
            {
                List<Vector3> validPositions = new();

                foreach (NodeGraph.Node node in nodes)
                {
                    float distance = Vector3.Distance(node.position, transform.position);
                    if (distance > min && distance < max)
                    {
                        validPositions.Add(node.position);
                    }
                }

                if (validPositions.Count <= 1)
                {
                    return new Vector3[] { transform.position };
                }

                return validPositions.ToArray();
            }

            public Vector3 PickTeleportPosition()
            {
                if (!SceneInfo.instance || !SceneInfo.instance.groundNodes)
                {
                    return transform.position;
                }

                NodeGraph.Node[] nodes = SceneInfo.instance.groundNodes.nodes;
                Vector3[] validPositions;
                validPositions = PickValidPositions(15, 35, nodes);
                return validPositions.GetRandom();
            }
        }
    }
}