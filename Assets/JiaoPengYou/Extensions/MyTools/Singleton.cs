namespace Lang
{
    public class Singleton<T> where T : new()
    {
        static T instance;

        public static T Inst
        {
            get
            {
                if (instance == null)
                {
                    instance = new T();
                }
                return instance;
            }
        }
    }
}