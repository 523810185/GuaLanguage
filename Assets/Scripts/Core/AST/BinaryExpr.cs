namespace GuaLanguage.AST
{
    using System.Collections.Generic;
    public class BinaryExpr : ASTList 
    {
        public BinaryExpr(List<ASTree> c) : base(c) {}
        public ASTree left() { return child(0); }
        public string operator_() { return ((ASTLeaf)child(1)).token().getText(); }
        public ASTree right() { return child(2); }

        public override object eval(Environment env)
        {
            var be = this;
            var op = be.operator_();
            if(op == "=")
            {
                var right = be.right().eval(env);
                return be.computeAssign(env, right);
            }
            else 
            {
                var left = be.left().eval(env);
                var right = be.right().eval(env);
                return be.computeOp(left, op, right);
            }
        }

        protected object computeAssign(Environment env, object rvalue)
        {
            var be = this;
            var l = be.left();
            if(l is Name) 
            {
                env.put((l as Name).name(), rvalue);
                return rvalue;
            }
            else 
            {
                throw new GuaException("bad assigmemt", be);
            }
        }

        protected object computeOp(object left, string op, object right)
        {
            var be = this;
            if(left is int && right is int) 
            {
                return be.computeNumber((int)left, op, (int)right);
            }
            else
            {
                if(op == "+") 
                {
                    return (string)left + (string)right;
                }
                else if(op == "==")
                {
                    if(left == null) 
                    {
                        return right == null ? TRUE : FALSE;
                    }
                    else 
                    {
                        return left.Equals(right) ? TRUE : FALSE;
                    }
                }
                else 
                {
                    throw new GuaException("bad type", be);
                }
            }
        }

        protected object computeNumber(int left, string op, int right) 
        {
            var be = this;
            int a = left;
            int b = right;
            if(op == "+") 
            {
                return a + b;
            }
            else if(op == "-")
            {
                return a - b;
            }
            else if(op == "*")
            {
                return a * b;
            }
            else if(op == "/")
            {
                return a / b;
            }
            else if(op == "%") 
            {
                return a % b;
            }
            else if(op == "==")
            {
                return a == b ? TRUE : FALSE;
            }
            else if(op == ">") 
            {
                return a > b ? TRUE : FALSE;
            }
            else if(op == "<") 
            {
                return a < b ? TRUE : FALSE;
            }
            else 
            {
                throw new GuaException("bad operator", be);
            } 
        }
    }
}