using System;
using RoR2.Navigation;
using WellRoundedBalance.Buffs;
using System.Collections;
using WellRoundedBalance.Gamemodes.Eclipse;
using EntityStates;

namespace WellRoundedBalance.Elites
{
    internal class Glacial : EliteBase<Glacial>
    {
        public static BuffDef slow;
        public static GameObject iceExplosionPrefab;
        public override string Name => ":: Elites : Glacial";
        public static GameObject IcePillarPrefab;
        public static GameObject IcePillarWalkerPrefab;

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

            IcePillarPrefab = PrefabAPI.InstantiateClone(Utils.Paths.GameObject.MageIcewallPillarProjectile.Load<GameObject>(), "Glacial Elite Pillar");

            /*
            var projectileImpactExplosion = IcePillarPrefab.GetComponent<ProjectileImpactExplosion>();
            projectileImpactExplosion.blastRadius = 0f;
            projectileImpactExplosion.blastDamageCoefficient = 0f;
            projectileImpactExplosion.blastProcCoefficient = 0f;
            projectileImpactExplosion.lifetime = 8f; // 8f - 0.05f * 8 for max without overlap
            projectileImpactExplosion.destroyOnEnemy = true;
            */

            IcePillarPrefab.GetComponent<ProjectileImpactExplosion>().enabled = false;

            var projectileSimple = IcePillarPrefab.AddComponent<ProjectileSimple>();
            projectileSimple.desiredForwardSpeed = 0f;
            projectileSimple.lifetime = 7f;

            var characterBody = IcePillarPrefab.AddComponent<CharacterBody>();
            characterBody.baseMaxHealth = 90f;
            characterBody.levelMaxHealth = 27f;
            characterBody.baseMoveSpeed = 0;
            characterBody.bodyFlags |= CharacterBody.BodyFlags.Masterless;
            characterBody.baseNameToken = "WRB_ICEPILLAR_NAME";
            characterBody.subtitleNameToken = "WRB_ICEPILLAR_SUB";

            LanguageAPI.Add("WRB_ICEPILLAR_NAME", "Ice Pillar");
            LanguageAPI.Add("WRB_ICEPILLAR_SUB", "How The Fuck");

            var healthComponent = IcePillarPrefab.AddComponent<HealthComponent>();
            healthComponent.dontShowHealthbar = false;

            var hurtboxGroup = IcePillarPrefab.AddComponent<HurtBoxGroup>();

            var hurtbox = IcePillarPrefab.AddComponent<HurtBox>();
            hurtbox.healthComponent = healthComponent;
            hurtbox.hurtBoxGroup = hurtboxGroup;
            hurtbox.isBullseye = true;
            hurtbox.isSniperTarget = true;

            var esm = IcePillarPrefab.AddComponent<EntityStateMachine>();
            esm.customName = "Body";
            esm.initialStateType = new SerializableEntityStateType(typeof(Idle));
            esm.mainStateType = new SerializableEntityStateType(typeof(GenericCharacterDeath));

            var nsm = IcePillarPrefab.AddComponent<NetworkStateMachine>();
            nsm.stateMachines = new EntityStateMachine[] { esm };

            var characterDeathBehavior = IcePillarPrefab.AddComponent<CharacterDeathBehavior>();
            characterDeathBehavior.deathState = new SerializableEntityStateType(typeof(GenericCharacterDeath));
            characterDeathBehavior.deathStateMachine = esm;
            /*
            var newImpact = PrefabAPI.InstantiateClone(Utils.Paths.GameObject.OmniImpactVFXFrozen.Load<GameObject>(), "Glacial Elite Pillar Broken VFX");
            newImpact.transform.localScale = new Vector3(3f, 3f, 3f);

            projectileImpactExplosion.impactEffect = newImpact;
            */
            IcePillarPrefab.layer = LayerIndex.world.intVal;
            IcePillarPrefab.transform.localScale = new Vector3(2f, 3f, 2f);

            var newGhost = PrefabAPI.InstantiateClone(Utils.Paths.GameObject.MageIcePillarGhost.Load<GameObject>(), "Glacial Elite Pillar Ghost");
            newGhost.transform.localScale = new Vector3(2f, 2f, 2f);
            var mesh = newGhost.transform.GetChild(1);
            mesh.localPosition = new Vector3(0f, 0f, -2.5f);
            mesh.transform.localScale = new Vector3(2f, 2f, 3f);

            // var effectComponent = newGhost.AddComponent<EffectComponent>();

            var projectileController = IcePillarPrefab.GetComponent<ProjectileController>();
            projectileController.ghostPrefab = newGhost;

            IcePillarWalkerPrefab = PrefabAPI.InstantiateClone(Utils.Paths.GameObject.MageIcewallWalkerProjectile.Load<GameObject>(), "Glacial Elite Pillar Walker");
            var projectileMageFirewallWalkerController = IcePillarWalkerPrefab.GetComponent<ProjectileMageFirewallWalkerController>();
            projectileMageFirewallWalkerController.firePillarPrefab = IcePillarPrefab;

            var projectileCharacterController = IcePillarWalkerPrefab.GetComponent<ProjectileCharacterController>();
            projectileCharacterController.lifetime = 0.5f;
            projectileCharacterController.velocity = 0.01f;

            // ContentAddition.AddEffect(newGhost);
            // ContentAddition.AddEffect(newImpact);
            PrefabAPI.RegisterNetworkPrefab(IcePillarPrefab);
            PrefabAPI.RegisterNetworkPrefab(IcePillarWalkerPrefab);

            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.GlobalEventManager.OnHitEnemy += GlobalEventManager_OnHitEnemy;
            On.RoR2.GlobalEventManager.OnHitAll += GlobalEventManager_OnHitAll;
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
            IL.RoR2.CharacterModel.UpdateOverlays += CharacterModel_UpdateOverlays;
            CharacterBody.onBodyInventoryChangedGlobal += CharacterBody_onBodyInventoryChangedGlobal;
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
                        GameObject visual = attackerBody.isPlayerControlled ? null : iceExplosionPrefab;
                        if (!damageInfo.procChainMask.HasProc(procType) && Util.CheckRoll(100f * damageInfo.procCoefficient))
                        {
                            ProcChainMask mask = new();
                            mask.AddProc(procType);
                            DebuffSphere(slow.buffIndex, attackerBody.teamComponent.teamIndex, damageInfo.position, 4f, 1.5f, visual, null, false, true, null);
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

        private void CharacterBody_onBodyInventoryChangedGlobal(CharacterBody characterBody)
        {
            var sfp = characterBody.GetComponent<GlacialController>();
            if (characterBody.HasBuff(RoR2Content.Buffs.AffixWhite))
            {
                if (sfp == null)
                {
                    characterBody.gameObject.AddComponent<GlacialController>();
                }
            }
            else if (sfp != null)
            {
                characterBody.gameObject.RemoveComponent<GlacialController>();
            }
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.HasBuff(slow))
            {
                args.moveSpeedReductionMultAdd += 0.8f;
            }
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

        public class GlacialController : MonoBehaviour
        {
            public float timer;
            public float interval = 7f;
            public int projectileCount = 10;

            public void Start()
            {
                projectileCount = Eclipse3.CheckEclipse() ? 15 : 10;
            }

            public void FixedUpdate()
            {
                timer += Time.fixedDeltaTime;
                if (timer >= interval)
                {
                    timer = 0f;
                    var point = PickRandomPosition();
                    StartCoroutine(SummonProjectiles(point));
                }
            }

            public IEnumerator SummonProjectiles(Vector3 point)
            {
                for (int i = 0; i < projectileCount; i++)
                {
                    var fpi = new FireProjectileInfo()
                    {
                        crit = false,
                        damage = 0,
                        force = 0,
                        owner = gameObject,
                        position = point + new Vector3(i * 2f, 0, 0),
                        rotation = Quaternion.Euler(270f, 0f, 0f),
                        projectilePrefab = IcePillarPrefab
                    };
                    ProjectileManager.instance.FireProjectile(fpi);
                    /*
                    var pillar = GameObject.Instantiate(IcePillarPrefab, point + new Vector3(i * 2f, 0, 0), Quaternion.Euler(270f, 0f, 0f));
                    NetworkServer.Spawn(pillar);
                    */
                    yield return new WaitForSeconds(0.05f);
                }

                yield return null;
            }

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

            public Vector3 PickRandomPosition()
            {
                if (!SceneInfo.instance || !SceneInfo.instance.groundNodes)
                {
                    return transform.position;
                }

                NodeGraph.Node[] nodes = SceneInfo.instance.groundNodes.nodes;
                Vector3[] validPositions;
                validPositions = PickValidPositions(5, 15, nodes);
                return validPositions.GetRandom();
            }
        }
    }
}