using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLogo.Interpret
{
    /// <summary>
    /// 原语表
    /// </summary>
    internal sealed class PrimitiveCollection
    {
        private Dictionary<string, Primitive> primes;

        public PrimitiveCollection()
            : this(null)
        {
        }

        public PrimitiveCollection(IEnumerable<Primitive> primes)
        {
            this.primes = new Dictionary<string, Primitive>();
            if (primes != null)
            {
                foreach (var prime in primes)
                {
                    this.Add(prime);
                }
            }
        }

        public void Add(Primitive prime)
        {
            if (prime != null)
            {
                //无视大小写
                this.primes.Add(prime.Name.ToLower(), prime);
            }
        }

        public bool Contains(string name)
        {
            return this.primes.ContainsKey(name.ToLower());
        }

        public bool Remove(string name)
        {
            return this.primes.Remove(name.ToLower());
        }

        public Primitive this[string name]
        {
            get
            {
                if (this.Contains(name))
                {
                    return this.primes[name.ToLower()];
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
