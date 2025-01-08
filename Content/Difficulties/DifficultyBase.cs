using BepInEx.Configuration;
using System;

namespace WellRoundedBalance.Difficulties
{
    public abstract class DifficultyBase : SharedBase
    {
        public override ConfigFile Config => Main.WRBDifficultyConfig;
        public abstract DifficultyIndex InternalDiff { get; }
        public abstract string DescText { get; }

        public static List<string> difficultyList = new();

        public override void Init()
        {
            base.Init();
            SetToken();
            difficultyList.Add(Name);
        }

        public void SetToken()
        {
            if (InternalDiff == DifficultyIndex.Invalid) return;
            DifficultyDef def = DifficultyCatalog.GetDifficultyDef(InternalDiff);
            def.descriptionToken += "_WRB";
            if (def != null) LanguageAPI.Add(def.descriptionToken, DescText);
        }
    }
}