namespace GuaLanguage
{
    using System.Collections.Generic;
    using GuaLanguage.Utility;
    public class HighlightHelper
    {
        private HighlightHelper() {}
        private static HighlightHelper m_pInstance = null;
        public static HighlightHelper Instance 
        {
            get 
            {
                if(m_pInstance == null)
                {
                    m_pInstance = new HighlightHelper();
                }

                return m_pInstance;
            }
        }

        public enum IdentityType 
        {
            Null = 0,
            FunctionCall = 1,
        }

        private Dictionary<string, IdentityType> m_mapIDRecorder = new Dictionary<string, IdentityType>();

        public void Clear()
        {
            m_mapIDRecorder.Clear();
        }

        public void Record(string name, IdentityType type) 
        {
            m_mapIDRecorder.TrySet(name, type);
        }

        public IdentityType GetIDType(string name)
        {
            var type = IdentityType.Null;
            this.m_mapIDRecorder.TryGetValue(name, out type);
            return type;
        }
    }
}