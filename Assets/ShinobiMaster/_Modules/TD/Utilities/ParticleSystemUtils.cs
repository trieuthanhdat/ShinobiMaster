using System;
using UnityEngine;


namespace TD.Utilities
{
    public static class ParticleSystemUtils
    {
        /// <summary>
        /// Sets the color over lifetime for a ParticleSystem using a dynamically created gradient.
        /// </summary>
        public static void SetColorOverLifetime(ParticleSystem particleSystem, Color color)
        {
            if (particleSystem == null)
            {
                Debug.LogWarning("[ParticleSystemUtils] ParticleSystem is null. Cannot set color.");
                return;
            }

            var colorModule = particleSystem.colorOverLifetime;

            if (!colorModule.enabled)
            {
                colorModule.enabled = true;
            }

            // Create a gradient using the provided color
            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[] { new GradientColorKey(color, 0f), new GradientColorKey(color, 1f) },
                new GradientAlphaKey[] { new GradientAlphaKey(color.a, 0f), new GradientAlphaKey(color.a, 1f) }
            );

            colorModule.color = new ParticleSystem.MinMaxGradient(gradient);
        }
        /// <summary>
        /// Sets the color over the lifetime of a ParticleSystem using specified GradientColorKey and GradientAlphaKey arrays.
        /// </summary>
        /// <param name="particleSystem">The ParticleSystem to modify.</param>
        /// <param name="colorKeys">An array of GradientColorKey defining the colors at different times.</param>
        /// <param name="alphaKeys">An array of GradientAlphaKey defining the alpha values at different times.</param>
        public static void SetColorOverLifetime
        (
            ParticleSystem particleSystem,
            GradientColorKey[] colorKeys,
            GradientAlphaKey[] alphaKeys
        )
        {
            if (particleSystem == null)
            {
                Debug.LogWarning("[ParticleSystemUtils] ParticleSystem is null. Cannot set color.");
                return;
            }

            var colorModule = particleSystem.colorOverLifetime;

            if (!colorModule.enabled)
            {
                colorModule.enabled = true;
            }

            Gradient gradient = new Gradient();
            gradient.SetKeys(colorKeys, alphaKeys);

            colorModule.color = new ParticleSystem.MinMaxGradient(gradient);
        }

        /// <summary>
        /// Sets the color over the lifetime of a ParticleSystem dynamically using an array of colors.
        /// The colors will be evenly distributed across the particle's lifetime.
        /// </summary>
        /// <param name="particleSystem">The ParticleSystem to modify.</param>
        /// <param name="colors">An array of Color to define the gradient.</param>
        public static void SetColorOverLifetime(ParticleSystem particleSystem, Color[] colors)
        {
            if (particleSystem == null)
            {
                Debug.LogWarning("[ParticleSystemUtils] ParticleSystem is null. Cannot set color.");
                return;
            }

            var colorModule = particleSystem.colorOverLifetime;

            if (!colorModule.enabled)
            {
                colorModule.enabled = true;
            }

            if (colors == null || colors.Length == 0)
            {
                Debug.LogWarning("[ParticleSystemUtils] Colors array is empty. Cannot set color.");
                return;
            }

            Gradient gradient = new Gradient();

            GradientColorKey[] colorKeys = new GradientColorKey[colors.Length];
            GradientAlphaKey[] alphaKeys = new GradientAlphaKey[colors.Length];

            for (int i = 0; i < colors.Length; i++)
            {
                float time = i / (float)(colors.Length - 1); // Normalize to 0-1
                colorKeys[i] = new GradientColorKey(colors[i], time);
                alphaKeys[i] = new GradientAlphaKey(colors[i].a, time);
            }

            gradient.SetKeys(colorKeys, alphaKeys);

            colorModule.color = new ParticleSystem.MinMaxGradient(gradient);
        }

        /// <summary>
        /// Dynamically sets the color over the lifetime of a ParticleSystem by scaling a base color using an intensity curve.
        /// </summary>
        /// <param name="particleSystem">The ParticleSystem to modify.</param>
        /// <param name="baseColor">The base Color to scale over the lifetime.</param>
        /// <param name="intensityCurve">An AnimationCurve defining the intensity of the base color over the particle's lifetime.</param>
        public static void SetColorOverLifetime
        (
            ParticleSystem particleSystem,
            Color baseColor,
            AnimationCurve intensityCurve
        )
        {
            if (particleSystem == null)
            {
                Debug.LogWarning("[ParticleSystemUtils] ParticleSystem is null. Cannot set color.");
                return;
            }

            var colorModule = particleSystem.colorOverLifetime;

            if (!colorModule.enabled)
            {
                colorModule.enabled = true;
            }

            Gradient gradient = new Gradient();
            GradientColorKey[] colorKeys = new GradientColorKey[100];
            GradientAlphaKey[] alphaKeys = new GradientAlphaKey[100];

            for (int i = 0; i < 100; i++)
            {
                float time = i / 99f; // Normalize time to 0-1
                float intensity = intensityCurve.Evaluate(time);
                Color dynamicColor = baseColor * intensity; // Adjust base color by intensity

                colorKeys[i] = new GradientColorKey(dynamicColor, time);
                alphaKeys[i] = new GradientAlphaKey(dynamicColor.a, time);
            }

            gradient.SetKeys(colorKeys, alphaKeys);

            colorModule.color = new ParticleSystem.MinMaxGradient(gradient);
        }

        /// <summary>
        /// Sets the size of particles over their lifetime using a curve.
        /// </summary>
        public static void SetSizeOverLifetime(ParticleSystem particleSystem, AnimationCurve sizeCurve)
        {
            if (particleSystem == null)
            {
                Debug.LogWarning("[ParticleSystemUtils] ParticleSystem is null. Cannot set size.");
                return;
            }

            var sizeModule = particleSystem.sizeOverLifetime;

            if (!sizeModule.enabled)
            {
                sizeModule.enabled = true;
            }

            sizeModule.size = new ParticleSystem.MinMaxCurve(1f, sizeCurve);
        }
        public static void SetColorOverLifetime
        (
            ParticleSystem particleSystem,
            Gradient gradientA,
            Gradient gradientB,
            float blendFactor
        )
        {
            if (particleSystem == null)
            {
                Debug.LogWarning("[ParticleSystemUtils] ParticleSystem is null. Cannot set color.");
                return;
            }

            var colorModule = particleSystem.colorOverLifetime;

            if (!colorModule.enabled)
            {
                colorModule.enabled = true;
            }

            blendFactor = Mathf.Clamp01(blendFactor);

            Gradient blendedGradient = new Gradient();

            GradientColorKey[] colorKeysA = gradientA.colorKeys;
            GradientColorKey[] colorKeysB = gradientB.colorKeys;
            GradientColorKey[] blendedColorKeys = new GradientColorKey[colorKeysA.Length];

            for (int i = 0; i < colorKeysA.Length; i++)
            {
                Color blendedColor = Color.Lerp(colorKeysA[i].color, colorKeysB[i].color, blendFactor);
                blendedColorKeys[i] = new GradientColorKey(blendedColor, colorKeysA[i].time);
            }

            blendedGradient.SetKeys(blendedColorKeys, gradientA.alphaKeys);

            colorModule.color = new ParticleSystem.MinMaxGradient(blendedGradient);
        }

        /// <summary>
        /// Sets the emission rate of the ParticleSystem dynamically.
        /// </summary>
        public static void SetEmissionRate(ParticleSystem particleSystem, float rate)
        {
            if (particleSystem == null)
            {
                Debug.LogWarning("[ParticleSystemUtils] ParticleSystem is null. Cannot set emission rate.");
                return;
            }

            var emissionModule = particleSystem.emission;

            if (!emissionModule.enabled)
            {
                emissionModule.enabled = true;
            }

            var rateOverTime = emissionModule.rateOverTime;
            rateOverTime.constant = rate;
            emissionModule.rateOverTime = rateOverTime;
        }

        /// <summary>
        /// Sets the speed of particles by modifying the start speed.
        /// </summary>
        public static void SetStartSpeed(ParticleSystem particleSystem, float speed)
        {
            if (particleSystem == null)
            {
                Debug.LogWarning("[ParticleSystemUtils] ParticleSystem is null. Cannot set start speed.");
                return;
            }

            var mainModule = particleSystem.main;
            mainModule.startSpeed = speed;
        }
        /// <summary>
        /// Sets the size of particles by modifying the start Size, preserving the original configuration
        /// (constant, random between 2 constants, curve or random between 2 curves)
        /// Optionally scale child particle systems as well
        /// </summary>
        /// <param name="particleSystem">The main particle system to modify</param>
        /// <param name="sizeMultiplier">The multiplier to apply to the start size</param>
        /// <param name="scaleChildParticles">If true, applies scaling to child paritcle systems.</param>
        public static void SetStartSize(ParticleSystem particleSystem, float sizeMultiplier, bool scaleChildParticles = false)
        {
            if (particleSystem == null)
            {
                Debug.LogWarning("[ParticleSystemUtils] ParticleSystem is null. Cannot set start speed.");
                return;
            }
            ScaleParticleSystem(particleSystem, sizeMultiplier);
            if(scaleChildParticles)
            {
                var childParticles = ComponentCache.GetComponentsInChildren<ParticleSystem>(particleSystem.gameObject, true);
                foreach (var child in childParticles)
                {
                    if (child != null && child != particleSystem)
                    {
                        ScaleParticleSystem(child, sizeMultiplier);
                    }
                }
            }
            
        }
        /// <summary>
        /// Scales the start size of the single Particle system based on its current configuration.
        /// </summary>
        /// <param name="particleSystem">The particle system to scale</param>
        /// <param name="sizeMultiplier">The multiplier to apply</param>
        public static void ScaleParticleSystem(ParticleSystem particleSystem, float sizeMultiplier)
        {
            var mainModule = particleSystem.main;
            ParticleSystem.MinMaxCurve updatedStartSize = default;
            switch(mainModule.startSize.mode)
            {
                default:
                case ParticleSystemCurveMode.Constant:
                    updatedStartSize = new ParticleSystem.MinMaxCurve
                    (   
                        mainModule.startSize.constant * sizeMultiplier
                    );
                    break;
                case ParticleSystemCurveMode.TwoConstants:
                    updatedStartSize = new ParticleSystem.MinMaxCurve
                    (
                        mainModule.startSize.constantMin * sizeMultiplier,
                        mainModule.startSize.constantMax * sizeMultiplier
                    );
                    Debug.Log($"[ParticleSystemUtils] Updated start size for {particleSystem.name} (Random Between Two Constants): Min = {updatedStartSize.constantMin}, Max = {updatedStartSize.constantMax}");
                    break;
                case ParticleSystemCurveMode.Curve:
                    AnimationCurve scaledCurve = ScaleAnimationCurve(mainModule.startSize.curve, sizeMultiplier);
                    if(scaledCurve != null)
                    {
                        updatedStartSize = new ParticleSystem.MinMaxCurve
                        (
                            mainModule.startSize.curveMultiplier * sizeMultiplier, scaledCurve
                        );
                    }
                    break;
                case ParticleSystemCurveMode.TwoCurves:
                    AnimationCurve scaledCurveMin = ScaleAnimationCurve(mainModule.startSize.curveMin, sizeMultiplier);
                    AnimationCurve scaledCurveMax = ScaleAnimationCurve(mainModule.startSize.curveMax, sizeMultiplier);
                    if (scaledCurveMin != null && scaledCurveMax != null)
                    {
                        updatedStartSize = new ParticleSystem.MinMaxCurve
                        (
                            mainModule.startSize.curveMultiplier * sizeMultiplier, 
                            scaledCurveMin,
                            scaledCurveMax
                        );
                    }
                    break;

            }
            mainModule.startSize = updatedStartSize;
            Debug.Log($"[ParticleSystemUtils]: new startSize for {particleSystem.name} - new sizeMultiplier {sizeMultiplier}");

        }
        /// <summary>
        /// Scales the key values of an Animation Curve by a multiplier
        /// </summary>
        /// <param name="curve">The original AnimationCurve to scale</param>
        /// <param name="sizeMultiplier">The scaling factor</param>
        /// <returns>A new Scaled AnimationCurve</returns>
        private static AnimationCurve ScaleAnimationCurve(AnimationCurve curve, float sizeMultiplier)
        {
            if (curve == null) return null;

            AnimationCurve scaledCurve = new AnimationCurve();
            foreach(var key in curve.keys)
            {
                scaledCurve.AddKey(new Keyframe(key.time, key.value * sizeMultiplier, key.inTangent * sizeMultiplier, key.outTangent * sizeMultiplier));
            }
            return scaledCurve;
        }

        /// <summary>
        /// Sets the lifetime of particles in the ParticleSystem.
        /// </summary>
        public static void SetLifetime(ParticleSystem particleSystem, float lifetime)
        {
            if (particleSystem == null)
            {
                Debug.LogWarning("[ParticleSystemUtils] ParticleSystem is null. Cannot set lifetime.");
                return;
            }

            var mainModule = particleSystem.main;
            mainModule.startLifetime = lifetime;
        }

        /// <summary>
        /// Resets the ParticleSystem's transformations and properties to default values.
        /// </summary>
        public static void ResetParticleSystem(ParticleSystem particleSystem)
        {
            if (particleSystem == null)
            {
                Debug.LogWarning("[ParticleSystemUtils] ParticleSystem is null. Cannot reset.");
                return;
            }

            particleSystem.Stop();
            particleSystem.Clear();

            var mainModule = particleSystem.main;
            mainModule.startSize = 1f;
            mainModule.startLifetime = 1f;
            mainModule.startSpeed = 5f;

            var emissionModule = particleSystem.emission;
            emissionModule.enabled = true;
            emissionModule.rateOverTime = 10f;

            var sizeModule = particleSystem.sizeOverLifetime;
            sizeModule.enabled = false;

            var colorModule = particleSystem.colorOverLifetime;
            colorModule.enabled = false;
        }

        /// <summary>
        /// Plays the ParticleSystem at a specified position and rotation.
        /// </summary>
        public static void PlayAtPosition(ParticleSystem particleSystem, Vector3 position, Quaternion rotation)
        {
            if (particleSystem == null)
            {
                Debug.LogWarning("[ParticleSystemUtils] ParticleSystem is null. Cannot play at position.");
                return;
            }

            particleSystem.transform.position = position;
            particleSystem.transform.rotation = rotation;
            particleSystem.Play();
        }

        /// <summary>
        /// Stops the ParticleSystem and optionally clears its particles.
        /// </summary>
        public static void StopParticleSystem(ParticleSystem particleSystem, bool clearParticles = true)
        {
            if (particleSystem == null)
            {
                Debug.LogWarning("[ParticleSystemUtils] ParticleSystem is null. Cannot stop.");
                return;
            }

            particleSystem.Stop();
            if (clearParticles)
            {
                particleSystem.Clear();
            }
        }
    }

}
