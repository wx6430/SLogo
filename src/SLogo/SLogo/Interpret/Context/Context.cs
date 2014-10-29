using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLogo.Interpret
{
    /// <summary>
    /// 脚本运行环境的变量上下文，根据变量名查询对应的值
    /// </summary>
    internal class Context : IEnumerable<Variable>, IReadOnlyContext
    {
        /// <summary>
        /// 维护变量的集合
        /// </summary>
        Dictionary<string, Variable> dict;

        public Context()
            : this(null)
        {
        }

        public Context(ICollection<Variable> vars)
        {
            this.dict = new Dictionary<string, Variable>();
            if (vars != null)
            {
                foreach (Variable var in vars)
                {
                    this.Add(var);
                }
            }
        }

        public void Add(Variable item)
        {
            if (item != null)
            {
                //无视大小写
                dict[item.Name.ToLower()] = item;
            }
        }

        public bool Contains(string name)
        {
            return dict.ContainsKey(name.ToLower());
        }

        public bool Remove(string name)
        {
            return dict.Remove(name.ToLower());
        }

        public Variable this[string name]
        {
            get
            {
                if (dict.ContainsKey(name.ToLower()))
                {
                    return dict[name.ToLower()];
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.Add(value);
            }
        }

        #region IEnumerable<Variable> 成员

        public IEnumerator<Variable> GetEnumerator()
        {
            foreach (Variable var in dict.Values)
            {
                yield return var;
            }
        }

        #endregion

        #region IEnumerable 成员

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
