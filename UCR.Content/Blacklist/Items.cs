using RoR2;
using UnityEngine;
using System.Linq;

namespace UltimateCustomRun.Blacklist
{
    public static class Items
    {
        public static void Blacklist()
        {
            var tt = Resources.Load<ItemDef>("resources/itemdefs/Bear");
			var nk = Resources.Load<ItemDef>("resources/itemdefs/NovaOnHeal");
			var tl = Resources.Load<ItemDef>("resources/itemdefs/ShockNearby");
			var ip = Resources.Load<ItemDef>("resources/itemdefs/Plant");
			tt.tags.Add(new ItemTag[]
			{
				ItemTag.AIBlacklist
			});
			nk.tags.Add(new ItemTag[]
			{
				ItemTag.AIBlacklist,
				ItemTag.BrotherBlacklist
			});
			tl.tags.Add(new ItemTag[]
			{
				ItemTag.AIBlacklist,
				ItemTag.BrotherBlacklist
			});
			ip.tags.Add(new ItemTag[]
			{
				ItemTag.AIBlacklist
			});
        }
    }
	public static class ArrayUtil
	{
		public static T[] Add<T>(this T[] array, params T[] items)
		{
			return (array ?? Enumerable.Empty<T>()).Concat(items).ToArray<T>();
		}

		public static T[] Remove<T>(this T[] array, params T[] items)
		{
			return (array ?? Enumerable.Empty<T>()).Except(items).ToArray<T>();
		}
	}
}
