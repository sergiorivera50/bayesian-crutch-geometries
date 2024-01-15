using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrutchConfiguration : MonoBehaviour
{
    public Animator animator;
    public Slider gammaSlider;
    public Slider alphaSlider;
    public Slider betaSlider;
    public Transform gamma;
    public Transform alpha;
    public Transform beta;

    void Start() {
        gammaSlider.minValue = 0f;
        gammaSlider.maxValue = 0.4f;
        gammaSlider.value = gamma.localPosition.x;
        gammaSlider.onValueChanged.AddListener(OnGammaChanged);

        alphaSlider.minValue = alpha.rotation.eulerAngles.z - 30f;
        alphaSlider.maxValue = alpha.rotation.eulerAngles.z + 30f;
        alphaSlider.value = alpha.rotation.eulerAngles.z;
        alphaSlider.onValueChanged.AddListener(OnAlphaChanged);

        betaSlider.minValue = beta.rotation.eulerAngles.z - 30f;
        betaSlider.maxValue = beta.rotation.eulerAngles.z + 30f;
        betaSlider.value = beta.rotation.eulerAngles.z;
        betaSlider.onValueChanged.AddListener(OnBetaChanged);
    }

    void OnGammaChanged(float value) {
        gamma.localPosition = new Vector3(value, gamma.localPosition.y, gamma.localPosition.z);
    }

    void OnAlphaChanged(float value) {
        alpha.rotation = Quaternion.Euler(alpha.rotation.eulerAngles.x, alpha.rotation.eulerAngles.y, value);
    }

    void OnBetaChanged(float value) {
        beta.rotation = Quaternion.Euler(beta.rotation.eulerAngles.x, beta.rotation.eulerAngles.y, value);
    }
}
