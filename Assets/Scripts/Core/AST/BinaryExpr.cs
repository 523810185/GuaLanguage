namespace GuaLanguage.AST
{
    using System.Collections.Generic;
    using static GuaObject;
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
            // 数组赋值 ==> a[index] = b;
            if(l is PrimaryExpr) 
            {
                var p = l as PrimaryExpr;
                if(p.hasPostfix(0) && p.postfix(0) is ArrayRef) 
                {
                    object a = p.evalSubExpr(env, 1);
                    if(a is object[]) 
                    {
                        ArrayRef aref = p.postfix(0) as ArrayRef;
                        object _index = aref.index().eval(env);
                        if(_index is int) 
                        {
                            ((object[])a)[(int)_index] = rvalue;
                            return rvalue;
                        }
                    }
                    throw new GuaException("bad array access", this);
                }
            }

            // 左边带有 "." ==> a.x = b;
            if(l is PrimaryExpr) 
            {
                var p = l as PrimaryExpr;
                if(p.hasPostfix(0) && p.postfix(0) is Dot) 
                {
                    object t = p.evalSubExpr(env, 1);
                    if(t is GuaObject) 
                    {
                        return setField(t as GuaObject, p.postfix(0) as Dot, rvalue);
                    }
                }
            }

            // 单纯赋值  ==> a = b;
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

        protected object setField(GuaObject obj, Dot expr, object rvalue) 
        {
            string name = expr.name();
            try 
            {
                obj.write(name, rvalue);
                return rvalue;
            }
            catch (AccessException e) 
            {
                throw new GuaException("bad member access " + location() + ": " + name);
            }
        }

        protected object computeOp(object left, string op, object right)
        {
            var be = this;
            if((left is int || left is float || left is double) && (right is int || right is float || right is double)) 
            {
                if(left is int && right is int) 
                {
                    return be.computeNumberInt((int)left, op, (int)right);
                }
                
                float _l = left is int ? (int)left : left is float ? (float)left : (float)((double)left);
                float _r = right is int ? (int)right : right is float ? (float)right : (float)((double)right);
                return be.computeNumberFloat(_l, op, _r);
            }
            else
            {
                if(op == "+") 
                {
                    // return (string)left + (string)right;
                    return left.ToString() + right.ToString();
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

        protected object computeNumberInt(int left, string op, int right) 
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
            else if(op == "<=") 
            {
                return a <= b ? TRUE : FALSE;
            }
            else if(op == ">=") 
            {
                return a >= b ? TRUE : FALSE;
            }
            else 
            {
                throw new GuaException("bad operator", be);
            } 
        }

        protected object computeNumberFloat(float left, string op, float right) 
        {
            var be = this;
            float a = left;
            float b = right;
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
            else if(op == "<=") 
            {
                return a <= b ? TRUE : FALSE;
            }
            else if(op == ">=") 
            {
                return a >= b ? TRUE : FALSE;
            }
            else 
            {
                throw new GuaException("bad operator", be);
            } 
        }
    }
}