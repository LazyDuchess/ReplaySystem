using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Reptile;

namespace ReplaySystem
{
    public class ReplayManager : MonoBehaviour
    {
        public static ReplayManager Instance => _instance;
        public ReplayState CurrentReplayState => _currentReplayState;
        public ReplayRecorder ReplayRecorder => _replayRecorder;
        public ReplayPlayer ReplayPlayer => _replayPlayer;

        private static ReplayManager _instance;
        private ReplayState _currentReplayState = null;
        private ReplayRecorder _replayRecorder = new ReplayRecorder();
        private ReplayPlayer _replayPlayer = new ReplayPlayer();

        public static ReplayManager Create()
        {
            var replayManagerObject = new GameObject("Replay Manager");
            return replayManagerObject.AddComponent<ReplayManager>();
        }

        private void Awake()
        {
            _instance = this;
        }

        private void Update()
        {
            if (Core.instance.IsCorePaused)
                return;
            if (_currentReplayState != null)
                _currentReplayState.OnUpdate();
            if (Input.GetKeyDown(KeyCode.F3))
            {
                if (_currentReplayState == null)
                {
                    _currentReplayState?.End();
                    _currentReplayState = _replayRecorder;
                    _replayRecorder.Start();
                }
                else if (_currentReplayState == _replayRecorder)
                {
                    _replayRecorder.End();
                    _currentReplayState = _replayPlayer;
                    _replayPlayer.Replay = _replayRecorder.Replay;
                    _replayPlayer.Start();
                }
                else if (_currentReplayState == _replayPlayer)
                {
                    _replayPlayer.End();
                    _currentReplayState = null;
                }
            }
        }

        public void OnFixedUpdate()
        {
            if (Core.instance.IsCorePaused)
                return;
            _currentReplayState?.OnFixedUpdate();
        }

        private void OnDestroy()
        {
            _instance = null; //
        }
    }
}
