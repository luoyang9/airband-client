
using UnityEngine;
using UnityEngine.UI;
using MySpace;

public class Keyboard : MonoBehaviour{
    private MySyntheStation syntheStation;
    [SerializeField]
    private KeyProperty blackKey = null;
    [SerializeField]
    private KeyProperty whiteKey = null;
    [SerializeField]
    private int baseNote = 60;

    public int BaseNote
    {
        get
        {
            return baseNote;
        }
        set
        {
            baseNote = value;
        }
    }
    public int PortNo
    {
        get;
        set;
    }
    public int ChNo
    {
        get;
        set;
    }

    private int numKeys;
    private int vel = 64;
    private int vol = 100;

    void OnKeyDown(int index)
    {
        //UnityEngine.Debug.Log("key dw:" + (baseNote + index));
        syntheStation.Instrument[PortNo].Channel[ChNo].NoteOn((byte)(BaseNote + index), (byte)vel);
    }
    void OnKeyUp(int index)
    {
        //UnityEngine.Debug.Log("key up:" + (baseNote + index));
        syntheStation.Instrument[PortNo].Channel[ChNo].NoteOff((byte)(BaseNote + index));
    }

    // Use this for initialization
    void Awake() {
        syntheStation = GameObject.FindObjectOfType<MySyntheStation>();

        RectTransform p = GetComponent<RectTransform>();
        RectTransform r = whiteKey.GetComponent<RectTransform>();
        float width = r.rect.width;
        Vector3 basePos = whiteKey.transform.localPosition;
        int[] ofs = { 0, 1, 2, 3, 4, 6, 7, 8, 9, 10, 11, 12 };
        numKeys = (int)((p.rect.width - (p.rect.width / 2 + (basePos.x - width / 2)) * 2) / width) * 12 / 7;

        for (int i = 0; i < numKeys; i++)
        {
            int o = ofs[i % 12];
            if((o & 1) != 0){
                continue;
            }
            int index = i;
            KeyProperty key = Instantiate(whiteKey);
            Vector3 pos = basePos;
            Vector3 scl = whiteKey.transform.localScale;
            Quaternion rot = whiteKey.transform.rotation;
            pos.x += ((i / 12) * 14 + o) * (width / 2);
            key.transform.SetParent(transform);
            key.transform.localPosition = pos;
            key.transform.localScale = scl;
            key.transform.rotation = rot;
            key.OnKeyDownEvent.AddListener(() => OnKeyDown(index));
            key.OnKeyUpEvent.AddListener(() => OnKeyUp(index));
        }
        for (int i = 0; i < numKeys; i++)
        {
            int o = ofs[i % 12];
            if ((o & 1) == 0)
            {
                continue;
            }
            int index = i;
            KeyProperty key = Instantiate(blackKey);
            Vector3 pos = basePos;
            Vector3 scl = blackKey.transform.localScale;
            Quaternion rot = blackKey.transform.rotation;
            pos.x += ((i / 12) * 14 + o) * (width / 2);
            key.transform.SetParent(transform);
            key.transform.localPosition = pos;
            key.transform.localScale = scl;
            key.transform.rotation = rot;
            key.OnKeyDownEvent.AddListener(() => OnKeyDown(index));
            key.OnKeyUpEvent.AddListener(() => OnKeyUp(index));
        }
        whiteKey.gameObject.SetActive(false);
        blackKey.gameObject.SetActive(false);

        Text position = transform.FindChild("Position").GetComponent<Text>();
        Button lshift = transform.FindChild("LShift").GetComponent<Button>();
        BaseNote -= BaseNote % 12;
        position.text = "^C" + (BaseNote / 12 - 2 + 1);
        lshift.onClick.AddListener(() =>
        {
            if (BaseNote - 12 >= 0)
            {
                BaseNote -= 12;
                position.text = "^C" + (BaseNote / 12 - 2 + 1);
            }
        });
        Button rshift = transform.FindChild("RShift").GetComponent<Button>();
        rshift.onClick.AddListener(() =>
        {
            if (BaseNote + numKeys + 12 <= 128)
            {
                BaseNote += 12;
                position.text = "^C" + (BaseNote / 12 - 2 + 1);
            }
        });
        Slider velocity = transform.FindChild("Velocity").GetComponent<Slider>();
        velocity.onValueChanged.AddListener((value) =>
        {
            vel = (int)value;
            velocity.gameObject.transform.FindChild("Value").GetComponent<Text>().text = "" + vel;
        });

        Slider volume = transform.FindChild("Volume").GetComponent<Slider>();
        volume.onValueChanged.AddListener((value) =>
        {
            vol = (int)value;
            syntheStation.Instrument[PortNo].MasterVolume((byte)value);
            volume.gameObject.transform.FindChild("Value").GetComponent<Text>().text = "" + (byte)value;
        });

        KeyProperty hold = transform.FindChild("Hold").GetComponent<KeyProperty>();
        hold.OnKeyDownEvent.AddListener(() => syntheStation.Instrument[PortNo].Channel[ChNo].Damper(+127));
        hold.OnKeyUpEvent.AddListener(() => syntheStation.Instrument[PortNo].Channel[ChNo].Damper(0));

        KeyProperty damp = transform.FindChild("Damp").GetComponent<KeyProperty>();
        damp.OnKeyDownEvent.AddListener(() => syntheStation.Instrument[PortNo].Channel[ChNo].Damper(-127 + 256));
        damp.OnKeyUpEvent.AddListener(() => syntheStation.Instrument[PortNo].Channel[ChNo].Damper(0));
    }
    void Start()
    {
        Slider velocity = transform.FindChild("Velocity").GetComponent<Slider>();
        velocity.value = vel;
        Slider volume = transform.FindChild("Volume").GetComponent<Slider>();
        volume.value = vol;
    }
}
