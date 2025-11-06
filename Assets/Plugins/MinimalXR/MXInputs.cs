namespace nkd.xr
{
    using UnityEngine.Events;
    using UnityEngine.XR;
    using UnityEngine;
    using System;

    public class MXInputs : MonoBehaviour
    {
        static bool _locked = false;
        public static bool active => instance != null && !_locked;


        public static MXInputs instance;
        private void OnEnable() { instance = this; }


        public MXRig Rig;


        private void Update()
        {
            if (Rig == null) return;

            if (Rig.TryGet(MXDevice.LEFT, out InputDevice LEFT))
            {
                if (LEFT.TryGetFeatureValue(CommonUsages.gripButton, out bool gripButton)) left_gripButton.Update(gripButton);
                if (LEFT.TryGetFeatureValue(CommonUsages.grip, out float grip)) left_grip.Update(grip);
                if (LEFT.TryGetFeatureValue(CommonUsages.triggerButton, out bool triggerButton)) left_triggerButton.Update(triggerButton);
                if (LEFT.TryGetFeatureValue(CommonUsages.trigger, out float trigger)) left_trigger.Update(trigger);
                if (LEFT.TryGetFeatureValue(CommonUsages.primaryButton, out bool primaryButton)) left_primaryButton.Update(primaryButton);
                if (LEFT.TryGetFeatureValue(CommonUsages.secondaryButton, out bool secondaryButton)) left_secondaryButton.Update(secondaryButton);
                if (LEFT.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out bool primary2DAxisClick)) left_primary2DAxisClick.Update(primary2DAxisClick);
                if (LEFT.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 primary2DAxis)) left_primary2DAxis.Update(primary2DAxis);
            }

            if (Rig.TryGet(MXDevice.RIGHT, out InputDevice RIGHT))
            {
                if (RIGHT.TryGetFeatureValue(CommonUsages.gripButton, out bool gripButton)) right_gripButton.Update(gripButton);
                if (RIGHT.TryGetFeatureValue(CommonUsages.grip, out float grip)) right_grip.Update(grip);
                if (RIGHT.TryGetFeatureValue(CommonUsages.triggerButton, out bool triggerButton)) right_triggerButton.Update(triggerButton);
                if (RIGHT.TryGetFeatureValue(CommonUsages.trigger, out float trigger)) right_trigger.Update(trigger);
                if (RIGHT.TryGetFeatureValue(CommonUsages.primaryButton, out bool primaryButton)) right_primaryButton.Update(primaryButton);
                if (RIGHT.TryGetFeatureValue(CommonUsages.secondaryButton, out bool secondaryButton)) right_secondaryButton.Update(secondaryButton);
                if (RIGHT.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out bool primary2DAxisClick)) right_primary2DAxisClick.Update(primary2DAxisClick);
                if (RIGHT.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 primary2DAxis)) right_primary2DAxis.Update(primary2DAxis);
            }

        }

        [Header("LEFT")]
        public InputEventLink<Vector2> left_primary2DAxis;
        public InputEventLink<bool> left_primary2DAxisClick, left_primaryButton, left_secondaryButton;
        public InputEventLink<bool> left_gripButton, left_triggerButton;
        public InputEventLink<float> left_grip, left_trigger;
        [Header("RIGHT")]
        public InputEventLink<Vector2> right_primary2DAxis;
        public InputEventLink<bool> right_primary2DAxisClick, right_primaryButton, right_secondaryButton;
        public InputEventLink<bool> right_gripButton, right_triggerButton;
        public InputEventLink<float> right_grip, right_trigger;

        [Serializable]
        public class InputEventLink<T> where T : struct, IEquatable<T>
        {
            public T value => valueState.value_current;
            public bool changed => valueState.changed;

            public InputValue<T> valueState;
            public UnityEvent<T> onUpdate;
            public UnityEvent<T> onChange;

            public void Update(T value)
            {
                valueState.Update(value);

                if (valueState.changed) onChange?.Invoke(value);

                onUpdate?.Invoke(value);
            }
        }

        [System.Serializable]
        public class InputValue<T> where T : struct, IEquatable<T>
        {
            public float last_change_at = 0;
            public bool changed = false;
            public T value_prev = default, value_current = default;

            public void Update(T value_new, bool ignore_change = false)
            {
                changed = ignore_change ? false : !value_new.Equals(value_current);

                if (changed) last_change_at = Time.time;

                value_prev = value_current;

                value_current = value_new;
            }
        }



        #region Input Enums

        public enum Axis
        {
            Left_Primary2DAxis, Right_Primary2DAxis,
        }

        public enum Button
        {
            Left_Primary2DAxisClick, Right_Primary2DAxisClick,
            Left_PrimaryButton, Right_PrimaryButton,
            Left_SecondaryButton, Right_SecondaryButton,
            Left_GripButton, Right_GripButton,
            Left_TriggerButton, Right_TriggerButton,
        }

        public enum Float
        {
            Left_Grip, Right_Grip,
            Left_Trigger, Right_Trigger,
        }

        #endregion

        #region Read Value from Enum

        public InputEventLink<float> GetFloat(Float value)
        {
            switch (value)
            {
                case Float.Left_Grip: return left_grip;
                case Float.Right_Grip: return right_grip;
                case Float.Left_Trigger: return left_trigger;
                case Float.Right_Trigger: return right_trigger;
            }

            return null;
        }

        public InputEventLink<Vector2> GetAxis(Axis axis)
        {
            switch (axis)
            {
                case Axis.Left_Primary2DAxis: return left_primary2DAxis;
                case Axis.Right_Primary2DAxis: return right_primary2DAxis;
            }

            return null;
        }

        public InputEventLink<bool> GetButton(Button button)
        {
            switch (button)
            {
                case Button.Left_Primary2DAxisClick: return left_primary2DAxisClick;
                case Button.Right_Primary2DAxisClick: return right_primary2DAxisClick;
                case Button.Left_PrimaryButton: return left_primaryButton;
                case Button.Right_PrimaryButton: return right_primaryButton;
                case Button.Left_SecondaryButton: return left_secondaryButton;
                case Button.Right_SecondaryButton: return right_secondaryButton;
                case Button.Left_GripButton: return left_gripButton;
                case Button.Right_GripButton: return right_gripButton;
                case Button.Left_TriggerButton: return left_triggerButton;
                case Button.Right_TriggerButton: return right_triggerButton;
            }

            return null;
        }

        #endregion

        #region Static 

        public static bool triggerButtonLeft => instance?.left_triggerButton.value ?? false;
        public static bool triggerButtonRight => instance?.right_triggerButton.value ?? false;
        public static bool triggerButtonAny => triggerButtonLeft || triggerButtonRight;

        public static bool gripButtonLeft => instance?.left_gripButton.value ?? false;
        public static bool gripButtonRight => instance?.right_gripButton.value ?? false;
        public static bool gripButtonAny => gripButtonLeft || gripButtonRight;

        public static bool primaryButtonLeft => instance?.left_primaryButton.value ?? false;
        public static bool primaryButtonRight => instance?.right_primaryButton.value ?? false;
        public static bool primaryButtonButtonAny => primaryButtonLeft || primaryButtonRight;

        public static bool secondaryButtonLeft => instance?.left_secondaryButton.value ?? false;
        public static bool secondaryButtonRight => instance?.right_secondaryButton.value ?? false;
        public static bool secondaryButtonButtonAny => secondaryButtonLeft || secondaryButtonRight;

        #endregion



    }
}