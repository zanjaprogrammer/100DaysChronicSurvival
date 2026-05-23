using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ChronicSurvival.Core;
using ChronicSurvival.Cards;
using ChronicSurvival.Disease;
using ChronicSurvival.Units;

namespace ChronicSurvival.UI
{
    /// <summary>
    /// Handles the micro-narrative events and progression changes between days.
    /// Provides a simple click interface to advance to the next day.
    /// </summary>
    public class RandomEventUI : MonoBehaviour
    {
        [Header("Texts")]
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI eventDescriptionText;

        [Header("Interaction")]
        [SerializeField] private Button continueButton;

        private void OnEnable()
        {
            GenerateEvent();
            if (continueButton != null)
            {
                continueButton.onClick.RemoveAllListeners();
                continueButton.onClick.AddListener(OnContinueClicked);
            }
        }

        private void GenerateEvent()
        {
            int currentDay = GameManager.Instance != null ? GameManager.Instance.CurrentDay : 1;

            // List of potential narrations
            string[] titles = {
                "Lonjakan Kortisol",
                "Lonjakan Gula Darah",
                "Pergeseran Metabolik",
                "Regenerasi Jaringan",
                "Respons Inflamasi",
                "Sinkronisasi Imun"
            };

            string[] descriptions = {
                "Stres emosional yang tinggi memicu lonjakan kortisol. Tekanan darah sedikit meningkat, menyebabkan ketegangan sementara pada dinding pembuluh darah.",
                "Konsumsi karbohidrat sederhana menyebabkan lonjakan glukosa darah secara mendadak, membebani sel beta pankreas.",
                "Tingkat oksigenasi sangat baik hari ini. Produksi energi meningkat, membantu pemulihan sel.",
                "Sitokin anti-inflamasi bersirkulasi dalam aliran darah, sedikit mengurangi stres kronis secara keseluruhan.",
                "Respons imun lokal meningkatkan permeabilitas pembuluh darah, mempermudah sel imun untuk bergerak.",
                "Sel imun berhasil mengidentifikasi dan menetralisir jaringan abnormal pada stadium awal."
            };

            // Select a semi-random event based on the current day
            int eventIndex = (currentDay + Random.Range(0, 3)) % titles.Length;
            
            if (titleText != null)
            {
                titleText.text = $"Ringkasan Hari {currentDay}: {titles[eventIndex]}";
            }

            if (eventDescriptionText != null)
            {
                string cardNote = "";
                if (CardBuffManager.Instance != null && !string.IsNullOrEmpty(CardBuffManager.Instance.LastCardName))
                {
                    string buff = CardBuffManager.Instance.GetBuffSummary();
                    cardNote = $"\n\n<b>Kartu dipilih:</b> {CardBuffManager.Instance.LastCardName}";
                    if (!string.IsNullOrEmpty(buff))
                    {
                        cardNote += $"\n<b>Bonus pertempuran berikutnya:</b> {buff}";
                    }
                }

                eventDescriptionText.text = descriptions[eventIndex] + cardNote +
                    "\n\nTekan tombol lanjutkan untuk menstabilkan sistem tubuh dan memulai Hari " + (currentDay + 1) + ".";
            }

            // Apply minor passive disease modifiers based on event to make the loop reactive!
            if (DiseaseManager.Instance != null)
            {
                if (eventIndex == 0) // Stress Spike
                {
                    DiseaseManager.Instance.IncreaseDiseaseProgression(Units.DiseaseType.Hypertension, 3f);
                }
                else if (eventIndex == 1) // Sugar Spike
                {
                    DiseaseManager.Instance.IncreaseDiseaseProgression(Units.DiseaseType.Diabetes, 4f);
                }
                else if (eventIndex == 3 || eventIndex == 5) // Recoveries
                {
                    DiseaseManager.Instance.ReduceDiseaseProgression(Units.DiseaseType.Cancer, 2f);
                    DiseaseManager.Instance.ReduceDiseaseProgression(Units.DiseaseType.Diabetes, 1f);
                    DiseaseManager.Instance.ReduceDiseaseProgression(Units.DiseaseType.Hypertension, 1f);
                }
            }
        }

        private void OnContinueClicked()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.EndRandomEvent();
            }
        }
    }
}
