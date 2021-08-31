namespace GuaLanguage 
{
    using System;
    using System.Reflection;
    public class Natives 
    {
        public Environment environment(Environment env) 
        {
            appendNatives(env);
            return env;
        }

        protected void appendNatives(Environment env) 
        {
            append(env, "__log", typeof(UnityEngine.Debug), "Log", new Type[]{typeof(string)});
            append(env, "__logError", typeof(UnityEngine.Debug), "LogError", new Type[]{typeof(string)});
            append(env, "__sqrt", typeof(Math), "Sqrt", new Type[]{typeof(float)});
            append(env, "__getTime", typeof(Natives), "GetCurTime", Type.EmptyTypes);
        }

        protected void append(Environment env, string name, Type type, string methodName, Type[] params_) 
        {
            MethodInfo m = null;
            try 
            {
                m = type.GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static, null, params_, null);
            }
            catch (Exception e)
            {

            }

            if(m == null) 
            {
                throw new GuaException("cannot find a native function: " + methodName);
            }

            env.put(name, new NativeFunction(methodName, m));
        }

        private static float GetCurTime() 
        {
            return UnityEngine.Time.realtimeSinceStartup;
        }
    }
}