namespace GuaLanguage 
{
    using GuaLanguage;
    using GuaLanguage.AST;
    public static class BasicEvaluator
    {
        public static readonly int TRUE = 1;
        public static readonly int FALSE = 0;

        public static object eval(this ASTree astree, Environment env) 
        {
            throw new GuaException("not imp eval ", astree);
        }

        public static object eval(this ASTList astlist, Environment env)
        {
            throw new GuaException("not imp eval " + astlist.ToString(), astlist);
        }

        public static object eval(this ASTLeaf astleaf, Environment env)
        {
            throw new GuaException("not imp eval " + astleaf.ToString(), astleaf);
        }

        public static object eval(this NumberLiteral nl, Environment env)
        {
            return nl.value();
        }

        public static object eval(this StringLiteral sl, Environment env)
        {
            return sl.value();
        }

        public static object eval(this Name name, Environment env)
        {
            object value = env.get(name.name());
            if(value == null) 
            {
                throw new GuaException("undefined name: " + name.name(), name);
            }
            else 
            {
                return value;
            }
        }

        public static object eval(this NegativeExpr ne, Environment env)
        {
            object v = ne.operand().eval(env);
            if(v is int) 
            {
                return -(int)v;
            }
            else 
            {
                throw new GuaException("bad type for -", ne);
            }
        }

        public static object eval(this BinaryExpr be, Environment env)
        {
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
 
        public static object computeAssign(this BinaryExpr be, Environment env, object rvalue) 
        {
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

        public static object computeOp(this BinaryExpr be, object left, string op, object right) 
        {
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

        public static object computeNumber(this BinaryExpr be, int left, string op, int right) 
        {
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

        public static object eval(this BlockStmnt bs, Environment env)
        {
            object result = 0;
            foreach (var t in bs)
            {
                if(!(t is NullStmnt)) 
                {
                    result = t.eval(env);
                }
            }

            return result;
        }

        public static object eval(this IfStmnt ifStmnt, Environment env)
        {
            object c = ifStmnt.condition().eval(env);
            if(c is int && (int)c != FALSE)
            {
                return ifStmnt.thenBlock().eval(env);
            }
            else 
            {
                var b = ifStmnt.elseBlock();
                return b == null ? 0 : b.eval(env);
            }
        }

        public static object eval(this WhileStmnt ws, Environment env)
        {
            object result = 0;
            for(;;) 
            {
                object c = ws.condition().eval(env);
                if(c is int && (int)c == FALSE) 
                {
                    return result;
                }
                else 
                {
                    result = ws.body().eval(env);
                }
            }
        }
    }
}