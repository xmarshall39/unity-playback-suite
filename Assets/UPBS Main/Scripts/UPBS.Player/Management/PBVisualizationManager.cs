using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace UPBS.Player
{
    public class PBVisualizationManager : MonoBehaviour
    {
        #region Singleton
        private static PBVisualizationManager _instance;
        public static PBVisualizationManager Instance
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

        private PBVisualization[] _allVisualizations;

        public PBVisualization[] GetPlaybackCameras()
        {
            return (PBVisualization[])_allVisualizations.Clone();
        }
        private void Initialize()
        {
            _allVisualizations = (PBVisualization[])FindObjectsOfType<PBVisualization>().Where(x => !(x is PBCameraVisualization));
        }
    }
}