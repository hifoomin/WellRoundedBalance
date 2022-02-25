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

        public static GameObject hook = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("prefabs/projectiles/LoaderYankHook"), "prefabs/projectiles/LoaderZasedHook", false);
        public static GameObject pylonn = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("prefabs/projectiles/LoaderPylon"), "prefabs/projectiles/LoaderZasedPylon", false);

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
            GetSkillsFromBodyObject(Resources.Load<GameObject>("prefabs/characterbodies/LoaderBody"));

            SkillDef grap = ScriptableObject.CreateInstance<SkillDef>();
            grap.activationState = new EntityStates.SerializableEntityStateType(typeof(ZasedGrapple));
            grap.activationStateMachineName = "Hook";
            grap.baseRechargeInterval = 0.5f;
            grap.baseMaxStock = 1;
            grap.rechargeStock = 1;
            grap.requiredStock = 1;
            grap.stockToConsume = 1;
            grap.resetCooldownTimerOnUse = false;
            grap.fullRestockOnAssign = true;
            grap.dontAllowPastMaxStocks = false;
            grap.beginSkillCooldownOnSkillEnd = true;
            grap.cancelSprintingOnActivation = true;
            grap.forceSprintDuringState = false;
            grap.canceledFromSprinting = false;
            grap.isCombatSkill = true;
            grap.mustKeyPress = true;
            grap.interruptPriority = EntityStates.InterruptPriority.Any;
            grap.icon = Resources.Load<Sprite>("textures/achievementicons/texloaderspeedrunicon");
            grap.skillDescriptionToken = "LOADER_YANKHOOK_DESCRIPTION";
            grap.skillName = "LOADER_YANKHOOK_NAME";
            grap.skillNameToken = "LOADER_YANKHOOK_NAME";

            SkillDef pylon = ScriptableObject.CreateInstance<SkillDef>();
            pylon.activationState = new EntityStates.SerializableEntityStateType(typeof(ZasedPylon));
            pylon.activationStateMachineName = "Pylon";
            pylon.baseRechargeInterval = 7f;
            pylon.baseMaxStock = 1;
            pylon.rechargeStock = 1;
            pylon.requiredStock = 1;
            pylon.stockToConsume = 1;
            pylon.resetCooldownTimerOnUse = false;
            pylon.fullRestockOnAssign = true;
            pylon.dontAllowPastMaxStocks = false;
            pylon.beginSkillCooldownOnSkillEnd = true;
            pylon.cancelSprintingOnActivation = true;
            pylon.forceSprintDuringState = false;
            pylon.canceledFromSprinting = false;
            pylon.isCombatSkill = true;
            pylon.mustKeyPress = true;
            pylon.interruptPriority = EntityStates.InterruptPriority.Any;
            pylon.icon = Resources.Load<SkillDef>("skilldefs/loaderbody/ThrowPylon").icon;
            pylon.skillDescriptionToken = "LOADER_SPECIAL_DESCRIPTION";
            pylon.skillName = "LOADER_SPECIAL_NAME";
            pylon.skillNameToken = "LOADER_SPECIAL_NAME";

            Main.projectilePrefabContent.Add(hook);
            Main.skillDefContent.Add(grap);
            Main.RegisterType(typeof(ZasedGrapple));

            Main.projectilePrefabContent.Add(pylonn);
            Main.skillDefContent.Add(pylon);
            Main.RegisterType(typeof(ZasedPylon));

            primary.variants[0].skillDef = grap;
            secondary.variants[0].skillDef = pylon;
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
            var h = Loader.hook.GetComponent<ProjectileGrappleController>();
            h.yankMassLimit = 0f;
            projectilePrefab = Loader.hook;
            base.OnEnter();
        }
    }

    public class ZasedPylon : EntityStates.Loader.ThrowPylon
    {
        public override void OnEnter()
        {
            var p = Loader.pylonn.GetComponent<ProjectileProximityBeamController>();
            Loader.pylonn.GetComponent<ProjectileSimple>().lifetime = 3f;
            p.attackFireCount = 1000;
            p.attackInterval = 0.25f;
            p.attackRange = 35f;
            p.procCoefficient = 0.3f;
            p.damageCoefficient = 0.25f;
            p.bounces = 3;
            projectilePrefab = Loader.pylonn;
            base.OnEnter();
        }
    }
}