using UnityEngine;

namespace Lang
{
    public class SetFrame : MonoBehaviour
    {
        void Awake()
        {
            Application.targetFrameRate = 30;
        }
    }
}