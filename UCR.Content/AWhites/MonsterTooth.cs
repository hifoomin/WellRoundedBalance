using RoR2;
using MonoMod.Cil;
using System;

namespace UltimateCustomRun
{
    static class MonsterTooth
    {
        public static void ChangeHealing(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchStfld<HealthPickup>(nameof(HealthPickup.flatHealing))
            );
            c.Index--;
            c.EmitDelegate<Func<float, float>>((vanilla) => { return Main.MonsterToothFlatHealing.Value; });
            c.Index += 2;
            c.EmitDelegate<Func<float, float>>((vanilla) => { return Main.MonsterToothPercentHealing.Value; });
            //thanks to RandomlyAwesome!
        }
    }
}
