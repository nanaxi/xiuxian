using UnityEngine;
using DG.Tweening;

namespace Lang
{
    public class MovementLoopAnim : MonoBehaviour
    {
        public Vector3 from;
        public Vector3 to;
        public float time = 5;
        public Ease ease;

        void Start()
        {
            var seq = DOTween.Sequence();
            seq.Append(transform.DOLocalMove(from, time));
            seq.Append(transform.DOLocalMove(to, time));
            seq.SetLoops(-1, LoopType.Yoyo);
            seq.SetEase(ease);
        }
    }
}