using UnityEngine;
using System.Collections.Generic;

public class ApiClientManager : MonoBehaviour
{
    public static ApiClientManager Instance;

    [Header("ApiClients")]
    public WebClient WebClient;
    public UserApiClient UserApiClient;
    public PatientApiClient PatientApiClient;
    public GuardianApiClient GuardianApiClient;
    public TreatmentApiClient TreatmentApiClient;
    public AppointmentApiClient AppointmentApiClient;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Get the ApiClients from the GameObjects
        if (WebClient == null) { WebClient = GetComponent<WebClient>(); }
        if (UserApiClient == null) { UserApiClient = GetComponent<UserApiClient>(); }
        if (PatientApiClient == null) { PatientApiClient = GetComponent<PatientApiClient>(); }
        if (GuardianApiClient == null) { GuardianApiClient = GetComponent<GuardianApiClient>(); }
        if (TreatmentApiClient == null) { TreatmentApiClient = GetComponent<TreatmentApiClient>(); }
        if (AppointmentApiClient == null) { AppointmentApiClient = GetComponent<AppointmentApiClient>(); }
    }

    // Properties and methodes for storing data like logged in user, etc.
    public User CurrentUser { get; private set; }
    public void SetCurrentUser(User user)
    {
        CurrentUser = user;
    }

    public Guardian CurrentGuardian { get; private set; }
    public void SetCurrentGuardian(Guardian guardian)
    {
        CurrentGuardian = guardian;
    }

    public Patient CurrentPatient { get; private set; }
    public void SetCurrentPatient(Patient patient)
    {
        CurrentPatient = patient;
    }

    public Treatment CurrentTreatment { get; private set; }
    public void SetCurrentTreatment(Treatment treatment)
    {
        CurrentTreatment = treatment;
    }

    public List<Appointment> CurrentAppointments { get; set; }

    public void ClearData()
    {
        CurrentUser = null;
        CurrentGuardian = null;
        CurrentPatient = null;
        CurrentTreatment = null;
    }
}
