namespace WellRoundedBalance.Gamemodes.Eclipse
{
    internal class ChangeDesc : GamemodeBase
    {
        public override string Name => ":: Gamemode : Eclipse";

        public override void Hooks()
        {
            var e1 = "Starts at baseline Monsoon difficulty.\n\n<mspace=0.5em>(1)</mspace> Enemies have <style=cIsHealth>better aim</style></style>";
            var e2 = "\n<mspace=0.5em>(2)</mspace> Elite Enemies <style=cIsHealth>predict ally movement</style></style>";
            var e3 = "\n<mspace=0.5em>(3)</mspace> Elite Enemies are <style=cIsHealth>stronger in unique ways</style></style>";
            var e4 = "\n<mspace=0.5em>(4)</mspace> Enemies: <style=cIsHealth>+40% Faster</style></style>";
            var e5 = "\n<mspace=0.5em>(5)</mspace> Combat Director <style=cIsHealth>gains +3% credits every stage</style></style>";
            var e6 = "\n<mspace=0.5em>(6)</mspace> Bosses: <style=cIsHealth>+100% More Aggressive</style></style>";
            var e7 = "\n<mspace=0.5em>(7)</mspace> Enemy Cooldowns: <style=cIsHealth>-50%</style></style>";
            var e8 = "\n<mspace=0.5em>(8)</mspace> Allies recieve <style=cIsHealth>permanent damage</style></style>";
            var e8prefix = "\"You only celebrate in the light... because I allow it.\" \n\n";
            LanguageAPI.Add("ECLIPSE_1_DESCRIPTION", e1);
            LanguageAPI.Add("ECLIPSE_2_DESCRIPTION", e1 + e2);
            LanguageAPI.Add("ECLIPSE_3_DESCRIPTION", e1 + e2 + e3);
            LanguageAPI.Add("ECLIPSE_4_DESCRIPTION", e1 + e2 + e3 + e4);
            LanguageAPI.Add("ECLIPSE_5_DESCRIPTION", e1 + e2 + e3 + e4 + e5);
            LanguageAPI.Add("ECLIPSE_6_DESCRIPTION", e1 + e2 + e3 + e4 + e5 + e6);
            LanguageAPI.Add("ECLIPSE_7_DESCRIPTION", e1 + e2 + e3 + e4 + e5 + e6 + e7);
            LanguageAPI.Add("ECLIPSE_8_DESCRIPTION", e8prefix + e1 + e2 + e3 + e4 + e5 + e6 + e7 + e8);
        }
    }
}