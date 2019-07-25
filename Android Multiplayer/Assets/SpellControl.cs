using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
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

    XML xml = new XML();
    Talents talents = new Talents();
    private string path = "";

    private void Awake()
    {
        DontDestroyOnLoad(this);
        path = Path.Combine(Application.persistentDataPath, "Talents");
    }

    // Start is called before the first frame update
    void Start()
    {
        xml.Load(path);
        talents = xml.ExtractTalents();
        //talents.Fire.talents.Add(new FireTalent(1, 1));
        //talents.Fire.talents.Add(new FireTalent(2, 2));
        //talents.Fire.talents.Add(new FireTalent(3, 2));
        //talents.Fire.talents.Add(new FireTalent(4, 1));
        //talents.Fire.talents.Add(new FireTalent(5, 2));
        //talents.Fire.talents.Add(new FireTalent(6, 1));
        //xml.BuildTalents(talents.TalentTreeList);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool ActivateTalent(TreeType type, int row, int pos)
    {
        switch (type)
        {
            case TreeType.Fire:
                {
                    if (!talents.Fire.talents.ContainsKey(row))
                    {
                        if (talents.Fire.talents.ContainsKey(row - 1))
                        {
                            talents.Fire.talents.Add(row, new Talent(row, pos));
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    if (talents.Fire.talents[row].Selected == pos)
                    {
                        return false;
                    }
                    else
                    {
                        talents.Fire.talents[row].Selected = pos;
                        return true;
                    }
                }
            case TreeType.Arcane:
                {
                    if (!talents.Arcane.talents.ContainsKey(row))
                    {
                        if (talents.Arcane.talents.ContainsKey(row - 1))
                        {
                            talents.Arcane.talents.Add(row, new Talent(row, pos));
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    if (talents.Arcane.talents[row].Selected == pos)
                    {
                        return false;
                    }
                    else
                    {
                        talents.Arcane.talents[row].Selected = pos;
                        return true;
                    }
                }
            case TreeType.Poison:
                if (!talents.Poison.talents.ContainsKey(row))
                {
                    if (talents.Poison.talents.ContainsKey(row - 1))
                    {
                        talents.Poison.talents.Add(row, new Talent(row, pos));
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                if (talents.Poison.talents[row].Selected == pos)
                {
                    return false;
                }
                else
                {
                    talents.Poison.talents[row].Selected = pos;
                    return true;
                }
            default:
                return false;
        }
    }
    public Dictionary<int, Talent> GetTalents(TreeType type)
    {
        switch (type)
        {
            case TreeType.Fire:
                return talents.Fire.talents;
            case TreeType.Arcane:
                return talents.Arcane.talents;
            case TreeType.Poison:
                return talents.Poison.talents;
            default:
                return null;
        }
    }
}
public class Talents
{
    private const int MIN_LEVEL = 1;
    private const int MAX_LEVEL = 6;

    public List<TalentTree> TalentTreeList = new List<TalentTree>();
    public TalentTree Fire = new TalentTree(TreeType.Fire);
    public TalentTree Arcane = new TalentTree(TreeType.Arcane);
    public TalentTree Poison = new TalentTree(TreeType.Poison);

    public Talents()
    {
        TalentTreeList.Add(Fire);
        TalentTreeList.Add(Arcane);
        TalentTreeList.Add(Poison);
    }
}
public class TalentTree
{
    public TreeType Type;
    public int Points_Spent
    {
        get { return talents.Count; }
    }
    public Dictionary<int, Talent> talents = new Dictionary<int, Talent>();

    public TalentTree(TreeType type)
    {
        Type = type;
    }
    public TalentTree(TreeType type, Dictionary<int, Talent> talents_)
    {
        Type = type;
        talents = talents_;
    }
}
public class Talent
{
    public int Selected = 0;
    public int Tier = 0;
    public Talent() { }
    public Talent(int tier, int selected)
    {
        Selected = selected;
        Tier = tier;
        // Grab the asociated talent from the talent dataset
    }
    public virtual void Apply(Spell spell)
    {

    }
}
public class FireTalent : Talent
{

    public FireTalent(int tier, int selected) : base (tier, selected)
    {

    }
}
public class ArcaneTalent : Talent
{

    public ArcaneTalent(int tier, int selected) : base(tier, selected)
    {

    }
}
public class PoisonTalent : Talent
{

    public PoisonTalent(int tier, int selected) : base(tier, selected)
    {

    }
}

#region Spells
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
#endregion

public class XML
{
    private string XMLFolder = "";
    private string XMLPath = "";
    private const string XMLFileName = "TXML.xml";
    private bool Ready = false;

    private XDocument Document;

    public XML()
    {
    }
    public XML(string path)
    {
        Load(path);
    }
    public void Load(string path)
    {
        XMLFolder = path;
        XMLPath = Path.Combine(path, "TXML.xml");
        if (XMLPath == "" || XMLFolder == "") return;
        if (!Directory.Exists(XMLFolder)) Directory.CreateDirectory(XMLFolder);
        if (!File.Exists(XMLPath)) return;

        Document = XDocument.Load(XMLPath);

        Ready = true;
    }

    public Talents ExtractTalents()
    {
        if (!File.Exists(XMLPath))
        {
            Load(XMLPath);
            return new Talents();
        }

        Talents Result = new Talents();

        Document = XDocument.Load(XMLPath);

        List<TalentTree> Trees = (from xnode in Document.Element("Talents").Elements("Tree")
                                  select new TalentTree((TreeType)Enum.Parse(typeof(TreeType), xnode.Attribute("Name").Value))
                                  {
                                      talents = new Dictionary<int, Talent>()
                                  }).ToList();

        foreach (TalentTree tt in Trees)
        {
            tt.talents = (from xnode in Document.Element("Talents").Element("Tree").Elements("Talent")
                          select new Talent
                          {
                              Tier = Convert.ToInt32(xnode.Attribute("Tier").Value),
                              Selected = Convert.ToInt32(xnode.Value)
                          }).ToDictionary(item => item.Tier);
        }

        Result.TalentTreeList = Trees;

        foreach (TalentTree tt in Result.TalentTreeList)
        {
            switch (tt.Type)
            {
                case TreeType.Fire:
                    Result.Fire = tt;
                    break;
                case TreeType.Arcane:
                    Result.Arcane = tt;
                    break;
                case TreeType.Poison:
                    Result.Poison = tt;
                    break;
                default:
                    Debug.Log("Error: XML Imported Talent Tree: " + tt.Type + " not found.");
                    break;
            }
        }

        return Result;
    }
    public void BuildTalents(List<TalentTree> talentTrees)
    {
        if (XMLPath == "" || !Directory.Exists(XMLFolder)) return;

        // Build talent tree setup from parameter list                                
        List<XElement> elements = new List<XElement>();
        List<XElement> subelements = new List<XElement>();

        // Cycle through each talent tree
        foreach (TalentTree tt in talentTrees)
        {
            // Cycle through each talent within the tree, adding them to the subelements list
            foreach (KeyValuePair<int, Talent> t in tt.talents)
            {
                subelements.Add(new XElement("Talent", new XAttribute("Tier", t.Value.Tier), t.Value.Selected));
            }
            // Add the talent tree as well as the subelements list array
            elements.Add(new XElement("Tree", new XAttribute("Name", tt.Type.ToString()), new XAttribute("Points", tt.Points_Spent), subelements.ToArray()));
            // Clear the subelements list ready for the next tree
            subelements.Clear();
        }

        // Create XML save file
        Document =
            new XDocument(
                new XDeclaration("1.0", Encoding.UTF8.HeaderName, string.Empty),
                new XComment("XML Talent Save File"),
                new XElement("Talents",
                    elements.ToArray()
                )
            );
        Document.Save(XMLPath, SaveOptions.None);
    }
}
