using UnityEngine;
using ChronicSurvival.Units;

namespace ChronicSurvival.Battle
{
    public class CancerCellBehaviour : MonoBehaviour
    {
        [SerializeField] private float minDivisionInterval = 3f;
        [SerializeField] private float maxDivisionInterval = 5f;

        private Enemy enemy;
        private CancerObjectiveController objectiveController;
        private float divisionTimer;

        public void Initialize(CancerObjectiveController controller)
        {
            objectiveController = controller;
            enemy = GetComponent<Enemy>();
            ResetTimer();
        }

        private void Awake()
        {
            enemy = GetComponent<Enemy>();
            ResetTimer();
        }

        private void Update()
        {
            if (enemy == null || enemy.IsDead || objectiveController == null)
            {
                return;
            }

            divisionTimer -= Time.deltaTime;
            if (divisionTimer > 0f)
            {
                return;
            }

            if (objectiveController.CanSpawnDivision())
            {
                objectiveController.RegisterDivision(enemy, transform.position);
            }

            ResetTimer();
        }

        private void ResetTimer()
        {
            divisionTimer = Random.Range(minDivisionInterval, maxDivisionInterval);
        }
    }
}
