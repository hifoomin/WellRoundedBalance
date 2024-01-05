using System;

namespace WellRoundedBalance.Enemies
{
    public static class TweakStatsAI
    {
        // TODO: get every master and body index, implement switch with if Class.instance.IsEnabled in cases (if needed)
        [SystemInitializer(new Type[] { typeof(BodyCatalog), typeof(MasterCatalog) })]
        public static void Init()
        {
        }
    }
}