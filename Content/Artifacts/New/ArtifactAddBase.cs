using BepInEx.Configuration;

namespace WellRoundedBalance.Artifacts.New
{
    public abstract class ArtifactAddBase : SharedBase
    {
        public abstract string ArtifactName { get; }

        public abstract string ArtifactLangTokenName { get; }

        public abstract string ArtifactDescription { get; }

        public abstract Sprite ArtifactEnabledIcon { get; }

        public abstract Sprite ArtifactDisabledIcon { get; }

        public ArtifactDef ArtifactDef;

        //For use only after the run has started.
        public bool ArtifactEnabled => RunArtifactManager.instance.IsArtifactEnabled(ArtifactDef);

        public override ConfigFile Config => Main.WRBArtifactConfig;

        public override void Init()
        {
            CreateLang();
            CreateArtifact();
            base.Init();
        }

        protected void CreateLang()
        {
            LanguageAPI.Add("ARTIFACT_" + ArtifactLangTokenName + "_NAME_WRB", ArtifactName);
            LanguageAPI.Add("ARTIFACT_" + ArtifactLangTokenName + "_DESCRIPTION_WRB", ArtifactDescription);
        }

        protected void CreateArtifact()
        {
            ArtifactDef = ScriptableObject.CreateInstance<ArtifactDef>();
            ArtifactDef.cachedName = "ARTIFACT_" + ArtifactLangTokenName + "_WRB";
            ArtifactDef.nameToken = "ARTIFACT_" + ArtifactLangTokenName + "_NAME_WRB";
            ArtifactDef.descriptionToken = "ARTIFACT_" + ArtifactLangTokenName + "_DESCRIPTION_WRB";
            ArtifactDef.smallIconSelectedSprite = ArtifactEnabledIcon;
            ArtifactDef.smallIconDeselectedSprite = ArtifactDisabledIcon;

            ContentAddition.AddArtifactDef(ArtifactDef);
        }
    }
}