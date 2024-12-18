using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class DamageEffect : MonoBehaviour
{
    public VolumeProfile damageProfile;

    public VolumeProfile defaultProfile;

    public float damageDuration = 0.5f;

    private bool isDamageEffectOn = false;

    public void DamageEffectOn()
    {
        if (isDamageEffectOn)
        {
            StopAllCoroutines();
        }
        StartCoroutine(DamageEffectCoroutine());
    }

    private Volume volume;

    IEnumerator DamageEffectCoroutine()
    {
        isDamageEffectOn = true;
        volume = gameObject.GetComponent<Volume>();
        volume.profile = damageProfile;
        yield return new WaitForSeconds(damageDuration);
        volume.profile = defaultProfile;
        isDamageEffectOn = false;
    }
}
