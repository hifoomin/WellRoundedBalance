using BepInEx.Configuration;

namespace WellRoundedBalance.Enemies
{
    public abstract class EnemyBase : SharedBase
    {
        public override ConfigFile Config => Main.WRBEnemyConfig;
        public static List<string> enemyList = new();

        public override void Init()
        {
            base.Init();
            enemyList.Add(Name);
        }
    }
}