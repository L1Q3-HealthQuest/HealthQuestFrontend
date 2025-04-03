using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class AppointmentListUI : MonoBehaviour
{
    public GameObject appointmentPrefab;
    public Transform contentParent;

    private AppointmentApiClient appointmentApiClient;
    private Treatment treatment;


    public async void Start()
    {
        appointmentApiClient = ApiClientManager.Instance.AppointmentApiClient;
        treatment = ApiClientManager.Instance.CurrentTreatment;
        var response = await appointmentApiClient.ReadAppointmentsByTreatmentIdAsync(treatment.id);
        if (response == null)
        {
            Debug.LogError("Failed to load appointments from API.");
            return;
        }

        switch (response)
        {
            case WebRequestData<List<AppointmentWithNr>> dataResponse:
                {
                    Debug.Log("Data: " + dataResponse.Data.Count);
                    foreach (var appointment in dataResponse.Data)
                    {
                        GameObject appointmentGO = Instantiate(appointmentPrefab, contentParent);
                        TextMeshProUGUI tmpText = appointmentGO.GetComponentInChildren<TextMeshProUGUI>();

                        if (tmpText != null)
                        {
                            tmpText.text = appointment.name;
                        }
                    }
                    break;
                }

            case WebRequestError errorResponse:
                {
                    Debug.Log("Error: " + errorResponse.ErrorMessage);
                    break;
                }
        }
    }
}
