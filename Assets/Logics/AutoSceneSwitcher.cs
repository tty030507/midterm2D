using UnityEngine;
using UnityEngine.Video;

public class AutoSceneSwitcher : MonoBehaviour {
    void Start() {
        // 视频播放结束时，自动调用跳转逻辑
        GetComponent<VideoPlayer>().loopPointReached += EndReached;
    }

    void EndReached(VideoPlayer vp) {
        GameFlowManager.Instance.LoadNextStep(); // 调用你已有的跳转逻辑
    }
}