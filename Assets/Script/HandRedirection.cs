using UnityEngine;
using Oculus.Interaction.HandGrab;

/// <summary>
/// 人差し指の接触判定に基づいて手の位置をリダイレクションするスクリプト
/// IndexFingerCollisionDetectorのisCollidingがtrueの時、手の位置を+10cmオフセット
/// </summary>
public class HandRedirection : MonoBehaviour
{
    [Header("Hand Grab Interactor")]
    [Tooltip("HandGrabInteractorのコンポーネント（手のInteractor）")]
    [SerializeField]
    private HandGrabInteractor _handGrabInteractor;

    [Header("Target Object (判定対象)")]
    [Tooltip("つかんでいるか判定したいオブジェクトのHandGrabInteractable")]
    [SerializeField]
    private HandGrabInteractable _targetInteractable;

    [Header("Collision Detector (接触判定)")]
    [Tooltip("IndexFingerCollisionDetectorのコンポーネント")]
    public IndexFingerCollisionDetector collisionDetector;

    [Tooltip("仮想の手（表示される手のモデル）")]
    public Transform virtualHand;

    [Header("Redirection Settings (リダイレクション設定)")]
    [Tooltip("オフセット量（メートル）デフォルト: 0.1m = 10cm")]
    public float offsetDistance = 0.1f;

    [Tooltip("オフセットの方向")]
    public Vector3 offsetDirection = Vector3.up;

    /// <summary>
    /// 対象オブジェクトをつかんでいるときtrue、離しているときfalse
    /// </summary>
    public bool isGrabbing { get; private set; }

    /// <summary>
    /// 指定したオブジェクトを現在つかんでいるかどうかを返す
    /// </summary>
    public bool IsGrabbingTarget()
    {
        if (_handGrabInteractor == null || _targetInteractable == null)
            return false;

        return _handGrabInteractor.HasSelectedInteractable
            && _handGrabInteractor.SelectedInteractable == _targetInteractable;
    }

    void LateUpdate()
    {
        isGrabbing = IsGrabbingTarget();
        // 接触判定に基づいてリダイレクション
        if (collisionDetector.isColliding)
        {
            // +10cm（または設定値）のオフセットを追加
            Vector3 offset = offsetDirection.normalized * offsetDistance;
            virtualHand.position = virtualHand.position + offset;
        }

        Debug.Log("GRAB"+ isGrabbing);
    }

}
