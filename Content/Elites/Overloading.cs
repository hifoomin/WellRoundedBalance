using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2.Navigation;
using RoR2.Orbs;
using WellRoundedBalance.Buffs;
using WellRoundedBalance.Eclipse;

namespace WellRoundedBalance.Elites
{
    internal class Overloading : EliteBase
    {
        public static BuffDef overloadingSpeedBuff;
        public override string Name => ":: Elites ::: Overloading";

        [ConfigField("Passive Movement Speed Gain", "Decimal.", 0.5f)]
        public static float passiveMovementSpeedGain;

        [ConfigField("Ally Buff Count", "", 3)]
        public static int allyBuffCount;

        [ConfigField("Ally Buff Count Eclipse 3+", "Only applies if you have Eclipse Changes enabled.", 4)]
        public static int allyBuffCountE3;

        [ConfigField("Ally Buff Movement Speed Gain", "Decimal.", 0.5f)]
        public static float allyBuffMovementSpeedGain;
        private static GameObject SpeedAura;

        public override void Init()
        {
            var speedBuff = Utils.Paths.Texture2D.texBuffKillMoveSpeed.Load<Texture2D>();

            overloadingSpeedBuff = ScriptableObject.CreateInstance<BuffDef>();
            overloadingSpeedBuff.isHidden = false;
            overloadingSpeedBuff.isDebuff = false;
            overloadingSpeedBuff.canStack = false;
            overloadingSpeedBuff.buffColor = new Color32(66, 98, 219, 255);
            overloadingSpeedBuff.iconSprite = Sprite.Create(speedBuff, new Rect(0f, 0f, (float)speedBuff.width, (float)speedBuff.height), new Vector2(0f, 0f));

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

            SpeedAura.GetComponent<BuffWard>().buffDef = overloadingSpeedBuff;
            SpeedAura.GetComponent<BuffWard>().expires = false;
            SpeedAura.GetComponent<BuffWard>().expireDuration = 10000;

            SpeedAura.RemoveComponent<SlowDownProjectiles>();

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
                Main.WRBLogger.LogError("Failed to apply Overloading Deletion 2 hook");
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
                Main.WRBLogger.LogError("Failed to apply Overloading Deletion 1 hook");
            }
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender && sender.HasBuff(overloadingSpeedBuff) && !sender.HasBuff(RoR2Content.Buffs.AffixBlue))
            {
                args.moveSpeedMultAdd += allyBuffMovementSpeedGain;
            }
        }

        private class OverloadingController : MonoBehaviour
        {
            public HealthComponent hc;
            public CharacterBody cb;
            public GameObject wardInstance;
            public BuffWard ward;
            public float stopwatch = 0f;

            public void Start() {
                hc = GetComponent<HealthComponent>();
                cb = hc.body;

                if (NetworkServer.active) {
                    wardInstance = GameObject.Instantiate(SpeedAura, transform);
                    ward = wardInstance.GetComponent<BuffWard>();
                    ward.radius = Util.Remap(cb.maxHealth, 0, 2500, 4, 25);
                    NetworkServer.Spawn(wardInstance);
                }
            }

            public void FixedUpdate() {
                stopwatch += Time.fixedDeltaTime;
                if (stopwatch >= GetDelay()) {
                    stopwatch = 0f;
                    HandleTeleport(PickTeleportPosition());
                }
            }

            public void OnDestroy() {
                if (NetworkServer.active) {
                    Destroy(wardInstance);
                }
            }

            public float GetDelay() {
                return (hc.health / hc.fullCombinedHealth) > 0.5 ? 3f : 5f;
            }

            public void HandleTeleport(Vector3 pos) {
                if (cb.isPlayerControlled) {
                    return;
                }
                Vector3 current = transform.position;
                EffectManager.SpawnEffect(Utils.Paths.GameObject.ParentTeleportEffect.Load<GameObject>(), new EffectData {
                    scale = 1,
                    origin = current
                }, true);
                EffectManager.SpawnEffect(Utils.Paths.GameObject.ParentTeleportEffect.Load<GameObject>(), new EffectData {
                    scale = 1,
                    origin = pos
                }, true);
                TeleportHelper.TeleportBody(cb, pos + new Vector3(0, 1, 0));
            }

            public Vector3 PickTeleportPosition() {
                if (!SceneInfo.instance || !SceneInfo.instance.groundNodes) {
                    return transform.position;
                }

                NodeGraph.Node[] nodes = SceneInfo.instance.groundNodes.nodes;
                if (GetDelay() == 3f) {
                    return PickValidPositions(0, 25, nodes).ToList().GetRandom();
                }
                else {
                    return PickValidPositions(20, 40, nodes).ToList().GetRandom();
                }
            }

            public Vector3[] PickValidPositions(float min, float max, NodeGraph.Node[] nodes) {
                NodeGraph.Node[] validNodes = nodes.Where(x => Vector3.Distance(x.position, transform.position) > min && Vector3.Distance(x.position, transform.position) < max).ToArray();
                if (validNodes.Length <= 1) {
                    return new Vector3[] { transform.position };
                }
                List<Vector3> guh = new();
                foreach (NodeGraph.Node node in validNodes) {
                    guh.Add(node.position);
                }
                return guh.ToArray();
            }
        }
    }
}