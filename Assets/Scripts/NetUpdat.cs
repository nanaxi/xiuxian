using System;
using System.Collections;
using System.Threading;

public class NetUpdat{

   
    static public void NetUpdate()
    {
        Thread netUpdateThread;
        netUpdateThread = new Thread(test);
        netUpdateThread.Start();
    }
    static void test()
    {
        while (true)
        {
            
            Thread.Sleep(100);
        }

    }
}
