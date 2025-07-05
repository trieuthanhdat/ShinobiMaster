using UnityEngine;

namespace TD.Utilities
{
    public static class IKUtils
    {
        #region ___FOOT PLACEMENT___

        /// <summary>
        /// Adjusts the character's foot placement using IK, aligning feet to the terrain.
        /// </summary>
        public static void ApplyFootPlacement(
            Animator animator,
            AvatarIKGoal foot,
            float footIKWeight,
            LayerMask layerMask,
            float rayOffset = 1f,
            float rayLength = 1.5f
        )
        {
            if (animator == null) return;

            Vector3 footPosition = animator.GetIKPosition(foot);
            Vector3 rayOrigin = footPosition + Vector3.up * rayOffset;
            Ray ray = new Ray(rayOrigin, Vector3.down);

            if (Physics.Raycast(ray, out RaycastHit hit, rayLength, layerMask))
            {
                animator.SetIKPositionWeight(foot, footIKWeight);
                animator.SetIKPosition(foot, hit.point);

                Quaternion footRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
                animator.SetIKRotationWeight(foot, footIKWeight);
                animator.SetIKRotation(foot, footRotation);
            }
            else
            {
                animator.SetIKPositionWeight(foot, 0);
                animator.SetIKRotationWeight(foot, 0);
            }
        }

        #endregion

        #region ___HAND ALIGNMENT___

        /// <summary>
        /// Aligns the character's hand to a target position and rotation using IK.
        /// </summary>
        public static void ApplyHandIK(
            Animator animator,
            AvatarIKGoal hand,
            Transform targetTransform,
            float handIKWeight
        )
        {
            if (animator == null || targetTransform == null) return;

            animator.SetIKPositionWeight(hand, handIKWeight);
            animator.SetIKRotationWeight(hand, handIKWeight);
            animator.SetIKPosition(hand, targetTransform.position);
            animator.SetIKRotation(hand, targetTransform.rotation);
        }

        #endregion

        #region ___LOOK-AT IK___

        /// <summary>
        /// Makes the character's head look at a target position.
        /// </summary>
        public static void ApplyLookAtIK(
            Animator animator,
            Vector3 lookAtPosition,
            float lookAtWeight
        )
        {
            if (animator == null) return;

            animator.SetLookAtWeight(lookAtWeight);
            animator.SetLookAtPosition(lookAtPosition);
        }

        #endregion

        #region ___SET IK WEIGHTS___

        /// <summary>
        /// Sets the IK weight for a specific goal.
        /// </summary>
        public static void SetIKGoalWeight(Animator animator, AvatarIKGoal goal, float weight)
        {
            if (animator == null) return;

            animator.SetIKPositionWeight(goal, weight);
            animator.SetIKRotationWeight(goal, weight);
        }

        /// <summary>
        /// Sets the LookAt weight for the character's head.
        /// </summary>
        public static void SetLookAtWeight(Animator animator, float weight)
        {
            if (animator == null) return;

            animator.SetLookAtWeight(weight);
        }

        #endregion

        #region ___UTILITY METHODS___

        /// <summary>
        /// Smoothly blends the IK weight to a target value over time.
        /// </summary>
        public static void SmoothSetIKWeight(ref float currentWeight, float targetWeight, float blendSpeed)
        {
            currentWeight = Mathf.Lerp(currentWeight, targetWeight, blendSpeed * Time.deltaTime);
        }

        /// <summary>
        /// Resets all IK weights for a character to zero.
        /// </summary>
        public static void ResetAllIKWeights(Animator animator)
        {
            if (animator == null) return;

            animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 0);
            animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 0);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 0);
            animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 0);

            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0);
            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);

            animator.SetLookAtWeight(0);
        }

        /// <summary>
        /// Checks if the IK target is valid (not null and active).
        /// </summary>
        public static bool IsIKTargetValid(Transform target)
        {
            return target != null && target.gameObject.activeInHierarchy;
        }

        #endregion

        #region ___GROUND CHECKING___

        /// <summary>
        /// Performs a ground check to find the terrain normal at a specific point.
        /// </summary>
        public static Vector3 GetGroundNormal(
            Vector3 position,
            LayerMask layerMask,
            float rayOffset = 1f,
            float rayLength = 1.5f
        )
        {
            Vector3 rayOrigin = position + Vector3.up * rayOffset;
            Ray ray = new Ray(rayOrigin, Vector3.down);

            if (Physics.Raycast(ray, out RaycastHit hit, rayLength, layerMask))
            {
                return hit.normal;
            }

            return Vector3.up;
        }

        #endregion

        #region ___EDITOR DEBUGGING___

#if UNITY_EDITOR
        /// <summary>
        /// Draws debug lines for IK systems in the Unity editor.
        /// </summary>
        public static void DrawDebugLine(Vector3 start, Vector3 end, Color color)
        {
            Debug.DrawLine(start, end, color);
        }

        /// <summary>
        /// Draws a debug sphere in the Unity editor.
        /// </summary>
        public static void DrawDebugSphere(Vector3 position, float radius, Color color)
        {
            Gizmos.color = color;
            Gizmos.DrawSphere(position, radius);
        }
#endif

        #endregion
    }

}

