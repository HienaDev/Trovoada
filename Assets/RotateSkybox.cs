using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateSkybox : MonoBehaviour
{

    [SerializeField] private float rotationSpeed = 5f;
    private float currentRotation = 0f;
    void Update()
    {
        RenderSettings.skybox.SetFloat("_Rotation", currentRotation);
        currentRotation += rotationSpeed * Time.deltaTime;

    }
}
