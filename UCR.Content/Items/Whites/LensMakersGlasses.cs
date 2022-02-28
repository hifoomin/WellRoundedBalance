using MonoMod.Cil;
using R2API;
using RoR2;

namespace UltimateCustomRun.Items.Whites
{
    public class LensMakersGlasses : ItemBase
    {
        public static float Crit;

        public override string Name => ":: Items : Whites :: Lens Makers Glasses";
        public override string InternalPickupToken => "critGlasses";
        public override bool NewPickup => false;

        public override string PickupText => "Chance to 'Critically Strike', dealing double damage.";
        // Change double to global Crit Damage

        public override string DescText => "Your attacks have a <style=cIsDamage>" + Crit + "%</style> <style=cStack>(+" + Crit + "% per stack)</style> Chance to '<style=cIsDamage>Critically Strike</style>', dealing <style=cIsDamage>double damage</style>.";

        // Change double to global Crit Damage
        public override void Init()
        {
            Crit = ConfigOption(10f, "Crit Chance", "Per Stack. Vanilla is 10");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.CharacterBody.RecalculateStats += ChangeCrit;
        }

        public static void ChangeCrit(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            c.GotoNext(MoveType.Before,
                x => x.MatchLdfld<CharacterBody>("levelCrit")
            );
            c.Index += 8;
            c.Next.Operand = Crit;
            // I dont actually know why the standard method doesnt work
            // Thanks to uhh someone for this instead
        }

        public static void AddBehavior(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.inventory)
            {
                var stack = sender.inventory.GetItemCount(RoR2Content.Items.CritGlasses);
                if (stack > 0)
                {
                    // waiting for RecalcStats update if possible to add critdamagemult :thonk: += Main.LensMakersCritDamage.Value * stack;
                    // OR PLEASE HELP TO FIX
                }
            }
        }
    }
}