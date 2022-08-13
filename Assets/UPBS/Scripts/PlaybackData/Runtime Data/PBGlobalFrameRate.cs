using UnityEngine;

public class PBGlobalFrameRate : MonoBehaviour
{
    #region Singleton
    private static PBGlobalFrameRate _instance;
    public static PBGlobalFrameRate Instance
    {
        get => _instance;
    }

    private void Awake()
    {
        if (_instance)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    }
    #endregion
    private readonly float _pollingTime = 1f;
    private float _time;
    private int _frameCount;

    public int FrameRate { get; private set; } = 0;

    void Update()
    {
        _time += Time.deltaTime;
        ++_frameCount;
        if( _time >= _pollingTime)
        {
            FrameRate = Mathf.RoundToInt(_frameCount / _time);

            _time -= _pollingTime;
            _frameCount = 0;
        }
    }
}
