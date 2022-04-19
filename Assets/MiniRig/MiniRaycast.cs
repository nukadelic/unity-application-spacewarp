using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniRaycast : MonoBehaviour
{
    public bool isLeftHand = false;
    public LayerMask layerMask;

    MiniRig rig;

    void Start()
    {
        rig = GetComponentInParent<MiniRig>();
        if( rig == null ) Debug.LogError("Rig not found");
        if( isLeftHand ) rig.onLeftTrigger += () => triggerPressed = true;
        else rig.onRightTrigger += () => triggerPressed = true;
    }

    bool triggerPressed = false;
    MiniButton lastButton;

    float idle = 0;

    void Update()
    {
        // restore button color 
        if( lastButton != null )
        {
            lastButton.SetColor( Color.black );
            lastButton = null;
        }

        // press cooldown 
        if( ( idle -= Time.deltaTime ) > 0 ) return;

        // raycast for colliders 
        RaycastHit[] hits = new RaycastHit[ 1 ];
        var ray = new Ray( transform.position, transform.forward );
        var c = Physics.RaycastNonAlloc( ray, hits , 4f, layerMask, QueryTriggerInteraction.Collide );   
        if( c == 0 ) return;

        // Lookup for a button 
        var button = hits[ 0 ].transform.GetComponentInParent<MiniButton>();
        if( button == null ) return;

        if( triggerPressed )
        {
            // Debug.Log("[Event:ButtonClick] " + button.label + ", " + button.valueStr + ", " + button.valueInt );

            triggerPressed = false;
            button.Press();         // invoke event 
            idle = 0.2f;            // 200 ms
        }
        else button.SetColor( Color.blue ); // focus 

        lastButton = button;
    }
}
