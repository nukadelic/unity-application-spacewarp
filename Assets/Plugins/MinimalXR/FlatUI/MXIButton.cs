namespace nkd.xr.ui
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.Events;

    [ExecuteAlways]
    public class MXIButton : MonoBehaviour
    {
        #region Label
        public string label = "Label";
        public void SetText( string value )
        {
            this.label = value;

            var label = GetComponentInChildren<TextMeshPro>();
            
            if( label ) label.text = value;
        }

        public void LogLabel() => Debug.Log( label , this );

        #endregion

        #region Value 
        
        [Space(8)]
        public string value;
        public float GetValueFloat() => float.TryParse( value, out float f ) ? f : 0;
        public uint GetValueUint() => uint.TryParse( value, out uint i ) ? i : 0;

        #endregion

        #region Color
        [Space(8)] 
        public MeshRenderer colorTarget;
        public string colorKeyword = "_BaseColor";
        Color colorActive = Color.black, colorTo = Color.black;

        bool focus = false;
        public bool isFocused => focus;
        public void Focus()
        {
            if( focus ) return;

            colorTo = Color.grey;

            focus = true;
        }

        public void Unfocus()
        {
            if( ! focus ) return;

            colorTo = Color.black;

            focus = false;
        }

        #endregion

        public virtual void OnValidate()
        {
            SetText( label );

            colorTarget = colorTarget ?? transform.GetComponentInChildren<MeshRenderer>();

            var text = GetComponentInChildren<TextMeshPro>();

            if( TryGetComponent( out BoxCollider col ) )
            {
                col.size = new Vector3(size.x, size.y, 0.01f);
            }

            if(colorTarget)
            {
                colorTarget.transform.localScale = new Vector3(size.x,size.y,1);
            }

            if (text)
            {
                text.rectTransform.sizeDelta = size * 10f;
                text.fontSize = font;
            }
        }

        #region Press Event 

        static float DURATION = 0.2f; 

        float cooldown = 0;
        public bool inCooldown => cooldown > 0;

        [Space(8)]
        public UnityEvent<string> OnPress;
        public virtual void Press()
        {
            if ( inCooldown ) return;

            cooldown = DURATION;

            colorTo = colorActive;

            colorActive = Color.green;

            OnPress?.Invoke(value);
        }

        #endregion

        private void Update()
        {
            cooldown -= Time.deltaTime;

            var t = 1 - cooldown / DURATION;

            if( colorTo == colorActive ) return;

            var c = Color.Lerp( colorActive , colorTo , Mathf.Clamp01( t ) );

            if( colorTarget != null ) colorTarget.material.SetColor( colorKeyword , c );

            if ( t > 1 ) colorActive = colorTo;
        }

        public Vector2 size = new Vector2(0.45f, 0.15f);
        public float font = 6;
    }
}