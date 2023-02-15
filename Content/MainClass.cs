using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using System;
using System.Reflection;
using WellRoundedBalance.Items;
using WellRoundedBalance.Equipment;
using WellRoundedBalance.Global;
using WellRoundedBalance.Interactables;
using WellRoundedBalance.Mechanic;
using WellRoundedBalance.Enemies;
using WellRoundedBalance.Projectiles;
using WellRoundedBalance.Eclipse;
using System.Runtime.CompilerServices;
using WellRoundedBalance.Elites;
using R2API.ContentManagement;
using WellRoundedBalance.Mechanics.Monsters;
using WellRoundedBalance.Misc;
using WellRoundedBalance.Artifacts;

[assembly: HG.Reflection.SearchableAttribute.OptIn]
// used for BodyCatalog

namespace WellRoundedBalance
{
    [BepInDependency("HIFU.Inferno", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.Moffein.RiskyArtifacts", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency(LanguageAPI.PluginGUID)]
    [BepInDependency(RecalculateStatsAPI.PluginGUID)]
    [BepInDependency(DirectorAPI.PluginGUID)]
    [BepInDependency(PrefabAPI.PluginGUID)]
    [BepInDependency(R2APIContentManager.PluginGUID)]
    [BepInDependency(ItemAPI.PluginGUID)]
    [BepInDependency(DamageAPI.PluginGUID)]
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    public class Main : BaseUnityPlugin
    {
        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "BALLS";
        public const string PluginName = "WellRoundedBalance";
        public const string PluginVersion = "1.0.0";
        public static ConfigFile WRBConfig;
        public static ConfigFile WRBItemConfig;
        public static ConfigFile WRBMechanicConfig;
        public static ConfigFile WRBEquipmentConfig;
        public static ConfigFile WRBInteractableConfig;
        public static ConfigFile WRBEnemyConfig;
        public static ConfigFile WRBEliteConfig;
        public static ConfigFile WRBGamemodeConfig;
        public static ConfigFile WRBArtifactConfig;
        public static ManualLogSource WRBLogger;

        public static AssetBundle wellroundedbalance;

        public static bool InfernoLoaded = false;
        public static bool RiskyArtifactsLoaded = false;
        public static DifficultyDef InfernoDef = null;

        public void Awake()
        {
            WRBLogger = Logger;
            Main.WRBConfig = base.Config;

            wellroundedbalance = AssetBundle.LoadFromFile(Assembly.GetExecutingAssembly().Location.Replace("WellRoundedBalance.dll", "wellroundedbalance"));

            WRBItemConfig = new ConfigFile(Paths.ConfigPath + "\\BALLS.WellRoundedBalance.Items.cfg", true);
            WRBMechanicConfig = new ConfigFile(Paths.ConfigPath + "\\BALLS.WellRoundedBalance.Mechanics.cfg", true);
            WRBEquipmentConfig = new ConfigFile(Paths.ConfigPath + "\\BALLS.WellRoundedBalance.Equipment.cfg", true);
            WRBInteractableConfig = new ConfigFile(Paths.ConfigPath + "\\BALLS.WellRoundedBalance.Interactables.cfg", true);
            WRBEnemyConfig = new ConfigFile(Paths.ConfigPath + "\\BALLS.WellRoundedBalance.Enemies.cfg", true);
            WRBEliteConfig = new ConfigFile(Paths.ConfigPath + "\\BALLS.WellRoundedBalance.Elites.cfg", true);
            WRBGamemodeConfig = new ConfigFile(Paths.ConfigPath + "\\BALLS.WellRoundedBalance.Gamemodes.cfg", true);
            WRBArtifactConfig = new ConfigFile(Paths.ConfigPath + "\\BALLS.WellRoundedBalance.Artifacts.cfg", true);

            InfernoLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("HIFU.Inferno");
            RiskyArtifactsLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Moffein.RiskyArtifacts");

            FunnyLabel.Hooks();
            Useless.Create();
            Buffs.Useless.Create();
            VoidBall.Create();
            BlazingProjectileVFX.Create();
            Molotov.Create();
            VoidLaserProjectileVFX.Create();
            DucleusLaser.Create();
            TitanFist.Create();

            IEnumerable<Type> enumerable = from type in Assembly.GetExecutingAssembly().GetTypes()
                                           where !type.IsAbstract && type.IsSubclassOf(typeof(MechanicBase))
                                           select type;

            WRBLogger.LogInfo("==+----------------==MECHANICS==----------------+==");

            foreach (Type type in enumerable)
            {
                MechanicBase based = (MechanicBase)Activator.CreateInstance(type);
                if (ValidateMechanic(based))
                {
                    based.Init();
                }
            }

            IEnumerable<Type> enumerable2 = from type in Assembly.GetExecutingAssembly().GetTypes()
                                            where !type.IsAbstract && type.IsSubclassOf(typeof(ItemBase))
                                            select type;

            WRBLogger.LogInfo("==+----------------==ITEMS==----------------+==");
            // List<ItemBase> baseds = new();

            foreach (Type type in enumerable2)
            {
                ItemBase based = (ItemBase)Activator.CreateInstance(type);
                if (ValidateItem(based))
                {
                    based.Init();
                }

                // done alphabetically
                // baseds.Add((ItemBase)Activator.CreateInstance(type));
            }

            // nvm this breaks the entire game what
            /*
            foreach (ItemBase itemBased in baseds.OrderBy(x => (char)x.InternalPickupToken.ToLower()[0]))
            {
                // Debug.Log(itemBased.InternalPickupToken);
                if (ValidateItem(itemBased))
                {
                    itemBased.Init();
                }
            }
            */

            IEnumerable<Type> enumerable3 = from type in Assembly.GetExecutingAssembly().GetTypes()
                                            where !type.IsAbstract && type.IsSubclassOf(typeof(EquipmentBase))
                                            select type;

            WRBLogger.LogInfo("==+----------------==EQUIPMENT==----------------+==");

            foreach (Type type in enumerable3)
            {
                EquipmentBase based = (EquipmentBase)Activator.CreateInstance(type);
                if (ValidateEquipment(based))
                {
                    based.Init();
                }
            }

            IEnumerable<Type> enumerable4 = from type in Assembly.GetExecutingAssembly().GetTypes()
                                            where !type.IsAbstract && type.IsSubclassOf(typeof(InteractableBase))
                                            select type;

            WRBLogger.LogInfo("==+----------------==INTERACTABLES==----------------+==");

            foreach (Type type in enumerable4)
            {
                InteractableBase based = (InteractableBase)Activator.CreateInstance(type);
                if (ValidateInteractable(based))
                {
                    based.Init();
                }
            }

            IEnumerable<Type> enumerable5 = from type in Assembly.GetExecutingAssembly().GetTypes()
                                            where !type.IsAbstract && type.IsSubclassOf(typeof(EnemyBase))
                                            select type;

            WRBLogger.LogInfo("==+----------------==ENEMIES==----------------+==");

            foreach (Type type in enumerable5)
            {
                EnemyBase based = (EnemyBase)Activator.CreateInstance(type);
                if (ValidateEnemy(based))
                {
                    based.Init();
                }
            }

            IEnumerable<Type> enumerable6 = from type in Assembly.GetExecutingAssembly().GetTypes()
                                            where !type.IsAbstract && type.IsSubclassOf(typeof(EliteBase))
                                            select type;

            WRBLogger.LogInfo("==+----------------==ELITES==----------------+==");

            foreach (Type type in enumerable6)
            {
                EliteBase based = (EliteBase)Activator.CreateInstance(type);
                if (ValidateElite(based))
                {
                    based.Init();
                }
            }

            IEnumerable<Type> enumerable7 = from type in Assembly.GetExecutingAssembly().GetTypes()
                                            where !type.IsAbstract && type.IsSubclassOf(typeof(GamemodeBase))
                                            select type;

            WRBLogger.LogInfo("==+----------------==GAMEMODES==----------------+==");

            foreach (Type type in enumerable7)
            {
                GamemodeBase based = (GamemodeBase)Activator.CreateInstance(type);
                if (ValidateGamemode(based))
                {
                    based.Init();
                }
            }

            IEnumerable<Type> enumerable8 = from type in Assembly.GetExecutingAssembly().GetTypes()
                                            where !type.IsAbstract && type.IsSubclassOf(typeof(ArtifactBase))
                                            select type;

            WRBLogger.LogInfo("==+----------------==ARTIFACTS==----------------+==");

            foreach (Type type in enumerable8)
            {
                ArtifactBase based = (ArtifactBase)Activator.CreateInstance(type);
                if (ValidateArtifact(based))
                {
                    based.Init();
                }
            }

            RemoveGesture.Based();
            Mechanic.Monsters.SpeedBoost.AddSpeedBoost();
            BetterScaling.NerfHealthScaling();
        }

        public bool ValidateMechanic(MechanicBase gb)
        {
            if (gb.isEnabled)
            {
                bool enabledfr = WRBMechanicConfig.Bind(gb.Name, "Enable Changes?", true, "Vanilla is false").Value;
                if (enabledfr) return true;
            }
            return false;
        }

        public bool ValidateItem(ItemBase ib)
        {
            if (ib.isEnabled)
            {
                bool enabledfr = WRBItemConfig.Bind(ib.Name, "Enable Changes?", true, "Vanilla is false").Value;
                if (enabledfr) return true;
            }
            return false;
        }

        public bool ValidateEquipment(EquipmentBase eqb)
        {
            if (eqb.isEnabled)
            {
                bool enabledfr = WRBEquipmentConfig.Bind(eqb.Name, "Enable Changes?", true, "Vanilla is false").Value;
                if (enabledfr) return true;
            }
            return false;
        }

        public bool ValidateInteractable(InteractableBase ib)
        {
            if (ib.isEnabled)
            {
                bool enabledfr = WRBInteractableConfig.Bind(ib.Name, "Enable Changes?", true, "Vanilla is false").Value;
                if (enabledfr) return true;
            }
            return false;
        }

        public bool ValidateEnemy(EnemyBase enb)
        {
            if (enb.isEnabled)
            {
                bool enabledfr = WRBEnemyConfig.Bind(enb.Name, "Enable Changes?", true, "Vanilla is false").Value;
                if (enabledfr) return true;
            }
            return false;
        }

        public bool ValidateElite(EliteBase elb)
        {
            if (elb.isEnabled)
            {
                bool enabledfr = WRBEliteConfig.Bind(elb.Name, "Enable Changes?", true, "Vanilla is false").Value;
                if (enabledfr) return true;
            }
            return false;
        }

        public bool ValidateGamemode(GamemodeBase gmb)
        {
            if (gmb.isEnabled)
            {
                bool enabledfr = WRBGamemodeConfig.Bind(gmb.Name, "Enable Changes?", true, "Vanilla is false").Value;
                if (enabledfr) return true;
            }
            return false;
        }

        public bool ValidateArtifact(ArtifactBase ab)
        {
            if (ab.isEnabled)
            {
                bool enabledfr = WRBArtifactConfig.Bind(ab.Name, "Enable Changes?", true, "Vanilla is false").Value;
                if (enabledfr) return true;
            }
            return false;
        }

        public static List<string> SortAlphabetically(List<string> input)
        {
            input.Sort();
            return input;

            // TODO - doesnt do anything currently, cant figure out a way to sort item names, etc alphabetically including the semicolons, so whites are first, then greens etc.
        }

        private void InfernoCompat()
        {
            if (InfernoLoaded)
            {
                InfernoDef = GetInfernoDef();
            }
        }

        public static float GetProjectileSimpleModifiers(float speed)
        {
            if (InfernoLoaded) speed *= GetInfernoProjectileSpeedMult();
            if (RiskyArtifactsLoaded) speed *= GetRiskyArtifactsWarfareProjectileSpeedMult();
            return speed;
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static float GetRiskyArtifactsWarfareProjectileSpeedMult()
        {
            if (RunArtifactManager.instance && RunArtifactManager.instance.IsArtifactEnabled(Risky_Artifacts.Artifacts.Warfare.artifact))
            {
                return Risky_Artifacts.Artifacts.Warfare.projSpeed;
            }
            return 1f;
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static DifficultyDef GetInfernoDef()
        {
            return Inferno.Main.InfernoDiffDef;
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static bool IsInfernoDef()
        {
            if (InfernoLoaded && Run.instance)
            {
                if (DifficultyCatalog.GetDifficultyDef(Run.instance.selectedDifficulty) == InfernoDef) return true;
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static float GetInfernoProjectileSpeedMult()
        {
            if (Run.instance && DifficultyCatalog.GetDifficultyDef(Run.instance.selectedDifficulty) == InfernoDef)
            {
                return Inferno.Main.ProjectileSpeed.Value;
            }
            return 1f;
        }
    }
}