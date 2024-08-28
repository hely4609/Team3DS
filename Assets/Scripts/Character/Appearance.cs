using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Appearance : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] Character owner;

    [SerializeField] bool ikActive = false;
    [SerializeField] Transform rightHand = null;
    [SerializeField] Transform leftHand = null;
    [SerializeField] Transform rightElbow = null;
    [SerializeField] Transform leftElbow = null;

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
        target.AnimBool += SetBool;
        target.AnimFloat -= SetFloat;
        target.AnimFloat += SetFloat;
        target.AnimInt -= SetInt;
        target.AnimInt += SetInt;
        target.AnimTrigger -= SetTrigger;
        target.AnimTrigger += SetTrigger;
    }
    public void DisConnectCharacter()
    {
        if (owner == null) return;
        owner.AnimBool -= SetBool;
        owner.AnimFloat -= SetFloat;
        owner.AnimInt -= SetInt;
        owner.AnimTrigger -= SetTrigger;
        owner = null;
    }
    public void SetBool(string name, bool value) => anim?.SetBool(name, value);
    public void SetFloat(string name, float value) => anim?.SetFloat(name, value);
    public void SetInt(string name, int value) => anim?.SetInteger(name, value);
    public void SetTrigger(string name) => anim?.SetTrigger(name);

    public void SetIK(bool value)
    {
        ikActive = value;
    }

    //a callback for calculating IK
    void OnAnimatorIK()
    {
        if (anim)
        {
            if (ikActive)
            {
                // Set the look target position, if one has been assigned
                //if (lookObj != null)
                //{
                //    animator.SetLookAtWeight(1);
                //    animator.SetLookAtPosition(lookObj.position);
                //}

                if (rightHand != null)
                {
                    anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
                    anim.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
                    anim.SetIKPosition(AvatarIKGoal.RightHand, rightHand.position);
                    anim.SetIKRotation(AvatarIKGoal.RightHand, rightHand.rotation);
                }

                if (rightElbow != null)
                {
                    anim.SetIKHintPositionWeight(AvatarIKHint.RightElbow, 1);
                    anim.SetIKHintPosition(AvatarIKHint.RightElbow, rightElbow.position);
                }

                if (leftHand != null)
                {
                    anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
                    anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
                    anim.SetIKPosition(AvatarIKGoal.LeftHand, leftHand.position);
                    anim.SetIKRotation(AvatarIKGoal.LeftHand, leftHand.rotation);
                }

                if (leftElbow != null)
                {

                    anim.SetIKHintPositionWeight(AvatarIKHint.LeftElbow, 1);
                    anim.SetIKHintPosition(AvatarIKHint.LeftElbow, leftElbow.position);
                }

            }

            else
            {
                anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
                anim.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);

                anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
                anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0);

                anim.SetIKHintPositionWeight(AvatarIKHint.RightElbow, 0);
                anim.SetIKHintPositionWeight(AvatarIKHint.LeftElbow, 0);
            }
        }
    }
}
