using System.Collections;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable 649
public class SliderAnimateColor : MonoBehaviour
{
    [SerializeField]
    private Color _fullHealthColor, _mediumHealthColor, _lowHealthColor;
    [SerializeField]
    private Image _targetImage;
    private Slider _slider;
    private IEnumerator _routine;

	void OnEnable ()
	{
	    _slider = GetComponent<Slider>();
	    _routine = UpdateInterval(1 / 10f);
        StartCoroutine(_routine);
	}

    IEnumerator UpdateInterval(float waitTime)
    {
        while (true)
        {
            if (_slider.value >= 7)
            {
                _targetImage.color = _fullHealthColor;
            }
            else if (_slider.value >= 4 && _slider.value <= 6)
            {
                _targetImage.color = _mediumHealthColor;
            }
            else
            {
                _targetImage.color = _lowHealthColor;
            }
            yield return new WaitForSeconds(waitTime);
            yield return new WaitForEndOfFrame();
        }
    }

    void OnDisable()
    {
        StopCoroutine(_routine);
    }
}
