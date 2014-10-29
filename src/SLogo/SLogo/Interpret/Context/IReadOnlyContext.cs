using System;
namespace SLogo.Interpret
{
    /// <summary>
    /// 只读的变量上下文环境
    /// </summary>
    internal interface IReadOnlyContext
    {
        bool Contains(string name);
        System.Collections.Generic.IEnumerator<Variable> GetEnumerator();
        Variable this[string name] { get; set; }
    }
}
