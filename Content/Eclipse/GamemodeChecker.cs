using System;

namespace WellRoundedBalance.Eclipse
{
    public abstract class GamemodeBase<T> : GamemodeBase where T : GamemodeBase<T>
    {
        public static T instance { get; set; }

        public GamemodeBase()
        {
            if (instance != null)
            {
                throw new InvalidOperationException("Singleton class " + typeof(T).Name + " was instantiated twice");
            }
            instance = this as T;
        }
    }
}