using System;
using Mono.Cecil.Cil;
using System.Reflection;

namespace WellRoundedBalance.Utils {
    public static class ILExtensions {
        public static void FindLocal(this ILCursor c, LocalType method, string type, out int local, string contentClass = "RoR2Content") {
            int tmp = -1;

            switch (method) {
                case LocalType.ItemCount:
                    c.TryGotoNext(MoveType.After, 
                        x => x.MatchLdsfld($"RoR2.{contentClass}/Items", type), 
                        x => x.MatchCallOrCallvirt(out _), 
                        x => x.MatchStloc(out tmp)
                    );
                    break;
                case LocalType.NextStloc:
                    c.TryGotoNext(MoveType.After,
                        x => x.MatchStloc(out tmp)
                    );
                    break;
            }

            local = tmp;
        }

        public static void FindLocalNoAdvance(this ILCursor c, LocalType method, string type, out int local) {
            int index = c.Index;
            c.FindLocal(method, type, out int tmp);
            c.Index = index;
            local = tmp;
        }

        public static int StepLocal(this ILCursor c, int local) {
            if (!c.TryGotoNext(MoveType.After, x => x.MatchLdloc(local))) {
                c.TryGotoNext(MoveType.After, x => x.MatchStloc(local));
            }

            return c.Index;
        }
    }

    public enum LocalType {
        ItemCount,
        NextStloc,
    }
}