using System.Linq;
using UnityEngine;

public class PressureDetector : MonoBehaviour {
    public float measurementInterval = 0.5f; // Time in seconds between measurements
    public string identifier;
    private float pressure = 0.0f;

    void Start() {
        InvokeRepeating("PrintMeasurement", 0.0f, measurementInterval);
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
        Debug.Log("[" + identifier + "] Pressure: " + pressure + " Pa");
    }
}
