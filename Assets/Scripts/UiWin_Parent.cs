using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class UiWin_Parent : MonoBehaviour
{
    public bool isInitVar = false;

    public virtual void Init()
    {
        //throw new NotImplementedException();
        
    }

    public virtual void Set_NetEvent()
    {
        //throw new NotImplementedException();
    }

    public virtual void Set_OnEventList<Component_>(Component_[] all_) where Component_ : Component
    {
        //if (typeof(Component_) == typeof(Button))
        //{
        //    for (int i = 0; i < all_.Length; i++)
        //    {
        //        Button btn_ = all_[i].GetComponent<Button>();
        //        UnityEngine.Events.UnityAction btnOnClck_ = null;
        //        switch (btn_.gameObject.name)
        //        {
        //            case "":
        //                btnOnClck_ = delegate () { };
        //                break;
        //            default:
        //                break;
        //        }
        //        if (btnOnClck_!=null)
        //        {
        //            btn_.onClick.AddListener(delegate ()
        //            {
        //                btnOnClck_.Invoke();
        //            });
        //        }
        //    }
        //}
    }

    static public void Destroy_(GameObject gm)
    {
        if (gm != null)
        {
            Destroy(gm);
        }
    }

    static public void Destroy_(Component cm)
    {
        if (cm != null)
        {
            Destroy(cm);
        }
    }
    
}
