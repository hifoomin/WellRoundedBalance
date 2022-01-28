using RoR2;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;

namespace UltimateCustomRun
{
    public static class ArmorPiercingRounds
    {
        public static void ChangeDamage(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(1f),
                x => x.MatchLdcR4(0.2f)
            );
            c.Index += 1;
            c.Next.Operand = Main.AprDamage.Value;
        }
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
    }
}
