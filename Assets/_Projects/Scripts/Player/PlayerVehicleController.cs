using System.Collections.Generic;
using Game.Player.Data;
using UnityEngine;

namespace Game.Player
{
    public class PlayerVehicleController : MonoBehaviour
    {
        private class SpringData
        {
            public float _currentLength;
            public float _currentVelocity;
        }

        private static readonly WheelType[] _wheelTypes = new WheelType[]
        {
            WheelType.BackLeft, WheelType.BackRight, WheelType.FrontLeft, WheelType.FrontRight
        };

        [SerializeField] private VehicleSettingsSO _vehicleSettings;
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private BoxCollider _hitcollider;
        [SerializeField] private Transform _vehicleTransform;

        private Dictionary<WheelType, SpringData> _springDatas;
        private float _steerInput;
        private float _accelerationInput;

        private void Awake()
        {
            _springDatas = new Dictionary<WheelType, SpringData>();
            foreach (WheelType wheelType in _wheelTypes)
            {
                _springDatas.Add(wheelType, new SpringData());
            }
        }

        private void Update()
        {
            SetSteerInput(Input.GetAxis("Horizontal"));
            SetAccelerationInput(Input.GetAxis("Vertical"));
        }

        private void FixedUpdate()
        {
            UpdateSuspension();
        }

        private void SetSteerInput(float steerInput)
        {
            _steerInput = Mathf.Clamp(steerInput, -1f, 1f);
        }

        private void SetAccelerationInput(float accelerationInput)
        {
            _accelerationInput = Mathf.Clamp(accelerationInput, -1f, 1f);
        }

        private void UpdateSuspension()
        {
            foreach (WheelType id in _springDatas.Keys)
            {
                CastSpring(id);
                float currentVelocity = _springDatas[id]._currentVelocity;
                float currentLength = _springDatas[id]._currentLength;

                float force = SpringMathExtensions.CalculateForceDamped(currentLength, currentVelocity,
                    _vehicleSettings.SpringRestLenght, _vehicleSettings.SpringStrenght,
                    _vehicleSettings.SpringDamper);

                _rigidbody.AddForceAtPosition(force * transform.up, GetSpringPosition(id));
            }
        }

        private void CastSpring(WheelType wheel)
        {
            Vector3 position = GetSpringPosition(wheel);

            float previousLength = _springDatas[wheel]._currentLength;

            float currentLength;

            if (Physics.Raycast(position, -_vehicleTransform.up, out var hit, _vehicleSettings.SpringRestLenght))
            {
                currentLength = hit.distance;
            }
            else
            {
                currentLength = _vehicleSettings.SpringRestLenght;
            }

            _springDatas[wheel]._currentVelocity = (currentLength - previousLength) / Time.fixedDeltaTime;
            _springDatas[wheel]._currentLength = currentLength;
        } 

        private Vector3 GetSpringPosition(WheelType wheel)
        {
            return _vehicleTransform.localToWorldMatrix.MultiplyPoint3x4(GetSpringRelativePosition(wheel));
        }

        private Vector3 GetSpringRelativePosition(WheelType wheel)
        {
            Vector3 boxSize = _hitcollider.size;
            float boxBottom = boxSize.y * -0.5f;

            float paddingX = _vehicleSettings.WheelsPaddingX;
            float paddingZ = _vehicleSettings.WheelsPaddingZ;

            return wheel switch
            {
                WheelType.FrontLeft => new Vector3(boxSize.x * (paddingX - 0.5f), boxBottom, boxSize.z * (0.5f - paddingZ)),
                WheelType.FrontRight => new Vector3(boxSize.x * (0.5f - paddingX), boxBottom, boxSize.z * (0.5f - paddingZ)),
                WheelType.BackLeft => new Vector3(boxSize.x * (paddingX - 0.5f), boxBottom, boxSize.z * (paddingZ - 0.5f)),
                WheelType.BackRight => new Vector3(boxSize.x * (0.5f - paddingX), boxBottom, boxSize.z * (paddingZ - 0.5f)),
                _ => default,
            };
        }
    }

    public static class SpringMathExtensions
    {
        public static float CalculateForce(float currentLength, float restLength, float strength)
        {
            float lengthOffset = restLength - currentLength;
            return lengthOffset * strength;
        }

        public static float CalculateForceDamped(float currentLength, float lengthVelocity, float restLength,
            float strength, float damper)
        {
            float lengthOffset = restLength - currentLength;
            return (lengthOffset * strength) - (lengthVelocity * damper);
        }
    }
}