using RoR2;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using R2API;
using UnityEngine;
using RoR2.Orbs;

namespace UltimateCustomRun
{
    public class Razorwire : ItemBase
    {

		public static BuffDef noRazorwire;

		public static bool enable;
		public static float cooldown;
		public static float damage;
		public static float procco;

		public override string Name => ":: Items :: Greens :: Razorwire";
		public override string InternalPickupToken => "thorns";
		public override bool NewPickup => false;
		public override string PickupText => "";
		public override string DescText => "Getting hit causes you to explode in a burst of razors, dealing <style=cIsDamage>" + d(damage) +" damage</style>. Hits up to <style=cIsDamage>5</style> <style=cStack>(+2 per stack)</style> targets in a <style=cIsDamage>25m</style> <style=cStack>(+10m per stack)</style> radius";


		public override void Init()
		{
			enable = ConfigOption(false, "Enable Razorwire Changes?", "Vanilla is false");
			cooldown = ConfigOption(0f, "Cooldown", "Vanilla is 0");
			damage = ConfigOption(1.6f, "Damage", "Decimal. Vanilla is 1.6");
			procco = ConfigOption(0.2f, "Proc Coefficient", "Decimal. Vanilla is 0.2");
			base.Init();
		}

		public override void Hooks()
		{
			if (enable)
            {
				AddRazorwireCooldown();
				IL.RoR2.HealthComponent.TakeDamage += ChangeBehavior;
				On.RoR2.Orbs.LightningOrb.Begin += ChangeProc;
			}
		}

		public static void AddRazorwireCooldown()
		{
			noRazorwire = ScriptableObject.CreateInstance<BuffDef>();

			noRazorwire.buffColor = Color.black;
			noRazorwire.canStack = false;
			noRazorwire.isDebuff = true;
			noRazorwire.name = "NoRazorwire";
			noRazorwire.iconSprite = Resources.Load<Sprite>("textures/bufficons/texBuffEntangleIcon");

			BuffAPI.Add(new CustomBuff(noRazorwire));
		}

		public static void ChangeBehavior(ILContext il)
		{
			ILCursor c = new ILCursor(il);

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
					body.AddTimedBuffAuthority(noRazorwire.buffIndex, cooldown);
				}

				return itemCount;
			});

			c.GotoNext(MoveType.Before,
				x => x.MatchLdcR4(out _),
				x => x.MatchLdarg(0),
				x => x.MatchLdfld<HealthComponent>("body")
				//,x => x.MatchCallOrCallvirt<CharacterBody>("damage")
				);
			c.Remove();
			c.Emit(OpCodes.Ldc_R4, damage);
		}

		public static void ChangeProc(On.RoR2.Orbs.LightningOrb.orig_Begin orig, LightningOrb self)
        {
			self.procCoefficient = procco;
			orig(self);
        }

		public static void ChangeRange(ILContext il)
        {
			// TODO
        }
	}
}
