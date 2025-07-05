using UnityEngine;

namespace TD.Utilities
{
    public static class RotationUtils
    {
        /// <summary>
        /// Rotates an object towards a target position smoothly.
        /// </summary>
        /// <param name="transform">The transform of the object to rotate.</param>
        /// <param name="targetPosition">The target position to rotate towards.</param>
        /// <param name="turnSpeed">The speed at which the object should rotate.</param>
        public static void RotateTowardsTarget(Transform transform, Vector3 targetPosition, float turnSpeed)
        {
            Vector3 direction = targetPosition - transform.position;
            direction.Normalize();
            direction.y = 0f; 

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        }

        /// <summary>
        /// Instantly rotates an object to face a target position.
        /// </summary>
        /// <param name="transform">The transform of the object to rotate.</param>
        /// <param name="targetPosition">The target position to rotate towards.</param>
        public static void InstantRotateTowardsTarget(Transform transform, Vector3 targetPosition)
        {
            Vector3 direction = targetPosition - transform.position;
            direction.Normalize();
            direction.y = 0f; // Ensure the rotation is only on the Y axis

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = targetRotation;
        }

        /// <summary>
        /// Rotates an object towards a specified direction.
        /// </summary>
        /// <param name="transform">The transform of the object to rotate.</param>
        /// <param name="direction">The direction to rotate towards.</param>
        /// <param name="turnSpeed">The speed at which the object should rotate.</param>
        public static void RotateTowardsDirection(Transform transform, Vector3 direction, float turnSpeed)
        {
            direction.Normalize();
            direction.y = 0f; // Ensure the rotation is only on the Y axis

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        }

        /// <summary>
        /// Rotates an object around a specified axis by a specified angle.
        /// </summary>
        /// <param name="transform">The transform of the object to rotate.</param>
        /// <param name="axis">The axis to rotate around (e.g., Vector3.up).</param>
        /// <param name="angle">The angle to rotate by.</param>
        public static void RotateAroundAxis(Transform transform, Vector3 axis, float angle)
        {
            transform.Rotate(axis, angle, Space.World);
        }

        /// <summary>
        /// Smoothly rotates an object to match the rotation of another transform.
        /// </summary>
        /// <param name="transform">The transform of the object to rotate.</param>
        /// <param name="targetTransform">The transform to match rotation with.</param>
        /// <param name="turnSpeed">The speed at which the object should rotate.</param>
        public static void SmoothRotateToMatchTransform(Transform transform, Transform targetTransform, float turnSpeed)
        {
            Quaternion targetRotation = targetTransform.rotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        }

        /// <summary>
        /// Rotates an object to look at a target position with a limited angle.
        /// </summary>
        /// <param name="transform">The transform of the object to rotate.</param>
        /// <param name="targetPosition">The target position to look at.</param>
        /// <param name="maxAngle">The maximum angle the object can rotate per frame.</param>
        public static void RotateTowardsTargetWithLimit(Transform transform, Vector3 targetPosition, float maxAngle)
        {
            Vector3 direction = targetPosition - transform.position;
            direction.y = 0f; // Ensure the rotation is only on the Y axis

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, maxAngle * Time.deltaTime);
        }

        /// <summary>
        /// Rotates an object to a random rotation within specified angles.
        /// </summary>
        /// <param name="transform">The transform of the object to rotate.</param>
        /// <param name="minAngles">The minimum rotation angles (Euler) for each axis.</param>
        /// <param name="maxAngles">The maximum rotation angles (Euler) for each axis.</param>
        public static void RandomRotation(Transform transform, Vector3 minAngles, Vector3 maxAngles)
        {
            float randomX = Random.Range(minAngles.x, maxAngles.x);
            float randomY = Random.Range(minAngles.y, maxAngles.y);
            float randomZ = Random.Range(minAngles.z, maxAngles.z);

            Quaternion randomRotation = Quaternion.Euler(randomX, randomY, randomZ);
            transform.rotation = randomRotation;
        }

        /// <summary>
        /// Locks the rotation of an object to the specified axis.
        /// </summary>
        /// <param name="transform">The transform of the object to rotate.</param>
        /// <param name="lockAxis">The axis to lock the rotation to (e.g., Vector3.up).</param>
        public static void LockRotationToAxis(Transform transform, Vector3 lockAxis)
        {
            transform.rotation = Quaternion.LookRotation(lockAxis);
        }

        /// <summary>
        /// Flips the rotation of an object by 180 degrees around the Y axis.
        /// </summary>
        /// <param name="transform">The transform of the object to rotate.</param>
        public static void FlipRotation(Transform transform)
        {
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0, 180, 0));
        }
        /// <summary>
        /// Flips the rotation of an object by 180 degrees around a specified axis.
        /// </summary>
        /// <param name="transform">The transform of the object to rotate.</param>
        /// <param name="axis">The axis to flip the rotation around (e.g., Vector3.up for Y axis).</param>
        public static void FlipRotation(Transform transform, Vector3 axis)
        {
            Quaternion rotation = Quaternion.AngleAxis(180, axis);
            transform.rotation = rotation * transform.rotation;
        }
    }

}
