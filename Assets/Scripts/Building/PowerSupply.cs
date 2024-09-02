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
    [SerializeField] protected int expCurrent; // 몹을 잡아서 Ore를 넣으면 .. 레벨업..
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
        if (player != null) // 플레이어가 가진 물건이 납품할 물건이라면
        {
            Deliver(player);
            return Interaction.Deliver;
        }
        else
        {
            return Interaction.None;
        }
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
        for (int i = 0; i < player?.OreAmount; i++)
        {
            ExpCurrent++;

            expFillImage.fillAmount = expCurrent / (float)expMax;
            expText.text = $"{expCurrent} / {expMax}";
        }
        ////if(player.BePicked != null)
        //{
        //    // if(player.BePicked == 운반품)
        //    {
        //        // expCurrent += 운반품.currentOre
        //        // 운반품을 비활성화.
        //        // player.bePicked = null
        //    }
        //}
    }
}
