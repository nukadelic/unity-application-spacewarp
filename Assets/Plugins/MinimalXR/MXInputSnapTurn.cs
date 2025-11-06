namespace nkd.xr
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class MXInputSnapTurn : MonoBehaviour
    {
        public MXInputs inputs;

        public float degrees = 70;

        public bool applyTransformation = false;

        [Range(0.2f,1f)]
        public float cooldown = 0.5f;

        bool down;
    
        float time;

        public UnityEngine.Events.UnityEvent<float> OnSnapTurn;

        [Header("Controllers")]
        public bool snapLeft = true;
        public bool snapRight = true;

        private void Update()
        {
            var dt = Time.deltaTime;

            if( snapLeft )
            {
                bool snapped = Delta(inputs.left_primary2DAxis.value.x , dt);
            }

            if( snapRight )
            {
                bool snapped = Delta(inputs.right_primary2DAxis.value.x, dt);
            }
        }

        // return true if snap turn was invoked this frame 
        public bool Delta(float turnAxis , float dt)
        {
            bool isDown = Mathf.Abs(turnAxis) > 0.5f;

            if (!down && isDown)
            {
                time = 0;
                down = true;

                var angle = Mathf.Sign(turnAxis) * degrees;

                if (applyTransformation) transform.localEulerAngles += new Vector3(0, angle, 0);

                OnSnapTurn?.Invoke(angle);

                return true;
            }

            time += dt;

            // reset only after state became false and 250ms have passed 
            if (!isDown && time > cooldown ) down = false;

            return false;
        }
    }
}