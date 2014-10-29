using System.Collections.Generic;
using SLogo.Lex;
using System;

namespace SLogo.Interpret
{
    public sealed partial class Interpreter
    {
        /// <summary>
        /// 关键字列表
        /// </summary>
        private string[] keywords = { "if", "ifelse", "to", "end", "repeat" };

        /// <summary>
        /// 词语序列
        /// </summary>
        public LinkedList<Token> tokenList;

        /// <summary>
        /// 全局变量上下文
        /// </summary>
        private Context globalContext;

        /// <summary>
        /// 变量上下文链
        /// </summary>
        private ContextChain contextChain;

        /// <summary>
        /// 原语表
        /// </summary>
        private PrimitiveCollection primes;

        /// <summary>
        /// 子流程表
        /// </summary>
        private ProcedureCollection procs;

        #region 解释过程状态记录

        private bool isError = false;
        private LinkedListNode<Token> current = null;

        #endregion

        /// <summary>
        /// 是否解释出错
        /// </summary>
        public bool IsError
        {
            get
            {
                return this.isError;
            }
        }

        /// <summary>
        /// 初始化函数
        /// </summary>
        private void init()
        {
            //初始化变量上下文
            globalContext = new Context();
            contextChain = new ContextChain(globalContext);

            //初始化原语表
            primes = new PrimitiveCollection();

            //初始化子流程表
            procs = new ProcedureCollection();
        }

        public Interpreter(ITokenFeed tokenFeed)
        {
            tokenList = new LinkedList<Token>();
            foreach (var token in tokenFeed)
            {
                tokenList.AddLast(token);
            }
            init();
            //初始化原语
            initPrimes();
        }

        public void AddPrimitive(string name, int argCount, PrimitiveHandler handler)
        {
            if (String.IsNullOrEmpty(name)
                || argCount <0
                || handler == null)
            {
                throw new ArgumentException();
            }
            this.primes.Add(new Primitive(name, argCount, handler));
        }

        private void initPrimes()
        {
            primes.Add(new Primitive("make", 2, this.makeHandler));
        }

        /// <summary>
        /// 开始解释执行
        /// </summary>
        public void Go()
        {
            isError = false;
            current = null;

            //无论何时，在分词正确的情况下，链表尾总有EOF节点
            if (tokenList.Count == 0)
            {
                //TODO: 解析出错
                return;
            }
            try
            {
                lisp(tokenList);
            }
            catch (SeriousException)
            {
                return;
            }
            catch (ProcStopSignal)
            {
                //捕获STOP信号
                return;
            }
        }
    }
}
