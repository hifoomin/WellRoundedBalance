using System;
using System.Collections.Generic;
using EntityStates;
using RoR2;
using RoR2.Skills;
using System.Linq;
using UnityEngine;

namespace UltimateCustomRun.BodyStatsSkills
{
    public class ROO
    {
        public static readonly IReadOnlyDictionary<string, InterruptPriority> SToInPr = new Dictionary<string, InterruptPriority>
        {
            { "Any",           InterruptPriority.Any           },
            { "Skill",         InterruptPriority.Skill         },
            { "PrioritySkill", InterruptPriority.PrioritySkill },
            { "Pain",          InterruptPriority.Pain          },
            { "Frozen",        InterruptPriority.Frozen        },
            { "Vehicle",       InterruptPriority.Vehicle       },
            { "Death",         InterruptPriority.Death         }
        };

        public static readonly List<Tuple<string, string, Func<CharacterBody, string>, Action<CharacterBody, string>>> SurvivorModifyableValues = new()
        {
            new("Acceleration", "float", (character) => character.baseAcceleration.ToString(), (character, value) => character.baseAcceleration = float.Parse(value)),
            new("Base Armor", "float", (character) => character.baseArmor.ToString(), (character, value) => character.baseArmor = float.Parse(value)),
            new("Level Armor", "float", (character) => character.levelArmor.ToString(), (character, value) => character.levelArmor = float.Parse(value)),
            new("Base Attack Speed", "float", (character) => character.baseAttackSpeed.ToString(), (character, value) => character.baseAttackSpeed = float.Parse(value)),
            new("Level Attack Speed", "float", (character) => character.levelAttackSpeed.ToString(), (character, value) => character.levelAttackSpeed = float.Parse(value)),
            new("Base Crit", "float", (character) => character.baseCrit.ToString(), (character, value) => character.baseCrit = float.Parse(value)),
            new("Level Crit", "float", (character) => character.levelCrit.ToString(), (character, value) => character.levelCrit = float.Parse(value)),
            new("Base Damage", "float", (character) => character.baseDamage.ToString(), (character, value) => character.baseDamage = float.Parse(value)),
            new("Level Damage", "float", (character) => character.levelDamage.ToString(), (character, value) => character.levelDamage = float.Parse(value)),
            new("Base Jumps", "int", (character) => character.baseJumpCount.ToString(), (character, value) => character.baseJumpCount = int.Parse(value)),
            new("Base Jump Height", "float", (character) => character.baseJumpPower.ToString(), (character, value) => character.baseJumpPower = float.Parse(value)),
            new("Level Jump Height", "float", (character) => character.levelJumpPower.ToString(), (character, value) => character.levelJumpPower = float.Parse(value)),
            new("Base Max HP", "float", (character) => character.baseMaxHealth.ToString(), (character, value) => character.baseMaxHealth = float.Parse(value)),
            new("Level Max HP", "float", (character) => character.levelMaxHealth.ToString(), (character, value) => character.levelMaxHealth = float.Parse(value)),
            new("Base Max Shield", "float", (character) => character.baseMaxShield.ToString(), (character, value) => character.baseMaxShield = float.Parse(value)),
            new("Level Max Shield", "float", (character) => character.levelMaxShield.ToString(), (character, value) => character.levelMaxShield = float.Parse(value)),
            new("Base Move Speed", "float", (character) => character.baseMoveSpeed.ToString(), (character, value) => character.baseMoveSpeed = float.Parse(value)),
            new("Level Move Speed", "float", (character) => character.levelMoveSpeed.ToString(), (character, value) => character.levelMoveSpeed = float.Parse(value)),
            new("Base Regen", "float", (character) => character.baseRegen.ToString(), (character, value) => character.baseRegen = float.Parse(value)),
            new("Level Regen", "float", (character) => character.levelRegen.ToString(), (character, value) => character.levelRegen = float.Parse(value)),
            new("Sprinting Speed Multiplier", "float", (character) => character.sprintingSpeedMultiplier.ToString(), (character, value) => character.sprintingSpeedMultiplier = float.Parse(value)),
            new("Size Multiplier", "float", (character) => { var a = character.GetComponent<ModelLocator>().modelTransform.localScale.ToString(); return a.Substring(1, a.Length - 2); }, (character, value) =>
            {
                var a = value.Split(',').Select(x => float.Parse(x)).ToArray();
                character.GetComponent<ModelLocator>().modelTransform.localScale = new Vector3(a[0], a[1], a[2]);
            })
        };

        public static readonly List<Tuple<string, string, Func<SkillDef, string>, Action<SkillDef, string>, string>> SkillModifyableValues = new()
        {
            new("Interrupt Priority", "interruptPriority", (skill) => skill.interruptPriority.ToString(), (skill, value) => skill.interruptPriority = SToInPr[value], "Priority of the skill. Explained on webpage."),
            new("Base Cooldown", "float", (skill) => skill.baseRechargeInterval.ToString(), (skill, value) => skill.baseRechargeInterval = float.Parse(value), "How long it takes for this skill to recharge after being used."),
            new("Base Max Charges", "int", (skill) => skill.baseMaxStock.ToString(), (skill, value) => skill.baseMaxStock = int.Parse(value), "Maximum number of charges this skill can carry."),
            new("Charge Recharge Count", "int", (skill) => skill.rechargeStock.ToString(), (skill, value) => skill.rechargeStock = int.Parse(value), "How much stock to restore on a recharge."),
            new("Required Charges", "int", (skill) => skill.requiredStock.ToString(), (skill, value) => skill.requiredStock = int.Parse(value), "How much stock is required to activate this skill."),
            new("Charges To Consume", "int", (skill) => skill.stockToConsume.ToString(), (skill, value) => skill.stockToConsume = int.Parse(value), "How much stock to deduct when the skill is activated."),
            new("Reset Cooldown On Use", "bool", (skill) => skill.resetCooldownTimerOnUse.ToString(), (skill, value) => skill.resetCooldownTimerOnUse = bool.Parse(value), "Whether or not it resets any progress on cooldowns."),
            new("Restock On Assign", "bool", (skill) => skill.fullRestockOnAssign.ToString(), (skill, value) => skill.fullRestockOnAssign = bool.Parse(value), "Whether or not to fully restock this skill when it's assigned."),
            new("Dont Allow Past Max Charges", "bool", (skill) => skill.dontAllowPastMaxStocks.ToString(), (skill, value) => skill.dontAllowPastMaxStocks = bool.Parse(value), "Whether or not this skill can hold past it's maximum stock."),
            new("Begin Cooldown On Skill End", "bool", (skill) => skill.beginSkillCooldownOnSkillEnd.ToString(), (skill, value) => skill.beginSkillCooldownOnSkillEnd = bool.Parse(value), "Whether or not the cooldown waits until it leaves the set state"),
            new("Cancel Sprinting On Activation", "bool", (skill) => skill.cancelSprintingOnActivation.ToString(), (skill, value) => skill.cancelSprintingOnActivation = bool.Parse(value), "Whether or not activating the skill forces off sprinting."),
            new("Force Sprint", "bool", (skill) => skill.forceSprintDuringState.ToString(), (skill, value) => skill.forceSprintDuringState = bool.Parse(value), "Whether or not this skill is considered 'mobility'. Currently just forces sprint."),
            new("Canceled From Sprinting", "bool", (skill) => skill.canceledFromSprinting.ToString(), (skill, value) => skill.canceledFromSprinting = bool.Parse(value), "Whether or not sprinting sets the skill's state to be reset."),
            new("Is Combat Skill", "bool", (skill) => skill.isCombatSkill.ToString(), (skill, value) => skill.isCombatSkill = bool.Parse(value), "Whether or not this is considered a combat skill. If true, will stop items like Red Whip on use."),
            new("Must Press", "bool", (skill) => skill.mustKeyPress.ToString(), (skill, value) => skill.mustKeyPress = bool.Parse(value), "The skill can't be activated if the key is held.")
        };

        public static readonly List<string> LunarSkills = new()
        {
            "LunarPrimaryReplacement",
            "LunarSecondaryReplacement",
            "LunarUtilityReplacement",
            "LunarSpecialReplacement"
        };
    }
}