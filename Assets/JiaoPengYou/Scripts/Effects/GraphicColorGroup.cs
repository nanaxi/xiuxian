using UnityEngine;
using UnityEngine.UI;

namespace Lang
{
    public class GraphicColorGroup : MonoBehaviour
    {
        [SerializeField, SetProperty("GraphicColor")]
        Color graphicColor = Color.white;
        public Color GraphicColor
        {
            set
            {
                var gs = GetComponentsInChildren<Graphic>();
                if (gs != null)
                {
                    foreach (var g in gs)
                    {
                        g.color = value;
                    }
                }
                graphicColor = value;
            }
        }

        [SerializeField, SetProperty("Interactable")]
        bool interactable = true;
        public bool Interactable
        {
            set
            {
                var g = GetComponent<Graphic>();
                if (g)
                {
                    g.raycastTarget = value;
                }
                var s = GetComponent<Selectable>();
                if (s)
                {
                    s.interactable = value;
                }
                interactable = value;
            }
        }

    }
}