using BepInEx.Configuration;
using System;

namespace WellRoundedBalance.Achievements
{
    public abstract class AchievementBase : SharedBase
    {
        public abstract string Token { get; }
        public abstract string Description { get; }
        public override ConfigFile Config => Main.WRBAchievementConfig;

        public static event Action onTokenRegister;

        public static List<string> achievementList = new();

        public override void Init()
        {
            base.Init();
            onTokenRegister += SetToken;
            achievementList.Add(Name);
        }

        [SystemInitializer(typeof(UnlockableCatalog))]
        public static void OnUnlockableInitialized()
        { if (onTokenRegister != null) onTokenRegister(); }

        public void SetToken()
        {
            if (Token != null)
            {
                var prefix = "ACHIEVEMENT_";
                var suffix = "_DESCRIPTION";
                LanguageAPI.Add(prefix + Token.ToUpper() + suffix, Description);
            };
        }
    }
}