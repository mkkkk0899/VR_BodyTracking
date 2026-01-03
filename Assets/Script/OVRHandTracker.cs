using UnityEngine;

/// <summary>
/// OVRハンドトラッキングをアバタに適用するスクリプト
/// シーン上のOVRハンドモデルの各ボーンの回転をアバタの手に直接反映
/// </summary>
public class OVRHandTracker : MonoBehaviour
{
    [Header("OVR Thumb (親指)")]
    public Transform ovrThumb0;
    public Transform ovrThumb1;
    public Transform ovrThumb2;

    [Header("OVR Index (人差し指)")]
    public Transform ovrIndex1;
    public Transform ovrIndex2;
    public Transform ovrIndex3;

    [Header("OVR Middle (中指)")]
    public Transform ovrMiddle1;
    public Transform ovrMiddle2;
    public Transform ovrMiddle3;

    [Header("OVR Ring (薬指)")]
    public Transform ovrRing1;
    public Transform ovrRing2;
    public Transform ovrRing3;

    [Header("OVR Pinky (小指)")]
    public Transform ovrPinky0;
    public Transform ovrPinky1;
    public Transform ovrPinky2;
    [Header("Avatar Thumb (親指)")]
    public Transform avatarThumb0;
    public Transform avatarThumb1;
    public Transform avatarThumb2;

    [Header("Avatar Index (人差し指)")]
    public Transform avatarIndex1;
    public Transform avatarIndex2;
    public Transform avatarIndex3;

    [Header("Avatar Middle (中指)")]
    public Transform avatarMiddle1;
    public Transform avatarMiddle2;
    public Transform avatarMiddle3;

    [Header("Avatar Ring (薬指)")]
    public Transform avatarRing1;
    public Transform avatarRing2;
    public Transform avatarRing3;

    [Header("Avatar Pinky (小指)")]
    public Transform avatarPinky0;
    public Transform avatarPinky1;
    public Transform avatarPinky2;

    void LateUpdate()
    {
        ApplyHandTracking();
    }

    /// <summary>
    /// OVRハンドの回転をアバタに適用
    /// </summary>
    private void ApplyHandTracking()
    {
        // 親指
        ApplyRotation(ovrThumb0, avatarThumb0);
        ApplyRotation(ovrThumb1, avatarThumb1);
        ApplyRotation(ovrThumb2, avatarThumb2);

        // 人差し指
        ApplyRotation(ovrIndex1, avatarIndex1);
        ApplyRotation(ovrIndex2, avatarIndex2);
        ApplyRotation(ovrIndex3, avatarIndex3);

        // 中指
        ApplyRotation(ovrMiddle1, avatarMiddle1);
        ApplyRotation(ovrMiddle2, avatarMiddle2);
        ApplyRotation(ovrMiddle3, avatarMiddle3);

        // 薬指
        ApplyRotation(ovrRing1, avatarRing1);
        ApplyRotation(ovrRing2, avatarRing2);
        ApplyRotation(ovrRing3, avatarRing3);

        // 小指
        ApplyRotation(ovrPinky0, avatarPinky0);
        ApplyRotation(ovrPinky1, avatarPinky1);
        ApplyRotation(ovrPinky2, avatarPinky2);
    }

    /// <summary>
    /// OVRボーンの回転をアバターボーンに適用
    /// </summary>
    private void ApplyRotation(Transform ovrBone, Transform avatarBone)
    {
        if (ovrBone != null && avatarBone != null)
        {
            avatarBone.localRotation = ovrBone.localRotation;
        }
    }
}