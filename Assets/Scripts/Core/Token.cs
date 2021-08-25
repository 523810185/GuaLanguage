namespace GuaLanguage
{
    public class Token 
    {
        public static readonly Token EOF = new Token(-1); // end of file
        public static readonly string EOL = "\\n";          // end of line 
        private int lineNumber;

        protected Token(int line) {
            lineNumber = line;
        }
        public virtual int getLineNumber() { return lineNumber; }
        public virtual bool isIdentifier() { return false; }
        public virtual bool isNumber() { return false; }
        public virtual bool isString() { return false; }
        public virtual int getNumber() { throw new GuaException("not number token"); }
        public virtual string getText() { return ""; }

        public override string ToString()
        {
            return getText();
        }
    }
}
