"""
ウィンドウを選択してMediaPipeでポーズ・ハンド推定を適用するスクリプト

依存ライブラリ:
    pip install mediapipe opencv-python pywin32 mss
"""

import sys
import cv2
import mediapipe as mp
import numpy as np
import mss
import win32gui


TARGET_WINDOW = "Meta Quest Casting"


def find_target_window() -> tuple[int, str] | None:
    """Meta Quest Castingウィンドウを検索して返す"""
    result = []

    def callback(hwnd, _):
        if win32gui.IsWindowVisible(hwnd):
            title = win32gui.GetWindowText(hwnd)
            if TARGET_WINDOW in title:
                result.append((hwnd, title))

    win32gui.EnumWindows(callback, None)
    return result[0] if result else None


def get_window_rect(hwnd: int) -> tuple[int, int, int, int]:
    """ウィンドウの画面上の矩形を取得する"""
    rect = win32gui.GetWindowRect(hwnd)
    left, top, right, bottom = rect
    return left, top, right - left, bottom - top  # x, y, w, h


def capture_window(sct: mss.mss, hwnd: int) -> np.ndarray | None:
    """指定ウィンドウをキャプチャしてBGR画像として返す"""
    # キャプチャ毎に矩形を再取得（ウィンドウ移動に追従）
    x, y, w, h = get_window_rect(hwnd)

    if w <= 0 or h <= 0:
        return None

    monitor = {"left": x, "top": y, "width": w, "height": h}
    screenshot = sct.grab(monitor)

    # BGRA → BGR 変換
    frame = np.array(screenshot)
    return cv2.cvtColor(frame, cv2.COLOR_BGRA2BGR)


def setup_mediapipe():
    """MediaPipeの右手ハンド推定器を初期化する"""
    mp_hands = mp.solutions.hands
    mp_drawing = mp.solutions.drawing_utils
    mp_drawing_styles = mp.solutions.drawing_styles

    hands = mp_hands.Hands(
        model_complexity=0,
        max_num_hands=2,
        min_detection_confidence=0.5,
        min_tracking_confidence=0.5,
    )

    return hands, mp_hands, mp_drawing, mp_drawing_styles


def process_frame(
    frame: np.ndarray,
    hands,
    mp_hands,
    mp_drawing,
    mp_drawing_styles,
) -> np.ndarray:
    """フレームに右手のハンド推定を適用して描画する"""
    # MediaPipeはRGBを期待するため変換
    rgb = cv2.cvtColor(frame, cv2.COLOR_BGR2RGB)
    rgb.flags.writeable = False

    hands_results = hands.process(rgb)

    rgb.flags.writeable = True
    output = cv2.cvtColor(rgb, cv2.COLOR_RGB2BGR)

    # 右手のみ描画（MediaPipeの "Right" は映像上の右手 = 鏡像なし環境では被写体の右手）
    if hands_results.multi_hand_landmarks and hands_results.multi_handedness:
        for hand_landmarks, handedness in zip(
            hands_results.multi_hand_landmarks, hands_results.multi_handedness
        ):
            if handedness.classification[0].label == "Right":
                mp_drawing.draw_landmarks(
                    output,
                    hand_landmarks,
                    mp_hands.HAND_CONNECTIONS,
                    mp_drawing_styles.get_default_hand_landmarks_style(),
                    mp_drawing_styles.get_default_hand_connections_style(),
                )

    return output


def main():
    # Meta Quest Castingウィンドウを自動検索
    found = find_target_window()
    if not found:
        print(f"「{TARGET_WINDOW}」ウィンドウが見つかりません。起動しているか確認してください。")
        sys.exit(1)

    hwnd, title = found
    print(f"検出: [{title}]")
    print("終了するには表示ウィンドウで 'q' を押してください。\n")

    hands, mp_hands, mp_drawing, mp_drawing_styles = setup_mediapipe()

    window_name = f"MediaPipe: {title}"
    # WINDOW_NORMAL で作成しておき、初回フレームのアスペクト比でリサイズする
    cv2.namedWindow(window_name, cv2.WINDOW_NORMAL)
    display_initialized = False

    with mss.mss() as sct:
        while True:
            # ウィンドウが閉じられていたら終了
            if not win32gui.IsWindow(hwnd):
                print("対象ウィンドウが閉じられました。")
                break

            frame = capture_window(sct, hwnd)
            if frame is None:
                continue

            # 初回フレームでキャプチャ解像度のアスペクト比に合わせて表示ウィンドウを設定
            if not display_initialized:
                h, w = frame.shape[:2]
                display_w = 960
                display_h = int(display_w * h / w)
                cv2.resizeWindow(window_name, display_w, display_h)
                display_initialized = True

            output = process_frame(frame, hands, mp_hands, mp_drawing, mp_drawing_styles)

            cv2.imshow(window_name, output)

            if cv2.waitKey(1) & 0xFF == ord("q"):
                break

    cv2.destroyAllWindows()
    hands.close()


if __name__ == "__main__":
    main()
