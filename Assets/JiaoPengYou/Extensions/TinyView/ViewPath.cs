using System.Collections.Generic;
using UnityEngine;

namespace Lang
{
    public class ViewPath : ISerializationCallbackReceiver
    {
        [SerializeField]
        List<string> keys;
        [SerializeField]
        List<string> values;

        public Dictionary<string, string> Paths { get; private set; }

        public void Init(int count)
        {
            Paths = new Dictionary<string, string>(count);
        }

        public void OnBeforeSerialize()
        {
            keys = new List<string>(Paths.Keys);
            values = new List<string>(Paths.Values);
        }

        public void OnAfterDeserialize()
        {
            Paths = new Dictionary<string, string>(keys.Count);
            for (int i = 0; i < keys.Count; i++)
            {
                Paths.Add(keys[i], values[i]);
            }
        }
    }
}