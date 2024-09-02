using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Appearance : NetworkBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] Character owner;

    [Networked, SerializeField] bool IKActive { get; set; }
    [SerializeField] GameObject ikTarget = null;
    [SerializeField] Transform rightHand = null;
    [SerializeField] Transform leftHand = null;
    [SerializeField] Transform rightElbow = null;
    [SerializeField] Transform leftElbow = null;

    float ikWeight;

    private void OnEnable()
    {
        ConnectCharacter(GetComponentInParent<Character>());
    }
    private void OnDisable()
    {
        DisConnectCharacter();
    }

    public void ConnectCharacter(Character target)
    {
        if (owner && owner != target) DisConnectCharacter();
        if (!target) return;
        owner = target; //이 친구한테 연결되는 거구나!
        target.AnimBool -= SetBool;
        target.AnimFloat -= SetFloat;
        target.AnimInt -= SetInt;
        target.AnimTrigger -= SetTrigger;
        target.AnimIK -= SetIK;

        target.AnimBool += SetBool;
        target.AnimFloat += SetFloat;
        target.AnimInt += SetInt;
        target.AnimTrigger += SetTrigger;
        target.AnimIK += SetIK;
    }
    public void DisConnectCharacter()
    {
        if (owner == null) return;
        owner.AnimBool -= SetBool;
        owner.AnimFloat -= SetFloat;
        owner.AnimInt -= SetInt;
        owner.AnimTrigger -= SetTrigger;
        owner.AnimIK -= SetIK;
        owner = null;
    }
    public void SetBool(string name, bool value) => anim?.SetBool(name, value);
    public void SetFloat(string name, float value) => anim?.SetFloat(name, value);
    public void SetInt(string name, int value) => anim?.SetInteger(name, value);
    public void SetTrigger(string name) => anim?.SetTrigger(name);

    public void SetIK(bool value) => IKActive = value;
    
    //a callback for calculating IK
    void OnAnimatorIK(int _layerIndex)
    {
        if (anim)
        {
            //if (_layerIndex != anim.GetLayerIndex("IKLayer")) return;

            ikWeight = Mathf.Lerp(ikWeight, IKActive ? 1f : 0f, 0.1f);
            anim.SetLayerWeight(anim.GetLayerIndex("IKLayer"), ikWeight);
            ikTarget.SetActive(IKActive);

            //if (IKActive)
            //{
            if (rightHand != null)
            {
                anim.SetIKPositionWeight(AvatarIKGoal.RightHand, ikWeight);
                anim.SetIKRotationWeight(AvatarIKGoal.RightHand, ikWeight);
                anim.SetIKPosition(AvatarIKGoal.RightHand, rightHand.position);
                anim.SetIKRotation(AvatarIKGoal.RightHand, rightHand.rotation);
            }

            if (rightElbow != null)
            {
                anim.SetIKHintPositionWeight(AvatarIKHint.RightElbow, ikWeight);
                anim.SetIKHintPosition(AvatarIKHint.RightElbow, rightElbow.position);
            }

            if (leftHand != null)
            {
                anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, ikWeight);
                anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, ikWeight);
                anim.SetIKPosition(AvatarIKGoal.LeftHand, leftHand.position);
                anim.SetIKRotation(AvatarIKGoal.LeftHand, leftHand.rotation);
            }

            if (leftElbow != null)
            {
                anim.SetIKHintPositionWeight(AvatarIKHint.LeftElbow, ikWeight);
                anim.SetIKHintPosition(AvatarIKHint.LeftElbow, leftElbow.position);
            }
            //}
            //else
            //{
            //    anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 0f);
            //    anim.SetIKRotationWeight(AvatarIKGoal.RightHand, 0f);

            //    anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0f);
            //    anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0f);

            //    anim.SetIKHintPositionWeight(AvatarIKHint.RightElbow, 0f);
            //    anim.SetIKHintPositionWeight(AvatarIKHint.LeftElbow, 0f);
            //}
        }
       
    }
}

