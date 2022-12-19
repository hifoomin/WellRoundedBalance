using R2API;

namespace UltimateCustomRun
{
    public abstract class SurvivorBase
    {
        public abstract string Name { get; }
        public abstract string InternalName { get; }
        public abstract bool PassiveExists { get; }
        public abstract string PassiveDesc { get; }
        public abstract bool AltPassiveExists { get; }
        public abstract string AltPassiveDesc { get; }
        public abstract string M1Desc { get; }
        public abstract bool AltM1Exists { get; }
        public abstract string AltM1Token { get; }
        public abstract string AltM1Desc { get; }
        public abstract bool SecondAltM1Exists { get; }
        public abstract string SecondAltM1Token { get; }
        public abstract string SecondAltM1Desc { get; }
        public abstract bool ThirdAltM1Exists { get; }
        public abstract string ThirdAltM1Token { get; }
        public abstract string ThirdAltM1Desc { get; }
        public abstract string M2Desc { get; }
        public abstract bool AltM2Exists { get; }
        public abstract string AltM2Token { get; }
        public abstract string AltM2Desc { get; }
        public abstract string UtilityDesc { get; }
        public abstract bool AltUtilityExists { get; }
        public abstract string AltUtilityToken { get; }
        public abstract string AltUtilityDesc { get; }
        public abstract string SpecialDesc { get; }
        public abstract bool AltSpecialExists { get; }
        public abstract string AltSpecialToken { get; }
        public abstract string AltSpecialDesc { get; }
        public abstract bool SecondAltSpecialExists { get; }
        public abstract string SecondAltSpecialToken { get; }
        public abstract string SecondAltSpecialDesc { get; }
        public abstract bool ThirdAltSpecialExists { get; }
        public abstract string ThirdAltSpecialToken { get; }
        public abstract string ThirdAltSpecialDesc { get; }

        public T ConfigOption<T>(T value, string name, string description)
        {
            return Main.UCRConfig.Bind<T>(Name, name, value, description).Value;
        }

        public abstract void Hooks();

        public string d(float f)
        {
            return (f * 100f).ToString() + "%";
        }

        public virtual void Init()
        {
            Hooks();
            if (PassiveExists)
            {
                LanguageAPI.Add(InternalName.ToUpper() + "_" + "PASSIVE_DESCRIPTION", PassiveDesc);
            }
            if (AltPassiveExists)
            {
                LanguageAPI.Add(InternalName.ToUpper() + "_" + "PASSIVE_ALT_DESCRIPTION", AltPassiveDesc);
            }
            LanguageAPI.Add(InternalName.ToUpper() + "_" + "PRIMARY_DESCRIPTION", M1Desc);
            if (AltM1Exists)
            {
                LanguageAPI.Add(InternalName.ToUpper() + "_" + AltM1Token.ToUpper() + "DESCRIPTION", AltM1Desc);
            }
            if (SecondAltM1Exists)
            {
                LanguageAPI.Add(InternalName.ToUpper() + "_" + SecondAltM1Token.ToUpper() + "DESCRIPTION", SecondAltM1Desc);
            }
            if (ThirdAltM1Exists)
            {
                LanguageAPI.Add(InternalName.ToUpper() + "_" + ThirdAltM1Token.ToUpper() + "DESCRIPTION", ThirdAltM1Desc);
            }

            LanguageAPI.Add(InternalName.ToUpper() + "_" + "SECONDARY_DESCRIPTION", M2Desc);
            if (AltM2Exists)
            {
                LanguageAPI.Add(InternalName.ToUpper() + "_" + AltM2Token.ToUpper() + "DESCRIPTION", AltM2Desc);
            }
            LanguageAPI.Add(InternalName.ToUpper() + "_" + "UTILITY_DESCRIPTION", UtilityDesc);
            if (AltUtilityExists)
            {
                LanguageAPI.Add(InternalName.ToUpper() + "_" + AltUtilityToken.ToUpper() + "DESCRIPTION", AltUtilityDesc);
            }
            LanguageAPI.Add(InternalName.ToUpper() + "_" + "SPECIAL_DESCRIPTION", SpecialDesc);
            if (AltSpecialExists)
            {
                LanguageAPI.Add(InternalName.ToUpper() + "_" + AltSpecialToken.ToUpper() + "DESCRIPTION", AltSpecialDesc);
            }
            if (SecondAltSpecialExists)
            {
                LanguageAPI.Add(InternalName.ToUpper() + "_" + SecondAltSpecialToken.ToUpper() + "DESCRIPTION", SecondAltSpecialDesc);
            }
            if (ThirdAltSpecialExists)
            {
                LanguageAPI.Add(InternalName.ToUpper() + "_" + ThirdAltSpecialToken.ToUpper() + "DESCRIPTION", ThirdAltSpecialDesc);
            }
        }
    }
}