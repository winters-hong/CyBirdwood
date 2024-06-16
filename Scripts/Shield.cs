using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    Renderer _renderer;
    [SerializeField]
    AnimationCurve _DisplacementCurve;
    [SerializeField]
    float _DisplacementMagnitude;
    [SerializeField]
    float _LerpSpeed;
    [SerializeField]
    float _DisolveSpeed;
    bool _shieldOn;
    Coroutine _disolveCoroutine;
    public GameController gctrl;
    // Start is called before the first frame update
    void Start()
    {
        _renderer = GetComponent<Renderer>();
        gctrl = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }

    // Update is called once per frame
    void Update()
    {
        // if (Input.GetMouseButtonDown(0))
        // {
        //     Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //     RaycastHit hit;
        //     if (Physics.Raycast(ray, out hit))
        //     {
        //         HitShield(hit.point);
        //     }
        // }
        // if (Input.GetKeyDown(KeyCode.F))
        // {
        //     OpenCloseShield();
        // }
    }

    public void HitShield(Vector3 hitPos)
    {
        _renderer.material.SetVector("_HitPos", hitPos);
        StopAllCoroutines();
        StartCoroutine(Coroutine_HitDisplacement());
        gctrl.HitBubble();
    }

    public void OpenCloseShield()
    {
        float target = 1;
        if (_shieldOn)
        {
            target = 0;
        }
        _shieldOn = !_shieldOn;
        if (_disolveCoroutine != null)
        {
            StopCoroutine(_disolveCoroutine);
        }
        _disolveCoroutine = StartCoroutine(Coroutine_DissolveShield(target));
    }

    IEnumerator Coroutine_HitDisplacement()
    {
        float lerp = 0;
        while (lerp < 1)
        {
            _renderer.material.SetFloat(
                "_DisplacementStrength",
                _DisplacementCurve.Evaluate(lerp) * _DisplacementMagnitude
            );
            lerp += Time.deltaTime * _LerpSpeed;
            yield return null;
        }
    }

    IEnumerator Coroutine_DissolveShield(float target)
    {
        float start = _renderer.material.GetFloat("_Dissolve");
        float lerp = 0;
        while (lerp < 1)
        {
            _renderer.material.SetFloat("_Dissolve", Mathf.Lerp(start, target, lerp));
            lerp += Time.deltaTime * _DisolveSpeed;
            yield return null;
        }
    }
}
