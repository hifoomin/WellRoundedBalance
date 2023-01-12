using System;
using System.Collections.Generic;
using System.Text;
using RoR2.Navigation;
using RoR2.Orbs;
using WellRoundedBalance.Enemies;

namespace WellRoundedBalance.Elites
{
    internal class Overloading : EnemyBase<Overloading>
    {
        public override string Name => "Elites ::: Overloading";
        public override void Hooks()
        {
            On.RoR2.Projectile.ProjectileController.Start += (orig, self) => {
                orig(self);
                if (self.gameObject.name.Contains("LightningStake")) {
                    GameObject.DestroyImmediate(self.gameObject);
                }
            };

            On.RoR2.CharacterBody.RecalculateStats += (orig, self) => {
                orig(self);
                if (NetworkServer.active && self.HasBuff(RoR2Content.Buffs.AffixBlue)) {
                    self.moveSpeed *= 1.5f;
                    if (!self.GetComponent<OverloadingController>()) {
                        self.gameObject.AddComponent<OverloadingController>();
                    }
                }
                if (!self.HasBuff(RoR2Content.Buffs.AffixBlue)) {
                    if (self.GetComponent<OverloadingController>()) {
                        self.gameObject.RemoveComponent<OverloadingController>();
                    }
                }
            };
        }
        private class OverloadingController : MonoBehaviour, IOnTakeDamageServerReceiver {
            private float stopwatch = 0f;
            private float teleportCooldown = 5f;
            private bool isOnCooldown = false;
            private SphereSearch search;
            private HealthComponent hc => GetComponent<HealthComponent>();
            private CharacterBody cb => GetComponent<CharacterBody>();
            private void Start() {
                List<IOnTakeDamageServerReceiver> receivers = hc.onTakeDamageReceivers.ToList();
                receivers.Add(this);
                hc.onTakeDamageReceivers = receivers.ToArray();
                search = new();
                search.radius = 60;
                search.mask = LayerIndex.entityPrecise.mask;
                search.queryTriggerInteraction = QueryTriggerInteraction.Ignore;
            }

            public void OnTakeDamageServer(DamageReport report) {
                if (NetworkServer.active && report.victimBody && report.victimBody == cb && !isOnCooldown) {
                    NodeGraph nodes = SceneInfo.instance.groundNodes;
                    if (nodes) {
                        List<NodeGraph.Node> validNodes = nodes.nodes.Where(x => Vector3.Distance(cb.corePosition, x.position) < 30).ToList();
                        NodeGraph.Node node = validNodes.GetRandom(Run.instance.spawnRng);
                        Vector3 position = node.position;
                        EffectManager.SpawnEffect(Utils.Paths.GameObject.ParentTeleportEffect.Load<GameObject>(), new EffectData {
                            origin = cb.corePosition,
                            scale = 2f,
                        }, true);
                        TeleportHelper.TeleportBody(cb, position);
                        isOnCooldown = true;
                        Invoke(nameof(BuffNearby), 0.2f);
                    }
                }
            }

            private void BuffNearby() {
                EffectManager.SpawnEffect(Utils.Paths.GameObject.LunarSecondaryExplosion.Load<GameObject>(), new EffectData {
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
                foreach (HurtBox box in boxes) {
                    if (box.teamIndex == cb.teamComponent.teamIndex && NetworkServer.active) {
                        if (box.healthComponent) {
                            LightningOrb orb = new();
                            orb.lightningType = LightningOrb.LightningType.Tesla;
                            orb.bouncesRemaining = 1;
                            orb.targetsToFindPerBounce = 1;
                            orb.attacker = base.gameObject;
                            orb.teamIndex = cb.teamComponent.teamIndex;
                            orb.damageValue = 0;
                            orb.damageType = DamageType.Silent;
                            orb.origin = cb.corePosition;
                            orb.range = float.PositiveInfinity;

                            OrbManager.instance.AddOrb(orb);
                            Debug.Log("added orb");
                            box.healthComponent.body.AddTimedBuff(DLC1Content.Buffs.KillMoveSpeed, 5f);
                        }
                    }
                }
            }

            private void FixedUpdate() {
                if (isOnCooldown) {
                    stopwatch += Time.fixedDeltaTime;

                    if (stopwatch >= teleportCooldown) {
                        stopwatch = 0f;
                        isOnCooldown = false;
                    }
                }
            }
        }
    }
}
