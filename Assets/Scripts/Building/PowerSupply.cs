using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Fusion;
using static UnityEngine.Rendering.DebugUI;

public class PowerSupply : InteractableBuilding
{
    protected TextMeshProUGUI levelText;
    protected TextMeshProUGUI expText;
    protected TextMeshProUGUI powerText;

    protected Image expFillImage;
    protected Image powerFillImage;

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

        //levelText.text = "Lv.1";
        //expText.text = "0 / 2";
        //powerText.text = "0 / 10";

        //expFillImage.fillAmount = 0;
        //powerFillImage.fillAmount = 0;

        //PowerMax = 100;
        //PowerCurrent = PowerMax;
        //ExpCurrent = 0;
        //ExpMax = 2;
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
        if (player != null) // �÷��̾ ���� ������ ��ǰ�� �����̶��
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
        for (int i = 0; i < player?.OreAmount; i++)
        {
            ExpCurrent++;

            //expFillImage.fillAmount = ExpCurrent / (float)ExpMax;
            //expText.text = $"{ExpCurrent} / {ExpMax}";
        }
        ////if(player.BePicked != null)
        //{
        //    // if(player.BePicked == ���ǰ)
        //    {
        //        // expCurrent += ���ǰ.currentOre
        //        // ���ǰ�� ��Ȱ��ȭ.
        //        // player.bePicked = null
        //    }
        //}
    }

    public override void Render()
    {
        foreach (var chage in _changeDetector.DetectChanges(this))
        {
            switch (chage)
            {
                case nameof(ExpCurrent):
                    if (ExpCurrent >= ExpMax)
                    {
                        ExpCurrent = 0;
                        ExpMax *= 2;
                        Level++;

                        levelText.text = $"Lv.{Level}";
                    }
                    expText.text = $"{ExpCurrent} / {ExpMax}";
                    expFillImage.fillAmount = ExpCurrent / (float)ExpMax;

                    break;
                case nameof(PowerMax):
                case nameof(PowerCurrent):
                    powerText.text = $"{PowerCurrent} / {PowerMax}";
                    powerFillImage.fillAmount = PowerCurrent / (float)PowerMax;
                    break;
                    
            }
        }
    }
}
