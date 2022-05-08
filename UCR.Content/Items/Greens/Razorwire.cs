using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using RoR2;
using RoR2.Orbs;
using System;
using UnityEngine;

namespace UltimateCustomRun.Items.Greens
{
    public class Razorwire : ItemBase
    {
        // ////////////
        //
        // Thanks to Borbo
        //
        // ///////////////

        public static BuffDef noRazorwire;
        public static float Cooldown;
        public static float Damage;
        public static float ProcCoefficient;

        public override string Name => ":: Items :: Greens :: Razorwire";
        public override string InternalPickupToken => "thorns";
        public override bool NewPickup => false;
        public override string PickupText => "";
        public override string DescText => "Getting hit causes you to explode in a burst of razors, dealing <style=cIsDamage>" + d(Damage) + " damage</style>. Hits up to <style=cIsDamage>5</style> <style=cStack>(+2 per stack)</style> targets in a <style=cIsDamage>25m</style> <style=cStack>(+10m per stack)</style> radius.";

        public override void Init()
        {
            Cooldown = ConfigOption(0f, "Cooldown", "Vanilla is 0");
            Damage = ConfigOption(1.6f, "Damage", "Decimal. Vanilla is 1.6");
            ProcCoefficient = ConfigOption(0.2f, "Proc Coefficient", "Decimal. Vanilla is 0.2");
            base.Init();
        }

        public override void Hooks()
        {
            AddRazorwireCooldown();
            IL.RoR2.HealthComponent.TakeDamage += ChangeBehavior;
            On.RoR2.Orbs.LightningOrb.Begin += ChangeProc;
        }

        public static void AddRazorwireCooldown()
        {
            noRazorwire = ScriptableObject.CreateInstance<BuffDef>();

            noRazorwire.buffColor = Color.black;
            noRazorwire.canStack = false;
            noRazorwire.isDebuff = true;
            noRazorwire.name = "NoRazorwire";
            noRazorwire.iconSprite = LegacyResourcesAPI.Load<Sprite>("textures/bufficons/texBuffEntangleIcon");
            noRazorwire.buffColor = new Color32();
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
                    body.AddTimedBuffAuthority(noRazorwire.buffIndex, Cooldown);
                }

                return itemCount;
            });

            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(out _),
                x => x.MatchLdarg(0),
                x => x.MatchLdfld<HealthComponent>("body")
                //,x => x.MatchCallOrCallvirt<CharacterBody>("Damage")
                );
            c.Remove();
            c.Emit(OpCodes.Ldc_R4, Damage);
        }

        public static void ChangeProc(On.RoR2.Orbs.LightningOrb.orig_Begin orig, LightningOrb self)
        {
            if (self.lightningType is LightningOrb.LightningType.RazorWire)
            {
                self.procCoefficient = ProcCoefficient;
            }
            orig(self);
            // THONK
        }

        public static void ChangeRange(ILContext il)
        {
            // TODO
        }
    }
}