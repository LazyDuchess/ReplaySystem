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
    public class ReplayRecorder : ReplayState
    {
        public Replay Replay => _currentReplay;
        public InputBuffer CurrentInput;
        public ReplayFrame LastFrame
        {
            get
            {
                return _currentReplay.Frames[_currentReplay.Frames.Count - 1];
            }
        }
        private Replay _currentReplay;

        public override void Start()
        {
            _currentReplay = new Replay(Time.fixedDeltaTime);
            _currentReplay.Init();
        }

        public void OnPlayerInput(Player player)
        {
            if (WorldHandler.instance.GetCurrentPlayer() != player)
                return;
            CurrentInput = player.inputBuffer;
        }

        public override void OnFixedUpdate()
        {
            RecordCurrentFrame();
        }

        private void RecordCurrentFrame()
        {
            var frame = new ReplayFrame(Replay);
            frame.Frame = Replay.Frames.Count;
            Replay.Frames.Add(frame);
            if (Replay.Player == null)
                return;
            var p = Replay.Player;
            frame.Valid = true;
            frame.PlayerPosition = p.transform.position;
            frame.PlayerRotation = p.transform.rotation;
            frame.MoveInput = p.moveInput;
            frame.JumpButtonNew = p.jumpButtonNew;
            frame.JumpButtonHeld = p.jumpButtonHeld;
            frame.SprayButtonHeld = p.sprayButtonHeld;
            frame.SprayButtonNew = p.sprayButtonNew;
            frame.Trick1ButtonHeld = p.trick1ButtonHeld;
            frame.Trick1ButtonNew = p.trick1ButtonNew;
            frame.Trick2ButtonHeld = p.trick2ButtonHeld;
            frame.Trick2ButtonNew = p.trick2ButtonNew;
            frame.Trick3ButtonHeld = p.trick3ButtonHeld;
            frame.Trick3ButtonNew = p.trick3ButtonNew;
            frame.SlideButtonHeld = p.slideButtonHeld;
            frame.SlideButtonNew = p.slideButtonNew;
            frame.DanceButtonHeld = p.danceButtonHeld;
            frame.DanceButtonNew = p.danceButtonNew;
            frame.BoostButtonNew = p.boostButtonNew;
            frame.BoostButtonHeld = p.boostButtonHeld;
            frame.SwitchStyleButtonNew = p.switchStyleButtonNew;
            frame.SwitchStyleButtonHeld = p.switchStyleButtonHeld;
            frame.WalkButtonNew = p.walkButtonNew;
            frame.WalkButtonHeld = p.walkButtonHeld;
            frame.PhoneButtonNew = p.phoneButtonNew;
            frame.Velocity = p.GetVelocity();
            frame.JumpRequested = p.jumpRequested;
            frame.MoveInputPlain = p.moveInputPlain;
            frame.BoostCharge = p.boostCharge;
            frame.HP = p.HP;
            frame.PhoneState = p.phone.state;
            frame.UsingEquippedMoveStyle = p.usingEquippedMovestyle;
            frame.Animation = p.curAnim;
            frame.AnimationTime = p.anim.playbackTime;
        }
    }
}
