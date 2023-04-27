using System;

namespace WellRoundedBalance.Achievements
{
    public abstract class AchievementBase<T> : AchievementBase where T : AchievementBase<T>
    {
        public static T instance { get; set; }

        public AchievementBase()
        {
            if (instance != null)
            {
                throw new InvalidOperationException("Singleton class " + typeof(T).Name + " was instantiated twice");
            }
            instance = this as T;
        }
    }
}