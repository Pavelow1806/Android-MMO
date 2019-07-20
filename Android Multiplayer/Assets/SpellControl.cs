using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum Target
{
    Player,
    Enemy,
    Both
}
public enum SpellType
{
    Fire,
    Arcane,
    Poison
}
public class SpellControl : MonoBehaviour
{
    // Special
    private float Special_Damage_Multiplier = 2.0f;
    private float Special_Max = 100.0f;

    Talents talents = new Talents();

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
public class Talents
{
    private const int MIN_LEVEL = 1;
    private const int MAX_LEVEL = 6;

    public TalentTree Fire = new TalentTree();
    public TalentTree Arcance = new TalentTree();
    public TalentTree Poison = new TalentTree();

    public Talents()
    {

    }
}
public class TalentTree
{
    TreeType Type;
    int Points_Spent;

}
public class Talent
{

    public virtual void Apply(Spell spell)
    {

    }
}
public class FireTalent : Talent
{

}
public class ArcaneTalent : Talent
{

}
public class PoisonTalent : Talent
{

}

public struct Instanstiation
{
    public Transform Spawn_Parent;
    public Transform Parent;
    public Vector3 Spawn_Offset;
    public Quaternion Direction;
    public float Scale;
    public Target target;
    public SpellType spellType;
    public GameObject Prefab;
    public Instanstiation
        (
            Transform Spawn_Parent_, 
            Transform Parent_,
            Vector3 Spawn_Offset_, 
            Quaternion Direction_,
            float Scale_,
            Target target_,
            SpellType spellType_,
            GameObject Prefab_
        )
    {
        Spawn_Parent = Spawn_Parent_;
        Parent = Parent_;
        Spawn_Offset = Spawn_Offset_;
        Direction = Direction_;
        Scale = Scale_;
        target = target_;
        spellType = spellType_;
        Prefab = Prefab_;
    }
}
public class Spell
{
    // Instanstiation variables
    public List<Instanstiation> Instanstiations = new List<Instanstiation>();
    public List<GameObject> Spawned_Spells;

    // Damage variables
    private float Damage_ = 1.0f;
    public virtual float Damage
    {
        get { return Damage_; }
        set { Damage_ = value; }
    }
    private float Critical_Chance_ = 0.1f;
    public virtual float Critical_Chance
    {
        get { return Critical_Chance_; }
        set { Critical_Chance_ = value; }
    }
    private float Critical_Damage_ = 2.0f;
    public virtual float CriticalDamage
    {
        get { return Critical_Chance_; }
        set { Critical_Chance_ = value; }
    }
    private float Cooldown_ = 0.0f;
    public virtual float Cooldown
    {
        get { return Cooldown_; }
        set { Cooldown_ = value; }
    }
    // Special
    private float Special_Increment_ = 10.0f;
    public virtual float Special_Increment
    {
        get { return Special_Increment_; }
        set { Special_Increment_ = value; }
    }
    public bool Special_Drain = false;
    private float Special_Drain_Per_Second_ = 0.0f;
    public virtual float Special_Drain_Per_Second
    {
        get { return Special_Drain_Per_Second_; }
        set { Special_Drain_Per_Second_ = value; }
    }
    public float Special_Scale = 5.0f;
    
    public Spell(List<Instanstiation> instanstiations)
    {
        Instanstiations = instanstiations;
        Spawned_Spells.Clear();
    }
    public virtual void Cast(float Special_Scale = 1.0f)
    {
        for (int i = 0; i < Instanstiations.Count; i++)
        {
            Spawned_Spells.Add(GameObject.Instantiate(Instanstiations[i].Prefab, Instanstiations[i].Spawn_Parent));
            GameObject temp = Spawned_Spells[Spawned_Spells.Count - 1];
            if (Instanstiations[i].Parent != null) temp.transform.SetParent(Instanstiations[i].Parent);
            temp.transform.position += Instanstiations[i].Spawn_Offset;
            temp.transform.rotation = Instanstiations[i].Direction;
            temp.transform.localScale *= Instanstiations[i].Scale;
            if (Special_Scale != 1.0f) temp.transform.localScale *= Special_Scale;
        }
    }
}
public class Fire : Spell
{
    public override float Damage { get => base.Damage; set => base.Damage = value; }
    public override float Critical_Chance { get => base.Critical_Chance; set => base.Critical_Chance = value; }
    public override float CriticalDamage { get => base.CriticalDamage; set => base.CriticalDamage = value; }
    public override float Special_Increment { get => base.Special_Increment; set => base.Special_Increment = value; }

    public Vector3 Velocity = new Vector3();

    public Fire(List<Instanstiation> instanstiations) : base (instanstiations)
    {

    }
    public override void Cast(float Special_Scale = 1.0f)
    {
        base.Cast(Special_Scale);
    }
}
public class Arcane : Spell
{
    public override float Damage { get => base.Damage; set => base.Damage = value; }
    public override float Critical_Chance { get => base.Critical_Chance; set => base.Critical_Chance = value; }
    public override float CriticalDamage { get => base.CriticalDamage; set => base.CriticalDamage = value; }
    public override float Special_Increment { get => base.Special_Increment; set => base.Special_Increment = value; }
    public override float Special_Drain_Per_Second { get => base.Special_Drain_Per_Second; set => base.Special_Drain_Per_Second = value; }

    public float Seconds_Between_Damage = 0.25f;

    public Arcane(List<Instanstiation> instanstiations) : base(instanstiations)
    {

    }
    public override void Cast(float Special_Scale = 1.0f)
    {
        base.Cast(Special_Scale);
    }
}
public class Poison : Spell
{
    public override float Damage { get => base.Damage; set => base.Damage = value; }
    public override float Critical_Chance { get => base.Critical_Chance; set => base.Critical_Chance = value; }
    public override float CriticalDamage { get => base.CriticalDamage; set => base.CriticalDamage = value; }
    public override float Special_Increment { get => base.Special_Increment; set => base.Special_Increment = value; }

    public Vector3 Velocity = new Vector3();

    public float Duration = 10.0f;
    public float Damage_Spread_Seconds = 1.0f;
    public Vector3 Linger_Position = new Vector3();
    public float Linger_Radius = 0.0f;
    public Vector3 Linger_Direction = new Vector3();
    public Vector3 Linger_Velocity = new Vector3();

    public Poison(List<Instanstiation> instanstiations) : base(instanstiations)
    {

    }
    public override void Cast(float Special_Scale = 1.0f)
    {
        base.Cast(Special_Scale);
    }
}
