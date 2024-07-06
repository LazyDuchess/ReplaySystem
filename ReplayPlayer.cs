using ReplaySystem.Patches;
using Reptile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Reptile.UserInputHandler;

namespace ReplaySystem
{
    public class ReplayPlayer : ReplayState
    {
        public float CurrentTime => _currentTime;
        public Replay Replay;
        public Action OnFrameSkip;
        private bool _paused = false;
        private float _playbackSpeed = 1f;
        private ReplayPlaybackCamera _replayCamera;
        private float _currentTime = 0f;
        public static GameplayUI ui;
        public override void Start()
        {
            Core.Instance.GameInput.DisableControllerMap(0);
            Core.Instance.GameInput.DisableControllerMap(1);
            Core.Instance.GameInput.DisableControllerMap(2);
            Core.Instance.GameInput.DisableControllerMap(3);
            Core.Instance.GameInput.DisableControllerMap(4);
            Core.Instance.GameInput.DisableControllerMap(5);
            _currentTime = 0f;
            _replayCamera = ReplayPlaybackCamera.Create();
            _paused = false;
            _playbackSpeed = 1f;
            UI.Instance.ShowNotification("C : toggle camera mode\r\nR : reset settings\r\nspace : pause\r\n0 to 4 : timeline \r\n(0,25,50,75,100%)\r\nMouse wheel up/down:\r\n -speed up/slow down clip\r\nQ and E : camera pan\r\nH : toggle UI\r\nG : toggle tooltip");

        }

        public void SkipTo(float time)
        {
            var frame = Replay.GetFrameForTime(time);
            if (frame >= Replay.FrameCount)
                frame = Replay.FrameCount - 1;
            if (frame < 0)
                frame = 0;
            ApplyFrameToWorld(Replay.Frames[frame], true);
            _currentTime = (float)frame * Replay.FPS;
        }

        public override void OnFixedUpdate()
        {
            var skip = false;
            if (_currentTime == 0f)
                skip = true;
            var frames = Replay.Frames;
            var frameIndex = Replay.GetFrameForTime(_currentTime);
            if (frameIndex >= frames.Count)
            {
                _currentTime = 0f;
                skip = true;
                frameIndex = Replay.GetFrameForTime(_currentTime);
            }
            else
                _currentTime += Time.fixedDeltaTime;
            ApplyFrameToWorld(frames[frameIndex], skip);
        }

        private void ApplyFrameToWorld(ReplayFrame frame, bool skip)
        {
            if (!frame.Valid)
                return;
            var p = Replay.Player;
            if (skip)
            {
                WorldHandler.instance.PlacePlayerAt(p, frame.PlayerPosition, frame.PlayerRotation, true);
                p.motor._rigidbody.rotation = frame.PlayerRotation;
            }
            else
            {
                p.transform.position = frame.PlayerPosition;
            }
            p.transform.rotation = frame.PlayerRotation;
            p.SetVelocity(frame.Velocity);
            p.moveInput = frame.MoveInput;
            p.jumpButtonHeld = frame.JumpButtonHeld;
            p.jumpButtonNew = frame.JumpButtonNew;
            p.sprayButtonHeld = frame.SprayButtonHeld;
            p.sprayButtonNew = frame.SprayButtonNew;
            p.trick1ButtonHeld = frame.Trick1ButtonHeld;
            p.trick1ButtonNew = frame.Trick1ButtonNew;
            p.trick2ButtonHeld = frame.Trick2ButtonHeld;
            p.trick2ButtonNew = frame.Trick2ButtonNew;
            p.trick3ButtonHeld = frame.Trick3ButtonHeld;
            p.trick3ButtonNew = frame.Trick3ButtonNew;
            p.slideButtonHeld = frame.SlideButtonHeld;
            p.slideButtonNew = frame.SlideButtonNew;
            p.danceButtonHeld = frame.DanceButtonHeld;
            p.danceButtonNew = frame.DanceButtonNew;
            p.boostButtonHeld = frame.BoostButtonHeld;
            p.boostButtonNew = frame.BoostButtonNew;
            p.switchStyleButtonHeld = frame.SwitchStyleButtonHeld;
            p.switchStyleButtonNew = frame.SwitchStyleButtonNew;
            p.walkButtonHeld = frame.WalkButtonHeld;
            p.walkButtonNew = frame.WalkButtonNew;
            p.phoneButtonNew = frame.PhoneButtonNew;
            if (frame.Jump)
                p.Jump();
            p.jumpRequested = frame.JumpRequested;
            p.moveInputPlain = frame.MoveInputPlain;
            p.boostCharge = frame.BoostCharge;
            p.HP = frame.HP;
            p.phone.state = frame.PhoneState;
            if (skip)
            {
                p.DropCombo();
                p.SwitchToEquippedMovestyle(frame.UsingEquippedMoveStyle, false, true, false);
                OnFrameSkip?.Invoke();
                p.PlayAnim(frame.Animation, true, true, frame.AnimationTime);
                p.StopCurrentAbility();
            }
            //_player.SetInputs(frame.PlayerInput);
        }

        public override void End()
        {
            var lastFrame = Replay.Frames[Replay.Frames.Count - 1];
            UI.Instance.HideNotification();
            ApplyFrameToWorld(lastFrame, true);
            Core.Instance.GameInput.EnableControllerMap(0);
            Core.Instance.GameInput.EnableControllerMap(1);
            Core.Instance.GameInput.EnableControllerMap(2);
            Core.Instance.GameInput.EnableControllerMap(3);
            Core.Instance.GameInput.EnableControllerMap(4);
            Core.Instance.GameInput.EnableControllerMap(5);
            Core.instance.UIManager.gameObject.SetActive(true);
            _replayCamera.Destroy();
            Time.timeScale = 1f;
        }

        private void RefreshTimeScale()
        {
            if (_paused)
                Time.timeScale = 0f;
            else
                Time.timeScale = _playbackSpeed;
        }

        public override void OnUpdate()
        {
            UpdatePlaybackInput();
            RefreshTimeScale();
        }

        private void UpdatePlaybackInput()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _paused = !_paused;
            }
            _playbackSpeed += Input.mouseScrollDelta.y * 0.05f;
            if (_playbackSpeed <= 0f)
                _playbackSpeed = 0f;
            if (Input.GetKeyDown(KeyCode.R))
            {
                _playbackSpeed = 1f;
                _paused = false;
            }
            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                _currentTime = 0f;
            }
            if (Input.GetKeyDown(KeyCode.H))
            {
                Core.instance.UIManager.gameObject.SetActive(!Core.instance.UIManager.gameObject.activeSelf);
            }
            if (Input.GetKeyDown(KeyCode.G))
            {
                UI.m_label1.gameObject.SetActive(!UI.m_label1.gameObject.activeSelf);
            }
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                SkipTo(0f);
            }
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                SkipTo(Replay.Length * 0.25f);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                SkipTo(Replay.Length * 0.5f);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                SkipTo(Replay.Length * 0.75f);
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                SkipTo(Replay.Length);
            }
        }
    }
}
