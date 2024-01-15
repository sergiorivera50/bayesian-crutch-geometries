using System.Linq;
using UnityEngine;
using System.IO;

public class PressureDetector : MonoBehaviour {
    public float measurementInterval = 0.5f; // Time in seconds between measurements
    public float experimentDuration = 20.0f;
    public string identifier;
    private float pressure = 0.0f;
    private string logPath;

    void Start() {
        logPath = Application.dataPath + "/forces.log";
    }

    public void StartLogging() {
        InvokeRepeating("PrintMeasurement", 0.0f, measurementInterval);
        Invoke("ExitGame", experimentDuration);
    }

    void LogMessageToFile(string message) {
        string entry = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " - " + message;
        File.AppendAllText(logPath, entry + "\n");
    }

    void OnCollisionStay(Collision collision) {
        // Check if the colliding entity has the tag "Human"
        if (collision.transform.CompareTag("Human")) {
            // Estimate contact area based on the contact points
            float contactArea = EstimateContactArea(collision);

            // Calculate impulse per unit area (approximation of pressure during dynamic interaction)
            float lastImpulse = collision.impulse.magnitude / Time.fixedDeltaTime;
            pressure = lastImpulse / contactArea;
        }
    }

    float EstimateContactArea(Collision collision) {
        // Calculate contact area based on the points of collision
        // This is a simple estimation that assumes all contact points form a convex shape
        var contactPoints = collision.contacts.Select(contact => contact.point).ToList();

        // For simplicity, assume contact area is a convex polygon and calculate its area
        // Note: This is a naive implementation and should be replaced with a proper algorithm
        // for convex hull area calculation if accuracy is required
        float area = 0.0f;
        for (int i = 0; i < contactPoints.Count - 1; i++) {
            Vector3 point1 = contactPoints[i];
            Vector3 point2 = contactPoints[i + 1];
            area += Vector3.Cross(point1, point2).magnitude * 0.5f;
        }

        // Prevent division by zero
        return Mathf.Max(area, 0.001f);
    }

    void OnCollisionExit(Collision collision) {
        pressure = 0.0f;
    }

    void PrintMeasurement() {
        string logMessage = "[" + identifier + "] Pressure: " + pressure + " Pa";
        Debug.Log(logMessage);
        LogMessageToFile(logMessage);
    }

    private void ExitGame() {
        #if UNITY_EDITOR
            // If running in the Unity editor
            UnityEditor.EditorApplication.isPlaying = false;
        #else
                // If running in a build
                Application.Quit();
        #endif
    }
}
