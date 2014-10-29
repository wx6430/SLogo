using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLogo.Lex;

namespace SLogo.Interpret
{
    internal static class TokenListHelper
    {
        /// <summary>
        /// 链表扩展方法，按指定索引复制新链表;
        /// 新链表的范围是：[start, end)
        /// 若end在start之前，则YankList返回从start至结束的所有节点构成的链表
        /// </summary>
        /// <param name="list">链表</param>
        /// <param name="start">开始节点</param>
        /// <param name="end">终止节点</param>
        /// <returns>抽取的新链表</returns>
        public static LinkedList<Token> YankList(this LinkedList<Token> list, LinkedListNode<Token> start, LinkedListNode<Token> end)
        {
            if (list == null || start == null || end == null)
            {
                throw new ArgumentNullException();
            }
            if (start.List != list || end.List != list)
            {
                throw new ArgumentException("节点不在链表里");
            }
            LinkedList<Token> ret = new LinkedList<Token>();
            for (var iter = start; iter != null && iter != end; iter = iter.Next)
            {
                ret.AddLast(iter.Value);
            }
            return ret;
            //if (start == list.First)
            //{
            //    while (list.First != null && list.First != end)
            //    {
            //        ret.AddLast(list.First.Value);
            //        list.RemoveFirst();
            //    }
            //    return ret;
            //}
            //else
            //{
            //    var iter = start.Previous;
            //    while (iter.Next != null && iter.Next != end)
            //    {
            //        ret.AddLast(iter.Next.Value);
            //        list.Remove(iter.Next);
            //    }
            //    return ret;
            //}
        }
    }
}
