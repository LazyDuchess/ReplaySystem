using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reptile;
using UnityEngine;
using static Reptile.UserInputHandler;

namespace ReplaySystem
{
    public class Replay
    {
        public float FPS => _fps;
        public Player Player => _player;
        public int FrameCount => _frames.Count;
        public float Length => (FrameCount - 1) * FPS;
        public List<ReplayFrame> Frames => _frames;
        private Player _player;
        private float _fps = 0f;
        private List<ReplayFrame> _frames = new List<ReplayFrame>();

        public Replay(float fps)
        {
            _fps = fps;
        }

        public void Init()
        {
            _player = WorldHandler.instance.GetCurrentPlayer();
        }

        public int GetFrameForTime(float time)
        {
            var divTime = Mathf.FloorToInt(time / _fps);
            return divTime;
        }
    }
}
