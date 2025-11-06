
using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;

[ExecuteAlways]
public class TMPLogger : MonoBehaviour
{
    public bool autoHookEditorLog = true;
    public bool clearOnStart = true;
    public bool showTimeStamp = false;

    [Header("Reference")]
    public Transform background;
    public TextMeshPro textarea;  
    public Follower follow;

    void Update()
    {
        if( follow == null ) follow = new Follower();

        follow.Update( transform );

        if( isValid )
        {
            // textScale.sizeDelta = new Vector2( width * 10 , height * 10 );
            // mask.offsetMin = new Vector2( padding * 10f, padding * 10f );
            // mask.offsetMax = - new Vector2( padding * 10f, padding * 10f );
            textarea.rectTransform.sizeDelta = new Vector2( width , height );
            textarea.transform.localPosition = new Vector3( width / 200f + padding / 100f, height / 200f - padding / 100f );
            textarea.fontSize = fontSize;
            if(logHistory != null )
            {
                logHistory.showTimeStamp = showTimeStamp;
                logHistory.font_size = fontSize;
            }
            background.localScale = new Vector3( width / 100f , height / 100f , 1 );

            if( ! Application.isPlaying && textarea.text.Split('\n').Length != pageSize )
            {
                var s = "1. Outpot log text"; 
                for( var i = 1; i < pageSize; ++i ) 
                    s += $"\n{i+1}.";
                textarea.text = s;  
            } 
        }
    }


    public bool isValid => background && textarea;

    [Header("Text Parameters")]
    [Range(10,350)] public int width = 100;
    [Range(10,150)] public int height = 50;
    [Range(0,3)] public float padding = 2f;
    [Range(4,40)] public int pageSize = 11;
    [Range(12,64)] public int fontSize = 28;

    public void Log( string rich_text )
    {
        WriteParameters();
        logHistory.history.Add( rich_text );
        textarea.text = logHistory.GetLines();
    }

    public void LogWarnning( string rich_text )
    {
        WriteParameters();
        logHistory.Format(ref rich_text, LogType.Warning );
        logHistory.history.Add( rich_text );
        textarea.text = logHistory.GetLines();
    }
    public void LogError( string rich_text )
    {
        WriteParameters();
        logHistory.Format(ref rich_text, LogType.Error ) ;
        logHistory.history.Add( rich_text );
        textarea.text = logHistory.GetLines();
    }

    public void PageUp()
    {
        WriteParameters();

        if( logHistory.page_size <= 0 ) return;

        logHistory.PageScroll( 1 );

        textarea.text = logHistory.GetLines();
    }

    public void PageDown()
    {
        WriteParameters();

        if (logHistory.page_size <= 0) return;

        logHistory.PageScroll( -1 );

        textarea.text = logHistory.GetLines();
    }

    public void Bottom()
    {
        logHistory.page_offset = 0;
    }

    public void Top()
    {
        logHistory.page_offset = Mathf.FloorToInt(logHistory.history.Count / logHistory.page_size);

    }


    public void Clear()
    {
        logHistory.history.Clear();

        logHistory.time_stamps.Clear();

        textarea.text = "";
    }

    void Start() 
    {
        if( ! Application.isPlaying ) return;

        if( clearOnStart ) Clear();
    }

    LogHistory logHistory;

    void OnEnable()
    {
        if( ! Application.isPlaying ) return;

        logHistory = new LogHistory();

        if( autoHookEditorLog ) 
            Application.logMessageReceived += Write;
    }

    void OnDisable()
    {
        if( ! Application.isPlaying ) return;

        logHistory = null;

        if( autoHookEditorLog ) 
            Application.logMessageReceived -= Write;
        
    }

    public void Write( string new_line , string stackTrace, LogType log_type = LogType.Log )
    {
        if( ! textarea ) return;

        // logHistory.Process( textarea, new_line, c );

        WriteParameters();

        logHistory.time_stamps.Add( $"[{(System.DateTime.Now):HH:mm:ss}]" );

        logHistory.Format(ref new_line, log_type);

        logHistory.history.AddRange( new_line.Split('\n').Select( x => { logHistory.Format( ref x , log_type ); return x; } ) );

        if( log_type == LogType.Error && ! string.IsNullOrEmpty( stackTrace ) ) 
        {
            logHistory.history.AddRange( stackTrace.Split('\n').Select( x => { logHistory.Format( ref x , log_type ); return $".. {x}"; } ) );
        }
        
        textarea.text = logHistory.GetLines();
    }

    void WriteParameters()
    {
        logHistory.page_size = pageSize;
    }

    class LogHistory
    {
        public List<string> time_stamps = new List<string>();

        public List<string> buffer = new List<string>();

        public List<string> history = new List<string>();

        public bool showTimeStamp = false;
        public int page_size = 0;
        public int page_offset = 0;
        public int font_size = 0;

        public void PageScroll( int value )
        {
            page_offset = Mathf.Clamp( page_offset + value, 0, Mathf.FloorToInt( history.Count / page_size ) );
        }

        public void Process( TextMeshProUGUI textarea, string new_line, LogType c )
        {
            Format(ref new_line, c);

            buffer.Add(new_line);

            if ( textarea == null ) return;

            foreach( var s in buffer )
            {
                history.Add( s );

                if ( page_size <= 0 )
                {
                    textarea.text = string.Join( "\n", history );
                    textarea.ForceMeshUpdate();
            
                    // count page size until it becomes truncated 
                    page_size -- ;
                    if ( textarea.isTextTruncated ) 
                        page_size = - page_size - 1;
                }
                else
                {
                    textarea.text = GetLines();
                    // logField.ForceMeshUpdate();
                }
            }

            buffer.Clear();
        }

        public string GetLines()
        {
            page_offset = Mathf.Max(0, page_offset);

            int offset = history.Count - page_size;

            offset -= Mathf.FloorToInt( page_offset * ( page_size * 1.0f ) ); 

            offset = Mathf.Clamp(offset, 0, history.Count - page_size );

            var count = Mathf.Min( offset + page_size , history.Count ) - offset;

            count = Mathf.Clamp( count, 0, history.Count );

            var lines = history.GetRange(offset, count );

            if( showTimeStamp )
            {
                for (var i = 0; i < lines.Count; ++i) lines[i] = $"<size={Mathf.FloorToInt(font_size/2f)}>{time_stamps[i]}</size> {lines[i]}";
            }


            for( var i = 0; i < lines.Count; ++i ) lines[ i ] = lines[ i ].Replace( "\n", " [\\n] " );

            return string.Join("\n", lines);
        }

        public void Format( ref string message , LogType type )
        {
            if (type == LogType.Warning) message = $"<color=\"yellow\">{message}</color>";
            else if (type != LogType.Log) message = $"<color=\"red\">{message}</color>";
        }
    }
    
    [System.Serializable]
    public class Follower
    {
        public Transform target;
        public Vector3 offsetPosition;
        public Vector3 offsetRotataion;

        public void Update( Transform t )
        {
            if( target == null ) return;

            t.position = target.position + target.TransformDirection( offsetPosition.normalized ) * offsetPosition.magnitude;

            t.LookAt( t.position + target.TransformDirection( Quaternion.Euler( offsetRotataion ) * Vector3.forward ) );
        }
    }

    #if UNITY_EDITOR

    [UnityEditor.CustomEditor(typeof(TMPLogger))]
    class Inspector2 : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var script = (TMPLogger) target;

            if ( script == null ) return;

            GUILayout.Space( 10 );

            UnityEditor.EditorGUI.BeginDisabledGroup( ! Application.isPlaying );

            GUILayout.Label("Runtime Actions" , UnityEditor.EditorStyles.boldLabel );

            if( GUILayout.Button("Page Up") ) script.PageUp();
            if( GUILayout.Button("Page Down") ) script.PageDown();
            if( GUILayout.Button("Clear") ) script.Clear();
            if( GUILayout.Button("Log Lines") ) 
                for(var i = 0; i < script.pageSize * 3.5f ; ++i ) 
                    if( Random.value > .2f ) Debug.Log( $"Debug.Log({i})" );
                    else Debug.LogError( $"Debug.LogError({i})" );

            UnityEditor.EditorGUI.EndDisabledGroup();
        }
    }
    #endif
    

}
