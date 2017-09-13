using UnityEngine;

namespace Lang
{
    public class View : MonoBehaviour
    {
        public ViewLayer DefaultLayer
        {
            get { return ViewLayer.Middle; }
        }

        public virtual ViewLayer GetLayer()
        {
            return DefaultLayer;
        }
    }
}