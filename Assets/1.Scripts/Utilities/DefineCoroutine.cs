using System;
using System.Collections;
using UnityEngine;

namespace DefineCoroutine
{
    static class CoroutineUtility
    {

        public static IEnumerator DelayAction(float seconds, Action action)
        {
            yield return new WaitForSeconds(seconds);
            action?.Invoke();
        }
    }


}
