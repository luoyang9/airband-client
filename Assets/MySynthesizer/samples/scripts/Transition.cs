
using UnityEngine;
using System;

public class Transition : MonoBehaviour {

    private Func<float, bool> callback;
    public void Do(Func<float, bool> update)
    {
        callback = update;
    }
    void Update()
    {
        float dt = Time.deltaTime;
        if (!callback(dt))
        {
            Destroy(this);
        }
    }
}
