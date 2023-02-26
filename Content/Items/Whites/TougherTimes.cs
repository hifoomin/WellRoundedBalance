using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using RoR2;

namespace WellRoundedBalance.Items.Whites
{
    public class TougherTimes : ItemBase
    {
        public override string Name => ":: Items : Whites :: Tougher Times";
        public override string InternalPickupToken => "bear";

        public override string PickupText => "Chance to block incoming damage.";

        public override string DescText => "<style=cIsHealing>" + blockChance + "%</style> <style=cStack>(+" + blockChance + "% per stack)</style> chance to <style=cIsHealing>block</style> incoming damage. <style=cIsUtility>Unaffected by luck</style>.";

        [ConfigField("Block Chance", "", 9f)]
        public static float blockChance;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.HealthComponent.TakeDamage += ChangeBlock;
        }

        public static void ChangeBlock(ILContext il)
        {
            ILCursor c = new(il);
            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdcI4(0),
                    x => x.Match(OpCodes.Ble_S),
                    x => x.MatchLdcR4(15f)))
            {
                c.Index += 2;
                c.Next.Operand = blockChance;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Tougher Times Block hook");
            }
        }
    }
}