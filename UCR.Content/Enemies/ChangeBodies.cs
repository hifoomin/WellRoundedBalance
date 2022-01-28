using BepInEx;
using R2API;
using R2API.Utils;
using RoR2;
using RoR2.Skills;
using RoR2.Orbs;
using RoR2.Projectile;
using UnityEngine;
using BepInEx.Configuration;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using UnityEngine.Networking;
using System.Collections.Generic;

namespace UltimateCustomRun.Enemies
{
    /*
    public static void Change(CharacterBody body, Base based)
    {
        body.baseMaxHealth = based.BaseMaxHealth;
        body.baseRegen = based.BaseRegen;
        body.baseMaxShield = based.BaseMaxShield;
        body.baseMoveSpeed = based.BaseMoveSpeed;
        body.baseAcceleration = based.BaseAcceleration;
        body.baseJumpPower = based.BaseJumpPower;
        body.baseDamage = based.BaseDamage;
        body.baseAttackSpeed = based.BaseAttackSpeed;
        body.baseCrit = based.BaseCrit;
        body.baseArmor = based.BaseArmor;
        body.baseJumpCount = based.BaseJumpCount;

        body.sprintingSpeedMultiplier = based.SprintingSpeedMultiplier;

        body.autoCalculateLevelStats = based.AutoCalculateLevelStats;

        body.levelMaxHealth = based.LevelMaxHealth;
        body.levelRegen = based.LevelRegen;
        body.levelMaxShield = based.LevelMaxShield;
        body.levelMoveSpeed = based.LevelMoveSpeed;
        body.levelJumpPower = based.LevelJumpPower;
        body.levelDamage = based.LevelDamage;
        body.levelAttackSpeed = based.LevelAttackSpeed;
        body.levelCrit = based.LevelCrit;
        body.levelArmor = based.LevelArmor;
    }
    public class ChangeBodies : Base
    {
         = BodyCatalog.FindBodyPrefab("VultureBody");
        Change()
    }
    */
}
