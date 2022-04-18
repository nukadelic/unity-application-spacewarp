
using UnityEngine;
using UnityEngine.Events;
using TMPro;

[ExecuteAlways]
public class MiniButton : MonoBehaviour 
{
    public string label = "Label";
    [Space(8)]
    public string valueStr;
    public int valueInt;
    [Space(8)]
    public UnityEvent<MiniButton> OnPress;
    public void Press() => OnPress?.Invoke( this );

    void OnValidate()
    {
        var label = GetComponentInChildren<TextMeshPro>();
        if( label ) label.text = this.label;
    }

    public void SetColor( Color color )
    {
        if( transform.GetChild( 0 ).TryGetComponent( out MeshRenderer R ) )
            R.material.SetColor( "_BaseColor", color );
    }

    #region Invoke button press event in inspector 
    #if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(MiniButton))]
    class Editor : UnityEditor.Editor {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            UnityEditor.EditorGUI.BeginDisabledGroup(!Application.isPlaying);
            GUILayout.Space(15);
            if (GUILayout.Button("Press")) ( (MiniButton) target ).Press();
            UnityEditor.EditorGUI.EndDisabledGroup();
        }
    }
    #endif
    #endregion
}