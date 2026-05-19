using UnityEngine;

namespace ChronicSurvival.ProceduralVisuals
{
    /// <summary>
    /// Creates trailing particle effect for moving blobs
    /// Adds visual feedback for movement and makes cells more dynamic
    /// </summary>
    public class BlobTrailEffect : MonoBehaviour
    {
        [Header("Trail Settings")]
        [SerializeField] private bool enableTrail = true;
        [SerializeField] private Color trailColor = new Color(0.4f, 0.7f, 1f, 0.5f);
        [SerializeField] private float trailLifetime = 0.5f;
        [SerializeField] private float emissionRate = 20f;
        [SerializeField] private float minVelocityForTrail = 0.5f;

        [Header("Particle Settings")]
        [SerializeField] private float particleSize = 0.2f;
        [SerializeField] private float particleSizeVariation = 0.1f;
        [SerializeField] private AnimationCurve sizeOverLifetime = AnimationCurve.Linear(0, 1, 1, 0);

        private ParticleSystem particleSystem;
        private Vector3 lastPosition;
        private float emissionTimer;

        private void Start()
        {
            SetupParticleSystem();
            lastPosition = transform.position;
        }

        private void SetupParticleSystem()
        {
            // Create particle system
            GameObject psObj = new GameObject("TrailParticles");
            psObj.transform.SetParent(transform);
            psObj.transform.localPosition = Vector3.zero;

            particleSystem = psObj.AddComponent<ParticleSystem>();
            
            // Main module
            var main = particleSystem.main;
            main.startLifetime = trailLifetime;
            main.startSpeed = 0f;
            main.startSize = particleSize;
            main.startColor = trailColor;
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            main.maxParticles = 100;
            main.loop = false;
            main.playOnAwake = false;

            // Emission
            var emission = particleSystem.emission;
            emission.enabled = false; // We'll emit manually

            // Shape
            var shape = particleSystem.shape;
            shape.enabled = true;
            shape.shapeType = ParticleSystemShapeType.Circle;
            shape.radius = 0.1f;

            // Size over lifetime
            var sizeOverLife = particleSystem.sizeOverLifetime;
            sizeOverLife.enabled = true;
            sizeOverLife.size = new ParticleSystem.MinMaxCurve(1f, sizeOverLifetime);

            // Color over lifetime (fade out)
            var colorOverLife = particleSystem.colorOverLifetime;
            colorOverLife.enabled = true;
            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[] { 
                    new GradientColorKey(trailColor, 0f),
                    new GradientColorKey(trailColor, 1f)
                },
                new GradientAlphaKey[] { 
                    new GradientAlphaKey(trailColor.a, 0f),
                    new GradientAlphaKey(0f, 1f)
                }
            );
            colorOverLife.color = new ParticleSystem.MinMaxGradient(gradient);

            // Renderer
            var renderer = particleSystem.GetComponent<ParticleSystemRenderer>();
            renderer.renderMode = ParticleSystemRenderMode.Billboard;
            renderer.sortingOrder = -2; // Behind blob
        }

        private void Update()
        {
            if (!enableTrail) return;

            // Calculate velocity
            Vector3 velocity = (transform.position - lastPosition) / Time.deltaTime;
            float speed = velocity.magnitude;

            // Emit particles if moving fast enough
            if (speed > minVelocityForTrail)
            {
                emissionTimer += Time.deltaTime;
                float emissionInterval = 1f / emissionRate;

                while (emissionTimer >= emissionInterval)
                {
                    EmitParticle();
                    emissionTimer -= emissionInterval;
                }
            }

            lastPosition = transform.position;
        }

        private void EmitParticle()
        {
            ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams();
            emitParams.position = transform.position;
            emitParams.startSize = particleSize + Random.Range(-particleSizeVariation, particleSizeVariation);
            emitParams.startColor = trailColor;
            emitParams.startLifetime = trailLifetime;

            particleSystem.Emit(emitParams, 1);
        }

        // Public methods
        public void SetTrailColor(Color color)
        {
            trailColor = color;
            
            if (particleSystem != null)
            {
                var main = particleSystem.main;
                main.startColor = color;
            }
        }

        public void SetTrailEnabled(bool enabled)
        {
            enableTrail = enabled;
        }

        public void SetEmissionRate(float rate)
        {
            emissionRate = rate;
        }
    }
}
