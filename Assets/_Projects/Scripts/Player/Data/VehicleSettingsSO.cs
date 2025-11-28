using UnityEngine;

namespace Game.Player.Data
{
    [CreateAssetMenu(menuName = "SOs/VehicleSettingsSO", fileName = "VehicleSettingsSO")]
    public class VehicleSettingsSO : ScriptableObject
    {
        [Header("Wheel Paddings")]
        [SerializeField] private float _wheelsPaddingX;

        [SerializeField] private float _wheelsPaddingZ;
        [Header("Spring Settings")]

        [SerializeField] private float _springRestLenght;
        [SerializeField] private float _springStrenght;
        [SerializeField] private float _springDamper;
        public float WheelsPaddingX => _wheelsPaddingX;
        public float WheelsPaddingZ => _wheelsPaddingZ;

        public float SpringRestLenght => _springRestLenght;

        public float SpringStrenght => _springStrenght;

        public float SpringDamper => _springDamper;
         
    }
}