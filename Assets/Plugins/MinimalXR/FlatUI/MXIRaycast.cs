namespace nkd.xr.ui
{
    using System;
    using UnityEngine;

    public class MXIRaycast : MonoBehaviour
    {
        public bool isTrigger;
        public float rayDist = 2f;
        public float colDist = 0.05f;

        void OnDrawGizmos()
        {
            Vector3 p = transform.position , f = transform.forward;

            var collides = Raycast<MXIButton>();

            Gizmos.color = collides != null ? Color.green : Color.cyan;

            if( colDist > 0 ) Gizmos.DrawWireSphere( p , colDist );

            if( rayDist > 0 ) Gizmos.DrawLine( p, p + f * rayDist );           
        }

        public void SetTrigger( bool value ) => isTrigger = value;

        public LayerMask layer = (LayerMask) 0b100000; // UI

        bool touching = false;

        MXIButton last;

        T Raycast<T>() where T : class 
        {
            touching = false;

            Vector3 p = transform.position , f = transform.forward;

            if( colDist > 0 )
            {
                var colliders = Physics.OverlapSphere( p, colDist, layer.value, QueryTriggerInteraction.Collide );

                if( colliders.Length > 0 ) 
                {
                    foreach( var col in colliders )
                    {
                        var btn = col.transform.GetComponent<T>() 
                            ?? col.transform.GetComponentInParent<T>() 
                            ?? col.transform.GetComponentInChildren<T>();

                        if( btn != null ) { touching = true; return btn; }
                    }
                }
            }

            if( rayDist <= 0 ) return null;

            var collided = Physics.Raycast( p, f, out RaycastHit h, rayDist , layer.value, QueryTriggerInteraction.Collide );

            if( ! collided ) return null;

            var button = h.transform.GetComponent<T>() 
                ?? h.transform.GetComponentInParent<T>() 
                ?? h.transform.GetComponentInChildren<T>();

            return button;
        }

        bool awaitRelease = false;

        void Update()
        {
            var collides = Raycast<MXIButton>();

            if( last != null && last != collides )
            {
                awaitRelease = false;

                last.Unfocus();

                last = null;
            }
            
            if( collides == null ) 
            {
                awaitRelease = false;
                return;
            }

            bool trigger = isTrigger || touching;

            if( trigger && ! awaitRelease )
            {
                awaitRelease = true;

                collides.Press();
            }
            else if( ! trigger && awaitRelease )
            {
                awaitRelease = false;   
            }
            else if( ! trigger && ! awaitRelease )
            {
                collides.Focus();
            }

            last = collides;
        }
    }
}
