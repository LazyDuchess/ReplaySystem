using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Reptile.UserInputHandler;
using Reptile.Phone;
using Reptile;

namespace ReplaySystem
{
    public class ReplayFrame
    {
        public int Frame;
        public bool Valid = false;
        public Vector3 PlayerPosition;
        public Quaternion PlayerRotation;
        public Vector2 MoveInputPlain;
        public Vector3 MoveInput;
        public bool JumpButtonNew;
        public bool JumpButtonHeld;
        public bool SprayButtonHeld;
        public bool SprayButtonNew;
        public bool Trick1ButtonHeld;
        public bool Trick1ButtonNew;
        public bool Trick2ButtonHeld;
        public bool Trick2ButtonNew;
        public bool Trick3ButtonHeld;
        public bool Trick3ButtonNew;
        public bool SlideButtonHeld;
        public bool SlideButtonNew;
        public bool DanceButtonHeld;
        public bool DanceButtonNew;
        public bool BoostButtonNew;
        public bool BoostButtonHeld;
        public bool SwitchStyleButtonNew;
        public bool SwitchStyleButtonHeld;
        public bool WalkButtonNew;
        public bool WalkButtonHeld;
        public bool PhoneButtonNew;
        public bool Jump = false;
        public bool JumpRequested;
        public Vector3 Velocity;
        public float BoostCharge;
        public float HP;
        public Phone.PhoneState PhoneState;
        public bool UsingEquippedMoveStyle;
        public int Animation;
        public float AnimationTime;
        private Replay _replay;
        public ReplayFrame(Replay replay)
        {
            _replay = replay;
        }
    }
}
