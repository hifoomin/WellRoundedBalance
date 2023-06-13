using System;
using System.Collections;

namespace WellRoundedBalance.Items.Greens
{
    public class HarvestersScythe : ItemBase<HarvestersScythe>
    {
        public override string Name => ":: Items :: Greens :: Harvesters Scythe";
        public override ItemDef InternalPickup => RoR2Content.Items.HealOnCrit;

        public override string PickupText => "Activating your Secondary skill also swings a scythe. Recharges over time.";

        public override string DescText => "Activating your <style=cIsUtility>Secondary skill</style> also swings a <style=cIsDamage>scythe</style> that deals <style=cIsDamage>" + d(baseDamage) + "</style>" +
                                           (damagePerStack > 0 ? " <style=cStack>(+" + d(damagePerStack) + " per stack)</style>" : "") +
                                           " base damage. Hitting enemies with the <style=cIsDamage>scythe</style> grants <style=cIsDamage>+" + baseCritGain + "%</style>" +
                                           (critGainPerStack > 0 ? " <style=cStack>(+" + critGainPerStack + "% per stack)</style>" : "") + " <style=cIsDamage>crit chance</style> for <style=cIsDamage>" + baseBuffDuration + "s</style>" +
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

        [ConfigField("Base Buff Duration", 4f)]
        public static float baseBuffDuration;

        [ConfigField("Buff Duration Per Stack", 0f)]
        public static float buffDurationPerStack;

        public static BuffDef scytheCooldown;
        public static BuffDef scytheCrit;

        public override void Init()
        {
            scytheCooldown = ScriptableObject.CreateInstance<BuffDef>();
            scytheCooldown.isCooldown = true;
            scytheCooldown.isDebuff = false;
            scytheCooldown.isHidden = false;
            scytheCooldown.canStack = false;
            scytheCooldown.buffColor = new Color(0.4151f, 0.4014f, 0.4014f, 1f); // wolfo consistency :kirn:
            scytheCooldown.iconSprite = Utils.Paths.BuffDef.bdPrimarySkillShurikenBuff.Load<BuffDef>().iconSprite;
            scytheCooldown.name = "Harvesters Scythe Cooldown";

            ContentAddition.AddBuffDef(scytheCooldown);

            scytheCrit = ScriptableObject.CreateInstance<BuffDef>();
            scytheCrit.isCooldown = false;
            scytheCrit.isDebuff = false;
            scytheCrit.isHidden = false;
            scytheCrit.canStack = false;
            scytheCrit.buffColor = new Color32(50, 200, 50, 255);
            scytheCrit.iconSprite = Utils.Paths.BuffDef.bdFullCrit.Load<BuffDef>().iconSprite;
            scytheCrit.name = "Harvesters Scythe Crit";

            ContentAddition.AddBuffDef(scytheCrit);

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
                if (stack > 0)
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
        public BoxCollider boxCollider;
        public OverlapAttack overlapAttack;
        public ModelLocator modelLocator;
        public Transform modelTransform;
        public HitBoxGroup hitBoxGroup;
        public HitBox hitBox;

        public void Start()
        {
            modelLocator = GetComponent<ModelLocator>();
            modelTransform = modelLocator?.modelTransform;
            if (modelTransform && hitBox == null && hitBoxGroup == null && boxCollider == null)
            {
                /*
                boxCollider = modelTransform.gameObject.AddComponent<BoxCollider>();
                boxCollider.
                */
                // the fucking
                // make the boxcollider real and give it a size similar to bandit's m2
                // idk if the code for overlapattack works, idk how to check if it hit something
                hitBox = modelTransform.gameObject.AddComponent<HitBox>();
                hitBoxGroup = modelTransform.gameObject.AddComponent<HitBoxGroup>();
                hitBoxGroup.groupName = "WRBScythe";
                hitBoxGroup.hitBoxes = new HitBox[] { hitBox };
            }
            damage = HarvestersScythe.baseDamage + HarvestersScythe.damagePerStack * (stack - 1);
            cooldown = HarvestersScythe.cooldown;
            critGain = HarvestersScythe.baseCritGain + HarvestersScythe.critGainPerStack * (stack - 1);
            buffDur = HarvestersScythe.baseBuffDuration + HarvestersScythe.buffDurationPerStack * (stack - 1);
            skillLocator = GetComponent<SkillLocator>();
            body.onSkillActivatedAuthority += Body_onSkillActivatedAuthority;
        }

        private void Body_onSkillActivatedAuthority(GenericSkill skill)
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
            if (body.HasBuff(HarvestersScythe.scytheCooldown))
            {
                return;
            }
            StartCoroutine(FireProjectile());
            body.AddTimedBuff(HarvestersScythe.scytheCooldown, cooldown);
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
                attackerFiltering = AttackerFiltering.NeverHitSelf
            };
            if (modelLocator)
            {
                overlapAttack.hitBoxGroup = Array.Find(modelTransform.GetComponents<HitBoxGroup>(), (HitBoxGroup x) => x.groupName == "WRBScythe");
            }

            Util.PlaySound("Play_bandit_M2_shot", gameObject);

            if (Util.HasEffectiveAuthority(gameObject))
            {
                if (overlapAttack.Fire(null))
                {
                    body.AddTimedBuff(HarvestersScythe.scytheCrit, buffDur);
                }
            }
            yield return null;
        }

        public void OnDestroy()
        {
            body.onSkillActivatedAuthority -= Body_onSkillActivatedAuthority;
        }
    }
}