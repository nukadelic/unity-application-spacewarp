namespace nkd.xr
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class MXInputNoClip : MonoBehaviour
    {
        public MXRig rig;
        public MXInputs inputs;

        public bool fly = true;
        public bool turn = true;

        public float moveSpeed = 5f;

        void Update()
        {
            if (moveSpeed > 0)
            {
                var dt = Time.deltaTime;

                if( fly )
                {
                    transform.position += rig.trackHead.forward * inputs.left_primary2DAxis.value.y * moveSpeed * dt;
                    transform.position += rig.trackHead.right * inputs.left_primary2DAxis.value.x * moveSpeed * dt;
                }

                if( turn )
                {
                    transform.rotation *= Quaternion.Euler(0, inputs.right_primary2DAxis.value.x * 120f * dt, 0);
                }
            }
        }
    }
}