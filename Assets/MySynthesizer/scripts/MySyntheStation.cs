using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

using System;
using System.Collections.Generic;
#if WINDOWS_UWP
using System.Threading.Tasks;
#else
using MySpace.Tasks;
#endif
using MySpace.Synthesizer;
using MySpace.Synthesizer.PM8;
using MySpace.Synthesizer.SS8;
using MySpace.Synthesizer.CT8;
using MySpace.Synthesizer.MMLSequencer;
using System.Reflection;
using System.Collections;

namespace MySpace
{
    public class MyMMLSequenceUnit
    {
        private List<MySpace.Synthesizer.MySynthesizer> synthesizers;
        private List<object> toneSet;
        private Dictionary<int, int> toneMap;
        private MyMMLSequence sequence;
        private string error;
        public MyMMLSequenceUnit(List<MySpace.Synthesizer.MySynthesizer> Synthesizers, List<object> ToneSet, Dictionary<int, int> ToneMap, MyMMLSequence Sequence, string Error)
        {
            synthesizers = Synthesizers;
            toneSet = ToneSet;
            toneMap = ToneMap;
            sequence = Sequence;
            error = Error;
        }
        public List<MySpace.Synthesizer.MySynthesizer> Synthesizers
        {
            get
            {
                return synthesizers;
            }
        }
        public List<object> ToneSet
        {
            get
            {
                return toneSet;
            }
        }
        public Dictionary<int, int> ToneMap
        {
            get
            {
                return toneMap;
            }
        }
        public MyMMLSequence Sequence
        {
            get
            {
                return sequence;
            }
        }
        public string Error
        {
            get
            {
                return (error != null) ? error : "";
            }
        }
    }

    [ExecuteInEditMode]
    [RequireComponent(typeof(AudioListener))]
    [DisallowMultipleComponent]
    [AddComponentMenu("MySynthesizer/MySyntheStation")]
    public class MySyntheStation : MonoBehaviour
    {
        [SerializeField, ReadOnly]
        private uint baseFrequency = 31250;
        [SerializeField, ReadOnly]
        private uint numVoices = 8;
        [SerializeField, ReadOnly]
        private uint mixingBufferLength = 200;

        private System.Collections.IEnumerator providerA()
        {
            for (;;)
            {
                if (jobListA.Count != 0)
                {
                    yield return StartCoroutine(jobListA.First.Value);
                    jobListA.RemoveFirst();
                }
                else
                {
                    yield return null;
                }
            }
            //yield break;
        }
        private System.Collections.IEnumerator providerB()
        {
            for (;;)
            {
                if (jobListB.Count != 0)
                {
                    yield return StartCoroutine(jobListB.First.Value);
                    jobListB.RemoveFirst();
                }
                else
                {
                    yield return null;
                }
            }
            //yield break;
        }
        private System.Collections.IEnumerator actionJob(Action Action)
        {
            Action();
            yield break;
        }
        private struct AudioClipGeneratorState
        {
            public const int frequency = 44100;
            public const int numChannels = 2;
            public MyMixer mixer;
            public List<MySynthesizer> synthesizers;
            public MyMMLSequencer sequencer;
        }
        private MyMixer mixer;
        private List<MySynthesizer> synthesizers;
        private List<MyMMLSequencer> sequencers;
        private LinkedList<System.Collections.IEnumerator> jobListA;
        private LinkedList<System.Collections.IEnumerator> jobListB;
        private System.Collections.IEnumerator coProviderA;
        private System.Collections.IEnumerator coProviderB;
        private AudioClipGeneratorState acgState;
        private volatile bool disabled = false;

        private void tick()
        {
            lock (sequencers)
            {
                for (int i = 0; i < sequencers.Count; i++)
                {
                    sequencers[i].Tick();
                }
            }
        }
        public void AddSequencer(MyMMLSequencer sequencer)
        {
            lock (sequencer)
            {
                sequencers.Add(sequencer);
            }
        }
        public void RemoveSequencer(MyMMLSequencer sequencer)
        {
            lock (sequencers)
            {
                sequencers.Remove(sequencer);
            }
        }
        public List<MySynthesizer> Instrument
        {
            get
            {
                return synthesizers;
            }
        }
        public float MixerVolume
        {
            get
            {
                return mixer.MasterVolume;
            }
            set
            {
                mixer.MasterVolume = value;
            }
        }
        public uint TickFrequency
        {
            get
            {
                return mixer.TickFrequency;
            }
        }
#if UNITY_EDITOR
        public bool LivingDead
        {
            get;
            private set;
        }
#endif
#if false
        public MySyntheStation()
        {
            UnityEngine.Debug.Log("MySyntheStation");
        }
#endif
        void Awake()
        {
            //UnityEngine.Debug.Log("Awake");
            mixer = new MyMixer((uint)AudioSettings.outputSampleRate, false, mixingBufferLength, baseFrequency);
            mixer.TickCallback = () =>
            {
                tick();
            };
            synthesizers = new List<MySynthesizer>();
            synthesizers.Add(new MySynthesizerPM8(mixer, numVoices));
            synthesizers.Add(new MySynthesizerSS8(mixer, numVoices));
            synthesizers.Add(new MySynthesizerCT8(mixer, numVoices));
            sequencers = new List<MyMMLSequencer>();

            acgState.mixer = new MyMixer(AudioClipGeneratorState.frequency, true, mixingBufferLength, baseFrequency);
            acgState.sequencer = new MyMMLSequencer(acgState.mixer.TickFrequency);
            acgState.synthesizers = new List<MySynthesizer>();
            acgState.synthesizers.Add(new MySynthesizerPM8(acgState.mixer, numVoices));
            acgState.synthesizers.Add(new MySynthesizerSS8(acgState.mixer, numVoices));
            acgState.synthesizers.Add(new MySynthesizerCT8(acgState.mixer, numVoices));
            acgState.mixer.TickCallback = () => acgState.sequencer.Tick();

            jobListA = new LinkedList<System.Collections.IEnumerator>();
            jobListB = new LinkedList<System.Collections.IEnumerator>();
            coProviderA = providerA();
            StartCoroutine(coProviderA);
            coProviderB = providerB();
            StartCoroutine(coProviderB);
#if UNITY_EDITOR
            LivingDead = true;
#endif
        }
        void OnDestroy()
        {
            //UnityEngine.Debug.Log("OnDestroy");
            StopCoroutine(coProviderA);
            StopCoroutine(coProviderB);
            jobListB = null;
            jobListA = null;
            foreach (var ss in synthesizers)
            {
                ss.Terminate();
            }
            synthesizers = null;
            mixer.Terminate();
            mixer = null;
            sequencers = null;
            foreach (var ss in acgState.synthesizers)
            {
                ss.Terminate();
            }
            acgState.synthesizers = null;
            acgState.mixer.Terminate();
            acgState.mixer = null;
            acgState.sequencer = null;
        }
        void OnEnable()
        {
#if UNITY_EDITOR
            if (mixer == null)
            {
                Awake();
            }
#endif
            //UnityEngine.Debug.Log("OnEnable");
            //UnityEngine.Debug.Log("sampleRate=" + AudioSettings.outputSampleRate + " numVoices=" + Synthesizer.MaxNumVoices);
#if false
            int len;
            int num;
            AudioSettings.GetDSPBufferSize(out len, out num);
            UnityEngine.Debug.Log("len=" + len);
            UnityEngine.Debug.Log("num=" + num);
#endif
            disabled = false;
        }
        void OnDisable()
        {
            //UnityEngine.Debug.Log("OnDisable");
            disabled = true;
            foreach (var ss in synthesizers)
            {
                ss.Reset();
            }
            mixer.Update();
            mixer.Reset();
        }
        void OnAudioFilterRead(float[] Data, int Channels)
        {
            var m = mixer;
            if (!disabled && (m != null))
            {
                m.Output(Data, Channels, Data.Length / Channels);
                m.Update();
            }
#if UNITY_EDITOR
            LivingDead = false;
#endif
        }

        public void PrepareClip(MyMMLClip clip)
        {
            AssetBundle bundle = null;
            ClipPreparingJob job = new ClipPreparingJob(this, clip, bundle, false);
            jobListA.AddLast(job.Prepare());
        }
        public void PrepareClipBlocked(MyMMLClip clip)
        {
            AssetBundle bundle = null;
            ClipPreparingJob job = new ClipPreparingJob(this, clip, bundle, true);
            var i = job.Prepare();
            while (i.MoveNext()) ;
        }
        private class ClipPreparingJob
        {
            private readonly MySyntheStation station;
            private readonly AssetBundle bundle;
            private readonly string mmlText;
            private MyMMLClip clip;
            private MyMMLSequence mml;
            private List<object> toneSet;
            private Dictionary<int, int> toneMap;
            private bool dontGenerate;
            public ClipPreparingJob(MySyntheStation Station, MyMMLClip Clip, AssetBundle Bundle, bool DontGenerate)
            {
                station = Station;
                clip = Clip;
                bundle = Bundle;
                mmlText = clip.Text;
                clip.Dirty = false;
                clip.Unit = null;
                dontGenerate = DontGenerate;
            }
            public System.Collections.IEnumerator Prepare()
            {
                string lastError = null;
                mml = null;
                {
                    Task task = Task.Run(() =>
                    {
                        mml = new MyMMLSequence(mmlText);
                    });
                    while (!task.IsCompleted)
                    {
                        yield return null;
                    }
                    //task.Dispose();
                }
                if (mml == null)
                {
                    // task aborted
                    yield break;
                }
                if (mml.ErrorLine != 0)
                {
                    lastError = "Failed: parsing mml <" + mml.ErrorLine.ToString() + ":" + mml.ErrorPosition.ToString() + "> : " + mml.ErrorString;
                    clip.Unit = new MyMMLSequenceUnit(null, null, null, null, lastError);
                    yield break;
                }

                toneSet = new List<object>();
                toneMap = new Dictionary<int, int>();
                Dictionary<string, float[]> resource = new Dictionary<string, float[]>();
                for (var i = 0; i < mml.ToneData.Count; i++)
                {
                    object tone = null;
                    for (int j = 0; j < station.synthesizers.Count; j++)
                    {
                        if ((mml.ToneMask[i] & (1 << j)) != 0)
                        {
                            tone = station.synthesizers[j].CreateToneObject(mml.ToneData[i]);
                            if (tone == null)
                            {
                                lastError = "Failed: CreateToneObject(): toneData[" + mml.ToneName[i] + "] :" + mml.ToneData[i];
                                clip.Unit = new MyMMLSequenceUnit(null, null, null, null, lastError);
                                yield break;
                            }
                        }
                    }
                    if (tone != null)
                    {
                        if (tone is MySpace.Synthesizer.SS8.ToneParam)
                        {
                            MySpace.Synthesizer.SS8.ToneParam ssTone = tone as MySpace.Synthesizer.SS8.ToneParam;
                            float[] samples;
                            if (!resource.TryGetValue(ssTone.ResourceName, out samples))
                            {
                                AudioClip ac;
                                if (bundle != null)
                                {
                                    AssetBundleRequest request;
                                    request = bundle.LoadAssetAsync(ssTone.ResourceName, typeof(AudioClip));
                                    yield return request;
                                    ac = request.asset as AudioClip;
                                }
                                else
                                {
                                    ac = Resources.Load<AudioClip>(ssTone.ResourceName);
                                }
                                if (ac == null)
                                {
                                    lastError = "Failed: Load(): toneData[" + mml.ToneName[i] + "] :" + mml.ToneData[i];
                                    clip.Unit = new MyMMLSequenceUnit(null, null, null, null, lastError);
                                    yield break;
                                }
                                while (ac.loadState == AudioDataLoadState.Loading)
                                {
                                    yield return null;
                                }
                                samples = new float[ac.samples * ac.channels];
                                ac.GetData(samples, 0);
                                if (ac.channels != 1)
                                {
                                    float[] mix = new float[ac.samples];
                                    for (int k = 0; k < ac.samples; k++)
                                    {
                                        float s = 0.0f;
                                        for (int l = 0; l < ac.channels; l++)
                                        {
                                            s += samples[k * ac.channels + l];
                                        }
                                        mix[k] = s;
                                    }
                                    samples = mix;
                                }
                                resource.Add(ssTone.ResourceName, samples);
                            }
                            ssTone.SetSamples(samples);
                        }
                        toneMap.Add(i, toneSet.Count);
                        toneSet.Add(tone);
                    }
                }
                clip.Unit = new MyMMLSequenceUnit(station.synthesizers, toneSet, toneMap, mml, null);
                if (dontGenerate)
                {
                    yield break;
                }
                if (clip.GenerateAudioClip)
                {
                    station.jobListB.AddLast(generate());
                }
                yield break;
            }
            private System.Collections.IEnumerator generate()
            {
                LinkedList<float[]> samples = null;
                {
                    Task task = Task.Run(() =>
                    {
                        samples = generateAudioSamples();
                    });
                    while (!task.IsCompleted)
                    {
                        yield return null;
                    }
                    //task.Dispose();
                }
                if(samples == null)
                {
                    yield break;
                }
                int totalSamples = 0;
                for (var i = samples.First; i != null; i = i.Next)
                {
                    var block = i.Value;
                    totalSamples += block.Length / AudioClipGeneratorState.numChannels;
                }
                AudioClip ac = AudioClip.Create(clip.Name, totalSamples, AudioClipGeneratorState.numChannels, AudioClipGeneratorState.frequency, false);
                int pos = 0;
                for (var i = samples.First; i != null; i = i.Next)
                {
                    var block = i.Value;
                    ac.SetData(block, pos);
                    pos += block.Length / AudioClipGeneratorState.numChannels;
                }
                clip.AudioClip = ac;
                yield break;
            }
            private LinkedList<float[]> generateAudioSamples()
            {
                MyMMLSequencer seq = station.acgState.sequencer;
                MyMixer mix = station.acgState.mixer;
                List<MySynthesizer> sss = station.acgState.synthesizers;
                if ((seq == null) || (mix == null) || (sss == null))
                {
                    return null;
                }
                mix.Reset();
                for (int j = 0; j < sss.Count; j++)
                {
                    seq.SetSynthesizer(j, sss[j], toneSet, toneMap, 0xffffffffU);
                }
                seq.KeyShift = clip.Key;
                seq.VolumeScale = clip.Volume;
                seq.TempoScale = clip.Tempo;
                seq.Play(mml, 0.0f, false);

                LinkedList<float[]> temp = new LinkedList<float[]>();
                const int workSize = 4096;
                float[] work = new float[workSize * AudioClipGeneratorState.numChannels];
                bool zeroCross = false;
                for (;;)
                {
                    if (station.disabled)
                    {
                        return null;
                    }
                    if (seq.Playing)
                    {
                        mix.Update();
                    }
                    Array.Clear(work, 0, work.Length);
                    int numSamples = mix.Output(work, AudioClipGeneratorState.numChannels, workSize);
                    if (numSamples == 0)
                    {
                        if (!zeroCross)
                        {
                            mix.Update();
                            continue;
                        }
                        break;
                    }
                    {
                        float[] block = new float[numSamples * AudioClipGeneratorState.numChannels];
                        Array.Copy(work, block, numSamples * AudioClipGeneratorState.numChannels);
                        temp.AddLast(block);
                        float v0 = work[(numSamples - 1) * AudioClipGeneratorState.numChannels + 0];
                        float v1 = work[(numSamples - 1) * AudioClipGeneratorState.numChannels + 1];
                        zeroCross = (v0 == 0.0f) && (v1 == 0.0f);
                    }
                }
                return temp;
            }
        }
    }

    public class ReadOnlyAttribute : PropertyAttribute
    {
    }
#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUI.PropertyField(position, property, label, true);
            EditorGUI.EndDisabledGroup();
        }
    }
#endif // UNITY_EDITOR

    public class ConditionalAttribute : PropertyAttribute
    {
        public readonly string VariableName;
        public readonly object Equal;
        public ConditionalAttribute(string variableName, object equal)
        {
            VariableName = variableName;
            Equal = equal;
        }
    }
#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(ConditionalAttribute))]
    public class ConditionalDrawer : PropertyDrawer
    {
        private bool valid;
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            ConditionalAttribute attr = attribute as ConditionalAttribute;
            string path = property.propertyPath;
            object obj = property.serializedObject.targetObject;
            var i = getInstance(obj, path.Substring(0, path.LastIndexOf('.')));
            var t = i.GetType();
            var f = t.GetField(attr.VariableName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            var v = f.GetValue(i);
            valid = true;
            if (!v.Equals(attr.Equal))
            {
                valid = false;
            }
            if (!valid)
            {
                return 0.0f;
            }
            return EditorGUI.GetPropertyHeight(property, label, true);
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!valid)
            {
                return;
            }
            EditorGUI.PropertyField(position, property, label, true);
        }
        private object getInstance(SerializedProperty prop)
        {
            string path = prop.propertyPath;
            object obj = prop.serializedObject.targetObject;
            return getInstance(obj, path);
        }
        private object getInstance(object obj, string path)
        {
            var elements = path.Replace(".Array.data[", "[").Split('.');
            foreach (var element in elements)
            {
                int bp0 = element.IndexOf("[");
                int bp1 = element.IndexOf("]");
                if ((bp0 >= 0) && (bp1 >= 0))
                {
                    var elementName = element.Substring(0, bp0);
                    int index;
                    int.TryParse(element.Substring(bp0 + 1, bp1 - bp0 - 1), out index);
                    obj = getValue(obj, elementName, index);
                }
                else
                {
                    obj = getValue(obj, element);
                }
            }
            return obj;
        }
        private object getValue(object parent, string name)
        {
            if (parent == null)
            {
                return null;
            }
            var type = parent.GetType();
            var field = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (field == null)
            {
                return null;
            }
            return field.GetValue(parent);
        }
        private object getValue(object source, string name, int index)
        {
            var enumerable = getValue(source, name) as IEnumerable;
            var enm = enumerable.GetEnumerator();
            while (index-- >= 0)
            {
                enm.MoveNext();
            }
            return enm.Current;
        }

    }
#endif // UNITY_EDITOR
}
