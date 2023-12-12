using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reptile;
using UnityEngine;

namespace ReplaySystem
{
    public class ReplayPlaybackCamera : MonoBehaviour
    {
        private const float MedFOV = 40f;
        private const float CameraMinTopDownDistance = 3f;
        private const float CameraMaxDistance = 30f;
        private const float CameraRadius = 0.25f;
        private const float MinPrevCameraDistance = 1f;
        private ReplayPlayer _replayPlayer;
        private Player _player;
        private float _currentCameraTypeEndTime = 0f;
        private CameraType _currentCameraType = CameraType.Gameplay;
        enum CameraType
        {
            Gameplay,
            LookAt,
            Orbit,
            FreeCam
        }
        private bool _manualCam = false;
        private GameplayCamera _gameplayCamera;
        private Camera _camera;
        private float _originalFOV;

        private float _freeCamX = 0f;
        private float _freeCamY = 0f;
        private float _freeCamRoll = 0f;
        private float _freeCamFOV = 65f;
        private float _freeCamDistance = 2.5f;
        private float _freeCamSensitivity = 1.5f;
        private float _freeCamSpeed = 12f;

        public static ReplayPlaybackCamera Create()
        {
            var gameObject = new GameObject("Replay Camera Controller");
            return gameObject.AddComponent<ReplayPlaybackCamera>();
        }

        private void Awake()
        {
            _gameplayCamera = GameplayCamera.instance;
            _camera = _gameplayCamera.cam;
            _originalFOV = _camera.fieldOfView;
            _gameplayCamera.enabled = false;
            _player = WorldHandler.instance.GetCurrentPlayer();
            _replayPlayer = ReplayManager.Instance.ReplayPlayer;
            _replayPlayer.OnFrameSkip += OnSkip;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                if (_currentCameraType == CameraType.Gameplay)
                    EndGameplayCamera();

                switch(_currentCameraType)
                {
                    case CameraType.Orbit:
                        DoFreeCamera();
                        break;
                    case CameraType.Gameplay:
                        DoOrbitCamera();
                        break;
                    case CameraType.FreeCam:
                        DoNewCamera();
                        break;
                    default:
                        DoManualGameplayCamera();
                        break;
                }
            }
            if (_manualCam)
            {
                UpdateManualCameras();
                return;
            }
            if (_replayPlayer.CurrentTime >= _currentCameraTypeEndTime)
                DoNewCamera();
            if (_currentCameraType != CameraType.LookAt)
                return;
            var lookTarget = GetLookTarget();
            var distance = Vector3.Distance(lookTarget, _camera.transform.position);
            if (distance >= CameraMaxDistance)
                DoNewCamera();
            if (!CheckSpotForCameraLookAt(_camera.transform.position))
                DoNewCamera();
            var heading = (lookTarget - _camera.transform.position).normalized;
            _camera.transform.rotation = Quaternion.LookRotation(heading, Vector3.up);
        }

        void UpdateManualCameras()
        {
            if (_currentCameraType == CameraType.Gameplay)
                return;
            _camera.fieldOfView = _freeCamFOV;
            
            if (Input.GetKey(KeyCode.T))
            {
                _freeCamRoll = 0f;
            }
            else
            {
                if (Input.GetKey(KeyCode.E))
                {
                    _freeCamRoll += _freeCamSpeed * Time.unscaledDeltaTime;
                }
                if (Input.GetKey(KeyCode.Q))
                {
                    _freeCamRoll -= _freeCamSpeed * Time.unscaledDeltaTime;
                }
            }

            if (Input.GetKey(KeyCode.Mouse0) || (_currentCameraType == CameraType.FreeCam && !Input.GetKey(KeyCode.Mouse2) && !Input.GetKey(KeyCode.Mouse1)))
            {
                _freeCamX += Input.GetAxisRaw("Mouse X") * _freeCamSensitivity;
                _freeCamY -= Input.GetAxisRaw("Mouse Y") * _freeCamSensitivity;
            }
            if (Input.GetKey(KeyCode.Mouse1))
            {
                if (_currentCameraType == CameraType.Orbit)
                {
                    _freeCamDistance -= Input.GetAxisRaw("Mouse Y") * _freeCamSensitivity;
                    if (_freeCamDistance < 0f)
                        _freeCamDistance = 0f;
                }
                else
                {
                    _freeCamSpeed += Input.GetAxisRaw("Mouse Y") * _freeCamSensitivity;
                    if (_freeCamSpeed < 0f)
                        _freeCamSpeed = 0f;
                }
            }
            if (Input.GetKey(KeyCode.Mouse2))
            {
                _freeCamFOV -= Input.GetAxisRaw("Mouse Y") * _freeCamSensitivity;
                if (_freeCamFOV < 1f)
                    _freeCamFOV = 1f;
            }
            if (_currentCameraType == CameraType.Orbit)
                UpdateOrbitCamera();
            if (_currentCameraType == CameraType.FreeCam)
                UpdateFreeCamera();
        }

        void UpdateFreeCamera()
        {
            var forward = Input.GetKey(KeyCode.W) ? 1f : 0f;
            var backward = Input.GetKey(KeyCode.S) ? 1f : 0f;
            var right = Input.GetKey(KeyCode.D) ? 1f : 0f;
            var left = Input.GetKey(KeyCode.A) ? 1f : 0f;

            var moveInputs = Vector3.zero;
            moveInputs += _camera.transform.forward * forward;
            moveInputs += -_camera.transform.forward * backward;
            moveInputs += _camera.transform.right * right;
            moveInputs += -_camera.transform.right * left;
            moveInputs.Normalize();

            var speed = _freeCamSpeed;
            if (Input.GetKey(KeyCode.LeftShift))
                speed *= 2f;

            _camera.transform.position += moveInputs * speed * Time.unscaledDeltaTime;
            var cameraRotation = Quaternion.Euler(_freeCamY, _freeCamX, _freeCamRoll);
            _camera.transform.rotation = cameraRotation;
        }

        void UpdateOrbitCamera()
        {
            var target = GetLookTarget();
            var cameraRotation = Quaternion.Euler(_freeCamY, _freeCamX, _freeCamRoll);
            var cameraForward = cameraRotation * Vector3.forward;
            var cameraPos = target - (cameraForward * _freeCamDistance);
            _camera.transform.position = cameraPos;
            _camera.transform.rotation = cameraRotation;
        }

        private void DoFreeCamera()
        {
            _camera.fieldOfView = _freeCamFOV;
            _manualCam = true;
            _currentCameraType = CameraType.FreeCam;
        }

        private void DoOrbitCamera()
        {
            _camera.fieldOfView = _freeCamFOV;
            _manualCam = true;
            _currentCameraType = CameraType.Orbit;
        }

        private void DoManualGameplayCamera()
        {
            _manualCam = true;
            DoGameplayCamera(0f);
        }

        private void OnSkip()
        {
            if (!_manualCam)
                DoNewCamera();
        }

        private bool TryDoCamera(int attempts, float duration)
        {
            for (var i = 0; i < attempts; i++)
            {
                var lookTarget = GetLookTarget();
                var medCameraheight = lookTarget.y + UnityEngine.Random.Range(0f, 1f);

                var canDoLowCamera = true;
                var canDoMedCamera = true;

                var timeAhead = UnityEngine.Random.Range(1f, 3f);

                var medCamera = Vector3.zero;
                var lowCamera = Vector3.zero;

                if (!FindSpotForMedCameraLookAt(out medCamera, medCameraheight, timeAhead))
                {
                    canDoMedCamera = FindSpotForMedCameraLookAt(out medCamera, medCameraheight, 0f);
                }

                if (!FindSpotForMedCameraLookAt(out lowCamera, medCameraheight, timeAhead, true))
                {
                    canDoLowCamera = FindSpotForMedCameraLookAt(out lowCamera, medCameraheight, 0f, true);
                }
                var doLowCamera = false;

                if (canDoLowCamera)
                {
                    if (_player.IsGrounded() || _player.IsGrinding())
                        doLowCamera = UnityEngine.Random.Range(0, 4) != 0;
                    else
                        doLowCamera = UnityEngine.Random.Range(0, 3) == 0;
                    if (!canDoMedCamera)
                        doLowCamera = true;
                }

                if (doLowCamera)
                {
                    DoLookAtCamera(lowCamera, duration);
                    return true;
                }
                if (canDoMedCamera)
                {
                    DoLookAtCamera(medCamera, duration);
                    return true;
                }
            }
            return false;
        }

        private void DoNewCamera()
        {
            _manualCam = false;
            if (_currentCameraType == CameraType.Gameplay)
                EndGameplayCamera();
            var newCameraDuration = UnityEngine.Random.Range(3f, 5f);
            if (TryDoCamera(3, newCameraDuration))
                return;
            DoGameplayCamera(newCameraDuration);
        }

        private void DoLookAtCamera(Vector3 from, float duration)
        {
            var target = GetLookTarget();
            _camera.transform.position = from;
            var heading = (target - from).normalized;
            _camera.transform.rotation = Quaternion.LookRotation(heading, Vector3.up);
            _currentCameraType = CameraType.LookAt;
            _currentCameraTypeEndTime = _replayPlayer.CurrentTime + duration;
            _camera.fieldOfView = MedFOV;
        }

        private void DoGameplayCamera(float duration)
        {
            _currentCameraType = CameraType.Gameplay;
            _currentCameraTypeEndTime = _replayPlayer.CurrentTime + duration;
            _gameplayCamera.enabled = true;
            _camera.fieldOfView = _originalFOV;
        }

        private void EndGameplayCamera()
        {
            _gameplayCamera.enabled = false;
        }

        private bool FindSpotForMedCameraLookAt(out Vector3 spot, float height, float time, bool onGround = false)
        {
            var replay = _replayPlayer.Replay;
            var referenceTime = _replayPlayer.CurrentTime + time;
            var referenceFrameIndex = replay.GetFrameForTime(referenceTime);
            if (referenceFrameIndex >= replay.FrameCount)
                referenceFrameIndex = replay.FrameCount - 1;
            var referenceFrame = replay.Frames[referenceFrameIndex];
            var referencePosition = GetLookTarget(referenceFrame.PlayerPosition);
            var target = GetLookTarget();
            if (Vector3.Distance(referencePosition, target) >= CameraMaxDistance)
                referencePosition = target;
            var separation = 1f;
            var max = 8f;
            var candidates = new List<Vector3>();

            for(var i=-max;i<=max;i+=separation)
            {
                for(var j=-max;j<=max;j+=separation)
                {
                    var pos = referencePosition + new Vector3(i, height, j);
                    if (Vector2.Distance(new Vector2(pos.x, pos.z), new Vector2(referencePosition.x, referencePosition.z)) < CameraMinTopDownDistance)
                        continue;
                    if (onGround)
                    {
                        if (Physics.Raycast(pos, -Vector3.up, out RaycastHit hit, 10f, 1, QueryTriggerInteraction.Ignore))
                        {
                            pos = hit.point + (Vector3.up * 1f);
                        }
                    }
                    if (_currentCameraType == CameraType.LookAt)
                    {
                        var camDist = Vector3.Distance(pos, _camera.transform.position);
                        if (camDist < MinPrevCameraDistance)
                            continue;
                    }
                    if (CheckSpotForCameraLookAt(pos))
                        candidates.Add(pos);
                }
            }

            spot = Vector3.zero;

            if (candidates.Count <= 0)
                return false;

            spot = candidates[UnityEngine.Random.Range(0, candidates.Count)];
            return true;
        }

        private bool CheckSpotForCameraLookAt(Vector3 spot)
        {
            var prevCheckBackfaces = Physics.queriesHitBackfaces;
            Physics.queriesHitBackfaces = true;
            var target = GetLookTarget();
            if (Physics.CheckSphere(spot, CameraRadius, 1, QueryTriggerInteraction.Ignore))
            {
                Physics.queriesHitBackfaces = prevCheckBackfaces;
                return false;
            }
            var lookAtDifference = target - spot;
            var heading = lookAtDifference.normalized;
            var distance = lookAtDifference.magnitude - 1f;
            if (Physics.Raycast(spot, heading, out RaycastHit _, distance, 1, QueryTriggerInteraction.Ignore))
            {
                Physics.queriesHitBackfaces = prevCheckBackfaces;
                return false;
            }
            Physics.queriesHitBackfaces = prevCheckBackfaces;
            return true;
        }

        private Vector3 GetLookTarget()
        {
            return GetLookTarget(_player.transform.position);
        }
        private Vector3 GetLookTarget(Vector3 playerPosition)
        {
            return playerPosition + (Vector3.up * 1f);
        }

        private void OnDestroy()
        {
            _camera.fieldOfView = _originalFOV;
            _gameplayCamera.enabled = true;
            _replayPlayer.OnFrameSkip -= OnSkip;
        }

        public void Destroy()
        {
            Destroy(gameObject);
        }
    }
}
