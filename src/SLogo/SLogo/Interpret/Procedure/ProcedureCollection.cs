using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLogo.Interpret
{
    /// <summary>
    /// 子流程集合
    /// </summary>
    internal sealed class ProcedureCollection
    {
        private Dictionary<string, Procedure> procs;

        public ProcedureCollection()
            : this(null)
        {
        }

        public ProcedureCollection(IEnumerable<Procedure> procs)
        {
            this.procs = new Dictionary<string, Procedure>();
            if (procs != null)
            {
                foreach (var proc in procs)
                {
                    this.Add(proc);
                }
            }
        }

        public void Add(Procedure proc)
        {
            if (proc != null)
            {
                //无视大小写
                this.procs[proc.Name.ToLower()] = proc;
            }
        }

        public bool Contains(string name)
        {
            return this.procs.ContainsKey(name.ToLower());
        }

        public bool Remove(string name)
        {
            return this.procs.Remove(name.ToLower());
        }

        public Procedure this[string name]
        {
            get
            {
                if (this.Contains(name))
                {
                    return this.procs[name.ToLower()];
                }
                return null;
            }
            set
            {
                this.Add(value);
            }
        }
    }
}
