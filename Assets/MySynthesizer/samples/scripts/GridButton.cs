
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class GridButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    private Image image;
    private bool downed;
    [SerializeField]
    private Color normal = Color.white;
    [SerializeField]
    private Color pushed = Color.green;
    public WaveTable WT
    {
        get;
        set;
    }
    public int Tx
    {
        get;
        set;
    }
    public int Ty
    {
        get;
        set;
    }
    private readonly UnityEvent onKeyDownEvent;
    public GridButton()
    {
        onKeyDownEvent = new UnityEvent();
    }
    public UnityEvent OnKeyDownEvent
    {
        get
        {
            return onKeyDownEvent;
        }
    }
    void Update()
    {
        image = GetComponent<Image>();
        image.color = (WT.WT[Tx] >= Ty) ? pushed : normal;
    }
    void Start()
    {
        downed = false;
    }
    private void down()
    {
        if (!downed)
        {
            downed = true;
            onKeyDownEvent.Invoke();
        }
    }
    private void up()
    {
        if (downed)
        {
            downed = false;
        }
    }
    public void OnPointerDown(PointerEventData data)
    {
        down();
    }
    public void OnPointerUp(PointerEventData data)
    {
        up();
    }
    public void OnPointerEnter(PointerEventData data)
    {
        if (data.eligibleForClick)
        {
            data.pointerPress = gameObject;
            data.pointerDrag = gameObject;
            //data.rawPointerPress = gameObject;
            down();
        }
    }
    public void OnPointerExit(PointerEventData data)
    {
        up();
    }
}
