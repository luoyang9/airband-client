using UnityEngine;
using System.Collections;

public class SampleTools : MonoBehaviour
{
    public void MoveCameraTo(Vector3 loc)
    {
        Transition tr = gameObject.GetComponent<Transition>();
        if (tr == null)
        {
            tr = gameObject.AddComponent<Transition>();
        }
        float t = 0.0f;
        Vector3 pc = Camera.main.transform.localPosition;
        Vector3 px = loc;
        Vector3 pt = new Vector3(px.x, px.y, pc.z);
        tr.Do((dt) =>
        {
            t += dt / 0.3f;
            if (t > 1.0f)
            {
                t = 1.0f;
            }
            Vector3 pos = pc + (pt - pc) * (0.5f - 0.5f * UnityEngine.Mathf.Cos(t * UnityEngine.Mathf.PI));
            Camera.main.transform.localPosition = pos;
            return t < 1.0f;
        });
    }
}
