namespace GuaLanguage
{
    public interface Environment 
    {
        void put(string name, object value);
        object get(string name);
        Environment where(string name);
        void putNew(string name, object value);
        void setOuter(Environment e);
    }
}