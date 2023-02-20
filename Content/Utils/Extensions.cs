using System;

namespace WellRoundedBalance.Utils
{
    public static class Extensions
    {
        public static PickupIndex GetPickupIndex(this ItemDef def)
        {
            return PickupCatalog.FindPickupIndex(def.itemIndex);
        }

        public static void RemoveComponent<T>(this GameObject go) where T : Component
        {
            GameObject.Destroy(go.GetComponent<T>());
        }

        public static void RemoveComponents<T>(this GameObject go) where T : Component
        {
            T[] coms = go.GetComponents<T>();
            for (int i = 0; i < coms.Length; i++)
            {
                GameObject.Destroy(coms[i]);
            }
        }

        public static T GetRandom<T>(this List<T> list, Xoroshiro128Plus rng = null)
        {
            if (rng == null)
            {
                return list[UnityEngine.Random.RandomRangeInt(0, list.Count)];
            }
            else
            {
                return list[rng.RangeInt(0, list.Count)];
            }
        }

        public static string ToPercentage(this float self)
        {
            return (self * 100).ToString() + "%";
        }
    }
}