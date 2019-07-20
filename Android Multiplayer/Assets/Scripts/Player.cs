using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private SpriteRenderer SR;
    private Sprite S;

    public bl_Joystick Movement;
    public bl_Joystick Ability;
    private Control control;
    public Rigidbody2D RB;
    private CircleCollider2D CC;

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
    public float Ability_Follow_Speed = 5.0f;
    public Transform MainCamera;
    public Vector3 Follow_Target;

    public GameObject Ability_Follow_Object;

    [Header("Teleporting")]
    public GameObject Teleport_Parent;
    public GameObject Teleport_Target;
    public bool Parent_Colliding = false;
    public DateTime TeleportReset = default;
    public float TeleportCooldown = 15.0f;
    public float TeleportRemaining = 0.0f;
    public ParticlePlay TeleportStart;
    public float CameraCatchup = 1.0f;
    public bool Teleporting = false;

    // Start is called before the first frame update
    void Start()
    {
        SR = GetComponent<SpriteRenderer>();
        S = SR.sprite;
        control = new Control(Movement, Ability);
        RB = GetComponent<Rigidbody2D>();
        Ability.StickFree = true;
        MainCamera = Camera.main.transform;
        CC = GetComponent<CircleCollider2D>();
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
        if (DateTime.Now < TeleportReset)
        {
            // Get the remaining amount of seconds
            TeleportRemaining = (float)(TeleportReset - DateTime.Now).TotalSeconds;
            // Set the Filled icon to the percentage between the remaining time and the total cooldown time
            UI.teleport.fillAmount = (TeleportCooldown - TeleportRemaining) / TeleportCooldown;
        }
        else
        {
            UI.teleport.fillAmount = 1.0f;
        }

        // Update the camera catchup rate
        if (CameraCatchup > 1.0f)
        {
            CameraCatchup -= 1.0f * Time.deltaTime;
        }
        else
        {
            CameraCatchup = 1.0f;
        }

        // Only move or rotate if the player isn't teleporting
        if (!Teleporting)
        {
            // Update player position based on new direction if the movement joystick is being used
            if (control.Movement_Direction != Vector2.zero)
            {
                transform.rotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, control.Movement_Angle));
                RB.AddForce(control.Movement_Direction * PlayerPrefs.GetFloat("Speed"));
            }
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

    private void FixedUpdate()
    {
        #region Camera Follow
        if (Follow_Object != null)
        {
            // Set the camera follow speeds to be equal to the players velocity magnitude in order to avoid the player getting ahead of the camera
            Follow_Speed = (CameraCatchup > 1.0f) ? CameraCatchup + RB.velocity.magnitude : RB.velocity.magnitude;

            // If following has been enabled then set the location to follow to be the player to begin with
            Follow_Target = gameObject.transform.position;

            if (Ability_Follow_Object == null) // If the ability follow object is null avoid using it
            {
                if (!control.M_StickFree)
                {
                    Follow_Target = Follow_Object.transform.position;
                }
            }
            else // Otherwise take into consideration both sticks and both follow objects
            {
                if (!control.StickFree)
                {
                    // Add the default ability follow speed onto the follow speed variable if the ability control is being used
                    Follow_Speed += Ability_Follow_Speed;
                }
                if (!control.M_StickFree && control.StickFree)      // If the movement stick is being used and the ability stick is free set the follow position to be the movement follow object
                {
                    Follow_Target = Follow_Object.transform.position;
                }
                else if (control.M_StickFree && !control.StickFree) // If the movement stick is free and the ability stick is being used set the follow position to be the ability follow object
                {
                    Follow_Target = Ability_Follow_Object.transform.position;
                }
                else if (!control.M_StickFree && !control.StickFree) // If both of the sticks are being used, get the centre point between both follow objects
                {
                    Follow_Target = Follow_Object.transform.position + (Ability_Follow_Object.transform.position - Follow_Object.transform.position) / 2;
                }
            }

            // Set the z position to be that of the cameras to avoid the camera moving forward towards the objects
            Follow_Target = new Vector3(Follow_Target.x, Follow_Target.y, MainCamera.transform.position.z);

            // Update the camera position based on its current position, the follow target and the follow speed
            MainCamera.position = Vector3.MoveTowards(MainCamera.position, Follow_Target, Follow_Speed * Time.deltaTime);
        }
        #endregion

        #region Teleport Indicator Follow
        // First check if teleport parent isn't colliding
        if (!Parent_Colliding)
        {
            Teleport_Target.transform.position = Teleport_Parent.transform.position;
        }
        else
        {
            // Setup the raycasting
            List<RaycastHit2D> hits = new List<RaycastHit2D>();
            ContactFilter2D filter = new ContactFilter2D();
            filter.layerMask = ~LayerMask.GetMask("Players");
            filter.useLayerMask = true;
            Vector2 Origin = new Vector2(transform.position.x, transform.position.y);
            Vector2 Dest = new Vector2(Teleport_Parent.transform.position.x, Teleport_Parent.transform.position.y);
            Vector2 Dir = (Origin - Dest).normalized;
            Dest += (-Dir * CC.radius * transform.localScale * 2);
            int count = 0;

            // Raycast from the mid-point between the player and the parent to see if there is a collision point we can find
            Vector2 MidPoint = Origin + (Dest - Origin) / 2;
            count = Physics2D.Raycast(Origin, MidPoint - Origin, filter, hits, Vector2.Distance(MidPoint, Dest));
            Debug.DrawRay(MidPoint, Dest - MidPoint);
            if (count > 0)
            {
                Debug.Log("Detected hit from MidPoint with " + hits[0].transform.name);
                Teleport_Target.transform.position = (hits[0].point + (Dir * CC.radius * transform.localScale * 2));
            }
            else
            {
                // The ray might be coming from inside, therefore check the mid-point between the current mid-point and destination
                MidPoint = MidPoint + (Dest - MidPoint) / 2;
                count = Physics2D.Raycast(Origin, MidPoint - Origin, filter, hits, Vector2.Distance(MidPoint, Dest));
                Debug.DrawRay(MidPoint, Dest - MidPoint);
                if (count > 0)
                {
                    Debug.Log("Detected hit from second MidPoint with " + hits[0].transform.name);
                    Teleport_Target.transform.position = (hits[0].point + (Dir * CC.radius * transform.localScale * 2));
                }
                else
                {
                    // Use a raycast from the player position to the destination
                    count = Physics2D.Raycast(Origin, Dest - Origin, filter, hits, Vector2.Distance(Dest, Origin));
                    Debug.DrawRay(Origin, Dest - Origin);
                    if (count > 0)
                    {
                        Debug.Log("Detected hit from player position with " + hits[0].transform.name);
                        Teleport_Target.transform.position = (hits[0].point + (Dir * CC.radius * transform.localScale * 2));
                    }
                    else
                    {
                        // There were no collisions on any of the casts, therefore just use the parent position regardless
                        Teleport_Target.transform.position = Teleport_Parent.transform.position;
                    }
                }
            }
        }
        #endregion
    }
    //public void MoveToClosest(Vector2 pos, Collider2D c)
    //{
    //    Debug.Log("MvoeToClosest: " + c.name);
    //    Debug.Log(Teleport_Target.transform.position + " " + Physics2D. .ClosestPoint(pos, c));
    //    Teleport_Target.transform.position = Physics2D.ClosestPoint(pos, c);
    //    Test.transform.position = Physics2D.ClosestPoint(pos, c);
    //}

    public void TakeDamage(SpellType type)
    {
        switch (type)
        {
            case SpellType.Fire:

                break;
            case SpellType.Arcane:
                break;
            case SpellType.Poison:
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

public class Sort : IComparer<Hit>
{
    int IComparer<Hit>.Compare(Hit x, Hit y)
    {
        return x.CompareTo(y);
    }
}
public class Hit
{
    public RaycastHit2D Hit_;
    public float Distance;
    public Hit(RaycastHit2D hit, float distance)
    {
        Hit_ = hit;
        Distance = distance;
    }
    internal int CompareTo(Hit y)
    {
        if (Distance > y.Distance)
            return 1;
        else if (Distance < y.Distance)
            return -1;
        else
            return 0;
    }
}
