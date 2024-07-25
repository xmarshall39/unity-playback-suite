using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UPBS.Data;
using System.Linq;
using UnityEngine.UI;

namespace UPBS.Player
{
    /// <summary>
    /// Using the collected [GlobalFrameData or CameraFrameData] as a base, this will update the simulation progress
    /// </summary>
    public class PBFrameController : MonoBehaviour
    {
        #region Singleton
        private static PBFrameController _instance;
        public static PBFrameController Instance
        {
            get
            {
                return _instance;
            }
        }
        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        #endregion
        #region Variables
        //PRIVATE FIELDS
        private int _currentFrameIndex = 0;
        private int _lastNumberOfFramesMoved = 0;
        [SerializeField, ReadOnly, Tooltip("1-100 Value which determines how playback speed should be incrimented\nA value of 50 will run at the recording rate")]
        private int _speed = 50; // 1-100 value
        private int _maxSpeed = 100;
        private int _minSpeed = 1;
        private float _recordingRate = .016f;
        private float _speedIncriment = 0.05f;
        private bool _isInitialized = false;
        private bool _fwdRunning, _rwdRunning;
        private Coroutine _fwdRoutine, _rwdRoutine;
        private Dictionary<ulong, PBGlobalFrameData> _fullFrameData;
        private List<PBGlobalFrameData> _frameDataList;

        //PUBLIC FIELDS
        public delegate void FrameUpdate(); //Denotes changes made to the playback frame to display
        public delegate void PlaybackUpdate(); //Denotes changes in the state of playback as a video player
        public static event FrameUpdate OnFrameUpdate;
        public static event FrameUpdate OnInit;
        public static event PlaybackUpdate OnPlay;
        public static event PlaybackUpdate OnPlayForward;
        public static event PlaybackUpdate OnPlayBackward;
        public static event PlaybackUpdate OnPaused;

        //PUBLIC PROPERTIES
        public int CurrentFrameIndex
        {
            get
            {
                return _currentFrameIndex;
            }
            private set
            {
                 _lastNumberOfFramesMoved = value - _currentFrameIndex;
                _currentFrameIndex = value;
                OnFrameUpdate?.Invoke();
            }
        }

        public ulong CurrentFrameKey
        {
            get
            {
                return _frameDataList[CurrentFrameIndex].Timestamp;
            }
        }

        public int LastNumberOfFramesMoved
        {
            get
            {
                return _lastNumberOfFramesMoved;
            }
        }

        public ulong GetFrameKey(int frameIndex)
        {
            ulong timestamp = 0;
            if (frameIndex < _frameDataList.Count && frameIndex > 0)
            {
                timestamp = _frameDataList[frameIndex].Timestamp;
            }

            return timestamp;
        }

        public int FinalFrame
        {
            get;
            private set;
        }

        public int MaxSpeed
        {
            get => _maxSpeed;
            set => _maxSpeed = Mathf.Max(2, value) > _minSpeed ? Mathf.Max(2, value) : _minSpeed + 1;
        }

        public int MinSpeed
        {
            get => _minSpeed;
            set => _minSpeed = Mathf.Max(1, value) < _maxSpeed ? Mathf.Max(1, value) : _maxSpeed - 1;
        }

        public int Speed
        {
            get => _speed;
            set => _speed = Mathf.Clamp(value, _minSpeed, _maxSpeed);
        }

        public bool LoopEnabled
        {
            get;
            private set;
        }
        #endregion
        #region Functions
        //PUBLIC MEMBER FUNCTIONS

        /// <summary>
        /// Get global frame data from the frame library.
        /// </summary>
        public void Initialize()
        {
            var globalData = PBFrameLibraryManager.Instance.GetGlobalFrameData();
            _fullFrameData = globalData.ToDictionary(kvp => kvp.Key, kvp => (PBGlobalFrameData)kvp.Value);
            _frameDataList = _fullFrameData.Values.ToList();
            _recordingRate = PBLoadingManager.Instance.GlobalTrackerInfo.recordingRate;
            FinalFrame = _fullFrameData.Count - 1;
            
            _isInitialized = true;
            OnInit?.Invoke();
            OnFrameUpdate?.Invoke();
            Debug.Log(_fullFrameData.Count);
        }

        /// <summary>
        /// Clear all frame data
        /// </summary>
        public void Clear()
        {
            if (_isInitialized)
            {
                SetFrame(0);
                _fullFrameData.Clear();
                _isInitialized = false;
            }
        }
        public bool IsRunning() => _fwdRunning || _rwdRunning;
        public int FrameCount => _fullFrameData.Count;
        /// <summary>Returns a copy of a requested frame of data</summary>
        public PBGlobalFrameData GetFrameDataByIndex(int frameIndex)
        {
            if(frameIndex >= 0 && frameIndex < _frameDataList.Count)
            {

                return new PBGlobalFrameData(_frameDataList[frameIndex]);
            }
            else
            {
                return null;
            }
        }
        /// <summary>Returns a copy of any currently displayed frame data</summary>
        public PBGlobalFrameData GetCurrentFrameData() => new PBGlobalFrameData(_frameDataList[_currentFrameIndex]);
        
        //PRIVATE FUNCTIONS
        /// <summary>Advance playback by some set number of frames</summary>
        private void AdvanceFrames(int frames)
        {
            if (LoopEnabled)
            {
                CurrentFrameIndex = CurrentFrameIndex + frames > FinalFrame ? CurrentFrameIndex + frames - FinalFrame - 1 : CurrentFrameIndex + frames;
            }
            else
            {
                CurrentFrameIndex = Mathf.Min(CurrentFrameIndex + frames, FinalFrame);
            }
        }

        /// <summary>
        /// Rewinds the playback by some number of frames.
        /// </summary>
        /// <param name="frames">Rewind amount in frames</param>
        private void RewindFrames(int frames)
        {
            if (LoopEnabled)
            {
                CurrentFrameIndex = CurrentFrameIndex - frames < 0 ? FinalFrame + (CurrentFrameIndex - frames) + 1 : CurrentFrameIndex - frames;
            }

            else
            {
                CurrentFrameIndex = Mathf.Max(CurrentFrameIndex - frames, 0);
            }
        }

        /// <summary>
        /// Converts current playback speed value into a frame delay time in seconds.
        /// </summary>
        private float SpeedToDelay()
        {
            float floatSpeed = (float)_speed;
            float floatMaxSpeed = (float)_maxSpeed;

            float delay = Mathf.Lerp(_recordingRate * 10, _recordingRate / 10, floatSpeed / floatMaxSpeed);
            if (floatSpeed / floatMaxSpeed == 0.5)
            {
                delay = _recordingRate;
            }
            else if (floatSpeed / floatMaxSpeed < 0.5)
            {
                float t = floatSpeed / (floatMaxSpeed / 2);
                delay = Mathf.Lerp(_recordingRate * 10, _recordingRate, t);
            }
            else
            {
                float halfSpeed = floatMaxSpeed / 2;
                float t = (floatSpeed - (floatMaxSpeed / 2)) / (floatMaxSpeed / 2);
                delay = Mathf.Lerp(_recordingRate, _recordingRate / 10, t);
            }

            return delay;
        }

        public float GetPlaybackFPS() => 1 / SpeedToDelay();

        /// <summary>
        /// Automatically progress the playback forward
        /// </summary>
        private IEnumerator PlayForwardRoutine()
        {
            while (LoopEnabled || CurrentFrameIndex != FinalFrame)
            {
                _fwdRunning = true;
                AdvanceFrames(1);
                yield return new WaitForSecondsRealtime(SpeedToDelay());
            }

            _fwdRunning = false;
        }

        /// <summary> Automatically progress the playback backward. </summary>
        private IEnumerator RewindRoutine()
        {
            while (LoopEnabled || CurrentFrameIndex != 0)
            {
                _rwdRunning = true;
                RewindFrames(1);
                yield return new WaitForSecondsRealtime(SpeedToDelay());
            }
            _rwdRunning = false;
        }

        //PUBLIC FUNCTIONS
        /// <summary>
        /// Sets the state of the current frame. Returns true if the provided frame index is valid.
        /// </summary>
        public bool SetFrame(int targetFrame)
        {
            if (targetFrame >= 0 && targetFrame <= FinalFrame)
            {
                CurrentFrameIndex = targetFrame;
                Pause();
                return true;
            }

            return false;
        }
        [EasyButtons.Button]
        ///<summary>Begin/continue playback forward in time</summary>
        public void PlayForward()
        {
            if (!_fwdRunning)
            {
                if (_rwdRunning)
                {
                    StopCoroutine(_rwdRoutine);
                    _rwdRunning = false;
                }
                _fwdRoutine = StartCoroutine(PlayForwardRoutine());
                OnPlayForward?.Invoke();
                OnPlay?.Invoke();
            }

        }
        [EasyButtons.Button]
        ///<summary>Begin/continue playback backward in time</summary>
        public void PlayBackward()
        {
            if (!_rwdRunning)
            {
                if (_fwdRunning)
                {
                    StopCoroutine(_fwdRoutine);
                    _fwdRunning = false;
                }
                _rwdRoutine = StartCoroutine(RewindRoutine());
                OnPlayBackward?.Invoke();
                OnPlay?.Invoke();
            }

        }
        [EasyButtons.Button]
        ///<summary>Stop all playback routines</summary>
        public void Pause()
        {
            bool wasRunning = false;
            if (_fwdRunning || _rwdRunning) wasRunning = true;

            if (_fwdRunning) StopCoroutine(_fwdRoutine);
            if (_rwdRunning) StopCoroutine(_rwdRoutine);
            _fwdRunning = false;
            _rwdRunning = false;

            if (wasRunning)
            {
                OnPaused?.Invoke();
            }
        }
        /// <summary>Toggles between the last playback direction and the pause state</summary>
        public void TogglePlayback()
        {
            if (IsRunning()) Pause();
            else PlayForward();
        }
        /// <summary>Advance some number of frames forward in playback. Triggers an auto-pause</summary>
        public void SkipFramesForward(int skip, bool pause = true)
        {
            if(pause) Pause();
            AdvanceFrames(skip);
        }
        /// <summary>Advance some number of frames backward in playback. Triggers an auto-pause</summary>
        public void SkipFramesBackward(int skip, bool pause = true)
        {
            if(pause) Pause();
            RewindFrames(skip);
        }
        #endregion
    }
}

