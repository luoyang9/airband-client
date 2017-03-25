using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace MySpace
{
    [AddComponentMenu("MySynthesizer/MyMMLBox")]
    public class MyMMLBox : MonoBehaviour
    {
        private readonly System.Random random = new System.Random();
        private MySyntheStation syntheStation = null;

        [SerializeField]
        private MyMMLPlayer player = null;
        [SerializeField]
        private AudioSource audioSource = null;
        [SerializeField]
        private List<MyMMLClip> clips;

        public MyMMLPlayer Player
        {
            get
            {
                return player;
            }
            set
            {
                player = value;
            }
        }
        public AudioSource Source
        {
            get
            {
                return audioSource;
            }
            set
            {
                audioSource = value;
            }
        }
        public int Count
        {
            get
            {
                return clips.Count;
            }
        }
        public MyMMLClip this[int index]
        {
            get
            {
                return clips[index];
            }
        }
        public void Add(MyMMLClip clip)
        {
            if ((syntheStation != null) && isActiveAndEnabled)
            {
                syntheStation.PrepareClip(clip);
            }
            clips.Add(clip);
        }
        public void RemoveAt(int index)
        {
            clips.RemoveAt(index);
        }
        void Reset()
        {
            player = FindObjectOfType<MyMMLPlayer>();
            audioSource = gameObject.GetComponent<AudioSource>();
            clips = new List<MyMMLClip>();
            if ((audioSource == null) && (player != null))
            {
                audioSource = player.GetComponent<AudioSource>();
            }
            if (audioSource == null)
            {
                audioSource = Camera.main.GetComponent<AudioSource>();
            }
#if false
            if (audioSource == null)
            {
                audioSource = FindObjectOfType<AudioSource>();
            }
#endif
        }
        void OnEnable()
        {
            if (syntheStation == null)
            {
                return;
            }
            foreach (var clip in clips)
            {
                syntheStation.PrepareClip(clip);
            }
        }
        void Start()
        {
            syntheStation = FindObjectOfType<MySyntheStation>();
            OnEnable();
        }
        void OnDisable()
        {
            if (syntheStation == null)
            {
                return;
            }
            foreach (var clip in clips)
            {
                clip.Flush();
            }
        }
        public void Play(int index)
        {
            if ((index < 0) || (index >= clips.Count))
            {
                return;
            }
            var clip = clips[index];
            if (clip.GenerateAudioClip)
            {
                if (clip.AudioClip == null)
                {
                    return;
                }
                if (audioSource == null)
                {
                    return;
                }
                if (clip.Loop)
                {
                    audioSource.Stop();
                    audioSource.loop = true;
                    audioSource.clip = clip.AudioClip;
                    audioSource.Play();
                }
                else
                {
                    audioSource.PlayOneShot(clip.AudioClip, 1.0f);
                }
            }
            else
            {
                if (player == null)
                {
                    return;
                }
                player.Play(clip);
            }
        }
        public void Play(string Name)
        {
            int cnt = 0;
            int index = -1;
            for (int i = 0; i < clips.Count; i++)
            {
                if (clips[i].Name == Name)
                {
                    if (!clips[i].Valid)
                    {
                        continue;
                    }
                    if (index < 0)
                    {
                        index = i;
                    }
                    cnt++;
                }
            }
            if (cnt == 0)
            {
                return;
            }
            if (cnt > 1)
            {
                int num = random.Next() % cnt;
                for (int i = 0; i < clips.Count; i++)
                {
                    if (clips[i].Name == Name)
                    {
                        if (!clips[i].Valid)
                        {
                            continue;
                        }
                        if (num-- == 0)
                        {
                            index = i;
                            break;
                        }
                    }
                }
            }
            Play(index);
        }

#if UNITY_EDITOR
        [CustomEditor(typeof(MyMMLBox))]
        private class MyMMLBoxEditor : Editor
        {
            private object getInstance(SerializedProperty prop)
            {
                var path = prop.propertyPath.Replace(".Array.data[", "[");
                object obj = prop.serializedObject.targetObject;
                var elements = path.Split('.');
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
            private MySyntheStation syntheStation;
            private MySpace.Synthesizer.MMLSequencer.MyMMLSequencer sequencer;
            void OnEnale()
            {
                syntheStation = FindObjectOfType<MySyntheStation>();
            }
            void onDisable()
            {
                if(sequencer != null)
                {
                    sequencer.Stop(0.0f);
                    syntheStation.RemoveSequencer(sequencer);
                    sequencer = null;
                }
                syntheStation = null;
            }
            public override void OnInspectorGUI()
            {
#if false
                if(null == FindObjectOfType<MySyntheStation>())
                {
                    EditorGUILayout.HelpBox("MyMMLBox requires MySyntheStation", MessageType.Warning);
                }
#endif
                {
                    var curSyntheStation = FindObjectOfType<MySyntheStation>();
                    if (curSyntheStation != syntheStation)
                    {
                        syntheStation = curSyntheStation;
                        if (sequencer != null)
                        {
                            sequencer.Stop(0.0f);
                            syntheStation.RemoveSequencer(sequencer);
                            sequencer = null;
                        }
                    }
                }
                serializedObject.Update();
                EditorGUILayout.PropertyField(serializedObject.FindProperty("player"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("audioSource"));
                editorGUILayout_Clips(serializedObject.FindProperty("clips"));
                serializedObject.ApplyModifiedProperties();
            }
            private void editorGUILayout_PropertyChildren(SerializedProperty property)
            {
                if (property.isExpanded)
                {
                    EditorGUI.indentLevel++;
                    foreach (SerializedProperty child in property)
                    {
                        GUIContent label = ((child.name == null) || (child.name.Length < 1)) ? GUIContent.none : new GUIContent(char.ToUpper(child.name[0]) + child.name.Substring(1));
                        EditorGUILayout.PropertyField(child, label, true);
                    }
                    EditorGUI.indentLevel--;
                }
            }
            private void editorGUILayout_Clips(SerializedProperty property)
            {
                MyMMLBox box = property.serializedObject.targetObject as MyMMLBox;
                bool expand = EditorGUILayout.Foldout(property.isExpanded, "Clips");
                property.isExpanded = expand;
                if (property.isExpanded)
                {
                    Repaint();
                    //
                    EditorGUI.indentLevel++;
                    for (int i = 0; i < property.arraySize; i++)
                    {
                        SerializedProperty prop1 = property.GetArrayElementAtIndex(i);
                        MyMMLClip clip = getInstance(prop1) as MyMMLClip;
                        EditorGUI.BeginChangeCheck();
                        SerializedProperty nameProp = prop1.FindPropertyRelative("name");
                        string name = (nameProp == null) ? prop1.name : nameProp.stringValue;
                        if ((name == null) || (name.Length < 1))
                        {
                            name = "New Clip";
                        }
                        GUIContent elmLabel = new GUIContent(char.ToUpper(name[0]) + name.Substring(1));
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.PropertyField(prop1, elmLabel, false);
                        if ((syntheStation == null) || (syntheStation.LivingDead) || (Application.isPlaying))
                        {
                            bool old = GUI.enabled;
                            GUI.enabled = false;
                            GUILayout.Button("Play");
                            GUI.enabled = old;
                        }
                        else
                        {
                            if ((sequencer != null) && (sequencer.Playing) && (clip.Unit != null) && (sequencer.Sequence == clip.Unit.Sequence))
                            {
                                if (GUILayout.Button("Stop"))
                                {
                                    sequencer.Stop(0.0f);
                                    return;
                                }
                                sequencer.VolumeScale = clip.Volume;
                                sequencer.TempoScale = clip.Tempo;
                                sequencer.KeyShift = clip.Key;
                            }
                            else
                            {
                                if (GUILayout.Button("Play"))
                                {
                                    if (!clip.Valid)
                                    {
                                        syntheStation.PrepareClipBlocked(clip);
                                        if (!clip.Valid)
                                        {
                                            return;
                                        }
                                    }
                                    if (sequencer == null)
                                    {
                                        sequencer = new Synthesizer.MMLSequencer.MyMMLSequencer(syntheStation.TickFrequency);
                                        syntheStation.AddSequencer(sequencer);
                                    }
                                    else
                                    {
                                        sequencer.Stop(0.0f);
                                    }
                                    for (int j = 0; j < clip.Unit.Synthesizers.Count; j++)
                                    {
                                        sequencer.SetSynthesizer(j, clip.Unit.Synthesizers[j], clip.Unit.ToneSet, clip.Unit.ToneMap, 0xffffffffU);
                                    }
                                    sequencer.VolumeScale = clip.Volume;
                                    sequencer.TempoScale = clip.Tempo;
                                    sequencer.KeyShift = clip.Key;
                                    sequencer.Play(clip.Unit.Sequence, 0.0f, false);
                                    return;
                                }
                            }
                        }
                        EditorGUILayout.Space();
                        if (GUILayout.Button("Del"))
                        {
                            //Debug.Log("Delete");
                            prop1.isExpanded = false;
                            prop1.DeleteCommand();
                            return;
                        }
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.Space();
                        editorGUILayout_PropertyChildren(prop1);
                        if (EditorGUI.EndChangeCheck())
                        {
                            clip.Dirty = true;
                            //clip.Flush();
                        }
                    }
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.Space();
                    if (GUILayout.Button("Add New Clip"))
                    {
                        //Debug.Log("Add New Clip");
                        Undo.RecordObject(box, "Add New Clip");
                        box.Add(new MyMMLClip());
                        return;
                    }
                    EditorGUILayout.Space();
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Space();
                    EditorGUI.indentLevel--;
                }
            }
        }
#endif // UNITY_EDITOR

#if UNITY_EDITOR
        [InitializeOnLoad]
        public class MyMMLClipCreator : Editor
        {
            private static GUIStyle style;
            static MyMMLClipCreator()
            {
#if false
                EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemOnGUI;
#else
                // コールバックの順序を入れ替えてみる:-)
                EditorApplication.HierarchyWindowItemCallback cb = HierarchyWindowItemOnGUI;
                cb += EditorApplication.hierarchyWindowItemOnGUI;
                EditorApplication.hierarchyWindowItemOnGUI = cb;
#endif
            }

            [MenuItem("GameObject/MySynthesizer/MyMMLBox")]
            private static void CreateMMLBox()
            {
                Transform parent = (Selection.activeGameObject == null) ? null : Selection.activeGameObject.transform;
                GameObject obj = CreateNewGameObject(parent);
                Undo.RegisterCreatedObjectUndo(obj, "Create gameObject:" + obj.name);
                obj.AddComponent<MyMMLBox>();
            }

            static readonly int border = 3;
            static int? CheckMousePosition(Rect selectionRect)
            {
                Vector2 mousePos = Event.current.mousePosition;
                if ((mousePos.x < selectionRect.xMin) || (mousePos.x > selectionRect.xMax))
                {
                    return null;
                }
                if (mousePos.y < selectionRect.yMin)
                {
                    return null;
                }
                if (mousePos.y < selectionRect.yMin + border)
                {
                    return -1;
                }
                if (mousePos.y < selectionRect.yMax - border)
                {
                    return 0;
                }
                if (mousePos.y < selectionRect.yMax)
                {
                    return +1;
                }
                //Debug.Log(mousePos + ":" + selectionRect);
                return null;
            }

            static private EventType previousEventType;
            static private bool acceptable = false;
            static private bool dragging = false;
            static private Rect targetRect;
            static private int targetInstanceID;
            static private int? targetDirection;
            static void HierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
            {
                //Debug.Log(Event.current.type + ":" + EditorUtility.InstanceIDToObject(instanceID).name);

                // ほかのコールバックによりイベントが途中でキャンセルされた場合
                // それ以降のオブジェクトにはusedが飛んでくるので、
                // usedの場合キャッシュしたイベントタイプで全オブジェクトを対象とする。
                // あとイベントの切り替わりでイベントの最初かどうかを判定。（あやしい）
                bool firstEvent = false;
                EventType eventType = Event.current.type;
                if (eventType == EventType.Used)
                {
                    eventType = previousEventType;
                }
                else
                {
                    if (eventType != previousEventType)
                    {
                        firstEvent = true;
                    }
                    previousEventType = eventType;
                }

                // 描画処理、ここでいいん？
                if (eventType == EventType.Repaint)
                {
                    if (style == null)
                    {
                        Texture2D tex = new Texture2D(4, 4);
                        Color col = new Color(0.3f, 0.5f, 1.0f, 0.3f);
                        for (int y = 0; y < tex.height; y++)
                        {
                            for (int x = 0; x < tex.width; x++)
                            {
                                tex.SetPixel(x, y, col);
                            }
                        }
                        tex.Apply();
                        style = new GUIStyle();
                        style.name = "dropTarget";
                        style.normal.background = tex;
                    }
                    if (dragging && (targetDirection != null) && (targetInstanceID == instanceID))
                    {
                        //targetRect = new Rect(0.0f, 0.0f, 64.0f, 64.0f);
                        //GUI.depth = 1;
                        GUI.Box(targetRect, GUIContent.none, style);
                    }
                }

                // ドラッグ中の処理
                if (eventType == EventType.DragUpdated)
                {
                    dragging = true;
                    if (firstEvent)
                    {
                        acceptable = false;
                        foreach (var obj in DragAndDrop.objectReferences)
                        {
                            if (IsAcceptable(obj))
                            {
                                acceptable = true;
                            }
#if true
                            else
                            {
                                // 他の型がまじってたら処理しない。
                                acceptable = false;
                                break;
                            }
#endif
                        }
                        targetDirection = null;
                    }
                    if (acceptable && (targetDirection == null))
                    {
                        targetDirection = CheckMousePosition(selectionRect);
                        if (targetDirection != null)
                        {
                            DragAndDrop.AcceptDrag();
                            // drag受け入れ可の場合カーソル形状を変更。
                            DragAndDrop.visualMode = DragAndDropVisualMode.Link;
                            Event.current.Use();
                            targetInstanceID = instanceID;
                            targetRect = selectionRect;
                            if (targetDirection < 0)
                            {
                                float y = targetRect.yMin;
                                targetRect.yMin = y - border;
                                targetRect.yMax = y + border;
                            }
                            else if (targetDirection > 0)
                            {
                                float y = targetRect.yMax;
                                targetRect.yMin = y - border;
                                targetRect.yMax = y + border;
                            }
                        }
                    }
                }

                // ドラッグ停止
                if (eventType == EventType.DragExited)
                {
                    dragging = false;
                }

                // ドロップ処理
                if (eventType == EventType.DragPerform)
                {
                    dragging = false;
                    if (targetDirection == null)
                    {
                        return;
                    }
                    if(instanceID != targetInstanceID)
                    {
                        return;
                    }
                    //Debug.Log(EditorUtility.InstanceIDToObject(instanceID).name);
                    var targetObj = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
                    if (targetObj == null)
                    {
                        return;
                    }
#if true
                    //Debug.Log("CheckObjectPosition:" + on);
                    Undo.RegisterFullObjectHierarchyUndo(targetObj, "");
                    MyMMLBox box = null;
                    if (targetDirection == 0)
                    {
                        box = targetObj.GetComponent<MyMMLBox>();
                    }
                    else
                    {
                        var parent = targetObj.transform.parent;
                        var index = targetObj.transform.GetSiblingIndex() + ((targetDirection < 0) ? 0 : 1);
                        targetObj = CreateNewGameObject(parent);
                        Undo.RegisterCreatedObjectUndo(targetObj, "Create gameObject:" + targetObj.name);
                        if (index >= 0)
                        {
                            //Undo.RegisterFullObjectHierarchyUndo(parent);
                            targetObj.transform.SetSiblingIndex(index);
                        }
                    }
                    if (box == null)
                    {
                        box = Undo.AddComponent<MyMMLBox>(targetObj);
                    }
                    foreach (var obj in DragAndDrop.objectReferences)
                    {
                        TextAsset textAsset = obj as TextAsset;
                        if (textAsset && (obj.name.EndsWith(".mml")))
                        {
                            //string path = AssetDatabase.GetAssetPath(obj);
                            //Debug.Log("path:" + path);
                            MyMMLClip clip = new MyMMLClip();
                            clip.TextC = textAsset;
                            clip.Name = obj.name.Substring(0, obj.name.LastIndexOf('.'));
                            box.Add(clip);
                        }
                    }
#endif
                }
            }

            // ドロップ受付可能なオブジェクトか判定。
            static bool IsAcceptable(Object obj)
            {
                if ((obj is TextAsset) && (obj.name.EndsWith(".mml")))
                {
                    return true;
                }
                return false;
            }

            // 同じ階層に同じ名前のものがあった場合、自分の名前にインデックスをつける。
            static void ValidateName(ref string name, Transform parent)
            {
                int count = 0;
                //var clips = GameObject.FindObjectsOfType<MyMMLClip>();
                var clips = GameObject.FindObjectsOfType<Transform>();
                for (var i = 0; i < clips.Length; i++)
                {
                    if (clips[i].transform.parent != parent)
                    {
                        continue;
                    }
                    if (clips[i].name.StartsWith(name))
                    {
                        count++;
                    }
                }
                if (count > 0)
                {
                    name = name + "(" + count + ")";
                }
            }

            static GameObject CreateNewGameObject(Transform parent)
            {
                string name = "MyMMLBox";
                // 兄弟と名前が被らないように。
                ValidateName(ref name, parent);

                // オブジェクトを作成。
                var gameObject = new GameObject(name);
                if (parent!= null)
                {
                    //Undo.SetTransformParent(gameObject.transform, parent.transform, "setParent:" + gameObject.name);
                    gameObject.transform.SetParent(parent);
                }
                return gameObject;
            }
        }
#endif //UNITY_EDITOR
    }
}
