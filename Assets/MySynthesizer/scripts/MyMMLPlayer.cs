
using UnityEngine;

using MySpace.Synthesizer.MMLSequencer;

namespace MySpace
{

    [AddComponentMenu("MySynthesizer/MyMMLPlayer")]
    public class MyMMLPlayer : MonoBehaviour
    {
        private MySyntheStation syntheStation;
        private MyMMLSequencer sequencer;
        private MyMMLClip clip;
        private enum PlayState{
            Stopped,
            Prepareing,
            Playing,
            Paused,
            Suspended,
        }
        private PlayState state;

        [SerializeField]
        private float fadeInTime = 0.0f;
        [SerializeField]
        private float playOutTime = 0.0f;
        [SerializeField]
        private float fadeOutTime = 0.0f;
        [SerializeField]
        private float volume = 1.0f;
        [SerializeField]
        private int keyShift = 0;
        [SerializeField]
        private float tempo = 1.0f;

        public float FadeInTime
        {
            get
            {
                return fadeInTime;
            }
            set
            {
                fadeInTime = value;
            }
        }
        public float PlayOutTime
        {
            get
            {
                return playOutTime;
            }
            set
            {
                playOutTime = value;
            }
        }
        public float FadeOutTime
        {
            get
            {
                return fadeOutTime;
            }
            set
            {
                fadeOutTime = value;
            }
        }
        public float Volume
        {
            get
            {
                return volume;
            }
            set
            {
                volume = value;
            }
        }
        /// <summary>+12 ~ -12</summary>
        public int KeyShift
        {
            get
            {
                return keyShift;
            }
            set
            {
                keyShift = value;
            }
        }
        public float Tempo
        {
            get
            {
                return tempo;
            }
            set
            {
                tempo = value;
            }
        }
        public bool Playing
        {
            get
            {
                if(sequencer == null)
                {
                    return false;
                }
                return (sequencer.Playing) || (state == PlayState.Prepareing);
            }
        }
        public MyMMLClip Clip
        {
            get
            {
                return clip;
            }
        }
        public AppDataEventFunc AppDataEvent
        {
            get;
            set;
        }
        public PlayingEventFunc PlayingEvent
        {
            get;
            set;
        }
        public void Play(MyMMLClip Clip)
        {
            if (sequencer == null)
            {
                return;
            }
            sequencer.Stop(playOutTime);
            clip = Clip;
            state = PlayState.Prepareing;
        }
        public void Stop()
        {
            if (state == PlayState.Stopped)
            {
                return;
            }
            sequencer.Stop(fadeOutTime);
            clip = null;
            state = PlayState.Stopped;
        }
        public void Pause()
        {
            if(state == PlayState.Stopped)
            {
                return;
            }
            if (state == PlayState.Playing)
            {
                sequencer.Pause(fadeOutTime);
                state = PlayState.Paused;
            }
            else if(state == PlayState.Prepareing)
            {
                state = PlayState.Suspended;
            }
        }
        public void Continue()
        {
            if (state == PlayState.Stopped)
            {
                return;
            }
            if (state == PlayState.Paused)
            {
                sequencer.Continue(fadeInTime);
                state = PlayState.Playing;
            }
            else if(state == PlayState.Suspended)
            {
                state = PlayState.Prepareing;
            }
        }

        void OnEnable()
        {
            if (syntheStation == null)
            {
                return;
            }
            sequencer = new MyMMLSequencer(syntheStation.TickFrequency);
            syntheStation.AddSequencer(sequencer);
            state = PlayState.Stopped;
            //Debug.Log("start:" + Application.isPlaying);
        }
        void Start()
        {
            syntheStation = GameObject.FindObjectOfType<MySyntheStation>();
            OnEnable();
        }
        void OnDisable()
        {
            //Debug.Log("OnDisable:" + Application.isPlaying);
            if (sequencer == null)
            {
                return;
            }
            if(sequencer.Playing)
            {
                sequencer.Stop(0.0f);
            }
            if(syntheStation != null)
            {
                syntheStation.RemoveSequencer(sequencer);
            }
            clip = null;
            sequencer = null;
            state = PlayState.Stopped;
        }
        void Update()
        {
            if (state == PlayState.Stopped)
            {
                return;
            }
            if (sequencer.Playing)
            {
                sequencer.KeyShift = clip.Key + keyShift;
                sequencer.VolumeScale = clip.Volume * volume;
                sequencer.TempoScale = clip.Tempo * tempo;
            }
            if (state == PlayState.Playing)
            {
                if (!sequencer.Playing)
                {
                    Stop();
                }
                return;
            }
            if (state != PlayState.Prepareing)
            {
                return;
            }
            if (sequencer.Playing)
            {
                return;
            }
            if (clip.Dirty)
            {
                syntheStation.PrepareClip(clip);
            }
            if (!clip.Prepared)
            {
                return;
            }
            if (!clip.Valid)
            {
                Stop();
                return;
            }
            for (int i = 0; i < clip.Unit.Synthesizers.Count; i++)
            {
                sequencer.SetSynthesizer(i, clip.Unit.Synthesizers[i], clip.Unit.ToneSet, clip.Unit.ToneMap, 0xffffffffU);
            }
            sequencer.AppDataEvent = AppDataEvent;
            sequencer.PlayingEvent = PlayingEvent;
            sequencer.KeyShift = clip.Key + keyShift;
            sequencer.VolumeScale = clip.Volume * volume;
            sequencer.TempoScale = clip.Tempo * tempo;
            sequencer.Play(clip.Unit.Sequence, fadeInTime, false);
            state = PlayState.Playing;
        }
    }
}
