namespace GuaLanguage
{
    using System.Collections.Generic;
    public class BasicEnv : Environment 
    {
        protected Dictionary<string, object> values;
        public BasicEnv() { values = new Dictionary<string, object>(); }
        public void put(string name, object value) 
        { 
            if(values.ContainsKey(name) == false)
            {
                values.Add(name, value);
            }
            else
            {
                values[name] = value;
            }
        }
        public object get(string name) 
        {
            object ans;
            values.TryGetValue(name, out ans);
            return ans;
        }
    }
}