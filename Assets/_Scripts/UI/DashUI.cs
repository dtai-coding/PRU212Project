using UnityEngine;
using UnityEngine.UI;

public class DashUI : MonoBehaviour
{
	[SerializeField] private Player player;
	public Slider slider;
	public Image fillAreaImage;
	// Start is called before the first frame update
	void Start()
    {
		player = FindObjectOfType<Player>();
		if (slider != null && slider.fillRect != null)
		{
			fillAreaImage = slider.fillRect.GetComponent<Image>();
			fillAreaImage.color = Color.cyan;
		}
	}

    // Update is called once per frame
    void Update()
    {
		try
		{
			float progress = player.timeSlowCooldownTimer / player.timeSlowCoolDown;
			slider.value = 1 - progress; // Invert the progress
										 
			if (slider.value != 1)		// Change color based on the value
			{
				fillAreaImage.color = Color.red;
			}
			else
			{
				fillAreaImage.color = Color.cyan; // Change to cyan when value is 1
			}
		}
		catch (System.NullReferenceException)
		{
			// Suppress the NullReferenceException
		}

	}
}
