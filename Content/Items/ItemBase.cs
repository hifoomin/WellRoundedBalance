using BepInEx.Configuration;
using RiskOfOptions;
using RiskOfOptions.Options;
using System;
using BepInEx.Logging;
using MonoMod.Cil;

namespace WellRoundedBalance.Items
{
    public abstract class ItemBase
    {
        public abstract string Name { get; }
        public virtual string InternalPickupToken { get; }
        public abstract string PickupText { get; }
        public abstract string DescText { get; }
        public virtual bool isEnabled { get; } = true;
        public ManualLogSource Logger => Main.WRBLogger;

        public abstract void Hooks();

        public string noop(float f) => f.ToString();

        public string d(float f) => (f * 100f).ToString() + "%";

        public string m(float f) => f + "m";

        public string s(float f, string suffix) => f + " " + suffix + (Mathf.Abs(f) > 1 ? "s" : string.Empty);

        public static string StackDesc(float init, float stack, Func<float, string> initFn, Func<float, string> stackFn)
        {
            if (init <= 0 && stack <= 0) return string.Empty;
            string ret = initFn(init);
            if (stack > 0) ret = ret.Replace("{Stack}", " <style=cStack>(" + (stack > 0 ? "+" : string.Empty) + stackFn(stack) + " per stack)</style>");
            return ret;
        }

        public static float StackAmount(float init, float stack, float count, float isHyperbolic = 0f)
        {
            if (count <= 0) return 0;
            float ret = init + (stack * (count - 1));
            if (isHyperbolic > 0) ret = GetHyperbolic(init, isHyperbolic, ret);
            return ret;
        }

        public static float GetHyperbolic(float firstStack, float cap, float chance) // Util.ConvertAmplificationPercentageIntoReductionPercentage but Better :zanysoup:
        {
            if (firstStack >= cap) return cap * (chance / firstStack); // should not happen, but failsafe
            float count = chance / firstStack;
            float coeff = (100 * firstStack) / (cap - firstStack); // should be good
            return cap * (1 - (100 / ((count * coeff) + 100)));
        }

        public static int GetItemLoc(ILCursor c, string item) // modify this on compat update
        {
            int ret = -1;
            if (c.TryGotoNext(x => x.MatchLdsfld(typeof(RoR2Content.Items), item), x => x.MatchCallOrCallvirt<Inventory>(nameof(Inventory.GetItemCount)), x => x.MatchStloc(out ret))) c.Index--;
            else if (c.TryGotoNext(x => x.MatchLdsfld(typeof(DLC1Content.Items), item), x => x.MatchCallOrCallvirt<Inventory>(nameof(Inventory.GetItemCount)), x => x.MatchStloc(out ret))) c.Index--;
            return ret;
        }

        public T ConfigOption<T>(T value, string name, string desc)
        {
            ConfigEntry<T> entry = Main.WRBConfig.Bind<T>(Name, name, value, desc);
            if (typeof(T) == typeof(int))
            {
                ModSettingsManager.AddOption(new IntSliderOption(entry as ConfigEntry<int>));
            }
            else if (typeof(T) == typeof(float))
            {
                ModSettingsManager.AddOption(new SliderOption(entry as ConfigEntry<float>));
            }
            else if (typeof(T) == typeof(string))
            {
                ModSettingsManager.AddOption(new StringInputFieldOption(entry as ConfigEntry<string>));
            }
            else if (typeof(T) == typeof(Enum))
            {
                ModSettingsManager.AddOption(new ChoiceOption(entry));
            }
            return entry.Value;
        }

        public virtual void Init()
        {
            ConfigManager.HandleConfigAttributes(GetType(), Name, Main.WRBItemConfig);
            Hooks();
            string pickupToken;
            string descriptionToken;

            pickupToken = "ITEM_" + InternalPickupToken.ToUpper() + "_PICKUP";
            descriptionToken = "ITEM_" + InternalPickupToken.ToUpper() + "_DESC";

            LanguageAPI.Add(pickupToken, PickupText);
            LanguageAPI.Add(descriptionToken, DescText);
        }

        public string GetToken(string addressablePath)
        {
            ItemDef def = Addressables.LoadAssetAsync<ItemDef>(addressablePath).WaitForCompletion();
            string token = def.nameToken;
            token = token.Replace("ITEM_", string.Empty);
            token = token.Replace("_NAME", string.Empty);
            return token;
        }
    }
}