namespace GuaLanguage.AST
{
    using System.Collections.Generic;

    public class ParameterList : ASTList 
    {
        public ParameterList(List<ASTree> c) :base(c) {}
        public string name(int i) { return (child(i) as ASTLeaf).token().getText(); }
        public int size() { return numChildren(); }

        public void eval(Environment env, int index, object value) {
            env.putNew(name(index), value);
        }
    }
}