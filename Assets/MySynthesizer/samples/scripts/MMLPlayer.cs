using UnityEngine;
using UnityEngine.UI;
using MySpace;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(MyMMLBox))]
public class MMLPlayer : MonoBehaviour {

    public TextAsset Sample1;
    public TextAsset Sample2;
    public TextAsset Sample3;
    public TextAsset Sample4;
    private MyMMLPlayer player;
    private SampleTools sampleTools;
    private GameObject ToneEditorCanvas;
    private GameObject ToneEditorCT8Canvas;
    private Text playButtonText;
    private Text cursolPosText;
    private InputField mmlField;
    private Text consoleText;
    private Text instructionText;
    private readonly System.Text.StringBuilder stb = new System.Text.StringBuilder();
    private int lineCount;
    private bool invalid = true;
    private bool playing = false;

    private MyMMLBox mmlBox;
    private MyMMLClip mmlClip = new MyMMLClip();

    public void SetMMLText(string mml)
    {
        mmlBox.Play("Click");
        mmlField.text = (mml != null) ? mml : "";
        if (!playing)
        {
            consoleText.text = "";
        }
    }
    public void LoadSample1()
    {
        mmlBox.Play("Click");
        mmlField.text = (Sample1 != null) ? Sample1.text : "";
        if (!playing)
        {
            consoleText.text = "";
        }
    }
    public void LoadSample2()
    {
        mmlBox.Play("Click");
        mmlField.text = (Sample2 != null) ? Sample2.text : "";
        if (!playing)
        {
            consoleText.text = "";
        }
    }
    public void LoadSample3()
    {
        mmlBox.Play("Click");
        mmlField.text = (Sample3 != null) ? Sample3.text : "";
        if (!playing)
        {
            consoleText.text = "";
        }
    }
    public void LoadSample4()
    {
        mmlBox.Play("Click");
        mmlField.text = (Sample4 != null) ? Sample4.text : "";
        if (!playing)
        {
            consoleText.text = "";
        }
    }
    private void resetStb()
    {
        lock (stb)
        {
            lineCount = 0;
            stb.Remove(0, stb.Length);
        }
    }
    private void appendStb(string str)
    {
        lock (stb)
        {
            if (lineCount >= 100)
            {
                char[] cbuf = new char[1];
                for (var i = 0; i < stb.Length; i++)
                {
                    stb.CopyTo(i, cbuf, 0, 1);
                    if (cbuf[0] == '\n')
                    {
                        stb.Remove(0, i + 1);
                        break;
                    }
                }
                lineCount--;
            }
            stb.AppendLine(str);
            lineCount++;
        }
    }

    private void appDataEventFunc(int Port, int Channel, int TrackNo, uint MeasureCount, uint TickCount, string Data)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.Append(Data);
        appendStb(sb.ToString());
    }
    private void playingEventFunc(int Port, int Channel, int TrackNo, uint MeasureCount, uint TickCount, uint Step, uint Gate, MySpace.Synthesizer.MMLSequencer.Instruction Inst, MySpace.Synthesizer.MMLSequencer.MyMMLSequence Sequence, int SectionIndex, int InstructionIndex)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        //sb.AppendFormat("{0:D2}:{1:D2}:", Port, Channel);
        sb.AppendFormat("{0:D2} {1:D3}:{2:D3} {3:D3}:{4:D3} ", TrackNo, MeasureCount, TickCount, Step, Gate);
        if ((int)Inst.N == 0)
        {
            sb.Append(Inst.N);
        }
        else if ((int)Inst.N < 128)
        {
            char c0 = "ccddeffggaab"[(int)Inst.N % 12];
            char c1 = " # #  # # # "[(int)Inst.N % 12];
            int oct = ((int)Inst.N / 12) - 2;
            sb.AppendFormat("{0}{1}{2} {3:D3}", c0, c1, oct, Inst.V);
        }
        else
        {
            sb.AppendFormat("{0} <0x{1:X4}>", Inst.N, (Inst.V | ((int)Inst.Q << 8)));
        }
        appendStb(sb.ToString());
    }
    void Awake()
    {
        player = GameObject.FindObjectOfType<MyMMLPlayer>();
        sampleTools = GameObject.FindObjectOfType<SampleTools>();
        mmlBox = sampleTools.GetComponent<MyMMLBox>();
#if false
        {
            string mml = "$@(h_close5) = @pm8[4 7 op1[0 0 15 0 7 0 0 0 0 31 0 0 0 0] op2[0 0 0 0 7 0 0 1 0 21 18 5 15 13] op3[0 0 8 0 3 0 0 0 0 31 0 0 0 14] op4[0 0 15 0 3 0 1 1 5 31 17 15 13 9] lfo[0 127 0 0 31 0 0 0 0] ]\nt0=@(h_close5)a32c32";
            MyMMLClip clip = new MyMMLClip("Click", mml);
            clip.GenerateAudioClip = true;
            mmlBox.Add(clip);
        }
#endif
        invalid = true;
        playing = false;
        ToneEditorCanvas = transform.parent.FindChild("ToneEditorCanvas").gameObject;
        {
            Button button = transform.FindChild("Panel/ToneEditorButton").GetComponent<Button>();
            button.onClick.AddListener(() =>
            {
                mmlBox.Play("Click");
                sampleTools.MoveCameraTo(ToneEditorCanvas.transform.localPosition);
            });
        }
        ToneEditorCT8Canvas = transform.parent.FindChild("ToneEditorCT8Canvas").gameObject;
        {
            Button button = transform.FindChild("Panel/ToneEditorCT8Button").GetComponent<Button>();
            button.onClick.AddListener(() =>
            {
                mmlBox.Play("Click");
                sampleTools.MoveCameraTo(ToneEditorCT8Canvas.transform.localPosition);
            });
        }
        {
            Button button = transform.FindChild("Panel/PlayButton").GetComponent<Button>();
            cursolPosText = transform.FindChild("Panel/CursolPosText").GetComponent<Text>();
            mmlField = transform.FindChild("Panel/MMLField").GetComponent<InputField>();
            consoleText = transform.FindChild("Panel/ConsolePanel").GetComponentInChildren<Text>();
            instructionText = transform.FindChild("Panel/InstructionPanel").GetComponentInChildren<Text>();
            playButtonText = button.GetComponentInChildren<Text>();
            mmlField.onValueChanged.AddListener((str) =>
            {
                invalid = true;
                if (player.Playing)
                {
                    player.Stop();
                    playButtonText.text = "Play";
                    consoleText.text = "";
                    playing = false;
                }
                else
                {
                    playButtonText.text = "Play";
                }
            });
            button.onClick.AddListener(() =>
            {
                if (invalid)
                {
                    if (player.Playing && (player.Clip == mmlClip))
                    {
                        player.Stop();
                        playButtonText.text = "Play";
                        consoleText.text = "";
                        playing = false;
                    }
                    else
                    {
                        mmlClip.TextB = mmlField.text;
                        player.AppDataEvent = appDataEventFunc;
                        player.PlayingEvent = playingEventFunc;
                        player.Play(mmlClip);
                        resetStb();
                        invalid = false;
                        consoleText.text = "Playing...";
                        playButtonText.text = "Pause";
                        playing = true;
                    }
                }
                else
                {
                    if (player.Playing)
                    {
                        player.Pause();
                        consoleText.text = "";
                        playButtonText.text = "Continue";
                        playing = false;
                    }
                    else
                    {
                        player.Continue();
                        consoleText.text = "Playing...";
                        playButtonText.text = "Pause";
                        playing = true;
                    }
                }
            });
        }
    }
    void Update()
    {
        if ((playing && !player.Playing) || (player.Clip != mmlClip))
        {
            invalid = true;
            playing = false;
            playButtonText.text = "Play";
            consoleText.text = mmlClip.Error;
        }
        if (playing)
        {
            string str;
            lock (stb)
            {
                str = stb.ToString();
            }
            instructionText.text = str;
        }
        {
            int line = 1;
            int pos = 1;
            for(int i = 0; i < mmlField.selectionFocusPosition; i++)
            {
                if(mmlField.text[i] == '\n')
                {
                    line++;
                    pos = 0;
                }
                pos++;
            }
            cursolPosText.text = line.ToString() + ":" + pos.ToString();
        }
    }
}
