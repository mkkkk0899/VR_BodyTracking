using UnityEngine;

/// <summary>
/// OVRハンドトラッキングの各指関節のワールド座標をアバターに適用するスクリプト
/// シーン上のOVRハンドモデルの各ボーンのワールド位置をアバタの手に直接反映
/// </summary>
public class OVRHandPositionTracker : MonoBehaviour
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

    [Header("Settings (設定)")]
    [Tooltip("位置オフセット（グローバル）")]
    public Vector3 positionOffset = Vector3.zero;

    [Tooltip("スケール係数（OVRハンドとアバターのサイズ差を調整）")]
    public float scaleFactor = 1.0f;

    [Tooltip("補間速度（0で即座に適用、大きいほど滑らか）")]
    [Range(0f, 50f)]
    public float smoothSpeed = 0f;

    void LateUpdate()
    {
        ApplyHandTracking();
    }

    /// <summary>
    /// OVRハンドのワールド座標をアバターに適用
    /// </summary>
    private void ApplyHandTracking()
    {
        // 親指
        ApplyPosition(ovrThumb0, avatarThumb0);
        ApplyPosition(ovrThumb1, avatarThumb1);
        ApplyPosition(ovrThumb2, avatarThumb2);

        // 人差し指
        ApplyPosition(ovrIndex1, avatarIndex1);
        ApplyPosition(ovrIndex2, avatarIndex2);
        ApplyPosition(ovrIndex3, avatarIndex3);

        // 中指
        ApplyPosition(ovrMiddle1, avatarMiddle1);
        ApplyPosition(ovrMiddle2, avatarMiddle2);
        ApplyPosition(ovrMiddle3, avatarMiddle3);

        // 薬指
        ApplyPosition(ovrRing1, avatarRing1);
        ApplyPosition(ovrRing2, avatarRing2);
        ApplyPosition(ovrRing3, avatarRing3);

        // 小指
        ApplyPosition(ovrPinky0, avatarPinky0);
        ApplyPosition(ovrPinky1, avatarPinky1);
        ApplyPosition(ovrPinky2, avatarPinky2);
    }

    /// <summary>
    /// OVRボーンのワールド座標をアバターボーンに適用
    /// </summary>
    private void ApplyPosition(Transform ovrBone, Transform avatarBone)
    {
        if (ovrBone == null || avatarBone == null)
            return;

        Vector3 targetPosition = ovrBone.position * scaleFactor + positionOffset;

        if (smoothSpeed > 0f)
        {
            avatarBone.position = Vector3.Lerp(
                avatarBone.position,
                targetPosition,
                Time.deltaTime * smoothSpeed
            );
        }
        else
        {
            avatarBone.position = targetPosition;
        }
    }
}
