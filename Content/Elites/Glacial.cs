using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using RoR2.Navigation;
using WellRoundedBalance.Buffs;

namespace WellRoundedBalance.Elites
{
    internal class Glacial : EliteBase
    {
        public static BuffDef slow;
        public static GameObject iceExplosionPrefab;
        public override string Name => ":: Elites :::: Glacial";
        public static GameObject IcePillarPrefab;

        public override void Init()
        {
            iceExplosionPrefab = Utils.Paths.GameObject.AffixWhiteExplosion.Load<GameObject>();

            var slow80 = Utils.Paths.Texture2D.texBuffSlow50Icon.Load<Texture2D>();

            slow = ScriptableObject.CreateInstance<BuffDef>();
            slow.name = "Glacial Elite Slow";
            slow.buffColor = new Color32(165, 222, 237, 255);
            slow.iconSprite = Sprite.Create(slow80, new Rect(0f, 0f, (float)slow80.width, (float)slow80.height), new Vector2(0f, 0f));
            slow.isDebuff = true;
            slow.canStack = false;
            slow.isHidden = false;

            ContentAddition.AddBuffDef(slow);

            IcePillarPrefab = Utils.Paths.GameObject.MageIcewallPillarProjectile.Load<GameObject>().InstantiateClone("GlacialPillar");
            IcePillarPrefab.RemoveComponent<ProjectileDamage>();
            IcePillarPrefab.RemoveComponent<ProjectileImpactExplosion>();
            IcePillarPrefab.RemoveComponent<ProjectileController>();
            IcePillarPrefab.layer = LayerIndex.world.intVal;
            IcePillarPrefab.transform.localScale *= 2;
            
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.GlobalEventManager.OnHitEnemy += GlobalEventManager_OnHitEnemy;
            On.RoR2.GlobalEventManager.OnHitAll += GlobalEventManager_OnHitAll;
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
            IL.RoR2.CharacterModel.UpdateOverlays += CharacterModel_UpdateOverlays;
        }

        private void GlobalEventManager_OnHitAll(On.RoR2.GlobalEventManager.orig_OnHitAll orig, GlobalEventManager self, DamageInfo damageInfo, GameObject hitObject)
        {
            var attacker = damageInfo.attacker;
            if (attacker)
            {
                var attackerBody = damageInfo.attacker.GetComponent<CharacterBody>();
                if (attackerBody)
                {
                    if (attackerBody.HasBuff(RoR2Content.Buffs.AffixWhite))
                    {
                        var procType = (ProcType)1258907;
                        if (!damageInfo.procChainMask.HasProc(procType) && Util.CheckRoll(100f * damageInfo.procCoefficient))
                        {
                            ProcChainMask mask = new();
                            mask.AddProc(procType);
                            DebuffSphere(slow.buffIndex, attackerBody.teamComponent.teamIndex, damageInfo.position, 4f, 1.5f, iceExplosionPrefab, null, false, true, null);
                        }
                    }
                }
                orig(self, damageInfo, hitObject);
            }
        }

        private void CharacterModel_UpdateOverlays(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                 x => x.MatchLdsfld(typeof(RoR2Content.Buffs), "Slow80")))
            {
                c.Index += 2;
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<bool, CharacterModel, bool>>((hasBuff, self) =>
                {
                    return hasBuff || (self.body.HasBuff(slow));
                });
            }
            else
            {
                Logger.LogError("Failed to apply Glacial Elite Overlay hook");
            }
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.HasBuff(slow))
            {
                args.moveSpeedReductionMultAdd += 0.8f;
            }

            /*bool flag = sender.HasBuff(RoR2Content.Buffs.AffixWhite);
            GlacialPillarController controller = sender.GetComponent<GlacialPillarController>();

            if (flag != controller)
            {
                if (flag)
                {
                    sender.gameObject.AddComponent<GlacialPillarController>();
                }
                else
                {
                    sender.gameObject.RemoveComponent<GlacialPillarController>();
                }
            }*/
        }

        private void GlobalEventManager_OnHitEnemy(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdsfld(typeof(RoR2Content.Buffs), "AffixWhite")))
            {
                c.Remove();
                c.Emit<Useless>(OpCodes.Ldsfld, nameof(Useless.uselessBuff));
            }
            else
            {
                Logger.LogError("Failed to apply Glacial Elite On Hit hook");
            }
        }

        private void DebuffSphere(BuffIndex buff, TeamIndex team, Vector3 position, float radius, float debuffDuration, GameObject effect, GameObject hitEffect, bool ignoreImmunity, bool falloff, NetworkSoundEventDef buffSound)
        {
            if (!NetworkServer.active)
            {
                return;
            }

            if (effect != null)
            {
                EffectManager.SpawnEffect(effect, new EffectData
                {
                    origin = position,
                    scale = radius
                }, true);
            }
            float radiusHalfwaySqr = radius * radius * 0.25f;
            List<HealthComponent> healthComponentList = new();
            Collider[] colliders = Physics.OverlapSphere(position, radius, LayerIndex.entityPrecise.mask);
            for (int i = 0; i < colliders.Length; i++)
            {
                HurtBox hurtBox = colliders[i].GetComponent<HurtBox>();
                if (hurtBox)
                {
                    var healthComponent = hurtBox.healthComponent;
                    var projectileController = colliders[i].GetComponentInParent<ProjectileController>();
                    if (healthComponent && !projectileController && !healthComponentList.Contains(healthComponent))
                    {
                        healthComponentList.Add(healthComponent);
                        if (healthComponent.body.teamComponent && healthComponent.body.teamComponent.teamIndex != team)
                        {
                            if (ignoreImmunity || (!healthComponent.body.HasBuff(RoR2Content.Buffs.Immune) && !healthComponent.body.HasBuff(RoR2Content.Buffs.HiddenInvincibility)))
                            {
                                float effectiveness = 1f;
                                if (falloff)
                                {
                                    float distSqr = (position - hurtBox.collider.ClosestPoint(position)).sqrMagnitude;
                                    if (distSqr > radiusHalfwaySqr)  //Reduce effectiveness when over half the radius away
                                    {
                                        effectiveness *= 0.5f;  //0.25 is vanilla sweetspot
                                    }
                                }
                                bool alreadyHasBuff = healthComponent.body.HasBuff(buff);
                                healthComponent.body.AddTimedBuff(buff, effectiveness * debuffDuration);
                                if (!alreadyHasBuff)
                                {
                                    if (hitEffect != null)
                                    {
                                        EffectManager.SpawnEffect(hitEffect, new EffectData
                                        {
                                            origin = healthComponent.body.corePosition
                                        }, true);
                                    }
                                    if (buffSound != null)
                                    {
                                        EffectManager.SimpleSoundEffect(buffSound.index, healthComponent.body.corePosition, true);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public class GlacialPillar : MonoBehaviour {
            public void Die(object s, EventArgs e) {
                Destroy(base.gameObject);
            }
        }

        public class GlacialPillarController : MonoBehaviour {
            private int totalActive;
            private int maxActive = 4;
            internal EventHandler onDeath;
            private float stopwatch = 0f;
            private float delay = 5f;

            public void FixedUpdate() {
                stopwatch += Time.fixedDeltaTime;
                if (totalActive < maxActive && stopwatch >= delay) {
                    stopwatch = 0f;

                    Vector3 point = PickTeleportPosition();
                    GameObject pillar = GameObject.Instantiate(IcePillarPrefab, point, Quaternion.Euler(-90, 0, 0));
                    onDeath += pillar.GetComponent<GlacialPillar>().Die;
                    totalActive++;
                }
            }

            public void OnDestroy() {
                onDeath?.Invoke(null, null);
            }

            public void OnDisable() {
                onDeath?.Invoke(null, null);
            }

            public Vector3 PickTeleportPosition()
            {
                if (!SceneInfo.instance || !SceneInfo.instance.groundNodes)
                {
                    return transform.position;
                }

                NodeGraph.Node[] nodes = SceneInfo.instance.groundNodes.nodes;
                return PickValidPositions(10, 40, nodes).ToList().GetRandom();
            }

            public Vector3[] PickValidPositions(float min, float max, NodeGraph.Node[] nodes)
            {
                NodeGraph.Node[] validNodes = nodes.Where(x => Vector3.Distance(x.position, transform.position) > min && Vector3.Distance(x.position, transform.position) < max).ToArray();
                if (validNodes.Length <= 1)
                {
                    return new Vector3[] { transform.position };
                }
                List<Vector3> guh = new();
                foreach (NodeGraph.Node node in validNodes)
                {
                    guh.Add(node.position);
                }
                return guh.ToArray();
            }
        }
    }
}