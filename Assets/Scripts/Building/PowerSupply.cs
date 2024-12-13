using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Fusion;

public class PowerSupply : InteractableBuilding
{
    [SerializeField] protected Animator anim;

    [SerializeField] protected Collider doorOpenTriggerCol;
    protected List<Player> doorNearPlayers = new List<Player>();


    protected TextMeshProUGUI levelText;
    protected TextMeshProUGUI expText;
    protected TextMeshProUGUI totalOreAmountText;
    protected TextMeshProUGUI powerText;
    protected TextMeshProUGUI levelUpEffect;

    protected Image expFillImage;
    protected Image powerFillImage;

    protected int level = 1;
    protected int expMax = 2;
    protected int expCurrent = 0;

    protected int powerMax = 100;
    protected int powerCurrent = 100;
    [Networked] public int TotalOreAmount { get; set; } = 100;

    [Networked] public int PowerMax
    {
        get => PowerMax; 
        set => PowerMax = value;
    }

    [Networked] public int PowerCurrent
    {
        get => PowerCurrent; 
        set => PowerCurrent = value;
    }

    [Networked, SerializeField] protected int Level { get; set; } = 1;
    //[SerializeField] protected int ExpCurrent; // 몹을 잡아서 Ore를 넣으면 .. 레벨업..
    [Networked, SerializeField] protected int ExpMax { get; set; }
    [Networked] public int ExpCurrent
    {
        get => ExpCurrent; 
        set => ExpCurrent = value;
    }

    protected override void Initialize()
    {
        GameManager.Instance.BuildingManager.supply = this;

        levelText = GameObject.FindGameObjectWithTag("LevelText").GetComponent<TextMeshProUGUI>();
        expText = GameObject.FindGameObjectWithTag("ExpText").GetComponent<TextMeshProUGUI>();
        totalOreAmountText = GameObject.FindGameObjectWithTag("TotalOreText").GetComponent<TextMeshProUGUI>();
        powerText = GameObject.FindGameObjectWithTag("PowerText").GetComponent<TextMeshProUGUI>();
        levelUpEffect = GameObject.FindGameObjectWithTag("LevelUpEffect").GetComponent<TextMeshProUGUI>();

        expFillImage = GameObject.FindGameObjectWithTag("ExpFillImage").GetComponent<Image>();
        powerFillImage = GameObject.FindGameObjectWithTag("PowerFillImage").GetComponent<Image>();

        anim = GetComponentInChildren<Animator>();

        objectName = "Power Supply";
        localeName = GameManager.Instance.LocaleManager.LocaleNameSet(objectName);
    }

    protected override void MyStart()
    {
        levelText.text = $"Lv.{Level}";

        expText.text = $"{ExpCurrent} / {ExpMax}";
        expFillImage.fillAmount = ExpCurrent / (float)ExpMax;

        totalOreAmountText.text = "x " + $"{TotalOreAmount}";

        powerText.text = $"{PowerCurrent} / {PowerMax}";
        powerFillImage.fillAmount = PowerCurrent / (float)PowerMax;

    }

    public override List<Interaction> GetInteractions(Player player)
    {
        List<Interaction> currentAbleInteractions = new List<Interaction>();

        currentAbleInteractions.Add(Interaction.Deliver);
            
        return currentAbleInteractions;
    }

    public override Interaction InteractionStart(Player player, Interaction interactionType)
    {
        switch (interactionType)
        {
            case Interaction.Deliver:
                if (player?.OreAmount != 0) Deliver(player);
                break;
        }

        return interactionType;
    }

    //public bool Interaction(GameObject target) // 납품 받기
    //{
    //if (target.TryGetComponent<Player>(out Player player)) // 상호작용한것이 플레이어인가?
    //{
    //// 그렇다면 bePicked가 있나?
    //// 그것이 납품하는 물건인가?
    //// 납품할 물건의 수가 0보다 큰가?
    //Debug.Log($"{gameObject.name}과 납품 상호작용을 하였습니다.");
    //return true;
    //}

    //return false;
    //}
    public void Deliver(Player player)
    {
        if(HasStateAuthority)
        anim.SetTrigger("DoorTrigger");

        for (int i = 0; i < player?.OreAmount; i++)
        {
            TotalOreAmount++;
            expCurrent++;

            if (expCurrent >= expMax)
            {
                expCurrent -= expMax;
                expMax *= 2;

                powerMax += 10;
                powerCurrent += 10;

                level++;
            }
        }

        Level = level;
       
        ExpCurrent = expCurrent;
        ExpMax = expMax;

        PowerMax = powerMax;
        PowerCurrent = powerCurrent;
    }

    public void ChangePowerConsumption(int value)
    {
        powerCurrent += value;

        PowerCurrent = powerCurrent;
    }

    public override void Render()
    {
        foreach (var chage in _changeDetector.DetectChanges(this))
        {
            switch (chage)
            {
                case nameof(Level):
                    levelText.text = $"Lv.{Level}";
                    levelUpEffect.GetComponent<Animator>().SetTrigger("LevelUp");
                    SoundManager.Play(ResourceEnum.SFX.Rise03, Camera.main.transform.position);
                    break;
                case nameof(ExpCurrent):
                    expText.text = $"{ExpCurrent} / {ExpMax}";
                    expFillImage.fillAmount = ExpCurrent / (float)ExpMax;
                    break;
                case nameof(PowerCurrent):
                    powerText.text = $"{PowerCurrent} / {PowerMax}";
                    powerFillImage.fillAmount = PowerCurrent / (float)PowerMax;
                    break;
                case nameof(TotalOreAmount):
                    totalOreAmountText.text = "x " + $"{TotalOreAmount}";
                    break;
                    
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (HasStateAuthority)
        {
            if (other.gameObject.TryGetComponent(out Player player))
            {
                doorNearPlayers.Add(player);
                if (doorNearPlayers.Count == 1)
                {
                    anim.SetBool("isDoorOpen", true);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other) 
    {
        if (HasStateAuthority)
        {
            if (other.gameObject.TryGetComponent(out Player player))
            {
                doorNearPlayers.Remove(player);
                if (doorNearPlayers.Count == 0)
                {

                    anim.SetBool("isDoorOpen", false);
                }
            }
        }
         
    }
}
