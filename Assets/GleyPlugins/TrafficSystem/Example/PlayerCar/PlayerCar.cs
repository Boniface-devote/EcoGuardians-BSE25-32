using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

namespace GleyTrafficSystem
{
    [System.Serializable]
    public class AxleInfo
    {
        public WheelCollider leftWheel;
        public WheelCollider rightWheel;
        public bool motor;
        public bool steering;
    }

    public class PlayerCar : MonoBehaviour
    {
        public List<AxleInfo> axleInfos;
        public Transform centerOfMass;
        public float maxMotorTorque;
        public float maxSteeringAngle;
        public float interactRadius = 3f;

        public Button emptyButton; // ✅ UI button to assign in Inspector

        bool mainLights;
        //bool brake;
        //bool reverse;
        bool blinkLeft;
        bool blinkRifgt;
        float realtimeSinceStartup;
        Rigidbody rb;

        UIInput inputScript;

        private void Start()
        {
            GetComponent<Rigidbody>().centerOfMass = centerOfMass.localPosition;
            inputScript = gameObject.AddComponent<UIInput>().Initializ();
            rb = GetComponent<Rigidbody>();

            if (emptyButton != null)
                emptyButton.onClick.AddListener(TryEmptyNearbyBin); // ✅ mobile interaction
        }

        public void ApplyLocalPositionToVisuals(WheelCollider collider)
        {
            if (collider.transform.childCount == 0) return;

            Transform visualWheel = collider.transform.GetChild(0);
            collider.GetWorldPose(out Vector3 position, out Quaternion rotation);
            visualWheel.transform.position = position;
            visualWheel.transform.rotation = rotation;
        }

        public void FixedUpdate()
        {
            float motor = maxMotorTorque * inputScript.GetVerticalInput();
            float steering = maxSteeringAngle * inputScript.GetHorizontalInput();

            float localVelocity = transform.InverseTransformDirection(rb.linearVelocity).z + 0.1f;
            //reverse = false;
            //brake = false;

            if (localVelocity < 0)
                //reverse = true;

            if (motor < 0)
            {
                //if (localVelocity > 0) brake = true;
            }
            else
            {
                //if (motor > 0 && localVelocity < 0) brake = true;
            }

            foreach (AxleInfo axleInfo in axleInfos)
            {
                if (axleInfo.steering)
                {
                    axleInfo.leftWheel.steerAngle = steering;
                    axleInfo.rightWheel.steerAngle = steering;
                }
                if (axleInfo.motor)
                {
                    axleInfo.leftWheel.motorTorque = motor;
                    axleInfo.rightWheel.motorTorque = motor;
                }
                ApplyLocalPositionToVisuals(axleInfo.leftWheel);
                ApplyLocalPositionToVisuals(axleInfo.rightWheel);
            }
        }

        private void Update()
        {
            realtimeSinceStartup += Time.deltaTime;

            if (Input.GetKeyDown(KeyCode.R))
                TryEmptyNearbyBin(); // ✅ keyboard interaction
        }

        private void TryEmptyNearbyBin()
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, interactRadius);
            foreach (Collider col in hitColliders)
            {
                TrashBin bin = col.GetComponent<TrashBin>();
                if (bin != null)
                {
                    bin.Empty();
                    break;
                }
            }
        }
    }
}
