using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum DamageType
{
    PROJECTILE,
    LASER,
    POISON
}

public class Player : MonoBehaviour
{
    private SpriteRenderer SR;
    private Sprite S;

    public bl_Joystick Movement;
    public bl_Joystick Ability;
    private Control control;
    private Rigidbody2D RB;

    public GameObject Ability_Direction_Pivot;
    public GameObject Ability_Direction_Marker;

    public GameObject Fire;
    public float FireCooldown = 1.0f;
    public float FireRemaining = 0.0f;
    public float FireSpecialIncrease = 10.0f;
    public DateTime FireReset = default;
    public GameObject Laser;
    public LineRenderer LaserLine;
    public float LaserSpecialIncrease = 5.0f;
    public float LaserDrainSpeed = 15.0f;
    public GameObject Poison;
    public float PoisonCooldown = 3f;
    public float PoisonRemaining = 0.0f;
    public float PoisonSpecialIncrease = 25.0f;
    public DateTime PoisonReset = default;
    public Transform Projectile_Spawn;
    public Transform Projectile_World;
    private bool SpecialReady = false;
    private bool SpecialDrained = true;

    public UIController UI;

    [Header("User Controls")]
    public float Horizontal = 0.0f;
    public float Vertical = 0.0f;
    public bool IsActive = false;

    public float Max_Speed = 1.0f;

    [Header("Camera Follow")]
    public GameObject Follow_Object;
    public float Follow_Speed = 5.0f;
    public Transform MainCamera;
    public Vector3 Follow_Target;

    // Start is called before the first frame update
    void Start()
    {
        SR = GetComponent<SpriteRenderer>();
        S = SR.sprite;
        control = new Control(Movement, Ability);
        RB = GetComponent<Rigidbody2D>();
        Ability.StickFree = true;
        MainCamera = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        // Update the control system
        control.Update();

        // Update the controls to be displayed in the editor
        Horizontal = control.A_Horizontal;
        Vertical = control.A_Vertical;
        IsActive = control.StickFree;

        if (Follow_Object != null)
        {
            if (control.M_StickFree)
            {
                Follow_Target = gameObject.transform.position;
            }
            else
            {
                Follow_Target = Follow_Object.transform.position;
            }
            Follow_Target = new Vector3(Follow_Target.x, Follow_Target.y, MainCamera.transform.position.z);
            MainCamera.position = Vector3.MoveTowards(MainCamera.position, Follow_Target, Follow_Speed * Time.deltaTime);
        }

        // Keep the ability directional marker on the players position
        Ability_Direction_Pivot.transform.position = transform.position;
        // Check if either of the abilities with a cooldown is currently on cooldown and alter the UI element based on the time remaining
        if (DateTime.Now < FireReset)
        {
            // Get the remaining amount of seconds
            FireRemaining = (float)(FireReset - DateTime.Now).TotalSeconds;
            // Set the Filled icon to the percentage between the remaining time and the total cooldown time
            UI.ability1icon.fillAmount = (FireCooldown - FireRemaining) / FireCooldown;
        }
        else
        {
            UI.ability1icon.fillAmount = 1.0f;
        }
        if (DateTime.Now < PoisonReset)
        {
            // Get the remaining amount of seconds
            PoisonRemaining = (float)(PoisonReset - DateTime.Now).TotalSeconds;
            // Set the Filled icon to the percentage between the remaining time and the total cooldown time
            UI.ability3icon.fillAmount = (PoisonCooldown - PoisonRemaining) / PoisonCooldown;
        }
        else
        {
            UI.ability3icon.fillAmount = 1.0f;
        }

        // Update player position based on new direction if the movement joystick is being used
        if (control.Movement_Direction != Vector2.zero)
        {
            transform.rotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, control.Movement_Angle));
            RB.AddForce(control.Movement_Direction * PlayerPrefs.GetFloat("Speed"));
        }

        // Check if the player has activated the selected ability
        if (!control.StickFree)
        {
            // Display the Ability Marker to show direction of ability
            Ability_Direction_Marker.SetActive(true);

            // And set its rotation to be the direction of the movement stick
            Ability_Direction_Pivot.transform.rotation = Quaternion.Euler(new Vector3(0.0f, 1.0f, control.Ability_Angle));

            // If the laser is selected, set it to active seeing as the thumbstick is down
            if (UI.Ability == SelectedAbility.Laser)
            {
                // Active the LASERRRRR
                Laser.SetActive(true);

                // If the laser is selected, notify the UIController that the special needs to be increased, also get weather its ready
                if (!SpecialReady && SpecialDrained)
                    SpecialReady = UI.IncreaseSpecial(LaserSpecialIncrease * Time.deltaTime, true);
                else
                {
                    SpecialDrained = UI.DrainSpecial(LaserDrainSpeed);
                    SpecialReady = !SpecialDrained;
                }

                // If the Special is ready then alter the size
                if (SpecialReady && !SpecialDrained)
                {
                    LaserLine.startWidth = 1.0f;
                    LaserLine.endWidth = 1.0f;
                    Laser.transform.localScale = new Vector3(12.4f, 12.4f, 12.4f);
                }
                else
                {
                    LaserLine.startWidth = 0.25f;
                    LaserLine.endWidth = 0.25f;
                    Laser.transform.localScale = new Vector3(6.2272f, 6.2272f, 6.2272f);
                }
            }
        }
        else
        {
            // Ensure that the laser isn't active
            Laser.SetActive(false);

            // Hide the ability marker
            Ability_Direction_Marker.SetActive(false);
        }
        // If the stick has been released and the ability is ready to fire then fire
        if ((control.Fire && control.StickFree) || (!control.StickFree && PlayerPrefs.GetInt("AutoFire") == 1))
        {
            GameObject temp;
            // Instanstiate new projectile/turn particle system on
            if (UI.Ability == SelectedAbility.Fire && DateTime.Now >= FireReset)
            {
                // Spawn the projectile
                temp = Instantiate(Fire, Projectile_Spawn.transform);
                if (SpecialReady)
                {
                    temp.transform.localScale *= 5.0f;
                    ParticleSystem.LightsModule light = temp.transform.GetChild(0).GetComponent<ParticleSystem>().lights;
                    light.rangeMultiplier = 3.0f;
                    UI.ResetSpecial();
                }
                // Set the reset time
                FireReset = DateTime.Now.AddSeconds(FireCooldown);
                // Increase the special and find weather it's ready
                if (!SpecialReady)
                    SpecialReady = UI.IncreaseSpecial(FireSpecialIncrease);
                else
                    SpecialReady = false;
            }
            else if (UI.Ability == SelectedAbility.Poison && DateTime.Now >= PoisonReset)
            {
                // Spawn the projectile
                temp = Instantiate(Poison, Projectile_Spawn.transform);
                if (SpecialReady)
                {
                    temp.transform.localScale *= 5.0f;
                    ParticleSystem.LightsModule light = temp.transform.GetChild(0).GetComponent<ParticleSystem>().lights;
                    light.rangeMultiplier = 3.0f;
                    UI.ResetSpecial();
                }
                // Set the reset time
                PoisonReset = DateTime.Now.AddSeconds(PoisonCooldown);
                // Increase the special and find weather it's ready
                if (!SpecialReady)
                    SpecialReady = UI.IncreaseSpecial(PoisonSpecialIncrease);
                else
                    SpecialReady = false;
            }
        }
    }

    public void TakeDamage(DamageType type)
    {
        switch (type)
        {
            case DamageType.PROJECTILE:

                break;
            case DamageType.LASER:
                break;
            case DamageType.POISON:
                break;
        }
    }
}
public class Control
{
    // Movement Variables
    private float M_Horizontal = 0.0f;
    private float M_Vertical = 0.0f;
    private Vector2 M_Direction = new Vector2();
    public Vector2 Movement_Direction
    {
        get
        {
            return M_Direction;
        }
    }
    private float M_Angle = 0.0f;
    public float Movement_Angle
    {
        get
        {
            return M_Angle;
        }
    }
    public bool Movement_Flip = true;
    private bl_Joystick Movement;
    public float Movement_Average
    {
        get
        {
            return (Mathf.Abs(M_Direction.x) + Mathf.Abs(M_Direction.y)) / 2.0f;
        }
    }

    // Movement Touch Checks
    public bool M_StickFree
    {
        get
        {
            return Movement.StickFree;
        }
    }

    // Ability Variables
    public float A_Horizontal = 0.0f;
    public float A_Vertical = 0.0f;
    private Vector2 A_Direction = new Vector2();
    public Vector2 Ability_Direction
    {
        get
        {
            return A_Direction;
        }
    }
    private float A_Angle = 0.0f;
    public float Ability_Angle
    {
        get
        {
            return A_Angle;
        }
    }
    public bool Ability_Flip = true;
    private bl_Joystick Ability;

    // Ability Touch Checks
    public bool IsDown = false;
    public bool StickFree
    {
        get
        {
            return Ability.StickFree;
        }
    }
    private bool fire = false;
    public bool Fire
    {
        get
        {
            if (fire)
            {
                fire = false;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    
    public bool Active
    {
        get
        {
            return (Mathf.Abs(A_Horizontal) > MIN_STICK_MOVEMENT || Mathf.Abs(A_Vertical) > MIN_STICK_MOVEMENT);
        }
    }

    private const float MIN_STICK_MOVEMENT = 0.01f;

    public Control(bl_Joystick movement, bl_Joystick ability)
    {
        Movement = movement;
        Ability = ability;
    }
    public void Update()
    {
        if (Movement != null)
        {
            M_Horizontal = Movement.Horizontal;
            M_Vertical = Movement.Vertical;
            M_Direction = new Vector2(M_Horizontal, M_Vertical);
            M_Angle = Mathf.Atan2(M_Horizontal, M_Vertical) * Mathf.Rad2Deg;
            if (Movement_Flip) M_Angle = -M_Angle;
        }
        if (Ability != null)
        {
            A_Horizontal = Ability.Horizontal;
            A_Vertical = Ability.Vertical;
            A_Direction = new Vector2(A_Horizontal, A_Vertical);
            A_Angle = Mathf.Atan2(A_Horizontal, A_Vertical) * Mathf.Rad2Deg;
            if (Ability_Flip) A_Angle = -A_Angle;
            if (StickFree && IsDown)
            {
                IsDown = false;
                fire = true;
            }
            else if (!StickFree && !IsDown)
            {
                IsDown = true;
            }
        }
    }
}
