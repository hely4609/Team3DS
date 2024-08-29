using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Appearance : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] Character owner;

    [SerializeField] bool ikActive = false;
    [SerializeField] GameObject ikTarget = null;
    [SerializeField] Transform rightHand = null;
    [SerializeField] Transform leftHand = null;
    [SerializeField] Transform rightElbow = null;
    [SerializeField] Transform leftElbow = null;


    float rightHandWeight;
    float leftHandWeight;
    float rightElbowWeight;
    float leftElbowWeight;


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

    public void SetIK()
    {
        ikActive = !ikActive;
        ikTarget.SetActive(ikActive);
    }

    //a callback for calculating IK
    void OnAnimatorIK()
    {
        if (anim)
        {
            rightHandWeight =  Mathf.Lerp(rightHandWeight,  ikActive ? 1f : 0f, 0.03f);
            leftHandWeight =   Mathf.Lerp(leftHandWeight,   ikActive ? 1f : 0f, 0.03f);
            rightElbowWeight = Mathf.Lerp(rightElbowWeight, ikActive ? 1f : 0f, 0.03f);
            leftElbowWeight =  Mathf.Lerp(leftElbowWeight,  ikActive ? 1f : 0f, 0.03f);

            if (ikActive)
            {

                if (rightHand != null)
                {
                    anim.SetIKPositionWeight(AvatarIKGoal.RightHand, rightHandWeight);
                    anim.SetIKRotationWeight(AvatarIKGoal.RightHand, rightHandWeight);
                    anim.SetIKPosition(AvatarIKGoal.RightHand, rightHand.position);
                    anim.SetIKRotation(AvatarIKGoal.RightHand, rightHand.rotation);
                }

                if (rightElbow != null)
                {
                    anim.SetIKHintPositionWeight(AvatarIKHint.RightElbow, rightElbowWeight);
                    anim.SetIKHintPosition(AvatarIKHint.RightElbow, rightElbow.position);
                }

                if (leftHand != null)
                {
                    anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, leftHandWeight);
                    anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, leftHandWeight);
                    anim.SetIKPosition(AvatarIKGoal.LeftHand, leftHand.position);
                    anim.SetIKRotation(AvatarIKGoal.LeftHand, leftHand.rotation);
                }

                if (leftElbow != null)
                {
                    anim.SetIKHintPositionWeight(AvatarIKHint.LeftElbow,leftElbowWeight);
                    anim.SetIKHintPosition(AvatarIKHint.LeftElbow, leftElbow.position);
                }
            }
            else
            { 
                anim.SetIKPositionWeight(AvatarIKGoal.RightHand, rightHandWeight);
                anim.SetIKRotationWeight(AvatarIKGoal.RightHand, rightHandWeight);

                anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, leftHandWeight);
                anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, leftHandWeight);

                anim.SetIKHintPositionWeight(AvatarIKHint.RightElbow, rightElbowWeight);
                anim.SetIKHintPositionWeight(AvatarIKHint.LeftElbow, leftElbowWeight);
            }
        }
    }
}
