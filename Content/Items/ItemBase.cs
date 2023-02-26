using BepInEx.Configuration;
using RiskOfOptions;
using RiskOfOptions.Options;
using System;
using BepInEx.Logging;

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

        public string d(float f)
        {
            return (f * 100f).ToString() + "%";
        }

        public string s(float f, string suffix)
        {
            return f + " " + suffix + (Mathf.Abs(f) > 1 ? "s" : "");
        }

        public static string StackDesc(float init, float stack, Func<float, string> initFn, Func<float, string> stackFn)
        {
            if (init <= 0 && stack <= 0) return "";
            string ret = initFn(init);
            if (stack > 0) ret = ret.Replace("{Stack}", " <style=cStack>(" + (stack > 0 ? "+" : "") + stackFn(stack) + " per stack)</style>");
            return ret;
        }

        public static float StackAmount(float init, float stack, float count)
        {
            if (count <= 0) return 0;
            return init + (stack * (count - 1));
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
            token = token.Replace("ITEM_", "");
            token = token.Replace("_NAME", "");
            return token;
        }
    }
}