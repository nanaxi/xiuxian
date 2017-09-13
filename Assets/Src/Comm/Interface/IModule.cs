using UnityEngine;
using System.Collections;

public interface IModule
{
    bool Init();
    bool UnInit();
    void Update();
}
