using R2API;
using RoR2;
using RoR2.Projectile;
using RoR2.Skills;
using System.Collections.Generic;
using UnityEngine;

namespace UltimateCustomRun.Survivors
{
    public static class Loader
    {
        public static SkillLocator skillLocator;
        public static SkillFamily primary;
        public static SkillFamily secondary;
        public static SkillFamily utility;
        public static SkillFamily special;

        public static GameObject ZasedFireHook = PrefabAPI.InstantiateClone(LegacyResourcesAPI.Load<GameObject>("prefabs/projectiles/LoaderYankHook"), "prefabs/projectiles/LoaderZasedHook", true);
        public static GameObject ZasedThrowPylon = PrefabAPI.InstantiateClone(LegacyResourcesAPI.Load<GameObject>("prefabs/projectiles/LoaderPylon"), "prefabs/projectiles/LoaderZasedPylon", true);

        public static void GetSkillsFromBodyObject(GameObject bodyObject)
        {
            if (bodyObject != null)
            {
                skillLocator = bodyObject.GetComponent<SkillLocator>();
                if (skillLocator)
                {
                    primary = skillLocator.primary.skillFamily;
                    secondary = skillLocator.secondary.skillFamily;
                    utility = skillLocator.utility.skillFamily;
                    special = skillLocator.special.skillFamily;
                }
            }
        }

        public static void Changes()
        {
            GetSkillsFromBodyObject(LegacyResourcesAPI.Load<GameObject>("prefabs/characterbodies/LoaderBody"));

            SkillDef ZasedFireHookDef = ScriptableObject.CreateInstance<SkillDef>();
            ZasedFireHookDef.activationState = new EntityStates.SerializableEntityStateType(typeof(ZasedGrapple));
            ZasedFireHookDef.activationStateMachineName = "Hook";
            ZasedFireHookDef.baseRechargeInterval = 0.5f;
            ZasedFireHookDef.baseMaxStock = 1;
            ZasedFireHookDef.rechargeStock = 1;
            ZasedFireHookDef.requiredStock = 1;
            ZasedFireHookDef.stockToConsume = 1;
            ZasedFireHookDef.resetCooldownTimerOnUse = false;
            ZasedFireHookDef.fullRestockOnAssign = true;
            ZasedFireHookDef.dontAllowPastMaxStocks = false;
            ZasedFireHookDef.beginSkillCooldownOnSkillEnd = true;
            ZasedFireHookDef.cancelSprintingOnActivation = true;
            ZasedFireHookDef.forceSprintDuringState = false;
            ZasedFireHookDef.canceledFromSprinting = false;
            ZasedFireHookDef.isCombatSkill = true;
            ZasedFireHookDef.mustKeyPress = true;
            ZasedFireHookDef.interruptPriority = EntityStates.InterruptPriority.Any;
            ZasedFireHookDef.icon = LegacyResourcesAPI.Load<Sprite>("textures/achievementicons/texloaderspeedrunicon");
            ZasedFireHookDef.skillDescriptionToken = "LOADER_YANKHOOK_DESCRIPTION";
            ZasedFireHookDef.skillName = "LOADER_YANKHOOK_NAME";
            ZasedFireHookDef.skillNameToken = "LOADER_YANKHOOK_NAME";

            SkillDef ZasedThrowPylonDef = ScriptableObject.CreateInstance<SkillDef>();
            ZasedThrowPylonDef.activationState = new EntityStates.SerializableEntityStateType(typeof(ZasedPylon));
            ZasedThrowPylonDef.activationStateMachineName = "Pylon";
            ZasedThrowPylonDef.baseRechargeInterval = 7f;
            ZasedThrowPylonDef.baseMaxStock = 1;
            ZasedThrowPylonDef.rechargeStock = 1;
            ZasedThrowPylonDef.requiredStock = 1;
            ZasedThrowPylonDef.stockToConsume = 1;
            ZasedThrowPylonDef.resetCooldownTimerOnUse = false;
            ZasedThrowPylonDef.fullRestockOnAssign = true;
            ZasedThrowPylonDef.dontAllowPastMaxStocks = false;
            ZasedThrowPylonDef.beginSkillCooldownOnSkillEnd = true;
            ZasedThrowPylonDef.cancelSprintingOnActivation = true;
            ZasedThrowPylonDef.forceSprintDuringState = false;
            ZasedThrowPylonDef.canceledFromSprinting = false;
            ZasedThrowPylonDef.isCombatSkill = true;
            ZasedThrowPylonDef.mustKeyPress = true;
            ZasedThrowPylonDef.interruptPriority = EntityStates.InterruptPriority.Any;
            ZasedThrowPylonDef.icon = LegacyResourcesAPI.Load<SkillDef>("skilldefs/loaderbody/ThrowPylon").icon;
            ZasedThrowPylonDef.skillDescriptionToken = "LOADER_SPECIAL_DESCRIPTION";
            ZasedThrowPylonDef.skillName = "LOADER_SPECIAL_NAME";
            ZasedThrowPylonDef.skillNameToken = "LOADER_SPECIAL_NAME";

            Main.projectilePrefabContent.Add(ZasedFireHook);
            Main.skillDefContent.Add(ZasedFireHookDef);
            Main.RegisterType(typeof(ZasedGrapple));

            Main.projectilePrefabContent.Add(ZasedThrowPylon);
            Main.skillDefContent.Add(ZasedThrowPylonDef);
            Main.RegisterType(typeof(ZasedPylon));

            primary.variants[0].skillDef = ZasedFireHookDef;
            secondary.variants[0].skillDef = ZasedThrowPylonDef;
            /*
            On.RoR2.Loadout.GenerateViewables += (orig) =>
            {
                List<SkillFamily.Variant> secondaries = new(secondary.variants);
                secondaries.RemoveAt(1);
                List<SkillFamily.Variant> specials = new(special.variants);
                specials.RemoveAt(0);
                orig();
            };
            */
            // skillfamily variants removal doesnt work ig??
        }
    }

    public class ZasedGrapple : EntityStates.Loader.FireHook
    {
        public override void OnEnter()
        {
            var h = Loader.ZasedFireHook.GetComponent<ProjectileGrappleController>();
            h.yankMassLimit = 0f;
            projectilePrefab = Loader.ZasedFireHook;
            base.OnEnter();
        }
    }

    public class ZasedPylon : EntityStates.Loader.ThrowPylon
    {
        public override void OnEnter()
        {
            var p = Loader.ZasedThrowPylon.GetComponent<ProjectileProximityBeamController>();
            Loader.ZasedThrowPylon.GetComponent<ProjectileSimple>().lifetime = 3f;
            p.attackFireCount = 1000;
            p.attackInterval = 0.25f;
            p.attackRange = 35f;
            p.procCoefficient = 0.3f;
            p.damageCoefficient = 0.25f;
            p.bounces = 3;
            projectilePrefab = Loader.ZasedThrowPylon;
            base.OnEnter();
        }
    }
}