using System;
using System.Collections.Generic;
using UnityEngine;

namespace Lang
{
    public class ViewManager : MonoBehaviour
    {
        public bool dontDestroyOnLoad;

        Transform[] layers;

        static ViewManager Instance;

        ViewPath viewPath;
        Dictionary<string, View> cachedViews = new Dictionary<string, View>();
        Dictionary<string, GameObject> tmpOthers = new Dictionary<string, GameObject>();

        void Awake()
        {
            Instance = this;
            if (dontDestroyOnLoad)
            {
                DontDestroyOnLoad(gameObject);
            }
        }

        void Start()
        {
            CreateLayer();

            View[] views = GetComponentsInChildren<View>(true);
            foreach (View view in views)
            {
                if (view.name.EndsWith("View"))
                {
                    cachedViews.Add(view.name, view);
                }
                else
                {
                    Debug.LogErrorFormat("incorrect naming of [ViewManager/{0}]，must be named like XxxView.", view.gameObject.name);
                }
            }

            TextAsset textAsset = Resources.Load<TextAsset>("Views/ViewPath");
            if (textAsset != null)
            {
                viewPath = JsonUtility.FromJson<ViewPath>(textAsset.text);
            }
            else
            {
                Debug.LogError("you must generate ViewPath first.");
            }
        }

        void CreateLayer()
        {
            layers = new Transform[3];
            layers[0] = CreateLayerTransform("Lowest");
            layers[1] = CreateLayerTransform("Middle");
            layers[2] = CreateLayerTransform("Topmost");
        }

        Transform CreateLayerTransform(string name)
        {
            var o = new GameObject(name);
            o.transform.SetParent(transform, false);
            var rt = o.AddComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = new Vector2(1, 1);
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;

            return o.transform;
        }

        void OnDestroy()
        {
            Instance = null;
        }

        public static T Open<T>(bool top = false) where T : View
        {
            string viewName = Instance.GetViewName<T>();
            View view;
            if (Instance.cachedViews.TryGetValue(viewName, out view))
            {
                if (top)
                {
                    view.transform.SetAsLastSibling();
                }
                if (!view.gameObject.activeSelf)
                {
                    view.gameObject.SetActive(true);
                }
                return (T)view;
            }
            else
            {
                string path = null;
                if (Instance.viewPath.Paths.TryGetValue(viewName, out path))
                {
                    T v = Instance.LoadView<T>(path);
                    Instance.cachedViews.Add(v.name, v);
                    return v;
                }
                else
                {
                    throw new Exception(viewName + " is not included in ViewPath");
                }
            }
        }

        public static T Open<T, K>(bool above) where T : View where K : View
        {
            var depth = GetDepth<K>();
            var view = Open<T>();
            if (above)
            {
                view.transform.SetSiblingIndex(depth + 1);
            }
            else
            {
                view.transform.SetSiblingIndex(depth);
            }

            return view;
        }

        public static T Get<T>() where T : View
        {
            string viewName = Instance.GetViewName<T>();
            View view;
            if (Instance.cachedViews.TryGetValue(viewName, out view))
            {
                return (T)view;
            }

            return null;
        }

        public static void Close<T>() where T : View
        {
            string viewName = Instance.GetViewName<T>();
            View view;
            if (Instance.cachedViews.TryGetValue(viewName, out view))
            {
                if (view.gameObject.activeSelf)
                {
                    view.gameObject.SetActive(false);
                }
            }
        }

        public static void Destroy<T>() where T : View
        {
            string viewName = Instance.GetViewName<T>();
            View view;
            if (Instance.cachedViews.TryGetValue(viewName, out view))
            {
                Destroy(view.gameObject);
                Instance.cachedViews.Remove(viewName);
            }
        }

        public static void DestroyAll()
        {
            foreach (var view in Instance.cachedViews.Values)
            {
                Destroy(view.gameObject);
            }
            Instance.cachedViews.Clear();

            foreach (var item in Instance.tmpOthers.Values)
            {
                if (item != null)
                {
                    Destroy(item);
                }
            }
            Instance.tmpOthers.Clear();

            Resources.UnloadUnusedAssets();
        }

        public static string GetHierarchy()
        {
            List<string> viewNames = new List<string>();
            foreach (var layer in Instance.layers)
            {
                foreach (Transform t in layer)
                {
                    viewNames.Add(t.name);
                }
            }
            return string.Join(",", viewNames.ToArray());
        }

        public static Transform Root { get { return Instance.transform; } }

        public static int GetDepth<T>() where T : View
        {
            string viewName = Instance.GetViewName<T>();
            foreach (Transform t in Instance.transform)
            {
                if (t.name.Equals(viewName))
                {
                    return t.GetSiblingIndex();
                }
            }

            return Instance.transform.childCount;
        }

        public static Transform GetLayer(ViewLayer layer)
        {
            return Instance.layers[(int)layer];
        }

        public static void AddOther(GameObject go)
        {
            Instance.tmpOthers[go.name] = go;
        }

        T LoadView<T>(string path) where T : View
        {
            GameObject res = Resources.Load<GameObject>(path);
            if (res == null)
            {
                throw new Exception("invalid path " + path);
            }

            GameObject o = Instantiate(res);
            o.name = res.name;
            T v = o.GetComponent<T>();
            if (v == null)
            {
                throw new Exception(o.name + " has no View on it");
            }
            var layer = v.GetLayer();
            o.transform.SetParent(layers[(int)layer], false);
            return v;
        }

        string GetViewName<T>() where T : View
        {
            return typeof(T).Name;
        }
    }
}