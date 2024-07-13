using System;
using System.Collections;

namespace WellRoundedBalance.Items.Greens
{
    public class HarvestersScythe : ItemBase<HarvestersScythe>
    {
        public override string Name => ":: Items :: Greens :: Harvesters Scythe";
        public override ItemDef InternalPickup => RoR2Content.Items.HealOnCrit;

        public override string PickupText => "Activating your Secondary skill also swings a scythe. Recharges over time.";

        public override string DescText => "Activating your <style=cIsUtility>Secondary skill</style> also swings a <style=cIsDamage>scythe</style> for <style=cIsDamage>" + d(baseDamage) + "</style>" +
                                           (damagePerStack > 0 ? " <style=cStack>(+" + d(damagePerStack) + " per stack)</style>" : "") +
                                           " damage, increasing <style=cIsDamage>crit chance</style> by <style=cIsDamage>+" + baseCritGain + "%</style>" +
                                           (critGainPerStack > 0 ? " <style=cStack>(+" + critGainPerStack + "% per stack)</style>" : "") + " for <style=cIsDamage>" + baseBuffDuration + "s</style>" +
                                           (buffDurationPerStack > 0 ? " <style=cStack>(+" + buffDurationPerStack + "s per stack)</style>" : "") + ". The <style=cIsDamage>scythe</style> renews over <style=cIsDamage>" + cooldown + "</style> seconds.";

        [ConfigField("Base Damage", 2f)]
        public static float baseDamage;

        [ConfigField("Damage Per Stack", 0f)]
        public static float damagePerStack;

        [ConfigField("Cooldown", 3f)]
        public static float cooldown;

        [ConfigField("Base Crit Gain", 30f)]
        public static float baseCritGain;

        [ConfigField("Crit Gain Per Stack", 30f)]
        public static float critGainPerStack;

        [ConfigField("Base Buff Duration", 5f)]
        public static float baseBuffDuration;

        [ConfigField("Buff Duration Per Stack", 0f)]
        public static float buffDurationPerStack;

        public static BuffDef scytheCooldown;
        public static BuffDef scytheCrit;

        public static NetworkSoundEventDef scytheSound = Addressables.LoadAssetAsync<NetworkSoundEventDef>("RoR2/Base/Bandit2/nseBandit2ShivHit.asset").WaitForCompletion();

        public static GameObject effect;

        public override void Init()
        {
            scytheCooldown = ScriptableObject.CreateInstance<BuffDef>();
            scytheCooldown.isCooldown = true;
            scytheCooldown.isDebuff = false;
            scytheCooldown.isHidden = false;
            scytheCooldown.canStack = false;
            scytheCooldown.buffColor = new Color(0.4151f, 0.4014f, 0.4014f, 1f); // wolfo consistency :kirn:
            scytheCooldown.iconSprite = Main.wellroundedbalance.LoadAsset<Sprite>("Assets/WellRoundedBalance/texBuffScythe.png");
            scytheCooldown.name = "Harvesters Scythe Cooldown";

            ContentAddition.AddBuffDef(scytheCooldown);

            scytheCrit = ScriptableObject.CreateInstance<BuffDef>();
            scytheCrit.isCooldown = true;
            scytheCrit.isDebuff = false;
            scytheCrit.isHidden = false;
            scytheCrit.canStack = false;
            scytheCrit.buffColor = new Color32(50, 182, 50, 255);
            scytheCrit.iconSprite = Main.wellroundedbalance.LoadAsset<Sprite>("Assets/WellRoundedBalance/texBuffScytheActive.png");
            scytheCrit.name = "Harvesters Scythe Crit";

            ContentAddition.AddBuffDef(scytheCrit);

            effect = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/Assassin2/AssassinSlash.prefab").WaitForCompletion(), "Harvesters Scythe Effect", false);

            var scaleParticleSystemDuration = effect.GetComponent<ScaleParticleSystemDuration>();
            // scaleParticleSystemDuration.initialDuration = 0.2f;

            var effectComponent = effect.GetComponent<EffectComponent>();
            effectComponent.applyScale = true;

            var swingTrail = effect.transform.GetChild(0);
            var swingTrailPS = swingTrail.GetComponent<ParticleSystem>();
            var main = swingTrailPS.main;
            // main.startLifetime = 0.2f;
            var rotationOverLifetime = swingTrailPS.rotationOverLifetime;
            rotationOverLifetime.zMultiplier = 1.1f;

            var swingTrailMat = swingTrail.GetComponent<ParticleSystemRenderer>();

            var newMat = GameObject.Instantiate(Utils.Paths.Material.matMercSwipe2.Load<Material>());
            newMat.SetColor("_TintColor", new Color32(26, 58, 27, 255));

            swingTrailMat.material = newMat;

            var swingDistortion = effect.transform.GetChild(1).GetComponent<ParticleSystem>();
            var main2 = swingDistortion.main;
            // main2.startLifetime = 0.2f;
            var rotationOverLifetime2 = swingDistortion.rotationOverLifetime;
            rotationOverLifetime2.zMultiplier = 1.13f;

            ContentAddition.AddEffect(effect);

            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.GlobalEventManager.OnCrit += GlobalEventManager_OnCrit;
            IL.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
            CharacterBody.onBodyInventoryChangedGlobal += CharacterBody_onBodyInventoryChangedGlobal;
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender && sender.inventory)
            {
                var stack = sender.inventory.GetItemCount(RoR2Content.Items.HealOnCrit);
                if (sender.HasBuff(scytheCrit))
                {
                    args.critAdd += baseCritGain + critGainPerStack * (stack - 1);
                }
            }
        }

        private void CharacterBody_onBodyInventoryChangedGlobal(CharacterBody body)
        {
            if (NetworkServer.active)
            {
                var stack = body.inventory.GetItemCount(RoR2Content.Items.HealOnCrit);
                body.AddItemBehavior<HarvestersScytheController>(stack);
            }
        }

        private void CharacterBody_RecalculateStats(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdloc(13),
                x => x.MatchLdcI4(0),
                x => x.MatchBle(out _),
                x => x.MatchLdloc(out _),
                x => x.MatchLdcR4(5f)))
            {
                c.Index += 4;
                c.Next.Operand = 0f;
            }
            else
            {
                Logger.LogError("Failed to apply Harvester's Scythe Deletion 2 hook");
            }
        }

        private void GlobalEventManager_OnCrit(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdsfld(typeof(RoR2Content.Items), "HealOnCrit")))
            {
                c.Remove();
                c.Emit<Useless>(OpCodes.Ldsfld, nameof(Useless.uselessItem));
            }
            else
            {
                Logger.LogError("Failed to apply Harvester's Scythe Deletion 1 hook");
            }
        }
    }

    public class HarvestersScytheController : CharacterBody.ItemBehavior
    {
        public SkillLocator skillLocator;
        public float damage = 2f;
        public float cooldown = 2f;
        public float buffDur = 0;
        public float critGain = 0;

        // public BoxCollider boxCollider;
        public OverlapAttack overlapAttack;

        public ModelLocator modelLocator;
        public Transform modelTransform;
        public GameObject scytheObject;
        public HitBoxGroup hitBoxGroup;
        public HitBox hitBox;

        public void Start()
        {
            modelLocator = GetComponent<ModelLocator>();
            modelTransform = modelLocator?.modelTransform;
            if (modelTransform && scytheObject == null)
            {
                scytheObject = new("WRB Scythe mf")
                {
                    layer = LayerIndex.defaultLayer.intVal
                };

                scytheObject.transform.localScale = new Vector3(20f, 10f, 20f);

                hitBox = scytheObject.AddComponent<HitBox>();
                hitBoxGroup = scytheObject.AddComponent<HitBoxGroup>();
                hitBoxGroup.groupName = "WRBScythe";
                hitBoxGroup.hitBoxes = new HitBox[] { hitBox };
            }
            damage = HarvestersScythe.baseDamage + HarvestersScythe.damagePerStack * (stack - 1);
            cooldown = HarvestersScythe.cooldown;
            critGain = HarvestersScythe.baseCritGain + HarvestersScythe.critGainPerStack * (stack - 1);
            buffDur = HarvestersScythe.baseBuffDuration + HarvestersScythe.buffDurationPerStack * (stack - 1);
            skillLocator = GetComponent<SkillLocator>();
            body.onSkillActivatedServer += Body_onSkillActivatedServer;
        }

        private void Body_onSkillActivatedServer(GenericSkill skill)
        {
            var body = skill.GetComponent<CharacterBody>();
            if (!body)
            {
                return;
            }
            if (skill != skillLocator.secondary)
            {
                return;
            }

            var hasCooldownCleaner = AboutEqual(HarvestersScythe.cooldown, buffDur) ? body.HasBuff(HarvestersScythe.scytheCooldown) || body.HasBuff(HarvestersScythe.scytheCrit) : body.HasBuff(HarvestersScythe.scytheCooldown);

            if (hasCooldownCleaner)
            {
                return;
            }

            StartCoroutine(FireProjectile());
        }

        public IEnumerator FireProjectile()
        {
            overlapAttack = new()
            {
                attacker = gameObject,
                inflictor = gameObject,
                teamIndex = TeamComponent.GetObjectTeam(gameObject),
                damage = body.damage * damage,
                forceVector = Vector3.zero,
                pushAwayForce = 0,
                attackerFiltering = AttackerFiltering.NeverHitSelf,
                impactSound = HarvestersScythe.scytheSound.index,
                procCoefficient = 0f,
                isCrit = body.RollCrit()
            };

            if (scytheObject && body.inputBank)
            {
                scytheObject.transform.forward = body.inputBank.aimDirection;
                scytheObject.transform.position = modelTransform.position;
                overlapAttack.hitBoxGroup = scytheObject.GetComponent<HitBoxGroup>();

                Util.PlaySound("Play_bandit2_m2_slash", gameObject);

                EffectData data = new() { scale = 1.66f, origin = body.corePosition, rotation = Util.QuaternionSafeLookRotation(new Vector3(body.inputBank.aimDirection.x, 0f, body.inputBank.aimDirection.z)) };
                EffectManager.SpawnEffect(HarvestersScythe.effect, data, true);

                var hasCooldownCleaner = AboutEqual(HarvestersScythe.cooldown, buffDur);

                if (overlapAttack.Fire())
                {
                    if (hasCooldownCleaner)
                    {
                        body.AddTimedBuffAuthority(HarvestersScythe.scytheCrit.buffIndex, buffDur);
                    }
                    else
                    {
                        body.AddTimedBuffAuthority(HarvestersScythe.scytheCooldown.buffIndex, HarvestersScythe.cooldown);
                        body.AddTimedBuffAuthority(HarvestersScythe.scytheCrit.buffIndex, buffDur);
                    }
                }
                else
                    body.AddTimedBuffAuthority(HarvestersScythe.scytheCooldown.buffIndex, HarvestersScythe.cooldown);
            }

            yield return null;
        }

        public void OnDestroy()
        {
            body.onSkillActivatedServer -= Body_onSkillActivatedServer;
        }

        public bool AboutEqual(float a, float b)
        {
            if (Mathf.Abs(a - b) < 0.05f)
                return true;
            return false;
        }
    }
}