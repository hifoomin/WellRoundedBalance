using System;
using System.Collections.Generic;
using BepInEx.Configuration;
using RoR2;

namespace UltimateCustomRun.Survivors
{
    public abstract class Base
    {
        // ZAMN
        // Thanks to FMRadio11
        public void WriteConfig(ConfigFile baseFile)
        {
            AutoLevel = baseFile.Bind<bool>($":: Survivors :: {EntityType} :: {EntityName}", "Automatic Levelups?", true, "Determines whether stat gains on leveling up are determined using the vanilla modifiers. If disabled, instead uses the per-level values below.");
            for (var i = 0; i < Entity.Item1.Length; i++)
            {
                String[] obj = EntityConfig.EntityStatBase[i];
                if (Entity.Item1[i] is float)
                    StatConfigFloat.Add(baseFile.Bind<float>($":: Survivors :: {EntityType} :: {EntityName}", obj[0], (float)Entity.Item1[i], obj[1] + EntityName + obj[2]));
                else if (Entity.Item1[i] is int)
                    StatConfigInt.Add(baseFile.Bind<int>($":: Survivors :: {EntityType} :: {EntityName}", obj[0], (int)Entity.Item1[i], obj[1] + EntityName + obj[2]));
            }
            ConfigEntry<float>[] StatConfig2 = StatConfigFloat.ToArray();
            ConfigEntry<int>[] StatConfig3 = StatConfigInt.ToArray(); // technically it's one value but I'm making it an array for future-proofing
            Entity.Item2.baseMaxHealth = StatConfig2[0].Value;
            Entity.Item2.baseRegen = StatConfig2[3].Value;
            Entity.Item2.baseMaxShield = StatConfig2[5].Value;
            Entity.Item2.baseMoveSpeed = StatConfig2[7].Value;
            Entity.Item2.baseAcceleration = StatConfig2[4].Value;
            Entity.Item2.baseJumpPower = StatConfig2[5].Value;
            Entity.Item2.baseDamage = StatConfig2[6].Value;
            Entity.Item2.baseAttackSpeed = StatConfig2[7].Value;
            Entity.Item2.baseCrit = StatConfig2[8].Value;
            Entity.Item2.baseArmor = StatConfig2[9].Value;
            Entity.Item2.baseJumpCount = StatConfig3[0].Value;
            Entity.Item2.sprintingSpeedMultiplier = StatConfig2[11].Value;
            Entity.Item2.autoCalculateLevelStats = AutoLevel.Value;
            Entity.Item2.levelMaxHealth = StatConfig2[1].Value;
            Entity.Item2.levelRegen = StatConfig2[12].Value;
            Entity.Item2.levelMaxShield = StatConfig2[13].Value;
            Entity.Item2.levelMoveSpeed = StatConfig2[14].Value;
            Entity.Item2.levelJumpPower = StatConfig2[15].Value;
            Entity.Item2.levelDamage = StatConfig2[16].Value;
            Entity.Item2.levelAttackSpeed = StatConfig2[17].Value;
            Entity.Item2.levelCrit = StatConfig2[18].Value;
            Entity.Item2.levelArmor = StatConfig2[19].Value;
        }
        public abstract String EntityName { get; }
        public abstract String EntityType { get; }
        // Order: HP x2, Regen x2, Shield x2, Move Speed x2, Acceleration, Sprint Modifier, Jump Power x2, Jumps, Attack Speed x2, Damage x2, Crit x2, Armor x2
        public abstract Tuple<System.Object[], CharacterBody> Entity { get; }
        public static List<ConfigEntry<float>> StatConfigFloat;
        public static List<ConfigEntry<int>> StatConfigInt;
        public static ConfigEntry<bool> AutoLevel;
    }
    public class EntityConfig
    {
        // values used by entity classes to determine how config is written
        // this is intentionally missing stuff in the heading and description - both of these are determined by the EntityName and EntityType string written in individual classes
        // the blank values here are also determined by the survivor class - this ideally allows for base stat config within each class to be limited to an array and two strings
        public static String[][] EntityStatBase = new String[][]
        {
            new String[] { "Base Maximum Health","Determines how much HP ", " has at level 1."},
            new String[] { "Level Maximum Health", "Determines how much HP ", " gains per level." },
            new String[] { "Base Regen","Determines how much HP ", " regenerates per second at level 1."},
            new String[] { "Level Regen","Determines how much additional HP ", " regenerates per second per level." },
            new String[] { "Base Maximum Shield","Determines how much shield ", " has at level 1."},
            new String[] { "Level Maximum Shield","Determines how much shield ", " gains per level." },
            new String[] { "Base Move Speed", "How quickly ", " moves at level 1 in m/s." },
            new String[] { "Level Move Speed", "How much speed ", " gains per level in m/s." },
            new String[] { "Acceleration", "How quickly ", " accelerates." },
            new String[] { "Sprint Modifier", "What sprinting multiplies ", "'s speed by." },
            new String[] { "Base Jump Power", "How high ", " jumps at level 1." },
            new String[] { "Level Jump Power", "How much higher ", " jumps per level." },
            new String[] { "Jump Count", "How many normal jumps ", " can perform before landing." },
            new String[] { "Base Damage", "The base damage value ", " has at level 1." },
            new String[] { "Level Damage", "How much ", " adds to base damage per level." },
            new String[] { "Base Attack Speed", "How quickly ", " attacks." },
            new String[] { "Level Attack Speed", "How much faster ", " attacks per level." },
            new String[] { "Base Attack Speed", "How quickly ", " attacks." },
            new String[] { "Level Attack Speed", "How much faster ", " attacks per level." },
            new String[] { "Base Crit", "The % chance ", " has to Critically Strike at level 1." },
            new String[] { "Level Crit", "The additional chance to Critically Strike ", " gains per level." },
            new String[] { "Base Armor", "How much armor ", " has at level 1." },
            new String[] { "Level Armor", "How much armor ", " gains per level." }
        };
        public static String[][] EntitySkillBase;
    }
}
