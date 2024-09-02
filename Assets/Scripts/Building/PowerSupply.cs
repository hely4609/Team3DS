using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PowerSupply : InteractableBuilding
{
    protected TextMeshProUGUI levelText;
    protected TextMeshProUGUI expText;
    protected TextMeshProUGUI powerText;

    protected Image expFillImage;
    protected Image powerFillImage;


    protected int powerMax;
    public int PowerMax
    {
        get { return powerMax; }
        set
        {
            powerMax = value;
            powerText.text = $"{powerCurrent} / {powerMax}";
            powerFillImage.fillAmount = powerCurrent / (float)powerMax;
        }
    }

    protected int powerCurrent;
    public int PowerCurrent
    {
        get { return powerCurrent; }
        set 
        { 
            powerCurrent = value;
            powerText.text = $"{powerCurrent} / {powerMax}";
            powerFillImage.fillAmount = powerCurrent / (float)powerMax;
        }
    }

    [SerializeField] protected int level =1;
    [SerializeField] protected int expCurrent; // ���� ��Ƽ� Ore�� ������ .. ������..
    [SerializeField] protected int expMax;
    public int ExpCurrent
    {
        get { return expCurrent; }
        set
        {
            expCurrent = value;
            if (expCurrent >= expMax)
            {
                expCurrent = 0;
                expMax *= 2;
                level++;

                levelText.text = $"Lv.{level}";
            }
        }
    }


    protected override void Initialize()
    {
        powerMax = 10;
        PowerCurrent = powerMax;
        expCurrent = 0;
        expMax = 2;

    }

    protected override void MyStart()
    {
        levelText = GameObject.FindGameObjectWithTag("LevelText").GetComponent<TextMeshProUGUI>();
        expText = GameObject.FindGameObjectWithTag("ExpText").GetComponent<TextMeshProUGUI>();
        powerText = GameObject.FindGameObjectWithTag("PowerText").GetComponent<TextMeshProUGUI>();

        expFillImage = GameObject.FindGameObjectWithTag("ExpFillImage").GetComponent<Image>();
        powerFillImage = GameObject.FindGameObjectWithTag("PowerFillImage").GetComponent<Image>();

        levelText.text = "Lv.1";
        expText.text = "0 / 2";
        powerText.text = "0 / 10";

        expFillImage.fillAmount = 0;
        powerFillImage.fillAmount = 0;
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

            expFillImage.fillAmount = expCurrent / (float)expMax;
            expText.text = $"{expCurrent} / {expMax}";
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
}
