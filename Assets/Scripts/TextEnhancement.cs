using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TextEnhancement : MonoBehaviour {

    private Text m_DialogText;
    private string m_Content;

    private void Awake()
    {
        m_DialogText = GetComponent<Text>();
        m_Content = m_DialogText.text;
    }

    private void OnEnable()
    {
        m_DialogText.text = null;
        Sequence sequence = DOTween.Sequence();
        //         sequence.Append(m_DialogText.DOText(m_Content, 2))
        //             .AppendInterval(1)
        //             .PrependInterval(0.5f);
        m_DialogText.DOText(m_Content, 2);
    }
}
