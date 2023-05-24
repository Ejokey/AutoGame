﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CarScript : MonoBehaviour
{

    WheelJoint2D[] wheelJoints;
    JointMotor2D frontWheel;
    JointMotor2D backWheel;

    Image healthBar;
    Image fuelBar;

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
    private float health = 1f;

    private float fuel = 1f;
    private float fuelconsumption = 0.01f;

    public ClickScript[] ControlCar;

    void Start()
    {
        healthBar = GameObject.FindGameObjectWithTag("HpBar").GetComponent<Image>();
        fuelBar = GameObject.FindGameObjectWithTag("FuelBar").GetComponent<Image>();
        wheelJoints = gameObject.GetComponents<WheelJoint2D>();
        backWheel = wheelJoints[1].motor;
        frontWheel = wheelJoints[0].motor;
        if (PlayerPrefs.HasKey("maxSpeedUpgrade")) {
            maxSpeedUpgrade += 0.1f * PlayerPrefs.GetInt("maxSpeedUpgrade");
        }
        if (PlayerPrefs.HasKey("accelerationUpgrade")) {
            accelerationUpgrade += 0.04f * PlayerPrefs.GetInt("accelerationUpgrade");
        }
        if (PlayerPrefs.HasKey("tireUpgrade")) {
            tireUpgrade += 0.02f * PlayerPrefs.GetInt("tireUpgrade");
            accelerationUpgrade += tireUpgrade;
        }
        if (PlayerPrefs.HasKey("chassisUpgrade")) {
            chassisUpgrade -= 0.04f * PlayerPrefs.GetInt("chassisUpgrade");
        }
        if (PlayerPrefs.HasKey("fuelUpgrade")) {
            fuelUpgrade -= 0.07f * PlayerPrefs.GetInt("fuelUpgrade");
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
            fuel -= fuelconsumption * fuelUpgrade * Time.deltaTime;
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
            fuel -= fuelconsumption * fuelUpgrade * Time.deltaTime;
        }
        else if (ControlCar[1].clickedIs == true && backWheel.motorSpeed < 0)
        {
            backWheel.motorSpeed = Mathf.Clamp(backWheel.motorSpeed + brakeForce * Time.deltaTime, maxSpeed * maxSpeedUpgrade, 0);
        }

        wheelJoints[1].motor = backWheel;
        wheelJoints[0].motor = frontWheel;

        healthBar.fillAmount = health;
        fuelBar.fillAmount = fuel;
    }

    public void collectFuel() {
        if (fuel <= 0.7f) {
            fuel += 0.3f;
        } else {
            fuel = 1f;
        }
    }

    public void policeStop() {
        if (backWheel.motorSpeed <= -800f) {
            health -= 0.2f;
        }
    }
}   