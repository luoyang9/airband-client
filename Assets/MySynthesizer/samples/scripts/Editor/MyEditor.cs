using UnityEngine;
using UnityEditor;

public class MyEditor : MonoBehaviour{

    // trasnformの子要素を再帰的に列挙
    private static void foreachTransformChild(Transform t, System.Action<Transform> action)
    {
        for (int i = 0; i < t.childCount; i++)
        {
            Transform ct = t.GetChild(i);
            action(ct);
            foreachTransformChild(ct, action);
        }
    }
    // uGUIのRectTransformのscaleを強制的に1.0fへ変更。
    // 子要素は大きさや位置が変わらないよう自動調整。
    // Textコンポーネントはフォントが表示領域のサイズに依存しないのでスケールで調整。
    // Imageコンポーネントのタイルパターンは比率が変わるので注意。
    [MenuItem("GameObject/MyEditor/NormalizeRectTransformScale")]
    public static void NormalizeScale()
    {
        foreach (GameObject obj in Selection.gameObjects)
        {
            // 現在の選択要素のうちRectTransformへのみ適用。
            RectTransform rt = obj.GetComponent<RectTransform>();
            if (rt == null)
            {
                continue;
            }
            Undo.RecordObject(rt, "NormalizeScale:" + rt);
            // 自身のスケールを正規化
            float sx = rt.localScale.x;
            float sy = rt.localScale.y;
            float sz = rt.localScale.z;
            {
                UnityEngine.UI.Text txt = obj.GetComponent<UnityEngine.UI.Text>();
                if(txt != null)
                {
                    Undo.RecordObject(txt, "NormalizeScale:" + rt);
                    sy = sx;
                    sz = sx;
                    txt.fontSize = (int)(txt.fontSize * sx);
                }
                float px = (rt.anchorMax.x - rt.anchorMin.x) * rt.pivot.x + rt.anchorMin.x;
                float py = (rt.anchorMax.y - rt.anchorMin.y) * rt.pivot.y + rt.anchorMin.y;
                rt.anchorMin = new Vector2((rt.anchorMin.x - px) * sx + px, (rt.anchorMin.y - py) * sy + py);
                rt.anchorMax = new Vector2((rt.anchorMax.x - px) * sx + px, (rt.anchorMax.y - py) * sy + py);
                rt.sizeDelta = new Vector2(rt.sizeDelta.x * sx, rt.sizeDelta.y * sy);
                rt.localScale = new Vector3(1.0f, rt.localScale.y / sy, 1.0f);
            }

            // 子要素の位置とサイズを調整。
            foreachTransformChild(rt, (ct) =>
            {
                RectTransform crt = ct as RectTransform;
                if (crt == null)
                {
                    return;
                }
                Undo.RecordObject(crt, "NormalizeScale:" + rt);

                UnityEngine.UI.Text txt = ct.GetComponent<UnityEngine.UI.Text>();
                if(txt != null)
                {
#if true
                    // テキスト要素の時は強制的に横を１に正規化、yzは比率を保つようにスケール。
                    Undo.RecordObject(txt, "NormalizeScale:" + rt);
                    float fs = crt.transform.localScale.x * sx;
                    float isx = fs / sx;
                    float isy = fs / sy;
                    float isz = fs / sz;
                    float cpx = (crt.anchorMax.x - crt.anchorMin.x) * crt.pivot.x + crt.anchorMin.x;
                    float cpy = (crt.anchorMax.y - crt.anchorMin.y) * crt.pivot.y + crt.anchorMin.y;
                    crt.anchorMin = new Vector2((crt.anchorMin.x - cpx) * isx + cpx, (crt.anchorMin.y - cpy) * isy + cpy);
                    crt.anchorMax = new Vector2((crt.anchorMax.x - cpx) * isx + cpx, (crt.anchorMax.y - cpy) * isy + cpy);
                    crt.sizeDelta = new Vector2(crt.sizeDelta.x * fs, crt.sizeDelta.y * fs);
                    crt.anchoredPosition3D = new Vector3(crt.anchoredPosition3D.x * sx, crt.anchoredPosition3D.y * sy, crt.anchoredPosition3D.z * sz);
                    crt.transform.localScale = new Vector3(crt.transform.localScale.x / isx, crt.transform.localScale.y / isy, crt.transform.localScale.z / isz);
                    txt.fontSize = (int)(txt.fontSize * fs);
#else
                    // テキスト要素の時はスケールで調整。
                    float isx = 1.0f / sx;
                    float isy = 1.0f / sy;
                    float cpx = (crt.anchorMax.x - crt.anchorMin.x) * crt.pivot.x + crt.anchorMin.x;
                    float cpy = (crt.anchorMax.y - crt.anchorMin.y) * crt.pivot.y + crt.anchorMin.y;
                    crt.anchorMin = new Vector2((crt.anchorMin.x - cpx) * isx + cpx, (crt.anchorMin.y - cpy) * isy + cpy);
                    crt.anchorMax = new Vector2((crt.anchorMax.x - cpx) * isx + cpx, (crt.anchorMax.y - cpy) * isy + cpy);
                    //crt.sizeDelta = new Vector2(crt.sizeDelta.x * sx, crt.sizeDelta.y * sy);
                    crt.anchoredPosition3D = new Vector3(crt.anchoredPosition3D.x * sx, crt.anchoredPosition3D.y * sy, crt.anchoredPosition3D.z * sz);
                    crt.transform.localScale = new Vector3(crt.transform.localScale.x * sx, crt.transform.localScale.y * sy, crt.transform.localScale.z * sz);
#endif
                }
                else
                {
                    // テキスト以外はサイズを調整。
                    crt.sizeDelta = new Vector2(crt.sizeDelta.x * sx, crt.sizeDelta.y * sy);
                    crt.anchoredPosition3D = new Vector3(crt.anchoredPosition3D.x * sx, crt.anchoredPosition3D.y * sy, crt.anchoredPosition3D.z * sz);
                }
            });
        }
    }

    [MenuItem("GameObject/MyEditor/NormalizeRectTransformScale", true)]
    public static bool ValidateNormalizeScale()
    {
        bool hasRT = false;
        // 現在選択されている物の中にRectTransformを持つ物があればメニューを有効化。
        foreach(GameObject obj in Selection.gameObjects)
        {
            RectTransform rt = obj.GetComponent<RectTransform>();
            if(rt == null)
            {
                continue;
            }
            hasRT = true;
        }
        return hasRT;
    }
}
