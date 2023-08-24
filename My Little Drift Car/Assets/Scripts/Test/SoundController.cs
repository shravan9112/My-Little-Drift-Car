using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (CarController))]

public class SoundController : MonoBehaviour
{
    [Header("Engine Sound")]
    [SerializeField] float PitchOffset = 0.5f;
    [SerializeField] AudioSource EngineSource;

    [Header("Slipping sounds")]
    [SerializeField] AudioSource SlipSource;
    [SerializeField] float MinSlipSound = 0.15f;
    [SerializeField] float MaxSlipForSound = 1f;

    CarController CarController;

    float MaxRPM {  get { return CarController.GetMaxRPM; } }

    float EngineRPM {  get { return CarController.EngineRPM; } }

    private void Awake()
    {
        CarController = GetComponent<CarController>();
    }

    private void Update()
    {
        EngineSource.pitch = (EngineRPM / MaxRPM) + PitchOffset;

        if (CarController.CurrentMaxSlip > MinSlipSound)
        {
            if (!SlipSource.isPlaying)
            {
                SlipSource.Play();
            }

            var newVolume = CarController.CurrentMaxSlip / MaxSlipForSound;
            SlipSource.volume = newVolume * 0.5f;
            SlipSource.pitch = Mathf.Clamp(newVolume, 0.75f, 1);
        }

        else
            SlipSource.Stop();
    }
}
