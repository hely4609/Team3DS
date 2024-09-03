using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Fusion;

public class PowerSupply : InteractableBuilding
{
    [SerializeField] protected Animator anim;

    protected TextMeshProUGUI levelText;
    protected TextMeshProUGUI expText;
    protected TextMeshProUGUI powerText;

    protected Image expFillImage;
    protected Image powerFillImage;

    protected int level = 1;
    protected int expMax = 2;
    protected int expCurrent = 0;

    protected int powerMax = 100;
    protected int powerCurrent = 100;

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
    //[SerializeField] protected int ExpCurrent; // ���� ��Ƽ� Ore�� ������ .. ������..
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
        powerText = GameObject.FindGameObjectWithTag("PowerText").GetComponent<TextMeshProUGUI>();

        expFillImage = GameObject.FindGameObjectWithTag("ExpFillImage").GetComponent<Image>();
        powerFillImage = GameObject.FindGameObjectWithTag("PowerFillImage").GetComponent<Image>();

        anim = GetComponentInChildren<Animator>();
    }

    protected override void MyStart()
    {
        levelText.text = $"Lv.{Level}";

        expText.text = $"{ExpCurrent} / {ExpMax}";
        expFillImage.fillAmount = ExpCurrent / (float)ExpMax;

        powerText.text = $"{PowerCurrent} / {PowerMax}";
        powerFillImage.fillAmount = PowerCurrent / (float)PowerMax;

    }

    public override Interaction InteractionStart(Player player)
    {
        if (player?.OreAmount != 0) // �÷��̾ ���� ������ ��ǰ�� �����̶��
        {
            Deliver(player);
            
            return Interaction.Deliver;
        }
        else
        {
            return Interaction.None;
        }
    }

    //public bool Interaction(GameObject target) // ��ǰ �ޱ�
    //{
    //if (target.TryGetComponent<Player>(out Player player)) // ��ȣ�ۿ��Ѱ��� �÷��̾��ΰ�?
    //{
    //// �׷��ٸ� bePicked�� �ֳ�?
    //// �װ��� ��ǰ�ϴ� �����ΰ�?
    //// ��ǰ�� ������ ���� 0���� ū��?
    //Debug.Log($"{gameObject.name}�� ��ǰ ��ȣ�ۿ��� �Ͽ����ϴ�.");
    //return true;
    //}

    //return false;
    //}
    public void Deliver(Player player)
    {
        anim.SetTrigger("DoorTrigger");

        for (int i = 0; i < player?.OreAmount; i++)
        {
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
                    break;
                case nameof(ExpCurrent):
                    expText.text = $"{ExpCurrent} / {ExpMax}";
                    expFillImage.fillAmount = ExpCurrent / (float)ExpMax;
                    break;
                case nameof(PowerCurrent):
                    powerText.text = $"{PowerCurrent} / {PowerMax}";
                    powerFillImage.fillAmount = PowerCurrent / (float)PowerMax;
                    break;
                    
            }
        }
    }
}
