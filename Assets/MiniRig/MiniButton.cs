
using UnityEngine;
using UnityEngine.Events;
using TMPro;

[ExecuteAlways]
public class MiniButton : MonoBehaviour 
{
    public string label = "Label";
    public string type;
    public int data;
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
}