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

    [Tooltip("実際の手（トラッキングされる手のTransform）")]
    public Transform realHand;

    [Header("Redirection Settings (リダイレクション設定)")]
    [Tooltip("オフセット量（メートル）デフォルト: 0.1m = 10cm")]
    public float offsetDistance = 1.2f;

    [Tooltip("オフセットの方向")]
    public Vector3 offsetDirection = Vector3.up;

    [Tooltip("この高さ（メートル）以上持ち上げたらリダイレクションを停止")]
    public float liftThreshold = 0.5f;

    /// <summary>
    /// 対象オブジェクトをつかんでいるときtrue、離しているときfalse
    /// </summary>
    public bool isGrabbing { get; private set; }

    private float _grabStartY;
    private bool _redirectionStopped = true;

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

    void Update()
    {
        isGrabbing = IsGrabbingTarget();

        // つかみ始めた瞬間に開始位置を記録（実際の手の位置を使用）
        if (isGrabbing && _redirectionStopped)
        {
            _grabStartY = realHand.position.y;
            _redirectionStopped = false;
        }

        // 離したらリセット
        if (!isGrabbing)
        {
            _redirectionStopped = true;
        }

        // 50cm以上持ち上げたらリダイレクション停止
        if (isGrabbing && !_redirectionStopped)
        {
            float liftHeight = realHand.position.y - _grabStartY;
            if (liftHeight >= liftThreshold)
            {
                _redirectionStopped = true;
            }
        }

        // リダイレクション適用（実際の手の移動量にオフセットを掛ける）
        if (isGrabbing && !_redirectionStopped)
        {
            float delta = realHand.position.y - _grabStartY;
            float redirectedY = _grabStartY + delta * offsetDistance;
            Debug.Log("GRAB delta=" + delta + " offset=" + (delta * (offsetDistance - 1)));
            virtualHand.position = new Vector3(virtualHand.position.x, realHand.position.y + 0.05f, virtualHand.position.z);
        }
    }

}