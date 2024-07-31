using Fusion;
using ResourceEnum;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class KYH_Test : NetworkBehaviour
{
    public TextMeshProUGUI nameText;
    [Networked] string _name { get; set; }

    private void Awake()
    {
        nameText = GetComponentInChildren<TextMeshProUGUI>();
        nameText.text = string.Empty;
    }
    private ChangeDetector _changeDetector;

    public override void Spawned()
    {
        _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
    }

    public void whatyourname()
    {
        Debug.Log(_name);
        if (_name != string.Empty) return;
        if(HasStateAuthority)
        {
            var spawndCharacter = FindAnyObjectByType<NetworkPhotonCallbacks>().SpawnedCharacter;

            List<PlayerRef> activePlayers = Runner.ActivePlayers.ToList();

            foreach (PlayerRef player in activePlayers)
            {
                // 로컬 플레이어를 제외한 다른 플레이어들의 PlayerRef 출력
                if(transform.parent.GetComponent<NetworkObject>() == spawndCharacter.TryGetValue(player, out NetworkObject value))
                {
                    _name = player.ToString();
                }
            }

        }

            
    }

    public override void Render()
    {
        foreach (var change in _changeDetector.DetectChanges(this))
        {
            switch (change)
            {
                case nameof(_name):
                    nameText.text = _name;
                    break;
            }
        }
    }
}
