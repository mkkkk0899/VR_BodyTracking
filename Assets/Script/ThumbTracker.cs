using UnityEngine;

/// <summary>
/// 右手の親指座標トラッキングスクリプト
/// スペースキーを押している間のみトラッキングし、押した瞬間の座標を原点として差分を記録する
/// </summary>
public class ThumbTracker : MonoBehaviour
{
    [Header("右手親指ジョイント（インスペクタから設定）")]
    [SerializeField] private Transform _thumbProximal;
    [SerializeField] private Transform _thumbIntermediate;
    [SerializeField] private Transform _thumbDistal;
    [SerializeField] private Transform _thumbTip;

    [Header("接触判定（インスペクタから設定）")]
    [SerializeField] private GameObject _targetCube;

    // Tipに付いているColliderとCubeのCollider（Start時にキャッシュ）
    private Collider _thumbTipCollider;
    private Collider _cubeCollider;

    public bool IsTouching { get; private set; }
    // 接触した瞬間・離れた瞬間のフラグ（1フレームのみtrue）
    public bool IsTouchEnter { get; private set; }
    public bool IsTouchExit { get; private set; }

    private bool _prevTouching;

    [Header("カーソル設定（インスペクタから設定）")]
    [SerializeField] private Transform _cursor;
    [Tooltip("指の動きに対するカーソルの移動倍率")]
    [SerializeField] private float _sensitivity = 5f;
    [Tooltip("Y座標閾値による制限を有効にする")]
    [SerializeField] private bool _enableTipYThreshold = true;
    [Tooltip("カーソルを動かすTipのY座標の上限（これ以下の時のみカーソルが動く）")]
    [SerializeField] private float _tipYThreshold = 0.7f;

    // スペースキーを押した瞬間の原点座標
    private Vector3 _proximalOrigin;
    private Vector3 _intermediateOrigin;
    private Vector3 _distalOrigin;

    // カーソルの原点位置（スペースキーを押した瞬間に記録）
    private Vector3 _cursorOrigin;
    private Vector3 _thumbTipOrigin;

    // 現在のトラッキング差分（外部から参照可能）
    public Vector3 ProximalDelta { get; private set; }
    public Vector3 IntermediateDelta { get; private set; }
    public Vector3 DistalDelta { get; private set; }

    public bool IsTracking { get; private set; }

    private bool IsYThreshold;

    void Start()
    {
        // TipのColliderとCubeのColliderをキャッシュ
        _thumbTipCollider = _thumbTip.GetComponent<Collider>();
        _cubeCollider = _targetCube.GetComponent<Collider>();
    }

    void Update()
    {
        // スペースキーを押した瞬間に原点を記録
        if (Input.GetKeyDown(KeyCode.Space) || IsTouchEnter)
        {
            _proximalOrigin = _thumbProximal.position;
            _intermediateOrigin = _thumbIntermediate.position;
            _distalOrigin = _thumbDistal.position;
            _thumbTipOrigin = _thumbTip.position;
            _cursorOrigin = _cursor.position;
            IsTracking = true;
        }

        if (Input.GetKeyUp(KeyCode.Space) || IsTouchExit)
        {
            IsTracking = false;
            ProximalDelta = Vector3.zero;
            IntermediateDelta = Vector3.zero;
            DistalDelta = Vector3.zero;
        }

        // TipのColliderがCubeのColliderと接触しているか判定
        IsTouching = _thumbTipCollider.bounds.Intersects(_cubeCollider.bounds);

        // 前フレームとの差分で接触開始・終了の瞬間を検出
        IsTouchEnter = IsTouching && !_prevTouching;
        IsTouchExit  = !IsTouching && _prevTouching;
        _prevTouching = IsTouching;

        if (IsTouchEnter) Debug.Log("[ThumbTracker] TouchEnter");
        if (IsTouchExit)  Debug.Log("[ThumbTracker] TouchExit");

        if (!IsTracking) return;

        // 原点からの差分を計算
        ProximalDelta = _thumbProximal.position - _proximalOrigin;
        IntermediateDelta = _thumbIntermediate.position - _intermediateOrigin;
        DistalDelta = _thumbDistal.position - _distalOrigin;


        // 指先（Tip）のXZ差分をセンシティビティ倍してカーソルに反映（Y座標は固定）
        Vector3 tipDelta = _thumbTip.position - _thumbTipOrigin;
        _cursor.position = new Vector3(
            _cursorOrigin.x + tipDelta.x * _sensitivity,
            _cursorOrigin.y + tipDelta.z * _sensitivity,
            _cursorOrigin.z
        );

        Debug.Log(tipDelta.y);
    }

    /// <summary>
    /// Tipのワールド座標をSceneビューに球で可視化（エディタのみ）
    /// </summary>
    void OnDrawGizmos()
    {
        if (_thumbTip == null) return;

        // Tip位置に黄色の球を描画
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(_thumbTip.position, 0.01f);

        // トラッキング中は原点から現在位置へのラインも表示
        if (IsTracking)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(_thumbTipOrigin, _thumbTip.position);
            Gizmos.DrawWireSphere(_thumbTipOrigin, 0.008f);
        }

    }
}
