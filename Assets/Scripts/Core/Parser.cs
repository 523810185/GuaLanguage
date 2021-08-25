namespace GuaLanguage
{
    using System.Collections.Generic;
    using System;
    using System.Text;
    using System.Reflection;
    public class Parser
    {
        protected abstract class Element 
        {
            internal abstract void parse(Lexer lexer, List<ASTree> res) ;
            internal abstract bool match(Lexer lexer) ;
        }

        protected class Tree : Element 
        {
            protected Parser parser;
            internal Tree(Parser p) { parser = p; }
            internal override void parse(Lexer lexer, List<ASTree> res)
            {
                res.Add(parser.parse(lexer));
            }
            internal override bool match(Lexer lexer)
            {
                return parser.match(lexer);
            }
        }

        protected class OrTree : Element
        {
            protected Parser[] parsers;
            internal OrTree(Parser[] p) { parsers = p; }
            internal override void parse(Lexer lexer, List<ASTree> res)
            {
                var p = choose(lexer);
                if(p == null) 
                {
                    throw new ParseException(lexer.peek(0));
                }
                else 
                {
                    res.Add(p.parse(lexer));
                }
            }
            internal override bool match(Lexer lexer)
            {
                return choose(lexer) != null;
            }
            protected Parser choose(Lexer lexer) 
            {
                foreach (var p in parsers)
                {
                    if(p.match(lexer))
                    {
                        return p;
                    }
                }

                return null;
            }
            internal void insert(Parser p) 
            {
                Parser[] newParsers = new Parser[parsers.Length + 1];
                newParsers[0] = p;
                Array.Copy(parsers, 0, newParsers, 1, parsers.Length);
                parsers = newParsers;
            }
        }

        protected class Repeat : Element 
        {
            protected Parser parser;
            protected bool onlyOnce;
            internal Repeat(Parser p, bool once) { parser = p; onlyOnce = once; }
            internal override void parse(Lexer lexer, List<ASTree> res)
            {
                while(parser.match(lexer)) 
                {
                    var t = parser.parse(lexer);
                    if(t.GetType() != typeof(ASTList) || t.numChildren() > 0) 
                    {
                        res.Add(t);
                    }
                    if(onlyOnce) 
                    {
                        break;
                    }
                }
            }
            internal override bool match(Lexer lexer)
            {
                return parser.match(lexer);
            }
        }

        protected abstract class AToken : Element 
        {
            protected Func<object, ASTree> factory;
            internal AToken(Type type) 
            {
                if(typeof(ASTLeaf).IsAssignableFrom(type) == false) 
                {
                    type = typeof(ASTLeaf);
                }
                factory = Factory.get(type, typeof(Token));
            }
            internal override void parse(Lexer lexer, List<ASTree> res)
            {
                var t = lexer.read();
                if(test(t))
                {
                    var leaf = factory(t);
                    res.Add(leaf);
                }
                else 
                {
                    throw new ParseException(t);
                }
            }
            internal override bool match(Lexer lexer)
            {
                return test(lexer.peek(0));
            }
            protected abstract bool test(Token t);
        }

        protected class IdToken : AToken 
        {
            HashSet<string> reserved;
            internal IdToken(Type type, HashSet<string> r) : base(type)
            {
                reserved = r != null ? r : new HashSet<string>();
            }
            protected override bool test(Token t)
            {
                return t.isIdentifier() && !reserved.Contains(t.getText());
            }
        }

        protected class NumToken : AToken 
        {
            internal NumToken(Type type) : base(type) {}
            protected override bool test(Token t)
            {
                return t.isNumber();
            }
        }

        protected class StrToken : AToken 
        {
            internal StrToken(Type type) : base(type) {}
            protected override bool test(Token t)
            {
                return t.isString();
            }
        }

        protected class Leaf : Element 
        {
            protected string[] tokens;
            internal Leaf(string[] pat) { tokens = pat; }
            internal override void parse(Lexer lexer, List<ASTree> res)
            {
                var t = lexer.read();
                if(t.isIdentifier()) 
                {
                    foreach (var token in tokens)
                    {
                        if(token == t.getText()) 
                        {
                            find(res, t);
                            return;
                        }
                    }
                }
                if(tokens.Length > 0) 
                {
                    throw new ParseException(tokens[0] + " expected.", t);
                }
                else 
                {
                    throw new ParseException(t);
                }
            }
            protected virtual void find(List<ASTree> res, Token t) 
            {
                res.Add(new ASTLeaf(t));
            }
            internal override bool match(Lexer lexer)
            {
                var t = lexer.peek(0);
                if(t.isIdentifier())
                {
                    foreach (var token in tokens)
                    {
                        if(token == t.getText()) 
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
        }

        protected class Skip : Leaf
        {
            internal Skip(string[] t) :base(t) {}
            protected override void find(List<ASTree> res, Token t)
            {

            }
        }

        public class Precedence
        {
            internal int value;
            internal bool leftAssoc; // left associative
            public Precedence(int v, bool a) 
            {
                value = v;
                leftAssoc = a;
            }
        }

        public class Operators : Dictionary<string, Precedence> 
        {
            public static bool LEFT = true;
            public static bool RIGHT = false;
            public void add(string name, int prec, bool leftAssoc) 
            {
                if(ContainsKey(name) == false)
                {
                    this.Add(name, new Precedence(prec, leftAssoc));
                }
                else 
                {
                    this[name] = new Precedence(prec, leftAssoc);
                }
            }
        }

        protected class Expr : Element
        {
            protected Func<object, ASTree> factory;
            protected Operators ops;
            protected Parser factor;
            internal Expr(Type type, Parser exp, Operators map) 
            {
                if(typeof(ASTree).IsAssignableFrom(type) == false) 
                {
                    throw new ParseException("Expr type wrong: " + type);
                }

                factory = Factory.getForASTList(type);
                ops = map;
                factor = exp;
            }
            internal override void parse(Lexer lexer, List<ASTree> res)
            {
                var right = factor.parse(lexer);
                Precedence prec;
                while((prec = nextOperator(lexer)) != null) 
                {
                    right = doShift(lexer, right, prec.value);
                }
            }
            private ASTree doShift(Lexer lexer, ASTree left, int prec) 
            {
                List<ASTree> list = new List<ASTree>();
                list.Add(left);
                list.Add(new ASTLeaf(lexer.read()));
                ASTree right = factor.parse(lexer);
                Precedence next;
                while((next = nextOperator(lexer)) != null && rightIsExpr(prec, next))
                {
                    right = doShift(lexer, right, next.value);
                }

                list.Add(right);
                return factory(list);
            }
            private Precedence nextOperator(Lexer lexer) 
            {
                var t = lexer.peek(0);
                if(t.isIdentifier() && ops.ContainsKey(t.getText()))
                {
                    return ops[t.getText()];
                }
                else 
                {
                    return null;
                }
            }
            private static bool rightIsExpr(int prec, Precedence nextPrec) 
            {
                if(nextPrec.leftAssoc)
                {
                    return prec < nextPrec.value;
                }
                else 
                {
                    return prec <= nextPrec.value;
                }
            }
            internal override bool match(Lexer lexer)
            {
                return factor.match(lexer);
            }
        }

        public static readonly string factoryName = "create";
        protected abstract class Factory 
        {
            protected abstract ASTree make0(object arg);
            protected ASTree make(object arg) 
            {
                try 
                {
                    return make0(arg);
                }
                catch (ArgumentException e1)
                {
                    throw e1;
                }
                catch (Exception e2)
                {
                    throw e2; // this compiler is broken.
                }
            }
            internal static Func<object, ASTree> getForASTList(Type typeOfT)
            {
                // if(typeof(ASTLeaf).IsAssignableFrom(typeOfT) == false) 
                // {
                //     // throw new ParseException("Factory get error " + typeOfT);
                //     return null;
                // }

                var f = get(typeOfT, typeof(List<>)); // TODO.. 类型有问题？
                if(f == null) 
                {
                    f = (arg) => 
                    {
                        List<ASTree> results = (List<ASTree>)arg;
                        if(results.Count == 1) 
                        {
                            return results[0];
                        }
                        else 
                        {
                            return new ASTList(results);
                        }
                    };
                }

                return f;
            }
            internal static Func<object, ASTree> get(Type typeOfT, Type argType)
            {
                // if(typeof(ASTLeaf).IsAssignableFrom(typeOfT) == false) 
                // {
                //     // throw new ParseException("Factory get error " + typeOfT);
                //     return null;
                // }
                if(typeOfT == null) 
                {
                    return null;
                }

                try 
                {
                    var m = typeOfT.GetMethod(factoryName, new Type[]{argType});
                    if(m != null) 
                    {
                        return (arg) => { return m.Invoke(null, new object[]{arg}) as ASTree; };
                    }
                }
                catch (Exception e)
                {
                    // throw e;
                }

                try 
                {
                    var ctor = typeOfT.GetConstructor(new Type[]{argType});
                    if(ctor != null)
                    {
                        return (arg) => { return ctor.Invoke(new object[]{arg}) as ASTree; };
                    }
                }
                catch (Exception e) 
                {
                    throw e;
                }

                return null;
            }
        }

        protected List<Element> elements;
        protected Func<object, ASTree> factory;

        public Parser(Type type) 
        {
            // if(typeof(ASTree).IsAssignableFrom(type)) 
            // {
            //     throw new ParseException("new Parser wrong type parm : " + type);
            // }
            reset(type);
        }
        protected Parser(Parser p) 
        {
            elements = p.elements;
            factory = p.factory;
        }

        public ASTree parse(Lexer lexer) 
        {
            List<ASTree> results = new List<ASTree>();
            foreach (var e in elements)
            {
                e.parse(lexer, results);
            }

            return factory(results);
        }

        protected bool match(Lexer lexer) 
        {
            if(elements.Count == 0) 
            {
                return true;
            }
            else 
            {
                var e = elements[0];
                return e.match(lexer);
            }
        }

        public static Parser rule() { return rule(null); }
        public static Parser rule(Type type) 
        {
            return new Parser(type);
        }

        public Parser reset() 
        {
            elements = new List<Element>();
            return this;
        }
        public Parser reset(Type type) 
        {
            elements = new List<Element>();
            factory = Factory.getForASTList(type);
            return this;
        }

        public Parser number() 
        {
            return number(null);
        }
        public Parser number(Type type) 
        {
            elements.Add(new NumToken(type));
            return this;
        }

        public Parser identifier(HashSet<string> reserved) 
        {
            return identifier(null, reserved);
        }
        public Parser identifier(Type type, HashSet<string> reserved) 
        {
            elements.Add(new IdToken(type, reserved));
            return this;
        }

        public Parser string_() 
        {
            return string_(null);
        }
        public Parser string_(Type type) 
        {
            elements.Add(new StrToken(type));
            return this;
        }

        public Parser token(params string[] pat) 
        {
            elements.Add(new Leaf(pat));
            return this;
        }

        public Parser sep(params string[] pat) 
        {
            elements.Add(new Skip(pat));
            return this;
        }

        public Parser ast(Parser p) 
        {
            elements.Add(new Tree(p));
            return this;
        }

        public Parser or(params Parser[] p) 
        {
            elements.Add(new OrTree(p));
            return this;
        }

        public Parser maybe(Parser p) 
        {
            Parser p2 = new Parser(p);
            p2.reset();
            elements.Add(new OrTree(new Parser[]{p, p2}));
            return this;
        }

        public Parser option(Parser p) 
        {
            elements.Add(new Repeat(p, true));
            return this;
        }

        public Parser repeat(Parser p) 
        {
            elements.Add(new Repeat(p, false));
            return this;
        }

        public Parser expression(Parser subexp, Operators operators) 
        {
            elements.Add(new Expr(null, subexp, operators));
            return this;
        }

        public Parser expression(Type type, Parser subexp, Operators operators) 
        {
            elements.Add(new Expr(type, subexp, operators));
            return this;
        }

        public Parser insertChoice(Parser p) {
            Element e = elements[0];
            if (e is OrTree)
                ((OrTree)e).insert(p);
            else {
                Parser otherwise = new Parser(this);
                reset(null);
                or(p, otherwise);
            }
            return this;
        }
    }
}
