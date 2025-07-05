using UnityEngine;

namespace TD.Utilities
{
    public static class AimingUtils
    {
        /// <summary>
        /// Updates the aiming direction by interpolating between the current position and the desired aim position.
        /// </summary>
        /// <param name="currentPosition">The current position of the aiming transform.</param>
        /// <param name="targetPosition">The target position to aim towards.</param>
        /// <param name="aimSensitivity">The sensitivity of the aim (how fast the aim moves towards the target).</param>
        /// <returns>The new position for the aiming transform.</returns>
        public static Vector3 UpdateAimingDirection(Vector3 currentPosition, Vector3 targetPosition, float aimSensitivity)
        {
            return Vector3.Lerp(currentPosition, targetPosition, aimSensitivity * Time.deltaTime);
        }

        /// <summary>
        /// Calculates the desired aim position based on the mouse position, player movement, and other parameters.
        /// </summary>
        /// <param name="currentPosition">The current position of the object.</param>
        /// <param name="mousePosition">The position of the mouse in world space.</param>
        /// <param name="movementInputY">The Y-axis input from the player movement.</param>
        /// <param name="minDistance">The minimum distance the aim can be from the object.</param>
        /// <param name="maxDistance">The maximum distance the aim can be from the object.</param>
        /// <param name="yOffset">The vertical offset for the aim position.</param>
        /// <returns>The calculated desired aim position.</returns>
        public static Vector3 GetDesiredCamaraPosition(Vector3 currentPosition, Vector3 mousePosition, float movementInputY, float minDistance, float maxDistance, float yOffset)
        {
            float actualMaxCamDistance = movementInputY < -0.5f ? minDistance : maxDistance;

            Vector3 aimDirection = mousePosition - currentPosition;
            aimDirection.Normalize();

            float distanceToDesirePosition = Vector3.Distance(currentPosition, mousePosition);
            distanceToDesirePosition = Mathf.Clamp(distanceToDesirePosition, minDistance, actualMaxCamDistance);

            Vector3 desiredAimPosition = currentPosition + aimDirection * distanceToDesirePosition;
            desiredAimPosition.y = currentPosition.y + yOffset;

            return desiredAimPosition;
        }
    }

}
