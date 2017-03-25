
using System;
using UnityEngine;

public class WaveTable : MonoBehaviour
{
    [HideInInspector]
    public readonly byte[] WT = new byte[32];
    [HideInInspector]
    public Action<int> OnUpdateAction;

    void Awake()
    {
        var masterButton = transform.FindChild("Button").GetComponent<GridButton>();
        RectTransform r = masterButton.GetComponent<RectTransform>();
        float width = r.rect.width;
        float height = r.rect.height;
        Vector3 basePos = r.transform.localPosition;
        Vector3 baseScl = masterButton.transform.localScale;
        Quaternion baseRot = masterButton.transform.rotation;
        for (int y = 0; y < 32; y++)
        {
            for(int x = 0; x < 32; x++)
            {
                GridButton btn = Instantiate(masterButton);
                Vector3 pos = new Vector3();
                pos.x = basePos.x + x * width;
                pos.y = basePos.y + y * height;
                btn.transform.SetParent(transform);
                btn.transform.localPosition = pos;
                btn.transform.localScale = baseScl;
                btn.transform.rotation = baseRot;
                btn.Tx = x;
                btn.Ty = y;
                btn.WT = this;
                int tx = x;
                int ty = y;
                btn.OnKeyDownEvent.AddListener(() =>
                {
                    WT[tx] = (byte)ty;
                    OnUpdateAction(tx);
                });
            }
        }
        masterButton.gameObject.SetActive(false);
    }
}
