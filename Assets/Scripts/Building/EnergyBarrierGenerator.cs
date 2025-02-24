using Fusion;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnergyBarrierGenerator : InteractableBuilding
{
    protected int hpMax;
    [SerializeField, Networked] protected int hpCurrent { get; set; }
    [Networked, SerializeField] public float PlayTime { get; set; }
    [Networked, SerializeField] public int KillCount { get; set; }
    [Networked] public bool IsWaveStart { get; set; }
    [Networked] public int MonsterCount { get; set; } = 0; // ���� �ʵ忡 �ִ� ���� ��.
    [Networked] public bool IsWaveLeft { get; set; }
    [Networked] public int GameSpeed { get; set; } = 1;
    //[Networked] public float WaveLeftTime { get; set; }

    protected TextMeshProUGUI hpText;
    protected Image hpFillImage;
    public GameObject speedUp;
    protected Button speedNormal;
    protected Button speedFaster;
    protected TextMeshProUGUI currentSpeed;

    protected GameObject[] energyBarrierArray;

    protected bool onOff; // on/off�� �ö���� �������� ���� like �����ٸ�����Ʈ 
    public bool OnOff { get { return onOff; } }

    protected override void MyStart()
    {
        speedUp = GameObject.FindGameObjectWithTag("SpeedUp");
        speedNormal = speedUp.GetComponentsInChildren<Button>()[0];
        speedFaster = speedUp.GetComponentsInChildren<Button>()[1];
        currentSpeed = speedUp.GetComponentInChildren<TextMeshProUGUI>();

        speedNormal.onClick.AddListener(() => 
        { 
            GameSpeed = 1;
        });
        
        speedFaster.onClick.AddListener(() =>
        {
            GameSpeed = Mathf.Clamp(GameSpeed * 2, 1, 8);
        });

        currentSpeed.text = $"x{GameSpeed}";

        speedNormal.gameObject.SetActive(Runner.IsServer);
        speedFaster.gameObject.SetActive(Runner.IsServer);
        speedUp.SetActive(false);
    }

    protected override void MyUpdate(float deltaTime)
    {
        if (HasStateAuthority && IsWaveStart) PlayTime += Runner.DeltaTime * GameSpeed;
    }
    public void SetActiveEnergyBarrier()  // ���� On���� Off���� 
    {
        for (int i = 0; i < energyBarrierArray.Length; i++)
        {
            energyBarrierArray[i].SetActive(onOff);
        }
    }
    public void TakeDamage(int damage)
    {
        hpCurrent -= damage;
        hpText.text = $"{hpCurrent} / {hpMax}";
        hpFillImage.fillAmount = hpCurrent / (float)hpMax;
        //Debug.Log($"{HpCurrent} / {gameObject.name}");
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (!HasStateAuthority) return;
        if (collision.gameObject.TryGetComponent(out Monster enemy))
        {
            enemy.Attack(this);
        }
    }
    public void RepairBarrier()
    {
        hpCurrent += 1;
        if (hpCurrent >= hpMax)
        {
            hpCurrent = hpMax;
            onOff = true;
            SetActiveEnergyBarrier();
        }
    }

    protected override void Initialize()
    {
        onOff = true;
        hpMax = 3;
        if (HasStateAuthority) hpCurrent = hpMax;
        energyBarrierArray = GameObject.FindGameObjectsWithTag("EnergyBarrier");

        hpText = GameObject.FindGameObjectWithTag("HPText").GetComponent<TextMeshProUGUI>();
        hpFillImage = GameObject.FindGameObjectWithTag("HPFillImage").GetComponent<Image>();
        

        hpText.text = $"{hpCurrent} / {hpMax}";
        hpFillImage.fillAmount = hpCurrent / (float)hpMax;

        SetActiveEnergyBarrier();
    }

    public override Interaction InteractionStart(Player player, Interaction interactionType)
    {
        if (!onOff) // �������溮�� ���峵�ٸ�
        {
            return Interaction.Repair;
        }
        else
        {
            return Interaction.None;
        }
    }

    public override float InteractionUpdate(float deltaTime, Interaction interaction)
    {
        RepairBarrier();
        return default;
    }

    public override void Render()
    {

        foreach (var change in _changeDetector.DetectChanges(this))
        {
            switch (change)
            {
                case nameof(hpCurrent):
                    hpText.text = $"{hpCurrent} / {hpMax}";
                    hpFillImage.fillAmount = hpCurrent / (float)hpMax;
                    if (hpCurrent <= 0)
                    {
                        onOff = false;
                        GameManager.Instance.Defeat();
                        GameManager.Instance.GameOver();
                    }
                    break;
                case nameof(IsWaveStart):
                    GameManager.Instance.WaveManager.waveInfoUI.SetActive(true);
                    break;
                case nameof(GameSpeed):
                    currentSpeed.text = $"x{GameSpeed}";
                    break;

            }

        }
    }
}