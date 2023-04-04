using System;

namespace WellRoundedBalance.Difficulties
{
    public abstract class DifficultyBase<T> : DifficultyBase where T : DifficultyBase<T>
    {
        public static T instance { get; set; }

        public DifficultyBase()
        {
            if (instance != null)
            {
                throw new InvalidOperationException("Singleton class " + typeof(T).Name + " was instantiated twice");
            }
            instance = this as T;
        }
    }
}