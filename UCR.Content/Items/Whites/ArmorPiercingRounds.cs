using MonoMod.Cil;

namespace UltimateCustomRun.Items.Whites
{
    public class ArmorPiercingRounds : ItemBase
    {
        public static float Damage;

        public override string Name => ":: Items : Whites :: Armor Piercing Rounds";
        public override string InternalPickupToken => "bossDamageBonus";
        public override bool NewPickup => false;
        public override string PickupText => "";
        /*
        List<string> aprStrings = new List<string>();
        if (AprB.Value) { aprStrings.Add("bosses"); }
        if (AprC.Value) { aprStrings.Add("champions"); }
        if (AprE.Value) { aprStrings.Add("elites"); }
        if (AprF.Value) { aprStrings.Add("fliers"); }

        string allEnemiesAffected = "";
        for (int i = 0; i < aprStrings.Count; i++)
        {
            if (i != aprStrings.Count - 1)
            {
                allEnemiesAffected += $"{aprStrings[i]}, ";
            }
            else
            {
                allEnemiesAffected += $"and {aprStrings[i]}";
            }
        }
        */

        // "Deal an additional <style=cIsDamage>" + d(AprDamage.Value) + "</style> Damage <style=cStack>(+" + d(AprDamage.Value) + " per stack)</style> to " + allEnemiesAffected + ".");
        public override string DescText => "Deal an additional <style=cIsDamage>" + d(Damage) + "</style> damage <style=cStack>(+" + d(Damage) + " per stack)</style> to bosses.";

        public override void Init()
        {
            Damage = ConfigOption(0.2f, "Damage Coefficient", "Decimal. Per Stack. Vanilla is 0.2");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.HealthComponent.TakeDamage += ChangeDamage;
        }

        public static void ChangeDamage(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(1f),
                x => x.MatchLdcR4(0.2f)
            );
            c.Index += 1;
            c.Next.Operand = Damage;
        }

        /*
        public static void ChangeType(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            c.GotoNext(MoveType.Before,
                x => x.MatchLdarg(0),
                x => x.MatchLdfld<CharacterBody>("body"),
                x => x.MatchCallOrCallvirt<CharacterBody>("get_isBoss")
            );
            // im not sure if i should be emitting the 'this' here
            c.Emit(OpCodes.Ldarg_0);
            // something here? c.Index += X? tried 0, 1, 2 and 3
            c.EmitDelegate<Func<bool, CharacterBody, bool>>((boss, body) =>
            {
                bool bosss = Main.AprB.Value && boss;
                bool champion = Main.AprC.Value && body.isChampion;
                bool elite = Main.AprE.Value && body.isElite;
                bool flying = Main.AprF.Value && body.isFlying;
                return boss || champion || elite || flying;
            });
            // PLEASE HELP IN FIXING
        }
        // THANK YOU BORBO HOLY
        // For the life of me I couldn't have figured out how to check for 24 possible combinations and put them into the description :P
        // play their game
        // https://plumicorn.itch.io/superbug
        */
    }
}