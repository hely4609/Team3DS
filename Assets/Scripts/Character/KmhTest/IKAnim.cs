using UnityEngine;
using System;
using System.Collections;

[RequireComponent(typeof(Animator))]

public class IKControl : MonoBehaviour
{

    //protected Animator animator;

    //public bool ikActive = false;
    //public Transform rightHand = null;
    //public Transform leftHand = null;
    //public Transform rightElbow = null;
    //public Transform leftElbow = null;

    //void Start()
    //{
    //    animator = GetComponent<Animator>();
    //}

    ////a callback for calculating IK
    //void OnAnimatorIK()
    //{
    //    if (animator)
    //    {
    //        if (ikActive)
    //        {
    //            // Set the look target position, if one has been assigned
    //            //if (lookObj != null)
    //            //{
    //            //    animator.SetLookAtWeight(1);
    //            //    animator.SetLookAtPosition(lookObj.position);
    //            //}

    //            if (rightHand != null )
    //            {
    //                animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
    //                animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
    //                animator.SetIKPosition(AvatarIKGoal.RightHand, rightHand.position);
    //                animator.SetIKRotation(AvatarIKGoal.RightHand, rightHand.rotation);
    //            }

    //            if (rightElbow != null)
    //            {
    //                animator.SetIKHintPositionWeight(AvatarIKHint.RightElbow, 1);
    //                animator.SetIKHintPosition(AvatarIKHint.RightElbow, rightElbow.position);
    //            }

    //            if (leftHand != null)
    //            {
    //                animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
    //                animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
    //                animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHand.position);
    //                animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHand.rotation);
    //            }

    //            if (leftElbow != null) 
    //            {

    //                animator.SetIKHintPositionWeight(AvatarIKHint.LeftElbow, 1);
    //                animator.SetIKHintPosition(AvatarIKHint.LeftElbow, leftElbow.position);
    //            }

    //        }

    //        else
    //        {
    //            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
    //            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);

    //            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
    //            animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0);

    //            animator.SetIKHintPositionWeight(AvatarIKHint.RightElbow, 0);
    //            animator.SetIKHintPositionWeight(AvatarIKHint.LeftElbow, 0);
    //        }
    //    }
    //}
}
