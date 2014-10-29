using System;
namespace SLogo.Interpret
{
    internal interface IContextChain
    {
        bool Contains(string name);
        SLogo.Interpret.IReadOnlyContext Head { get; }
        SLogo.Interpret.IReadOnlyContext Pop();
        void Push(SLogo.Interpret.IReadOnlyContext context);
        SLogo.Interpret.IReadOnlyContext Tail { get; }
        SLogo.Interpret.Variable this[string name] { get; }
    }
}
