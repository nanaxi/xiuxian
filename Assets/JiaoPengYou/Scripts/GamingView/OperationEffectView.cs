using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace Lang
{
    public class OperationEffectView : View
    {
        public Transform[] positions;
        public GameObject[] effects;

        Dictionary<EffectType, GameObject> cachedEffects;

        public enum EffectType
        {
            Che = 0,
            LianZi,
            PengYouChuXian,
            Qiang,
            YiTiaoLong,
            ZhaDan,
            ZhaDanHuo
        }

        void OnDestroy()
        {
            Clear();
        }

        public void Show(EffectType type, Seat seat)
        {
            if (cachedEffects == null)
            {
                cachedEffects = new Dictionary<EffectType, GameObject>(effects.Length);
            }
            StartCoroutine(DelayShow(type, seat));
        }

        public void Clear()
        {
            foreach (var go in cachedEffects.Values)
            {
                Destroy(go);
            }
            cachedEffects.Clear();
        }

        IEnumerator DelayShow(EffectType type, Seat seat)
        {
            GameObject effectObj = null;
            if (!cachedEffects.TryGetValue(type, out effectObj))
            {
                var effect = effects[(int)type];
                effectObj = GameObject.Instantiate(effect);
                cachedEffects.Add(type, effectObj);
            }

            effectObj.transform.position = positions[(int)seat].position;

            if (type == EffectType.LianZi)
            {
                effectObj.transform.position += new Vector3(1, 0, 0);
            }
            else if (type == EffectType.PengYouChuXian)
            {
                effectObj.transform.position = positions[0].position;
            }

            effectObj.gameObject.SetActive(true);
            yield return new WaitForSeconds(2);
            effectObj.gameObject.SetActive(false);
        }
    }
}