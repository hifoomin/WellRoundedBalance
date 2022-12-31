using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using RoR2;
using RoR2.Orbs;
using System;
using UnityEngine;

namespace WellRoundedBalance.Items.Greens
{
    public class Razorwire : ItemBase
    {
        // ////////////
        //
        // Thanks to Borbo
        //
        // ///////////////

        public static BuffDef noRazorwire;

        public override string Name => ":: Items :: Greens :: Razorwire";
        public override string InternalPickupToken => "thorns";

        public override string PickupText => "Retaliate in a burst of razors on taking damage.";
        public override string DescText => "Getting hit causes you to explode in a burst of razors, dealing <style=cIsDamage>160% damage</style>. Hits up to <style=cIsDamage>5</style> <style=cStack>(+2 per stack)</style> targets in a <style=cIsDamage>25m</style> <style=cStack>(+10m per stack)</style> radius.";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            AddRazorwireCooldown();
            IL.RoR2.HealthComponent.TakeDamage += ChangeBehavior;
        }

        public static void AddRazorwireCooldown()
        {
            noRazorwire = ScriptableObject.CreateInstance<BuffDef>();

            noRazorwire.buffColor = Color.black;
            noRazorwire.canStack = false;
            noRazorwire.isDebuff = true;
            noRazorwire.name = "Razorwire Cooldown :smirk_cat:";
            noRazorwire.buffColor = new Color32();
            noRazorwire.isHidden = true;
            R2API.ContentAddition.AddBuffDef(noRazorwire);
        }

        public static void ChangeBehavior(ILContext il)
        {
            ILCursor c = new(il);

            c.GotoNext(MoveType.After,
                x => x.MatchLdflda<HealthComponent>("itemCounts"),
                x => x.MatchLdfld<HealthComponent.ItemCounts>("thorns"),
                x => x.MatchLdcI4(0)
                );
            c.Index--;
            c.Emit(OpCodes.Ldarg_0);
            c.EmitDelegate<Func<Int32, HealthComponent, Int32>>((itemCount, hc) =>
            {
                CharacterBody body = hc.body;
                if (body.HasBuff(noRazorwire))
                {
                    itemCount = 0;
                }
                else if (itemCount > 0)
                {
                    body.AddTimedBuffAuthority(noRazorwire.buffIndex, 0.5f);
                }

                return itemCount;
            });
        }
    }
}