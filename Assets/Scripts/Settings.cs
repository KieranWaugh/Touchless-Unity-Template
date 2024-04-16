using System.Collections;
using System.Collections.Generic;
using Leap.Unity;
using UnityEngine;

public static class Settings
{
    public static SettingsController controller;
    #region Interaction Parameters

    [SerializeField] public static bool enable_proxemics = false;
    [SerializeField] public static InteractionType pointing_method = InteractionType.DirectMap;
    [SerializeField] public static int pointing_method_index;
    [SerializeField] public static Chirality tracked_hand = Chirality.Right;
    [SerializeField] public static int tracked_hand_index;
    [SerializeField] public static SelectionType gesture = SelectionType.Pinch;
    [SerializeField] public static int gesture_index;
    [SerializeField] public static bool calibrated = false;
    [SerializeField] public static Vector2 top, bottom, left, right;
    [SerializeField] public static float gain = 1;
    [SerializeField] public static float pinch_distance = 30;
    [SerializeField] public static float airpush_position = -0.1f;
    [SerializeField] public static int dwell_time = 300;
    [SerializeField] public static int occlusion_offset = 0;
    [SerializeField] public static float filter_strength;
    [SerializeField] public static TrackedPosition tracked_point = TrackedPosition.PinchPoint;
    [SerializeField] public static CursorType cursor = CursorType.Standard;
    [SerializeField] public static int cursor_index;
    #endregion

    #region UI Feedback
    [SerializeField] public static bool enable_light_bar = false;
    [SerializeField] public static bool enable_tracking_lost = true;
    [SerializeField] public static bool enable_widget_outline = true;
    [SerializeField] public static bool enable_widget_click_feedback = true;

    #endregion

    #region Logging

    [SerializeField] public static bool enable_logging = false;

    #endregion

}


