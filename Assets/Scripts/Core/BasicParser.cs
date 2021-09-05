namespace GuaLanguage
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using GuaLanguage.AST;
    using static GuaLanguage.Parser;
    public class BasicParser 
    {
        protected HashSet<string> reserved = new HashSet<string>();
        protected Operators operators = new Operators();
        protected Parser expr0;
        protected Parser primary;
        protected Parser factor;
        protected Parser expr;

        protected Parser statement0;
        protected Parser block;
        protected Parser simple;
        protected Parser statement;

        protected Parser program;

        protected Parser repeatEmptyLine; // Add by zzy: 使得块代码的花括号不一定需要紧跟在上一行

        public BasicParser() 
        {
            repeatEmptyLine = rule().SetName("repeatEmptyLine").repeat(rule().sep(Token.EOL));

            expr0 = rule();
            primary = rule(typeof(PrimaryExpr))
            .or(rule().sep("(").ast(expr0).sep(")"),
                rule().number(typeof(NumberLiteral)),
                rule().identifier(typeof(Name), reserved),
                rule().string_(typeof(StringLiteral))
            );
            factor = rule().or(rule(typeof(NegativeExpr)).sep("-").ast(primary), 
                primary);
            expr = expr0.expression(typeof(BinaryExpr), factor, operators);
            statement0 = rule();
            block = rule(typeof(BlockStmnt))
                .sep("{").option(statement0)
                .repeat(rule().sep(";", Token.EOL).option(statement0))
                .sep("}");
            simple = rule(typeof(PrimaryExpr)).ast(expr);
            /*
                不能支持else之前有EOL。
                因为else本身是个可选项，如果支持else之前可以有换行，那么由于LL(1)只预读一个字符，下面这种代码会有歧义而无法解析：
                if (1) 
                {
                    __log("111");
                }
                
                __log("222");
                原因在于，当读了一个EOL以后，文法认为可以进入else块，但是实际上后面并没有else导致解析失败。
                考虑使用Lua的if-then-else文法，有一个显式的end来指定if块的结束
            */
            statement = statement0.or(
                // rule(typeof(IfStmnt)).sep("if").ast(expr).option(repeatEmptyLine).ast(block)
                //     .option(rule().sep("else").ast(block)),
                // 上面是最初的if
                rule(typeof(IfStmnt)).sep("if").ast(expr).option(repeatEmptyLine).ast(block)
                    // .option(rule().SetName("elsePart").option(repeatEmptyLine).sep("else").option(repeatEmptyLine).ast(block)
                    // .option(repeatEmptyLine).sep("end")
                    // ),
                    // 上面的 option(repeatEmptyLine) 会导致歧义，end前的EOL也会先进入elsePart中
                    .option(repeatEmptyLine) // 这里需要抽出共同的部分，option(repeatEmptyLine)，以免直接进入elsePart
                    .option(rule().SetName("elsePart").sep("else").option(repeatEmptyLine).ast(block).option(repeatEmptyLine)) // else的最后需要挂一个EOL，保证end可以换行，或者也可以直接在end前面挂一个EOL
                    .sep("end")
                ,
                rule(typeof(WhileStmnt)).sep("while").ast(expr).option(repeatEmptyLine).ast(block),
                simple
            );
            program = rule().or(statement, rule(typeof(NullStmnt)))
                .sep(";", Token.EOL);

            reserved.Add(";");
            reserved.Add("}");
            reserved.Add(Token.EOL);

            operators.add("=", 1, Operators.RIGHT);
            operators.add("==", 2, Operators.LEFT);
            operators.add(">", 2, Operators.LEFT);
            operators.add("<", 2, Operators.LEFT);
            operators.add("<=", 2, Operators.LEFT);
            operators.add(">=", 2, Operators.LEFT);
            operators.add("+", 3, Operators.LEFT);
            operators.add("-", 3, Operators.LEFT);
            operators.add("*", 4, Operators.LEFT);
            operators.add("/", 4, Operators.LEFT);
            operators.add("%", 4, Operators.LEFT);
        }

        public ASTree parse(Lexer lexer) 
        {
            return program.parse(lexer);
        }
    }
}