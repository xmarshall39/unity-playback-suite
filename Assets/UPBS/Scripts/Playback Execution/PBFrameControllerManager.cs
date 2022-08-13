using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UPBS.Data;

namespace UPBS.Execution
{
    /// <summary>
    /// Using the collected [GlobalFrameData or CameraFrameData] as a base, this will update the simulation progress
    /// </summary>
    public class PBFrameControllerManager : MonoBehaviour
    {
        #region Singleton
        private static PBFrameControllerManager _instance;
        public static PBFrameControllerManager Instance
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
        private int _currentFrame = 0;
        private float _speed = 1f;
        private bool _isInitialized = false;
        private bool _fwdRunning, _rwdRunning;
        private Coroutine _fwdRoutine, _rwdRoutine;
        private List<PBGlobalFrameData> _fullFrameData;
        
        //PUBLIC FIELDS
        public delegate void FrameUpdate();
        public delegate void PlaybackUpdate();
        public event FrameUpdate OnFrameUpdate;
        public event PlaybackUpdate OnPlay;
        public event PlaybackUpdate OnPlayForward;
        public event PlaybackUpdate OnPlayBackward;
        public event PlaybackUpdate OnPaused;

        //PUBLIC PROPERTIES
        public int CurrentFrame
        {
            get
            {
                return _currentFrame;
            }
            private set
            {
                _currentFrame = value;
                OnFrameUpdate?.Invoke();
            }
        }

        public int FinalFrame
        {
            get;
            private set;
        }

        public bool LoopEnabled
        {
            get;
            private set;
        }
        #endregion
        #region Functions
        //PUBLIC MEMBER FUNCTIONS
        public void Initialize(PBFrameParser globalParser, string[] dataRows)
        {
            _fullFrameData = new List<PBGlobalFrameData>();
            for (int i = 1; i < dataRows.Length; i++)
            {
                var frame = new PBGlobalFrameData();
                if(frame.ParseRow(globalParser, dataRows[i].Split(','), i))
                {
                    _fullFrameData.Add(frame);
                }

                else
                {
                    Debug.LogWarning($"Unable to parse row {i}");
                }
                
            }
            
            _isInitialized = true;
        }
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
        /// <summary>Returns a copy of a requested frame of data</summary>
        public PBGlobalFrameData GetFrameData(int frameIndex)
        {
            if(frameIndex >= 0 && frameIndex < _fullFrameData.Count)
            {
                return new PBGlobalFrameData(_fullFrameData[frameIndex]);
            }
            else
            {
                return null;
            }
        }
        /// <summary>Returns a copy of any currently displayed frame data</summary>
        public PBGlobalFrameData GetCurrentFrameData() => new PBGlobalFrameData(_fullFrameData[_currentFrame]);
        
        //PRIVATE FUNCTIONS
        /// <summary>Advance playback by some set number of frames</summary>
        private void AdvanceFrames(int frames)
        {
            if (LoopEnabled)
            {
                CurrentFrame = CurrentFrame + frames > FinalFrame ? CurrentFrame + frames - FinalFrame - 1 : CurrentFrame + frames;
            }
            else
            {
                CurrentFrame = Mathf.Min(CurrentFrame + frames, FinalFrame);
            }
        }
        private void RewindFrames(int frames)
        {
            if (LoopEnabled)
            {
                CurrentFrame = CurrentFrame - frames < 0 ? FinalFrame + (CurrentFrame - frames) + 1 : CurrentFrame - frames;
            }

            else
            {
                CurrentFrame = Mathf.Max(CurrentFrame - frames, 0);
            }
        }
        private float SpeedToDelay() => 1 / _speed;
        private IEnumerator PlayForwardRoutine()
        {
            while (LoopEnabled || CurrentFrame != FinalFrame)
            {
                _fwdRunning = true;
                AdvanceFrames(1);
                yield return new WaitForSeconds(SpeedToDelay());
            }

            _fwdRunning = false;
        }
        private IEnumerator RewindRoutine()
        {
            while (LoopEnabled || CurrentFrame != 0)
            {
                _rwdRunning = true;
                RewindFrames(1);
                yield return new WaitForSeconds(SpeedToDelay());
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

                CurrentFrame = targetFrame;
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
            if (_fwdRunning) StopCoroutine(_fwdRoutine);
            if (_rwdRunning) StopCoroutine(_rwdRoutine);
            _fwdRunning = false;
            _rwdRunning = false;
            OnPaused?.Invoke();
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

