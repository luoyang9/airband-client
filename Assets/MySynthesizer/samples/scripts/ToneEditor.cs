
using System;

using UnityEngine;
using UnityEngine.UI;

using MySpace;
using MySpace.Synthesizer.PM8;


public class ToneEditor : MonoBehaviour
{
    private MySyntheStation syntheStation;
    private SampleTools sampleTools;
    private MyMMLBox mmlBox;
    public int PortNo = 0;
    public int ChNo = 15;

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
    private void applyTone()
    {
        syntheStation.Instrument[PortNo].Channel[ChNo].ProgramChange(param.Clone());
        inputField.text = param.ToString();
    }

    private void loadTone(ToneParam tone)
    {
        param = tone;
        {
            GameObject obj = transform.FindChild("Panel/FM").gameObject;
            setSliderValue(obj, "Algorithm", "al:", 0, 7, param.Al);
            setSliderValue(obj, "Feedback", "fb:", 0, 7, param.Fb);
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
        for (int i = 0; i < 4; i++)
        {
            int n = i;
            GameObject obj = transform.FindChild("Panel/OP" + (i + 1)).gameObject;
            setSliderValue(obj, "WaveStyle", "ws:", 0, 7, param.Op[n].WS);
            setSliderValue(obj, "AMEnable", "ae:", 0, 1, param.Op[n].AE);
            setSliderValue(obj, "Multiple", "ml:", 0, 15, param.Op[n].Ml);
            setSliderValue(obj, "Detune", "dt:", 0, 7, param.Op[n].Dt);
            setSliderValue(obj, "KeyScale", "ks:", 0, 3, param.Op[n].Env.KS);
            setSliderValue(obj, "VelocitySense", "vs:", 0, 7, param.Op[n].Env.VS);
            setSliderValue(obj, "TotalLevel", "tl:", 0, 127, param.Op[n].Env.TL);
            if (param.Extended)
            {
                setSliderValue(obj, "AttackRate", "ar:", 0, 127, param.Op[n].Env.xAR);
                setSliderValue(obj, "DecayRate", "dr:", 0, 127, param.Op[n].Env.xDR);
                setSliderValue(obj, "SustainLevel", "sl:", 0, 127, param.Op[n].Env.xSL);
                setSliderValue(obj, "SustainRate", "sr:", 0, 127, param.Op[n].Env.xSR);
                setSliderValue(obj, "ReleaseRate", "rr:", 0, 127, param.Op[n].Env.xRR);
            }
            else
            {
                setSliderValue(obj, "AttackRate", "ar:", 0, 31, param.Op[n].Env.AR);
                setSliderValue(obj, "DecayRate", "dr:", 0, 31, param.Op[n].Env.DR);
                setSliderValue(obj, "SustainLevel", "sl:", 0, 15, param.Op[n].Env.SL);
                setSliderValue(obj, "SustainRate", "sr:", 0, 31, param.Op[n].Env.SR);
                setSliderValue(obj, "ReleaseRate", "rr:", 0, 15, param.Op[n].Env.RR);
            }
        }
        applyTone();
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
                    inputField.text = param.ToString();
                });
            }
        }
        {
            GameObject obj = transform.FindChild("Panel/FM").gameObject;
            setupSlider(obj, "Algorithm", "al:", (val) => { param.Al = (Byte)val; applyTone(); });
            setupSlider(obj, "Feedback", "fb:", (val) => { param.Fb = (Byte)val; applyTone(); });
            setupSlider(obj, "WaveForm", "lw:", (val) => { param.Lfo.WS = (Byte)val; applyTone(); });
            setupSlider(obj, "Frequency", "lf:", (val) => { param.Lfo.LF = (Byte)val; applyTone(); });
            setupSlider(obj, "PMPower", "lp:", (val) => { param.Lfo.LP = (Byte)val; applyTone(); });
            setupSlider(obj, "AMPower", "la:", (val) => { param.Lfo.LA = (Byte)val; applyTone(); });
            setupSlider(obj, "AttackRate", "ar:", (val) => { if (param.Extended) { param.Lfo.Env.xAR = (Byte)val; } else { param.Lfo.Env.AR = (Byte)val; }; applyTone(); });
            setupSlider(obj, "DecayRate", "dr:", (val) => { if (param.Extended) { param.Lfo.Env.xDR = (Byte)val; } else { param.Lfo.Env.DR = (Byte)val; }; applyTone(); });
            setupSlider(obj, "SustainLevel", "sl:", (val) => { if (param.Extended) { param.Lfo.Env.xSL = (Byte)val; } else { param.Lfo.Env.SL = (Byte)val; }; applyTone(); });
            setupSlider(obj, "SustainRate", "sr:", (val) => { if (param.Extended) { param.Lfo.Env.xSR = (Byte)val; } else { param.Lfo.Env.SR = (Byte)val; }; applyTone(); });
            setupSlider(obj, "ReleaseRate", "rr:", (val) => { if (param.Extended) { param.Lfo.Env.xRR = (Byte)val; } else { param.Lfo.Env.RR = (Byte)val; }; applyTone(); });
        }
        for (int i = 0; i < 4; i++)
        {
            int n = i;
            GameObject obj = transform.FindChild("Panel/OP" + (i + 1)).gameObject;
            setupSlider(obj, "WaveStyle", "ws:", (val) => { param.Op[n].WS = (Byte)val; applyTone(); });
            setupSlider(obj, "AMEnable", "ae:", (val) => { param.Op[n].AE = (Byte)val; applyTone(); });
            setupSlider(obj, "Multiple", "ml:", (val) => { param.Op[n].Ml = (Byte)val; applyTone(); });
            setupSlider(obj, "Detune", "dt:", (val) => { param.Op[n].Dt = (Byte)val; applyTone(); });
            setupSlider(obj, "KeyScale", "ks:", (val) => { param.Op[n].Env.KS = (Byte)val; applyTone(); });
            setupSlider(obj, "VelocitySense", "vs:", (val) => { param.Op[n].Env.VS = (Byte)val; applyTone(); });
            setupSlider(obj, "TotalLevel", "tl:", (val) => { param.Op[n].Env.TL = (Byte)val; applyTone(); });
            setupSlider(obj, "AttackRate", "ar:", (val) => { if (param.Extended) { param.Op[n].Env.xAR = (Byte)val; } else { param.Op[n].Env.AR = (Byte)val; }; applyTone(); });
            setupSlider(obj, "DecayRate", "dr:", (val) => { if (param.Extended) { param.Op[n].Env.xDR = (Byte)val; } else { param.Op[n].Env.DR = (Byte)val; }; applyTone(); });
            setupSlider(obj, "SustainLevel", "sl:", (val) => { if (param.Extended) { param.Op[n].Env.xSL = (Byte)val; } else { param.Op[n].Env.SL = (Byte)val; }; applyTone(); });
            setupSlider(obj, "SustainRate", "sr:", (val) => { if (param.Extended) { param.Op[n].Env.xSR = (Byte)val; } else { param.Op[n].Env.SR = (Byte)val; }; applyTone(); });
            setupSlider(obj, "ReleaseRate", "rr:", (val) => { if (param.Extended) { param.Op[n].Env.xRR = (Byte)val; } else { param.Op[n].Env.RR = (Byte)val; }; applyTone(); });
        }
        {
            Keyboard keyboard = transform.FindChild("Panel/Keyboard").GetComponent<Keyboard>();
            keyboard.PortNo = PortNo;
            keyboard.ChNo = ChNo;
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

