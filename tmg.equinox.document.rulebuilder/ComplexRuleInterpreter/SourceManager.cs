using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace tmg.equinox.document.rulebuilder.ComplexRuleInterpreter
{
    class SourceManager
    {
        private static object _locker = new object();
        public static Dictionary<Thread, Dictionary<string, JToken>> _dicSources = new Dictionary<Thread, Dictionary<string, JToken>>();

        public static void Add(Thread t, string key, JToken data)
        {
            Dictionary<string, JToken> val;
            lock (_locker)
            {
                if (_dicSources.ContainsKey(t))
                {
                    _dicSources.TryGetValue(t, out val);
                    if (!val.ContainsKey(key))
                    {
                        val.Add(key, data);
                    }
                }
                else
                {
                    val = new Dictionary<string, JToken>();
                    val.Add(key, data);
                    _dicSources.Add(t, val);
                }
            }
        }

        public static void AddTarget(Thread t, JToken targetData)
        {
            Dictionary<string, JToken> val;
            lock (_locker)
            {
                if (_dicSources.ContainsKey(t))
                {
                    _dicSources.TryGetValue(t, out val);
                    if (!val.ContainsKey("target"))
                    {
                        val.Add("target", targetData);
                    }
                }
                else
                {
                    val = new Dictionary<string, JToken>();
                    val.Add("target", targetData);
                    _dicSources.Add(t, val);
                }
            }
        }

        public static JToken Get(Thread t, string key)
        {
            Dictionary<string, JToken> val = null;
            JToken value = null;
            lock (_locker)
            {
                if (_dicSources.ContainsKey(t))
                {
                    _dicSources.TryGetValue(t, out val);
                }

                if (val.ContainsKey(key))
                {
                    val.TryGetValue(key, out value);
                }
            }
            return value;
        }

        public static void Set(Thread t, string key, JToken data)
        {
            Dictionary<string, JToken> val = null;
            lock (_locker)
            {
                if (_dicSources.ContainsKey(t))
                {
                    _dicSources.TryGetValue(t, out val);
                }
                if (val.ContainsKey(key))
                {
                    val[key] = data;
                }
            }
        }

        public static void Remove(Thread t)
        {
            lock (_locker)
            {
                if (_dicSources.ContainsKey(t))
                {
                    _dicSources.Remove(t);
                }
            }
        }
    }
}
