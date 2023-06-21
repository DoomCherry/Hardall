using System;
using System.Collections;
using UnityEngine;

namespace CoroutineExtension
{
    public static class CoroutineExtension
    {
        public static Coroutine RepeatUntil(this MonoBehaviour behaviour, Func<bool> condition, Action repeatAction, Action callback, float delay)
        {
            return behaviour.StartCoroutine(RepeatUntil(condition, repeatAction, callback, delay));
        }

        public static IEnumerator RepeatUntil(Func<bool> condition, Action repeatAction, Action callback, float delay)
        {
            delay = Mathf.Clamp(delay, Time.deltaTime, float.MaxValue);

            do
            {
                repeatAction.Invoke();
                yield return new WaitForSeconds(delay);

            } while (condition.Invoke());

            callback.Invoke();
        }
    }
}

