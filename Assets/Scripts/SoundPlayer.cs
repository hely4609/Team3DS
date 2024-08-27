using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    public virtual void PlaySFX(int sfxEnum)
    {
        SoundManager.Play((ResourceEnum.SFX)sfxEnum, transform.position);
    }
}
