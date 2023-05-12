﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CarScript : MonoBehaviour
{

    WheelJoint2D[] wheelJoints;
    JointMotor2D frontWheel;
    JointMotor2D backWheel;

    private float maxSpeed = -1000f;
    private float maxBackSpeed = 300f;
    private float acceleration = 400f;
    private float deacceleration = -100f;
    private float brakeForce = 3000f;
    private float gravity = 9.8f;
    private float angleCar = 0;
    private float maxSpeedUpgrade = 1;
    private float accelerationUpgrade = 1;
    private float fuelUpgrade = 1;
    private float tireUpgrade = 1;
    private float chassisUpgrade = 1;
    public int maxHealth = 150;
    public int health { get { return currentHealth; } }
    int currentHealth;

    //public float fuel = 3;
    //public float fuelconsumption = 1f;

    public ClickScript[] ControlCar;

    void Start()
    {
        currentHealth = 100;
        wheelJoints = gameObject.GetComponents<WheelJoint2D>();
        backWheel = wheelJoints[1].motor;
        frontWheel = wheelJoints[0].motor;
        if (PlayerPrefs.HasKey("maxSpeedUpgrade")) {
            maxSpeedUpgrade = PlayerPrefs.GetFloat("maxSpeedUpgrade");
        }
        if (PlayerPrefs.HasKey("accelerationUpgrade")) {
            accelerationUpgrade = PlayerPrefs.GetFloat("accelerationUpgrade");
        }
        if (PlayerPrefs.HasKey("tireUpgrade")) {
            tireUpgrade = PlayerPrefs.GetFloat("tireUpgrade");
            accelerationUpgrade += tireUpgrade % 1;
        }
        if (PlayerPrefs.HasKey("chassisUpgrade")) {
            chassisUpgrade = PlayerPrefs.GetFloat("chassisUpgrade");
        }
        if (PlayerPrefs.HasKey("fuelUpgrade")) {
            fuelUpgrade = PlayerPrefs.GetFloat("fuelUpgrade");
        }
    }

    void FixedUpdate()
    {
        frontWheel.motorSpeed = backWheel.motorSpeed;
        angleCar = transform.localEulerAngles.z;

        if (angleCar >= 180)
        {
            angleCar = angleCar - 360;
        }

        if (ControlCar[0].clickedIs == true)
        {
            backWheel.motorSpeed = Mathf.Clamp(backWheel.motorSpeed - (acceleration * accelerationUpgrade - gravity * Mathf.PI * (angleCar / 180) * 100) * Time.deltaTime, maxSpeed * maxSpeedUpgrade, maxBackSpeed);
        }
        else if ((backWheel.motorSpeed < 0) || (ControlCar[0].clickedIs == false && backWheel.motorSpeed == 0 && angleCar < 0))
        {
            backWheel.motorSpeed = Mathf.Clamp(backWheel.motorSpeed - (deacceleration - gravity * Mathf.PI * (angleCar / 180) * 100) * Time.deltaTime, maxSpeed * maxSpeedUpgrade, 0);
        }
        if ((ControlCar[0].clickedIs == false && backWheel.motorSpeed > 0) || (ControlCar[0].clickedIs == false && backWheel.motorSpeed == 0 && angleCar > 0))
        {
            backWheel.motorSpeed = Mathf.Clamp(backWheel.motorSpeed - (-deacceleration - gravity * Mathf.PI * (angleCar / 180) * 100) * Time.deltaTime, 0, maxBackSpeed);
        }
        else if (ControlCar[0].clickedIs == false && backWheel.motorSpeed < 0)
        {
            backWheel.motorSpeed = Mathf.Clamp(backWheel.motorSpeed - deacceleration * Time.deltaTime, maxSpeed * maxSpeedUpgrade, 0);
        }
        else if (ControlCar[0].clickedIs == false && backWheel.motorSpeed > 0)
        {
            backWheel.motorSpeed = Mathf.Clamp(backWheel.motorSpeed + deacceleration * Time.deltaTime, 0, maxBackSpeed);
        }

        if (ControlCar[1].clickedIs == true && backWheel.motorSpeed >= 0)
        {
            backWheel.motorSpeed = Mathf.Clamp(backWheel.motorSpeed + brakeForce * 0.1f * Time.deltaTime, 0, maxBackSpeed);
        }
        else if (ControlCar[1].clickedIs == true && backWheel.motorSpeed < 0)
        {
            backWheel.motorSpeed = Mathf.Clamp(backWheel.motorSpeed + brakeForce * Time.deltaTime, maxSpeed * maxSpeedUpgrade, 0);
        }

        wheelJoints[1].motor = backWheel;
        wheelJoints[0].motor = frontWheel;

        //fuel -= fuelconsumption * Mathf.Abs(movement) * Time.fixedDeltaTime;
    }

    public void ChangeHealth(int amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        HealthBar.instance.SetValue(currentHealth / (float)maxHealth);
        Debug.Log(currentHealth + "/" + maxHealth);
    }

}   