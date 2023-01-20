using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2.Navigation;
using RoR2.Orbs;
using WellRoundedBalance.Eclipse;

namespace WellRoundedBalance.Elites
{
    internal class Overloading : EliteBase
    {
        public static BuffDef overloadingSpeedBuff;
        public static BuffDef useless;
        public override string Name => ":: Elites ::: Overloading";

        public override void Init()
        {
            var speedBuff = Utils.Paths.Texture2D.texBuffKillMoveSpeed.Load<Texture2D>();

            overloadingSpeedBuff = ScriptableObject.CreateInstance<BuffDef>();
            overloadingSpeedBuff.isHidden = false;
            overloadingSpeedBuff.isDebuff = false;
            overloadingSpeedBuff.canStack = false;
            overloadingSpeedBuff.buffColor = new Color32(66, 98, 219, 255);
            overloadingSpeedBuff.iconSprite = Sprite.Create(speedBuff, new Rect(0f, 0f, (float)speedBuff.width, (float)speedBuff.height), new Vector2(0f, 0f));

            useless = ScriptableObject.CreateInstance<BuffDef>();
            useless.name = "Overloading Deletion";
            useless.isHidden = true;
            useless.isDebuff = false;
            useless.canStack = false;

            ContentAddition.AddBuffDef(overloadingSpeedBuff);
            ContentAddition.AddBuffDef(useless);

            base.Init();
        }

        public override void Hooks()
        {
            On.RoR2.Projectile.ProjectileController.Start += (orig, self) =>
            {
                orig(self);
                if (self.gameObject.name.Contains("LightningStake"))
                {
                    GameObject.DestroyImmediate(self.gameObject);
                }
            };

            On.RoR2.CharacterBody.RecalculateStats += (orig, self) =>
            {
                orig(self);
                if (NetworkServer.active && self.HasBuff(RoR2Content.Buffs.AffixBlue))
                {
                    self.moveSpeed *= 1.5f;
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
            };

            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
            IL.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
        }

        private void CharacterBody_RecalculateStats(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdsfld(typeof(RoR2Content.Buffs), "AffixBlue")))
            {
                c.Remove();
                c.Emit<Overloading>(OpCodes.Ldsfld, nameof(useless));
            }
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender && sender.HasBuff(overloadingSpeedBuff))
            {
                args.moveSpeedMultAdd += 0.5f;
            }
        }

        private class OverloadingController : MonoBehaviour, IOnTakeDamageServerReceiver
        {
            private float stopwatch = 0f;
            private float teleportCooldown = 6f;
            private bool isOnCooldown = false;
            private SphereSearch search;
            private HealthComponent hc => GetComponent<HealthComponent>();
            private CharacterBody cb => GetComponent<CharacterBody>();

            private void Start()
            {
                List<IOnTakeDamageServerReceiver> receivers = hc.onTakeDamageReceivers.ToList();
                receivers.Add(this);
                hc.onTakeDamageReceivers = receivers.ToArray();
                search = new();
                search.radius = 60;
                search.mask = LayerIndex.entityPrecise.mask;
                search.queryTriggerInteraction = QueryTriggerInteraction.Ignore;
            }

            public void OnTakeDamageServer(DamageReport report)
            {
                if (NetworkServer.active && report.victimBody && report.victimBody == cb && !isOnCooldown)
                {
                    NodeGraph nodes = SceneInfo.instance.groundNodes;
                    if (nodes)
                    {
                        List<NodeGraph.Node> validNodes = nodes.nodes.Where(x => Vector3.Distance(cb.corePosition, x.position) < 30).ToList();
                        NodeGraph.Node node = validNodes.GetRandom(Run.instance.spawnRng);
                        Vector3 position = node.position;
                        EffectManager.SpawnEffect(Utils.Paths.GameObject.ParentTeleportEffect.Load<GameObject>(), new EffectData
                        {
                            origin = cb.corePosition,
                            scale = 2f,
                        }, true);
                        TeleportHelper.TeleportBody(cb, position);
                        isOnCooldown = true;
                        Invoke(nameof(BuffNearby), 0.2f);
                    }
                }
            }

            private void BuffNearby()
            {
                bool e3 = Run.instance.selectedDifficulty >= DifficultyIndex.Eclipse3 && Eclipse3.instance.isEnabled;
                EffectManager.SpawnEffect(Utils.Paths.GameObject.LunarSecondaryExplosion.Load<GameObject>(), new EffectData
                {
                    origin = cb.corePosition,
                    scale = 2f,
                }, true);
                AkSoundEngine.PostEvent(Events.Play_moonBrother_orb_slam_impact, base.gameObject);
                search.ClearCandidates();
                search.origin = cb.corePosition;
                search.RefreshCandidates();
                search.FilterCandidatesByDistinctHurtBoxEntities();
                search.OrderCandidatesByDistance();
                HurtBox[] boxes = search.GetHurtBoxes();
                foreach (HurtBox box in boxes)
                {
                    if (box.teamIndex == cb.teamComponent.teamIndex && NetworkServer.active)
                    {
                        if (box.healthComponent)
                        {
                            LightningOrb orb = new()
                            {
                                lightningType = LightningOrb.LightningType.Tesla,
                                bouncesRemaining = e3 ? 4 : 3,
                                targetsToFindPerBounce = Run.instance.selectedDifficulty >= DifficultyIndex.Eclipse3 ? 4 : 3,
                                attacker = base.gameObject,
                                teamIndex = cb.teamComponent.teamIndex,
                                damageValue = 0,
                                damageType = DamageType.Silent,
                                origin = cb.corePosition,
                                range = 10000f
                            };

                            OrbManager.instance.AddOrb(orb);
                            // Debug.Log("added orb");
                            box.healthComponent.body.AddTimedBuff(overloadingSpeedBuff, 6f);
                        }
                    }
                }
            }

            private void FixedUpdate()
            {
                if (isOnCooldown)
                {
                    stopwatch += Time.fixedDeltaTime;

                    if (stopwatch >= teleportCooldown)
                    {
                        stopwatch = 0f;
                        isOnCooldown = false;
                    }
                }
            }
        }
    }
}