using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLogo.Interpret
{
    /// <summary>
    /// 上下文链，按局部变量优先的顺序维护当前执行环境的上下文。
    /// 例如：
    /// 在子过程中，子过程的实参处于全局环境之前。
    /// 因此，引用变量时先检查实参上下文，再检查全局变量上下文
    /// </summary>
    internal class ContextChain : SLogo.Interpret.IContextChain
    {
        LinkedList<IReadOnlyContext> chain = new LinkedList<IReadOnlyContext>();

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="contexts">上下文环境序列，后进优先</param>
        public ContextChain(params IReadOnlyContext[] contexts)
        {
            foreach (var context in contexts)
            {
                chain.AddFirst(context);
            }
        }

        /// <summary>
        /// 按照上下文链的顺序查找变量是否存在。
        /// 局部变量优先于全局变量
        /// </summary>
        /// <param name="name">变量名</param>
        /// <returns>变量是否在上下文中存在</returns>
        public bool Contains(string name)
        {
            foreach (var context in chain)
            {
                if (context.Contains(name))
                {
                    return true;
                }
            }
            return false;
        }

        public Variable this[string name]
        {
            get
            {
                foreach (var context in chain)
                {
                    Variable var = context[name];
                    if (var != null)
                    {
                        return var;
                    }
                }
                return null;
            }
        }

        public void Push(IReadOnlyContext context)
        {
            chain.AddFirst(context);
        }

        public IReadOnlyContext Pop()
        {
            if (chain.Count != 0)
            {
                var ret = chain.First.Value;
                chain.RemoveFirst();
                return ret;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 链头
        /// </summary>
        public IReadOnlyContext Head
        {
            get
            {
                return chain.First.Value;
            }
        }

        /// <summary>
        /// 链尾
        /// </summary>
        public IReadOnlyContext Tail
        {
            get
            {
                return chain.Last.Value;
            }
        }
    }
}
