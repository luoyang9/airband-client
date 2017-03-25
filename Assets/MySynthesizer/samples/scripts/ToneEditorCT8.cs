
using System;

using UnityEngine;
using UnityEngine.UI;

using MySpace;
using MySpace.Synthesizer.CT8;


public class ToneEditorCT8 : MonoBehaviour
{
    private MySyntheStation syntheStation;
    private SampleTools sampleTools;
    private MyMMLBox mmlBox;
    public int PortNo = 2;
    public int ChNo = 0;

    private ToneParam param;
    private InputField inputField;

    private bool disabledEvent = false;
    private void setSliderValue(GameObject obj, string name, string label, float min, float max, int val)
    {
        Text text = obj.transform.FindChild(name + "/Text").GetComponent<Text>();
        Slider slider = obj.transform.FindChild(name + "/Slider").GetComponent<Slider>();
        disabledEvent = true;
        text.text = label + val;
        slider.minValue = min;
        slider.maxValue = max;
        slider.value = val;
        disabledEvent = false;
    }
    private void setupSlider(GameObject obj, string name, string label, Action<float> apply)
    {
        Text text = obj.transform.FindChild(name + "/Text").GetComponent<Text>();
        Slider slider = obj.transform.FindChild(name + "/Slider").GetComponent<Slider>();
        slider.wholeNumbers = true;
        slider.onValueChanged.AddListener((value) =>
        {
            if (!disabledEvent)
            {
                text.text = label + (int)value;
                apply(value);
            }
        });
    }
    private void updateParam()
    {
        syntheStation.Instrument[PortNo].Channel[ChNo].ProgramChange(param);
        inputField.text = param.ToString();
    }

    private void loadTone(ToneParam tone)
    {
        param = tone.Clone();
        {
            GameObject obj = transform.FindChild("Panel/LFO").gameObject;
            setSliderValue(obj, "WaveForm", "lw:", 0, 7, param.Lfo.WS);
            setSliderValue(obj, "Frequency", "lf:", 0, 127, param.Lfo.LF);
            setSliderValue(obj, "PMPower", "lp:", 0, 127, param.Lfo.LP);
            setSliderValue(obj, "AMPower", "la:", 0, 127, param.Lfo.LA);
            if (param.Extended)
            {
                setSliderValue(obj, "AttackRate", "ar:", 0, 127, param.Lfo.Env.xAR);
                setSliderValue(obj, "DecayRate", "dr:", 0, 127, param.Lfo.Env.xDR);
                setSliderValue(obj, "SustainLevel", "sl:", 0, 127, param.Lfo.Env.xSL);
                setSliderValue(obj, "SustainRate", "sr:", 0, 127, param.Lfo.Env.xSR);
                setSliderValue(obj, "ReleaseRate", "rr:", 0, 127, param.Lfo.Env.xRR);
            }
            else
            {
                setSliderValue(obj, "AttackRate", "ar:", 0, 31, param.Lfo.Env.AR);
                setSliderValue(obj, "DecayRate", "dr:", 0, 31, param.Lfo.Env.DR);
                setSliderValue(obj, "SustainLevel", "sl:", 0, 15, param.Lfo.Env.SL);
                setSliderValue(obj, "SustainRate", "sr:", 0, 31, param.Lfo.Env.SR);
                setSliderValue(obj, "ReleaseRate", "rr:", 0, 15, param.Lfo.Env.RR);
            }
        }
        {
            GameObject obj = transform.FindChild("Panel/ENV").gameObject;
            setSliderValue(obj, "WaveStyle", "ws:", 0, 7, param.WS);
            setSliderValue(obj, "KeyScale", "ks:", 0, 3, param.Env.KS);
            setSliderValue(obj, "VelocitySense", "vs:", 0, 7, param.Env.VS);
            setSliderValue(obj, "TotalLevel", "tl:", 0, 127, param.Env.TL);
            if (param.Extended)
            {
                setSliderValue(obj, "AttackRate", "ar:", 0, 127, param.Env.xAR);
                setSliderValue(obj, "DecayRate", "dr:", 0, 127, param.Env.xDR);
                setSliderValue(obj, "SustainLevel", "sl:", 0, 127, param.Env.xSL);
                setSliderValue(obj, "SustainRate", "sr:", 0, 127, param.Env.xSR);
                setSliderValue(obj, "ReleaseRate", "rr:", 0, 127, param.Env.xRR);
            }
            else
            {
                setSliderValue(obj, "AttackRate", "ar:", 0, 31, param.Env.AR);
                setSliderValue(obj, "DecayRate", "dr:", 0, 31, param.Env.DR);
                setSliderValue(obj, "SustainLevel", "sl:", 0, 15, param.Env.SL);
                setSliderValue(obj, "SustainRate", "sr:", 0, 31, param.Env.SR);
                setSliderValue(obj, "ReleaseRate", "rr:", 0, 15, param.Env.RR);
            }
        }
        {
            WaveTable wt = transform.FindChild("Panel/WaveTable").GetComponent<WaveTable>();
            for (int i = 0; i < 32; i++)
            {
                wt.WT[i] = param.WT[i];
            }
        }
        updateParam();
    }
    //private string defaultTone = "@fmt[7 0 op1[0 1 1 0 0 0 0 1 0 31 0 0 0 7] op2[0 1 1 0 0 0 0 1 127 31 0 0 0 7] op3[0 1 1 0 0 0 0 1 127 31 0 0 0 7] op4[0 1 1 0 0 0 0 1 127 31 0 0 0 7] lfo[0 127 0 0 31 0 0 0 0] ]";
    private string defaultTone = "@pm8[7 0 op1[0 0 1 0 0 0 0 1 0 31 0 0 0 15] op2[0 0 1 0 0 0 0 1 127 31 0 0 0 15] op3[0 0 1 0 0 0 0 1 127 31 0 0 0 15] op4[0 0 1 0 0 0 0 1 127 31 0 0 0 15] lfo[0 127 0 0 31 0 0 0 0]]";
    void Awake()
    {
        syntheStation = GameObject.FindObjectOfType<MySyntheStation>();
        sampleTools = GameObject.FindObjectOfType<SampleTools>();
        mmlBox = sampleTools.GetComponent<MyMMLBox>();
        {
            Button extend = transform.FindChild("Panel/Extend").GetComponent<Button>();
            extend.onClick.AddListener(() => {
                param.Extended = !param.Extended;
                updateParam();
                loadTone(param);
            });
        }
        {
            inputField = transform.FindChild("Panel/InputField").GetComponent<InputField>();
            inputField.onEndEdit.AddListener((v) =>
            {
                if ((v != null) && (v.Length != 0))
                {
                    loadTone(new ToneParam(v));
                }
                else
                {
                    loadTone(new ToneParam(defaultTone));
                }
            });

            PresetButton [] presets = GetComponentsInChildren<PresetButton>();
            foreach(PresetButton pb in presets)
            {
                Button btn = pb.GetComponent<Button>();
                ToneParam tp = new ToneParam(pb.ToneData);
                btn.onClick.AddListener(() =>
                {
                    loadTone(tp);
                });
            }
        }
        {
            GameObject obj = transform.FindChild("Panel/LFO").gameObject;
            setupSlider(obj, "WaveForm", "lw:", (val) => { param.Lfo.WS = (Byte)val; updateParam(); });
            setupSlider(obj, "Frequency", "lf:", (val) => { param.Lfo.LF = (Byte)val; updateParam(); });
            setupSlider(obj, "PMPower", "lp:", (val) => { param.Lfo.LP = (Byte)val; updateParam(); });
            setupSlider(obj, "AMPower", "la:", (val) => { param.Lfo.LA = (Byte)val; updateParam(); });
            setupSlider(obj, "AttackRate", "ar:", (val) => { param.Lfo.Env.AR = (Byte)val; updateParam(); });
            setupSlider(obj, "DecayRate", "dr:", (val) => { param.Lfo.Env.DR = (Byte)val; updateParam(); });
            setupSlider(obj, "SustainLevel", "sl:", (val) => { param.Lfo.Env.SL = (Byte)val; updateParam(); });
            setupSlider(obj, "SustainRate", "sr:", (val) => { param.Lfo.Env.SR = (Byte)val; updateParam(); });
            setupSlider(obj, "ReleaseRate", "rr:", (val) => { param.Lfo.Env.RR = (Byte)val; updateParam(); });
        }
        {
            GameObject obj = transform.FindChild("Panel/ENV").gameObject;
            setupSlider(obj, "WaveStyle", "ws:", (val) => { param.WS = (Byte)val; updateParam(); });
            setupSlider(obj, "KeyScale", "ks:", (val) => { param.Env.KS = (Byte)val; updateParam(); });
            setupSlider(obj, "VelocitySense", "vs:", (val) => { param.Env.VS = (Byte)val; updateParam(); });
            setupSlider(obj, "TotalLevel", "tl:", (val) => { param.Env.TL = (Byte)val; updateParam(); });
            setupSlider(obj, "AttackRate", "ar:", (val) => { param.Env.AR = (Byte)val; updateParam(); });
            setupSlider(obj, "DecayRate", "dr:", (val) => { param.Env.DR = (Byte)val; updateParam(); });
            setupSlider(obj, "SustainLevel", "sl:", (val) => { param.Env.SL = (Byte)val; updateParam(); });
            setupSlider(obj, "SustainRate", "sr:", (val) => { param.Env.SR = (Byte)val; updateParam(); });
            setupSlider(obj, "ReleaseRate", "rr:", (val) => { param.Env.RR = (Byte)val; updateParam(); });
        }
        {
            WaveTable wt = transform.FindChild("Panel/WaveTable").GetComponent<WaveTable>();
            wt.OnUpdateAction = (idx) =>
            {
                param.WT[idx] = wt.WT[idx];
                updateParam();
            };
        }
        {
            Keyboard keyboard = transform.FindChild("Panel/Keyboard").GetComponent<Keyboard>();
            keyboard.ChNo = ChNo;
            keyboard.PortNo = PortNo;
        }
        {
            Transform mmlEditorCanvas = transform.parent.FindChild("MMLPlayerCanvas");
            Button button = transform.FindChild("Panel/MMLEditorButton").GetComponent<Button>();
            button.onClick.AddListener(() =>
            {
                mmlBox.Play("Click");
                sampleTools.MoveCameraTo(mmlEditorCanvas.localPosition);
            });
        }
    }
    void Start()
    {
        loadTone(new ToneParam(defaultTone));
    }
}

