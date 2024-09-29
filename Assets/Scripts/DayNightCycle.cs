using UnityEngine;

public class DayNightCycle : MonoBehaviour{
  [Range(0f, 1f)] public float timeOfDay = 0f; // 0 is midnight, 1 is midday.
  public float dayDuration = 120f; // Duration of a full day-night cycle in seconds.
  public float nightIntensity = 0.2f; // Global light intensity at night.
  public float dayIntensity = 1.0f; // Global light intensity during the day.

  // Day colors
  public Color daySkyColorTop = new Color(135 / 255f, 206 / 255f, 235 / 255f); // Light Sky Blue
  public Color daySkyColorHorizon = new Color(173 / 255f, 216 / 255f, 230 / 255f); // Light Blue
  public Color daySkyColorBottom = new Color(224 / 255f, 255 / 255f, 255 / 255f); // Pale Light Cyan

  // Night colors
  public Color nightSkyColorTop = new Color(0f, 0f, 0.15f); // Very Dark Blue
  public Color nightSkyColorHorizon = new Color(0.04f, 0.05f, 0.1f); // Midnight Blue
  public Color nightSkyColorBottom = new Color(0 / 255f, 0 / 255f, 25 / 255f); // Near Black

  // Sunrise and Sunset colors
  public Color sunriseColor = new Color(255 / 255f, 153 / 255f, 102 / 255f); // Soft Peachy Orange
  public Color sunsetColor = new Color(0.04f, 0.05f, 0.1f); // Color for sunset

  public static float globalLightIntensity;

  // Customizable ranges for sunrise and sunset
  public float sunriseStart = 0.2f;
  public float sunriseEnd = 0.35f; // Extended the sunrise period slightly
  public float sunsetStart = 0.6f;
  public float sunsetEnd = 0.85f; // Extended the sunset period slightly

  void Update(){
    // Update time of day based on elapsed time
    timeOfDay += (Time.deltaTime / dayDuration);
    if (timeOfDay > 1f) timeOfDay -= 1f;

    // Lerp between night and day intensity based on time of day
    globalLightIntensity = Mathf.Lerp(nightIntensity, dayIntensity, Mathf.Sin(timeOfDay * Mathf.PI));

    // Set global light intensity in shader
    Shader.SetGlobalFloat("_GlobalLightIntensity", globalLightIntensity);

    // Update sky colors based on time of day
    UpdateSkyColors();
  }

  void UpdateSkyColors(){
    Color topColor, horizonColor, bottomColor;

    if (timeOfDay >= sunriseStart && timeOfDay <= sunriseEnd){
      // Sunrise transition
      float t = Mathf.SmoothStep(0, 1, (timeOfDay - sunriseStart) / (sunriseEnd - sunriseStart));
      horizonColor = Color.Lerp(nightSkyColorHorizon, sunriseColor, t);
      topColor = Color.Lerp(nightSkyColorTop, sunriseColor, t);
      bottomColor = Color.Lerp(nightSkyColorBottom, sunriseColor, t);
    }
    else if (timeOfDay >= sunsetStart && timeOfDay <= sunsetEnd){
      // Sunset transition
      float t = Mathf.SmoothStep(0, 1, (timeOfDay - sunsetStart) / (sunsetEnd - sunsetStart));
      horizonColor = Color.Lerp(daySkyColorHorizon, sunsetColor, t);
      topColor = Color.Lerp(daySkyColorTop, sunsetColor, t);
      bottomColor = Color.Lerp(daySkyColorBottom, sunsetColor, t);
    }
    else if (timeOfDay < sunriseStart || timeOfDay > sunsetEnd){
      // Night colors
      topColor = nightSkyColorTop;
      horizonColor = nightSkyColorHorizon;
      bottomColor = nightSkyColorBottom;
    }
    else{
      // Smooth Day Transition
      float dayProgress = Mathf.InverseLerp(sunriseEnd, sunsetStart, timeOfDay);
      horizonColor = Color.Lerp(sunriseColor, daySkyColorHorizon, dayProgress);
      topColor = Color.Lerp(sunriseColor, daySkyColorTop, dayProgress);
      bottomColor = Color.Lerp(sunriseColor, daySkyColorBottom, dayProgress);
    }

    // Update the skybox material with the new colors
    Shader.SetGlobalColor("_SkyColorTop", topColor);
    Shader.SetGlobalColor("_SkyColorHorizon", horizonColor);
    Shader.SetGlobalColor("_SkyColorBottom", bottomColor);
  }
}