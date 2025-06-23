using UnityEngine;
using System;
using XaviEssencials.Runtime;

namespace XaviGames.Car
{
    [CreateAssetMenu(fileName = "_CarParameter", menuName = "Xavi Games/Car/Car Parameter")]
    public class CarParameter : ScriptableObject
    {
        [field: SerializeField]
        [field: ReadOnly]
        public string Id { get; private set; } = Guid.NewGuid().ToString("N").Substring(0, 8);
        
        [field: Header("Car Parameter")]
        [field: SerializeField]
        public string CarName { get; private set; }
        
        [field: SerializeField]
        public string CarDescription { get; private set; }

        [field: SerializeField]
        public Sprite CarImage { get; private set; }

        [field: SerializeField]
        public GameObject CarGameObject { get; private set; }
        
        [field: Header("Physical Parameters")]

        [field: SerializeField]
        public float TopSpeed { get; private set; }

        [field: SerializeField]
        public float Acceleration { get; private set; }

        [field: SerializeField]
        public float BreakForce { get; private set; }

        [field: SerializeField]
        public float SteeringRange { get; private set; }

        [field: SerializeField]
        public float SteeringRangeAtMaxSpeed { get; private set; }

        [field: SerializeField]
        public float CentreOfGravityOffset { get; private set; }

        [Header("Drift Friction Curve")]
        [SerializeField]
        private float _extremumSlip;

        [SerializeField]
        private float _extremumValue;
        
        [SerializeField]
        private float _asymptoteSlip;
        
        [SerializeField] 
        private float _asymptoteValue;
        
        [SerializeField] 
        private float _stiffness;

        [field: SerializeField]
        public float DriftAngularDamping { get; private set; }

        [field: Header("UI Parameters")]
        [field: SerializeField]
        public int UiTopSpeed { get; private set; }

        [field: SerializeField]
        public int UiAcceleration { get; private set; }

        [field: SerializeField]
        public int UiBreakForce { get; private set; }

        [field: SerializeField]
        public int UiSteeringRange { get; private set; }

        public WheelFrictionCurve DriftFrictionCurve
        {
            get
            {
                return new WheelFrictionCurve
                {
                    extremumSlip = _extremumSlip,
                    extremumValue = _extremumValue,
                    asymptoteSlip = _asymptoteSlip,
                    asymptoteValue = _asymptoteValue,
                    stiffness = _stiffness
                };
            }
        }
    }
}

