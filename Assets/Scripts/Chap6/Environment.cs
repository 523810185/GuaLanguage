namespace GuaLanguage
{
    public interface Environment 
    {
        void put(string name, object value);
        object get(string name);
    }
}