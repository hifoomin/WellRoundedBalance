using BepInEx.Configuration;
using System;

namespace WellRoundedBalance.Difficulties
{
    public abstract class DifficultyBase : SharedBase
    {
        public override ConfigFile Config => Main.WRBDifficultyConfig;
        public abstract string InternalDiffToken { get; }
        public abstract string DescText { get; }

        public static event Action onTokenRegister;

        public override void Init()
        {
            base.Init();
            onTokenRegister += SetToken;
        }

        public void SetToken()
        {
            LanguageAPI.Add(InternalDiffToken, DescText);
        }
    }
}