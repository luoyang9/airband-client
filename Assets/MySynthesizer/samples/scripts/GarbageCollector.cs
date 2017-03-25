#if !UNITY_EDITOR
//#   define ENABLE_GC_TEST
#endif
using UnityEngine;
using System;

public class GarbageCollector : MonoBehaviour
{
#if ENABLE_GC_TEST
    private System.Threading.Thread thread;
    private System.Threading.AutoResetEvent jobEvent;
    private volatile bool exit;
    private volatile bool deep;
    private void threadMain()
    {
        while (!exit)
        {
            jobEvent.WaitOne();
            System.Threading.Thread.Sleep(1);   // delay
            if (deep)
            {
                deep = false;
#if true
                GC.Collect(2);
                GC.Collect(1);
                GC.Collect(0);
#else
                GC.Collect();
#endif
            }
            else
            {
                GC.Collect(0);
            }
        }
        exit = false;
    }

    private readonly WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
    private System.Collections.IEnumerator coGarbageCollection;
    private System.Collections.IEnumerator garbageCollection()
    {
        for (;;)
        {
            jobEvent.Set();
            yield return waitForEndOfFrame;
        }
    }

    void OnEnable()
    {
        jobEvent = new System.Threading.AutoResetEvent(false);
        exit = false;
        deep = false;
        thread = new System.Threading.Thread(threadMain);
        thread.Priority = System.Threading.ThreadPriority.AboveNormal;
        thread.Start();
        coGarbageCollection = garbageCollection();
        StartCoroutine(coGarbageCollection);
    }
    void OnDisable()
    {
        StopCoroutine(coGarbageCollection);
        coGarbageCollection = null;
        if (thread != null)
        {
            exit = true;
            jobEvent.Set();
            thread.Join();
            thread = null;
        }
    }
#endif
    public void DeepCollect()
    {
#if ENABLE_GC_TEST
        deep = true;
        jobEvent.Set();
#endif
    }
}
